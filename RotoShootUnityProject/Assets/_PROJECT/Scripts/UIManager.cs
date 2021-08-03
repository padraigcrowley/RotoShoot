using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using BeautifulTransitions.Scripts.Transitions;
public class UIManager : Singleton<UIManager>
{
  public TextMeshProUGUI HighPlayerScoreText, CurrentPlayerScoreText, CurrentEnemyKillCount, RequiredEnemyKillCount, levelPlayTimeCounterText, starCoinCountText, MissionStartLCCText;

  public TMP_Text LeveCompleteTextObject;

  public GameObject MissionStartLCCTextObject;
  public Button gameRestartButton, gameExitButton;
  public GameObject PlayerDiedPanel;

  [SerializeField] private GameObject LevelCompletePanel;
  public Image PauseButtonBGImage, PauseButtonFGImage;

  public Febucci.UI.TextAnimatorPlayer lccTextAnimatorPlayer;
  public Febucci.UI.TextAnimatorPlayer levelCompleteTextAnimatorPlayer;

  public GameObject MainPauseMenuButtonTransitions;
  public GameObject TopInGameHUDTransitions;
  public GameObject LevelCompletePanelTransitions;
  private bool hudOn = false;
  private bool doingLevelCompleteText = false;

  // Start is called before the first frame update
  void Start()
  {
    //MissionStartLCCTextObject.SetActive(false);
    CurrentPlayerScoreText.text = GameplayManager.Instance.currentPlayerScore.ToString();
    HighPlayerScoreText.text = GameplayManager.Instance.highPlayerScore.ToString();
    CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
    RequiredEnemyKillCount.text = "/" + LevelManager.Instance.levelSetupData.lccEnemyKills.ToString();
    starCoinCountText.text = GameController.Instance.starCoinCount.ToString();
  }

  
  public IEnumerator FadeOutBigText(TMP_Text myTextObject, float fadeOutDuration = 1f)
	{

    float aValue = 0f;
    float alpha = MissionStartLCCText.color.a;
        
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeOutDuration)
    {
      Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
      myTextObject.color = newColor;
      yield return null;
    }
        
  }

  public IEnumerator DoMissionStartLCCText(float appearDelay, float disappearDelay, string myText)
  {
    
    float aValue = 0f, fadeOutDuration = 2f;
    float alpha = MissionStartLCCText.color.a;

    yield return new WaitForSeconds(appearDelay);
    
    MissionStartLCCTextObject.SetActive(true);
    lccTextAnimatorPlayer.ShowText(myText);
        
    yield return new WaitForSeconds(disappearDelay);
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeOutDuration)
    {
      Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
      MissionStartLCCText.color = newColor;
      yield return null;
    }

		MissionStartLCCTextObject.SetActive(false);
    MissionStartLCCText.color = new Color (1,1,1,alpha); //reset the alpha back to the original value
  }

  // Update is called once per frame
  void Update() 
  {
    switch (GameplayManager.Instance.currentGameState)
    {
      case GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS:
        {
          
          RequiredEnemyKillCount.text = "/" + LevelManager.Instance.levelSetupData.lccEnemyKills.ToString();
          doingLevelCompleteText = false;
          break;
        }
      case GameplayManager.GameState.LEVEL_IN_PROGRESS:
        {
          //LevelCompletePanel.gameObject.SetActive(false);// just so it will trigger on enable at level end 

          //playerHealthBarObject.SetActive(true);
          //UltimateStatusBar.UpdateStatus("playerStatusBar", GameplayManager.Instance.currentPlayerHP, GameplayManager.Instance.MAX_PLAYER_HP); 

          CurrentPlayerScoreText.text = (GameplayManager.Instance.currentPlayerScore).ToString();
          //CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
          HighPlayerScoreText.text = "HI:" + GameplayManager.Instance.highPlayerScore.ToString();
          levelPlayTimeCounterText.text = LevelManager.Instance.levelPlayTimeElapsed.ToString("0.00");

          if (!hudOn)
          {
            TransitionHelper.TransitionIn(TopInGameHUDTransitions);
            hudOn = true;
          }

          break;
        }
      case GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS:
        {
          //if (hudOn)
          //{
          //  TransitionHelper.TransitionOut(TopInGameHUDTransitions);
          //  hudOn = false;
          //}
          //GameplayManager.Instance.currentGameState = GameplayManager.GameState.WAITING_FOR_LEVELCOMPLETE_BUTTONS;

          break;
        }
      case GameplayManager.GameState.WAITING_FOR_LEVELCOMPLETE_BUTTONS:
        {
          if (doingLevelCompleteText == false)
          {
            //LevelCompletePanel.gameObject.SetActive(true);
            LeveCompleteTextObject.gameObject.SetActive(true);
            LeveCompleteTextObject.color = new Color(1, 1, 1, 1); //reset the alpha back to the original value
            LeveCompleteTextObject.text = "MISSION\nCOMPLETE!";
            doingLevelCompleteText = true;
          }
          //levelCompleteTextAnimatorPlayer.ShowText("MISSION\nCOMPLETE!");

          break;
        }
      case GameplayManager.GameState.WAITING_FOR_PLAYERDIED_BUTTONS:
        {
          PlayerDiedPanel.SetActive(true);
          break;
        }
      default:
        break;
    }
  }

  public void handleLevelCompletePanelOKButtonPress()
	{
    //TransitionHelper.TransitionOut(LevelCompletePanelTransitions);
    //LevelCompletePanel.gameObject.SetActive(false);
    StartCoroutine(FadeOutBigText(LeveCompleteTextObject));
    GameplayManager.Instance.initializeMainGameplayLoopForNextLevel();
  }

  public void handlePauseButtonPress()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS || GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (GameplayManager.Instance.isGamePaused)
      {
        TransitionHelper.TransitionOut(MainPauseMenuButtonTransitions);
        GameplayManager.Instance.UnpauseGame();
        SetButtonImageAlpha(PauseButtonFGImage, .4f);
        SetButtonImageAlpha(PauseButtonBGImage, .4f);
      }
      else
      {
        TransitionHelper.TransitionIn(MainPauseMenuButtonTransitions);
        GameplayManager.Instance.PauseGame();
        SetButtonImageAlpha(PauseButtonFGImage, 1f);
        SetButtonImageAlpha(PauseButtonBGImage, 1f);
      }
    }
  }

  void SetButtonImageAlpha(Image ImageButton, float newAlpha)
  {
    var tempColor = ImageButton.color;
    tempColor.a = newAlpha;
    ImageButton.color = tempColor;
  }

  public void handleGameRestartButtonPress()
  {
    print("Start Button Pressed!");
    CurrentPlayerScoreText.text = "0";
    //GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS;

    //PlayerDiedPanel.SetActive(false);
    TransitionHelper.TransitionOut(PlayerDiedPanel);

    GameplayManager.Instance.initializeMainGameplayLoopForLevelRestart();
    //GameplayManager.Instance.ResumeGame();
  }

  public void handleGameExitButtonPress()
  {
    GameController.Instance.currentLevelPlaying = 0;
    TransitionHelper.TransitionOut(PlayerDiedPanel);
    SceneManager.LoadScene("MainMenu");
  }

}
