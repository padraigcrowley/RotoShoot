using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>, IPowerUpEvents
{

  public Queue mouseClickQueue;
  [HideInInspector] public bool playerShipRotating = false;

  [HideInInspector] public enum GameState {
    WAITING_FOR_LEVELCOMPLETE_BUTTONS, WAITING_FOR_PLAYERDIED_BUTTONS, WAITING_FOR_START_BUTTON, LEVEL_INTRO_IN_PROGRESS, LEVEL_IN_PROGRESS, LEVEL_FAILED, LEVEL_OUTRO_IN_PROGRESS, 
                                            LEVEL_COMPLETE, GAME_OVER_SCREEN, PLAYER_DYING, PLAYER_DIED }
  [HideInInspector] public enum PlayerFiringState { STRAIGHT_SINGLE, ANGLED_TRIPLE, STRAIGHT_TRIPLE, RAPID_FIRE_SINGLE }
  [HideInInspector] public int currentPlayerScore = 0;
  public int highPlayerScore = 0;
   public int currentPlayerHP;
  [HideInInspector] public float screenEdgeX, screenEdgeY, screenCollisionBoundaryX, screenCollisionBoundaryY;

  public GameObject playerShipObject;
  public GameObject playerShipGFXObject;

  public GameObject leftBoundary;                   //
  public GameObject rightBoundary;                  // References to the screen bounds: Used to ensure the player
  public GameObject topBoundary;                    // is not able to leave the screen.
  public GameObject bottomBoundary;

  public float angleToRotatePlayerShip; //set in inspector, maybe shouldn't be?

  public float currentPlayerShipRotationDuration;
  public float basePlayerShipRotationDuration;

  public float currentPlayerMissileSpeedMultiplier;
  public float basePlayerMissileSpeedMultiplier;
  public float PlayerMissileDamage;

  public bool isGamePaused = false;
  public bool playerShipInvulnerable = false;
  public bool playerShipMovementAllowed = false;
  public bool playerShipFiring = false;
  public bool playerShieldVisible = false;
  public float currentPlayerShipFireRate; // the lower the number, the faster the rate.
  public float basePlayerShipFireRate; // the lower the number, the faster the rate.
  
  public int maxEnemy0001HP; //set in the inspector
  public float enemy0001BaseSpeed = 1.0f;

  public int MAX_PLAYER_HP = 100;

  public Vector2[] shipLanes;// = new[] { new Vector2(-3.85f, -6f), new Vector2(-1.29f, -6f), new Vector2(1.29f, -6f), new Vector2(3.84f, -6f) };

  public GameState currentGameState;
  public PlayerFiringState currentPlayerFiringState;
  [HideInInspector] public Vector3 playerShipPos;
  [HideInInspector] public int levelControlType;
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 
  public int totalEnemyKillCount = 0;
  public int enemyKillPowerUpDropFrequency = 5;

  public LevelManager levelManagerScript;
  public PlayerShip playerShipScript;

  public float powerupDurationSeconds, playerShieldPowerupDurationSeconds;
  public int starCoinCount;
  private bool dyingInProgress = false;

  public CapsuleCollider playerShipCollider;

  public float tripleFirePowerupRemainingDuration = -1;

  public GameObject playerShieldPrefab;


  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("<color=red> Active Scene : " + SceneManager.GetActiveScene().name + "</color>");
    SceneManager.SetActiveScene(SceneManager.GetSceneByName("BaseGameScene"));
    Debug.Log("<color=red> Active Scene : " + SceneManager.GetActiveScene().name + "</color>");

    levelManagerScript.InitialiseLevel();

    currentPlayerFiringState = PlayerFiringState.STRAIGHT_SINGLE;
    currentGameState = GameState.LEVEL_INTRO_IN_PROGRESS;
    currentPlayerMissileSpeedMultiplier = basePlayerMissileSpeedMultiplier;
    currentPlayerHP = 0; //setting it to zero here, then its proper value in PlayerShip.CS, line, "GameplayManager.Instance.currentPlayerHP = GameplayManager.Instance.MAX_PLAYER_HP;"
                         //so the gradual fill-in of the health bar can kick in.
    currentPlayerShipFireRate = basePlayerShipFireRate;
    
    PlayerMissileDamage = 10f;//todo - read from CSV
    
    mouseClickQueue = new Queue();
    currentPlayerScore = 0;
    
  }

  // Update is called once peer frame
  void Update()
  {

    switch (currentGameState)
    {
      case GameState.LEVEL_INTRO_IN_PROGRESS:
        {
          playerShipCollider.enabled = true;
          break;
        }
      case GameState.LEVEL_IN_PROGRESS:
        {
          //if (Input.GetKey(KeyCode.Space))
          //  Time.timeScale = 1f / 4f; //set time to 1/4 speed
          

          //if it's a boss level don't allow the player ship to move or fire just yet. The boss code will do it.
          if(LevelManager.Instance.LevelCompletionCriteria.ContainsKey("KillBoss") == false)
					{
            playerShipFiring = true;
            playerShipMovementAllowed = true;
					}
          
          if (currentPlayerScore > highPlayerScore)
          {
            highPlayerScore = currentPlayerScore;
          }

          if (currentPlayerHP <= 0)
          {
            currentGameState = GameState.PLAYER_DYING;
          }
          break;
        }
      case GameState.LEVEL_OUTRO_IN_PROGRESS:
        {
          GameplayManager.Instance.playerShipFiring = false;
          GameplayManager.Instance.playerShipMovementAllowed = false;
          break;
        }
      case GameState.LEVEL_COMPLETE:
        {
          ES3.Save("starCoinCount", GameController.Instance.starCoinCount);
          if (GameController.Instance.currentLevelPlaying+1 > GameController.Instance.highestLevelPlayed)
          {
            GameController.Instance.highestLevelPlayed = GameController.Instance.currentLevelPlaying+1;
            ES3.Save("highestLevelPlayed", GameController.Instance.highestLevelPlayed);
          }
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.WAITING_FOR_LEVELCOMPLETE_BUTTONS;
          break;
        }
      
      case GameState.PLAYER_DYING:
        {
          if (!dyingInProgress)
          {
            dyingInProgress = true;
            playerShipCollider.enabled = false;
            StartCoroutine(playerShipScript.DoPlayerDeath());
          }
          //currentGameState = GameState.PLAYER_DIED;
          //PauseGame();
          break;
        }
      case GameState.PLAYER_DIED:
        {
          dyingInProgress = false;
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.WAITING_FOR_PLAYERDIED_BUTTONS;
          //playerShipObject.SetActive(false);
          //PauseGame();
          break;
        }
      case GameState.LEVEL_FAILED:
        {
          print("Level Failed!");
          break;
        }
      default:
        break;
    }
  }

  public void PauseGame()
  {
    Time.timeScale = 0;
    isGamePaused = true;
  }

  public void UnpauseGame()
  {
    Time.timeScale = 1;
    isGamePaused = false;
  }

  public void initializeMainGameplayLoopForLevelRestart()
  {
    currentPlayerHP = MAX_PLAYER_HP;
    //gameState = 0;
    mouseClickQueue = new Queue();
    currentGameState = GameState.LEVEL_INTRO_IN_PROGRESS;
    currentPlayerScore = 0;
    //numEnemyKills = 0;
    enemy0001BaseSpeed = 1.0f;
    currentPlayerShipRotationDuration = basePlayerShipRotationDuration;

    Wait(2.5f, () => {
      Instantiate(playerShieldPrefab, playerShipGFXObject.transform.position, Quaternion.identity, playerShipGFXObject.transform);
    });
  }
  public void initializeMainGameplayLoopForNextLevel()
  {
    
    //currentPlayerHP = MAX_PLAYER_HP;
    GameController.Instance.UnloadSpecificLevel(GameController.Instance.currentLevelBackground);
    GameController.Instance.currentLevelPlaying++;
    GameController.Instance.LoadSpecificLevelAndBaseGame(GameController.Instance.currentLevelPlaying);
    
    levelManagerScript.InitialiseLevel();
    mouseClickQueue = new Queue();
    currentGameState = GameState.LEVEL_INTRO_IN_PROGRESS;
  }
  void IPowerUpEvents.OnPowerUpCollected(PowerUp powerUp, PlayerShip player)
  {
    //// We dont bother storing those that expire immediately
    //if (!powerUp.expiresImmediately)
    //{
    //  activePowerUps.Add(powerUp);
    //  //print($"powerup type: {powerUp.GetType()} powerup name: {powerUp.name}");
    //  //((PowerUpInvulnerability)powerUp).duration = 10.0f;

    //  UpdateActivePowerUpUi();
    //}

    //uiText.text = powerUp.powerUpExplanation;
    //uiSubtext.text = powerUp.powerUpQuote;
    //uiTextDisplayTimer = uiTextDisplayDuration;
  }

  void IPowerUpEvents.OnPowerUpExpired(PowerUp powerUp, PlayerShip player)
  {
    //activePowerUps.Remove(powerUp);
    //UpdateActivePowerUpUi();
  }

}
