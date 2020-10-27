using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{

  public Queue mouseClickQueue;
  [HideInInspector] public bool playerShipRotating = false;

  [HideInInspector] public enum GameState { WAITING_FOR_START_BUTTON, LEVEL_INTRO_IN_PROGRESS, LEVEL_IN_PROGRESS, LEVEL_FAILED, LEVEL_OUTRO_IN_PROGRESS, LEVEL_COMPLETE, GAME_OVER_SCREEN }
  [HideInInspector] public enum PlayerFiringState { STRAIGHT_SINGLE, ANGLED_TRIPLE, STRAIGHT_TRIPLE}
  [HideInInspector] public int currentPlayerScore = 0;
  public int highPlayerScore = 0;
  [HideInInspector] public int currentPlayerHP;
  [HideInInspector] public float screenEdgeX, screenEdgeY, screenCollisionBoundaryX, screenCollisionBoundaryY;
  
  public GameObject leftBoundary;                   //
  public GameObject rightBoundary;                  // References to the screen bounds: Used to ensure the player
  public GameObject topBoundary;                    // is not able to leave the screen.
  public GameObject bottomBoundary;

  public float angleToRotatePlayerShip; //set in inspector, maybe shouldn't be?

  public float currentPlayerShipRotationDuration;
  public float basePlayerShipRotationDuration;

  public float currentPlayerMissileSpeedMultiplier;
  public float basePlayerMissileSpeedMultiplier;

  public float currentPlayerShipFireRate; // the lower the number, the faster the rate.
  public float basePlayerShipFireRate; // the lower the number, the faster the rate.

  public int maxEnemy0001HP; //set in the inspector
  public float enemy0001BaseSpeed = 1.0f;

  public int maxPlayerHP;

  public Vector2[] shipLanes;// = new[] { new Vector2(-3.85f, -6f), new Vector2(-1.29f, -6f), new Vector2(1.29f, -6f), new Vector2(3.84f, -6f) };

  public GameState currentGameState;
  public PlayerFiringState currentPlayerFiringState;
  [HideInInspector] public Vector3 playerShipPos;
  [HideInInspector] public int levelControlType;
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 


  // Start is called before the first frame update
  void Start()
  {
    currentPlayerFiringState = PlayerFiringState.STRAIGHT_SINGLE;
    currentGameState = GameState.LEVEL_INTRO_IN_PROGRESS;
    currentPlayerMissileSpeedMultiplier = basePlayerMissileSpeedMultiplier;
    currentPlayerHP = maxPlayerHP;
    currentPlayerShipFireRate = basePlayerShipFireRate;
    mouseClickQueue = new Queue();
    currentPlayerScore = 0;
  }

  // Update is called once peer frame
  void Update()
  {

    switch (currentGameState)
    {
      case GameState.LEVEL_IN_PROGRESS:
        {
          if (currentPlayerScore > highPlayerScore)
          {
            highPlayerScore = currentPlayerScore;
          }

          if (currentPlayerHP <= 0)
          {
            print("Game Over!");
            currentGameState = GameState.GAME_OVER_SCREEN;
          }
          break;
        }
      case GameState.LEVEL_COMPLETE:
        {

          print("Level Completion Criteria TRUE!, Next Level is:" + GameManagerX.Instance.currentLevel + 1);
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


  public void initializeMainGameplayLoop()
  {
    
    
    
    currentPlayerHP = maxPlayerHP;
    //gameState = 0;
    mouseClickQueue = new Queue();
    currentPlayerScore = 0;
    //numEnemyKills = 0;
    enemy0001BaseSpeed = 1.0f;
    currentPlayerShipRotationDuration = basePlayerShipRotationDuration;
    
  }

}
