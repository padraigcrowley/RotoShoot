
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using TMPro;

public class MainMenuContoller : ExtendedBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject PlayButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;
  public GameObject MainMenuSettingsPanel;
  public GameObject MainMenuUpgradesPanel;
  public GameObject LevelSelectButtonsPanel;


  public Image prevLevelButtonImage;
  public Image nextLevelButtonImage;
  public Image logoImage;
  private Material logoImageMaterial;
  public TMP_Text selectedLevelText;
  [SerializeField]
  private Move logoMoveTransitionOut;
  private Move logoMoveTransitionIn;

  public TMP_Text UpgradesItem0LevelText, UpgradesItem1LevelText, UpgradesItem2LevelText, UpgradesItem3LevelText;
  public TMP_Text UpgradesItem0UpgradeCostText, UpgradesItem1UpgradeCostText, UpgradesItem2UpgradeCostText, UpgradesItem3UpgradeCostText;

  void Start()
  {
    //See the "performance" section from: https://www.textanimator.febucci.com/docs/troubleshooting/#editor
    Febucci.UI.Core.TAnimBuilder.InitializeGlobalDatabase();

    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
		TransitionHelper.TransitionIn(LogoImageTransitions);
    logoImageMaterial = logoImage.material;

    //we're not running these 2 transitions now, just setting them up for future use.
    logoMoveTransitionOut = new Move(logoGameObject, startPosition: logoGameObject.transform.localPosition, endPosition: new Vector3(0, 1620, 0), duration: .3f, delay: 0f, tweenType: TransitionHelper.TweenType.easeInBack, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    
    logoMoveTransitionIn = new Move(logoGameObject, startPosition: new Vector3(0, 1620, 0), endPosition: new Vector3(0, 281, 0), duration: .5f, delay: 0f, tweenType: TransitionHelper.TweenType.spring, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);

    UpdateUpgradesMenuStatsText();
  }

  public void UpdateUpgradesMenuStatsText()
	{
    UpgradesItem0LevelText.text = $"Level : {GameController.Instance.maxPlayerHPLevel}";
    UpgradesItem1LevelText.text = $"Level : {GameController.Instance.playerMissileDamageLevel}";
    UpgradesItem2LevelText.text = $"Level : {GameController.Instance.shieldDurationLevel}";
    UpgradesItem3LevelText.text = $"Level : {GameController.Instance.powerupDurationLevel}";

    
    UpgradesItem0UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue("starCoinCost", GameController.Instance.maxPlayerHPLevel+ 1) }";
    UpgradesItem1UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue("starCoinCost", GameController.Instance.playerMissileDamageLevel + 1) }";
    UpgradesItem2UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue("starCoinCost", GameController.Instance.shieldDurationLevel + 1) }";
    UpgradesItem3UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue("starCoinCost", GameController.Instance.powerupDurationLevel + 1) }";
    

  }

  public void HandleUpgradeItem0UpgradeButtonPress(int buttonPressed)
	{
    switch (buttonPressed)
		{
      case 0:
        break;
      case 1:
        break;
      case 2:
        break;
      case 3:
        break;
      default:
        break;
		}
	}

