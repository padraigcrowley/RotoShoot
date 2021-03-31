using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
  public TextMeshProUGUI HighPlayerScoreText, CurrentPlayerScoreText, CurrentEnemyKillCount, RequiredEnemyKillCount, levelPlayTimeCounterText, starCoinCountText;
  public Button gameRestartButton, gameExitButton;
  public Image playerHealthHeart03, playerHealthHeart02, playerHealthHeart01;
  [SerializeField] private GameObject LevelCompletePanel;

  public UltimateStatusBar playerStatusBar;
  public GameObject playerHealthBarObject;

  // Start is called before the first frame update
  void Start()
  {
    playerHealthBarObject.SetActive(false);
    CurrentPlayerScoreText.text = GameplayManager.Instance.currentPlayerScore.ToString();
    HighPlayerScoreText.text = GameplayManager.Instance.highPlayerScore.ToString();
    CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
    RequiredEnemyKillCount.text = "/" + LevelManager.Instance.levelSetupData.lccEnemyKills.ToString();

  }

  // Update is called once per frame
  void Update() 
  {
    switch (GameplayManager.Instance.currentGameState)
    {
      case GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS:
        {
          LevelCompletePanel.gameObject.SetActive(false);
          RequiredEnemyKillCount.text = "/" + LevelManager.Instance.levelSetupData.lccEnemyKills.ToString();
          break;
        }
      case GameplayManager.GameState.LEVEL_IN_PROGRESS:
        {
          //playerHealthBarObject.SetActive(true);
          //UltimateStatusBar.UpdateStatus("playerStatusBar", GameplayManager.Instance.currentPlayerHP, GameplayManager.Instance.MAX_PLAYER_HP); 

          CurrentPlayerScoreText.text = (GameplayManager.Instance.currentPlayerScore).ToString();
          CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
          HighPlayerScoreText.text = "HI:" + GameplayManager.Instance.highPlayerScore.ToString();
          levelPlayTimeCounterText.text = LevelManager.Instance.levelPlayTimeElapsed.ToString("0.00");

          if (GameplayManager.Instance.currentPlayerHP == 3)
          {
            playerHealthHeart03.enabled = true;
            playerHealthHeart02.enabled = true;
            playerHealthHeart01.enabled = true;
          }
          else if (GameplayManager.Instance.currentPlayerHP == 2)
          {
            playerHealthHeart03.enabled = false;
          }
          else if (GameplayManager.Instance.currentPlayerHP == 1)
          {
            playerHealthHeart02.enabled = false;
          }
          else if (GameplayManager.Instance.currentPlayerHP == 0)
          {
            playerHealthHeart01.enabled = false;
          }

          break;
        }
      case GameplayManager.GameState.LEVEL_COMPLETE:
        {
          LevelCompletePanel.gameObject.SetActive(true);

          break;
        }
      case GameplayManager.GameState.GAME_OVER_SCREEN:
        {
          gameRestartButton.gameObject.SetActive(true);
          gameExitButton.gameObject.SetActive(true);
          break;
        }
      default:
        break;
    }
  }

  public void handleGameRestartButtonPress()
  {
    print("Start Button Pressed!");
    CurrentPlayerScoreText.text = "0";
    //GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS;
    gameRestartButton.gameObject.SetActive(false);
    gameExitButton.gameObject.SetActive(false);
    GameplayManager.Instance.initializeMainGameplayLoopForLevelRestart();
  }

  public void handleGameExitButtonPress()
  {
    GameManagerX.Instance.currentLevel = 0;
    SceneManager.LoadScene("MainMenu");
  }

}
