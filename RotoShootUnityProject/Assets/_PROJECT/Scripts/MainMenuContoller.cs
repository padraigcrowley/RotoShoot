
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.Components;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using TMPro;
using DarkTonic.MasterAudio;

public class MainMenuContoller : ExtendedBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject PlayButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;
  public GameObject MainMenuSettingsPanel;
  public GameObject MainMenuUpgradesPanel;
  public GameObject LevelSelectButtonsPanel;
  public GameObject GetMoreCoinsPanel;

  public TMP_Text iap0199PriceText;
  public TMP_Text iap0899PriceText;
  public TMP_Text iap4999PriceText;

  public enum ShopItems { MISSILE_POWER, SHIP_DEFENCES , SHIELD_DURATION, POWERUP_DURATION}

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
  public TMP_Text starCoinCountText;
  public TMP_Text getMoreCoinsPanelStarCoinCountText;

  public Slider musicSlider, soundsSlider;

  void Start()
  {
    musicSlider.value = GameController.Instance.musicVolume;
    soundsSlider.value = GameController.Instance.soundsVolume;
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
    UpgradesItem0LevelText.text = $"{GameController.Instance.playerMissileDamageLevel}";
    UpgradesItem1LevelText.text = $"{GameController.Instance.maxPlayerHPLevel}";
    UpgradesItem2LevelText.text = $"{GameController.Instance.shieldDurationLevel}";
    UpgradesItem3LevelText.text = $"{GameController.Instance.powerupDurationLevel}";

    
    UpgradesItem0UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.playerMissileDamageLevel + 1) }";
    UpgradesItem1UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.maxPlayerHPLevel+ 1) }";
    UpgradesItem2UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.shieldDurationLevel + 1) }";
    UpgradesItem3UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.powerupDurationLevel + 1) }";
    
    starCoinCountText.text = (""+GameController.Instance.starCoinCount);
    getMoreCoinsPanelStarCoinCountText.text = ("" + GameController.Instance.starCoinCount);

    //TODO - HANDLE WHAT TO DO/DISPLAY WHEN ITEM IS MAX UPGRADED

  }

  public void HandleUpgradeItemUpgradeButtonPress(int buttonPressed)
	{

    //TODO - HANDLE WHAT TO DO/DISPLAY WHEN ITEM IS MAX UPGRADED

    switch (buttonPressed)
		{
      case 0:
        //TODO - HANDLE WHAT TO DO/DISPLAY WHEN ITEM IS MAX UPGRADED
        print("Upgrade Item0 playerMissileDamage pressed");
        TryToUpgrade(ref  GameController.Instance.playerMissileDamageLevel, ref  GameController.Instance.playerMissileDamage, "playerMissileDamage", "playerMissileDamageLevel", buttonPressed);
        
        break;
        
      case 1:
        print("Upgrade Item1 maxPlayerHPLevel pressed");
        TryToUpgrade(ref GameController.Instance.maxPlayerHPLevel, ref GameController.Instance.maxPlayerHP, "maxPlayerHP", "maxPlayerHPLevel", buttonPressed);

        break;
      
      case 2:
        print("Upgrade Item2 shieldDurationLevel pressed");
        TryToUpgrade(ref GameController.Instance.shieldDurationLevel, ref GameController.Instance.shieldDuration, "shieldDuration", "shieldDurationLevel", buttonPressed);
        break;
      case 3:
        print("Upgrade Item3 powerupDurationLevel pressed");
        TryToUpgrade(ref GameController.Instance.powerupDurationLevel, ref GameController.Instance.powerupDuration, "powerupDuration", "powerupDurationLevel", buttonPressed);
        break;
      default:
        break;
		}
    
	}
  public void TryToUpgrade(ref int itemLevel, ref float ItemValue, string itemNameString, string itemLevelString, int itemMenuNum)
  {
    if (GameController.Instance.starCoinCount >= GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", itemLevel + 1))
    {
      itemLevel++;
      GameController.Instance.starCoinCount -= (int)GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", itemLevel);
      ItemValue = (int)GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, itemNameString, itemLevel);
      
      //UpdateUpgradesMenuStatsText();
      switch (itemMenuNum)
      {
        case 0:
          UpgradesItem0LevelText.text = $"{GameController.Instance.playerMissileDamageLevel}";
          UpgradesItem0UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.playerMissileDamageLevel + 1) }";
          break;
        case 1:
          UpgradesItem1LevelText.text = $"{GameController.Instance.maxPlayerHPLevel}";
          UpgradesItem1UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.maxPlayerHPLevel + 1) }";
          break;
        case 2:
          UpgradesItem2LevelText.text = $"{GameController.Instance.shieldDurationLevel}";
          UpgradesItem2UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.shieldDurationLevel + 1) }";
          break;
        case 3:
          UpgradesItem3LevelText.text = $"{GameController.Instance.powerupDurationLevel}";
          UpgradesItem3UpgradeCostText.text = $"{ GameController.Instance.GetSheetStatValue(GameController.Instance.playerStatsSpreadsheet, "starCoinCost", GameController.Instance.powerupDurationLevel + 1) }";
          break;
        default:
          break;
      }
      starCoinCountText.text = ("" + GameController.Instance.starCoinCount);
      getMoreCoinsPanelStarCoinCountText.text = ("" + GameController.Instance.starCoinCount);

      ES3.Save("starCoinCount", GameController.Instance.starCoinCount);
      ES3.Save(itemLevelString, itemLevel);
    }
    else
    {
      HandleGetMoreCoinsInitiate();
    }
  }

  public void HandleGetMoreCoinsInitiate()
	{
    iap0199PriceText.text = AdsIAPController.Instance.iapsLocalizedPrices["iap_coins_0199"];
    iap0899PriceText.text = AdsIAPController.Instance.iapsLocalizedPrices["iap_coins_0899"];
    iap4999PriceText.text = AdsIAPController.Instance.iapsLocalizedPrices["iap_coins_4999"];

    var moveTransitionOut = new Move (MainMenuUpgradesPanel, startPosition: MainMenuUpgradesPanel.transform.localPosition, endPosition: new Vector3(-1200, 0, 0), duration: .5f, delay: 0f, tweenType: TransitionHelper.TweenType.easeInBack, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    moveTransitionOut.Start();

    var moveTransitionIn = new Move(GetMoreCoinsPanel, startPosition: GetMoreCoinsPanel.transform.localPosition, endPosition: new Vector3(0, 0, 0), duration: .5f, delay: .5f, tweenType: TransitionHelper.TweenType.spring, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    moveTransitionIn.Start();
  }

  public void HandleGetMoreCoinsCloseButtonPress()
  {
    
    var moveTransitionIn = new Move(MainMenuUpgradesPanel, startPosition: MainMenuUpgradesPanel.transform.localPosition, endPosition: new Vector3(0, 0, 0), duration: .5f, delay: .5f, tweenType: TransitionHelper.TweenType.spring, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    moveTransitionIn.Start();
    
    var moveTransitionOut = new Move(GetMoreCoinsPanel, startPosition: GetMoreCoinsPanel.transform.localPosition, endPosition: new Vector3(1200, 0, 0), duration: .5f, delay: 0f, tweenType: TransitionHelper.TweenType.easeInBack, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    moveTransitionOut.Start();


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
    UpdateUpgradesMenuStatsText();
    DoLogoMoveTransitionOut();
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(MainMenuUpgradesPanel);

  }


  public void HandleUpgradeMenuCloseButtonPress()
	{

    var trans = MainMenuUpgradesPanel.GetComponent<TransitionBase>();
    trans.TransitionOutConfig.OnTransitionComplete.AddListener(FrontEndTransitioIn);

    //do the transition out
    trans.TransitionOut();

    //oncomplete:
    
  }

  public void HandleSettingsResetProgressYesButtonPress()
  {
    GameController.Instance.starCoinCount = 0;
    
    GameController.Instance.highestLevelPlayed = 1;
    GameController.Instance.currentLevelPlaying = 1;

		GameController.Instance.playerMissileDamageLevel = 1;
		GameController.Instance.maxPlayerHPLevel = 1;
		GameController.Instance.shieldDurationLevel = 1;
		GameController.Instance.powerupDurationLevel = 1;

    ES3.Save("starCoinCount", GameController.Instance.starCoinCount);
    ES3.Save("highestLevelPlayed", GameController.Instance.highestLevelPlayed);
    ES3.Save("playerMissileDamageLevel", GameController.Instance.playerMissileDamageLevel);
    ES3.Save("maxPlayerHPLevel", GameController.Instance.maxPlayerHPLevel);
    ES3.Save("shieldDurationLevel", GameController.Instance.shieldDurationLevel);
    ES3.Save("powerupDurationLevel", GameController.Instance.powerupDurationLevel);

    UpdateUpgradesMenuStatsText();
    SetCorrectPrevNextLevelAlphaButtons();
  }


  void FrontEndTransitioIn(object parameter)
  {
    
    DoLogoMoveTransitionIn();
    //Do MainManuButtonsPanel -> MoveTraget.TransitionIn
    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
    
    Debug.Log("Complete with parameter: " + parameter);
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
      SetButtonImageAlpha(prevLevelButtonImage, 0f);
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

  public void HandleMusicSliderChange()
  {
    print($"Music slider changed. Value now: {musicSlider.value} Converted: {Mathf.Log10(musicSlider.value) * 20}");
    MasterAudio.PlaylistMasterVolume = musicSlider.value;
    ES3.Save("musicVolume", musicSlider.value);
    //MasterAudio.PlaylistMasterVolume = Mathf.Log10(musicSlider.value) * 20; //no logarethmic conversion needed, handled by MasterAudio
  }
  public void HandleSoundsSliderChange()
  {
    print($"Sound slider changed. Value now: {soundsSlider.value} Converted: {Mathf.Log10(soundsSlider.value) * 20}");
    MasterAudio.MasterVolumeLevel = soundsSlider.value;
    ES3.Save("soundsVolume", soundsSlider.value);
    //MasterAudio.MasterVolumeLevel = Mathf.Log10(soundsSlider.value) * 20; //no logarethmic conversion needed, handled by MasterAudio
  }

  void Update()
  {

  }
}