public void MainMenuStartButtonTransitionOut()
  {
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    //selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();

    // start level count from 100 down to 1??
    int levelFrom100Down = 100 - GameController.Instance.currentLevelPlaying+1;
    selectedLevelText.text = "LEVEL " + levelFrom100Down.ToString();


    SetCorrectPrevNextLevelAlphaButtons();
    
    Wait(.6f, () => {
      LevelSelectButtonsPanel.SetActive(true);
    });
  }
  public void HandleMainMenuSettingsButtonPress()
  {
    
    DoLogoMoveTransitionOut();
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(MainMenuSettingsPanel);

  }
  public void HandleMainMenuUpgradesButtonPress()
  {
    
    DoLogoMoveTransitionOut();
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(MainMenuUpgradesPanel);

  }

  public void HandleLevelSelectorBackButtonPress()
	{
    TransitionHelper.TransitionOut(LevelSelectButtonsPanel);
    Wait(.3f, () => {
       TransitionHelper.TransitionIn(MainMenuButtonTransitions);
    });
  }

    void SetCorrectPrevNextLevelAlphaButtons()
	{
    if (GameController.Instance.currentLevelPlaying == 1)
    {
      SetButtonImageAlpha(prevLevelButtonImage, .3f);
    }

    if (GameController.Instance.highestLevelPlayed == 1)
    {
      SetButtonImageAlpha(nextLevelButtonImage, .3f);
    }
    if (GameController.Instance.currentLevelPlaying == GameController.Instance.highestLevelPlayed)
    {
      SetButtonImageAlpha(nextLevelButtonImage, .3f);
    }
    else
    {
      SetButtonImageAlpha(nextLevelButtonImage, 1f);
    }
    if (GameController.Instance.currentLevelPlaying == 1)
    {
      SetButtonImageAlpha(prevLevelButtonImage, .3f);
    }
    else
    {
      SetButtonImageAlpha(prevLevelButtonImage, 1f);
    }
  }

  void SetButtonImageAlpha(Image ImageButton, float newAlpha)
	{
    var tempColor = ImageButton.color;
    tempColor.a = newAlpha;
    ImageButton.color = tempColor;
  }

  public void DoLogoMoveTransitionOut()
  {
    
    logoMoveTransitionOut.Start();
  }

  public void DoLogoMoveTransitionIn()
  {

    logoMoveTransitionIn.Start();
  }

  public void MainMenuPlayButtonTransitionOut()
  {
    DoLogoMoveTransitionOut();
    TransitionHelper.TransitionOut(PlayButtonTransitions);
  }
  public void MainMenuUpgradesButtonTransitionOut()
  {
    DoLogoMoveTransitionOut();
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
  }

  public void DoLogoShine() // a separate method because calling this from the OnTransitionComplete event on the logoimage's TransitionOut component.
	{
    StartCoroutine(LogoShine());
  }

  IEnumerator LogoShine(float duration = 1f, float interval = 10f, float delay = 0f)
	{
    float elapsedTime = 0f;
    float currentVal;
    if (delay > 0f) yield return new WaitForSeconds(delay);

    var waithandle = new WaitForSeconds(interval); // caches the WaitForSeconds(interval) insetad of having this repeatedly called within the while loop? // https://forum.unity.com/threads/invokerepeating-or-coroutine.875965/
    while (true)  
    {
      elapsedTime = 0f;
      while (elapsedTime <= duration) //from normal to red
      {
        currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
        logoImageMaterial.SetFloat("_ShineLocation", currentVal);
        elapsedTime += Time.deltaTime;
        yield return new WaitForEndOfFrame();
      }
      elapsedTime = 0f;
      while (elapsedTime <= duration) //from normal to red
      {
        currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / duration));
        logoImageMaterial.SetFloat("_ShineLocation", currentVal);
        elapsedTime += Time.deltaTime;
        yield return new WaitForEndOfFrame();
      }
      yield return waithandle;
    }
  }

  public void IncrementCurrentSelectedLevel()
	{
    if((GameController.Instance.currentLevelPlaying + 1 <= GameController.Instance.highestLevelPlayed) &&(GameController.Instance.currentLevelPlaying+1 <= 100))
		{
      GameController.Instance.currentLevelPlaying++;
      //selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
      
      // start level count from 100 down to 1??
      int levelFrom100Down = 100 - GameController.Instance.currentLevelPlaying+1;
      selectedLevelText.text = "LEVEL " + levelFrom100Down.ToString();
      
      SetCorrectPrevNextLevelAlphaButtons();
    }
	}

  public void DecrementCurrentSelectedLevel()
  {
    if (GameController.Instance.currentLevelPlaying - 1 >= 1)
    {
      GameController.Instance.currentLevelPlaying--;
      //selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
      // start level count from 100 down to 1??
      int levelFrom100Down = 100 - GameController.Instance.currentLevelPlaying+1;
      selectedLevelText.text = "LEVEL " + levelFrom100Down.ToString();
      
      SetCorrectPrevNextLevelAlphaButtons();
    }
  }

  void Update()
  {

  }
}





