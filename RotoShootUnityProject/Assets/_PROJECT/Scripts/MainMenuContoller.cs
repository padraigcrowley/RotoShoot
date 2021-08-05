
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
  public GameObject LevelSelectButtonsPanel;


  public Image prevLevelButtonImage;
  public Image nextLevelButtonImage;
  public Image logoImage;
  private Material logoImageMaterial;
  public TMP_Text selectedLevelText;
  [SerializeField]
  private Move logoMoveTransitionOut;
  private Move logoMoveTransitionIn;

  void Start()
  {
    //See the "performance" section from: https://www.textanimator.febucci.com/docs/troubleshooting/#editor
    Febucci.UI.Core.TAnimBuilder.InitializeGlobalDatabase();

    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
		TransitionHelper.TransitionIn(LogoImageTransitions);
    logoImageMaterial = logoImage.material;

    logoMoveTransitionOut = new Move(logoGameObject, startPosition: logoGameObject.transform.localPosition, endPosition: new Vector3(0, 1620, 0), duration: .3f, delay: 0f, tweenType: TransitionHelper.TweenType.easeInBack, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    
      logoMoveTransitionIn = new Move(logoGameObject, startPosition: new Vector3(0, 1620, 0), endPosition: new Vector3(0, 281, 0), duration: .3f, delay: 0f, tweenType: TransitionHelper.TweenType.spring, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
  }
  public void MainMenuStartButtonTransitionOut()
  {
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
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
      selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
      SetCorrectPrevNextLevelAlphaButtons();
    }
	}

  public void DecrementCurrentSelectedLevel()
  {
    if (GameController.Instance.currentLevelPlaying - 1 >= 1)
    {
      GameController.Instance.currentLevelPlaying--;
      selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
      SetCorrectPrevNextLevelAlphaButtons();
    }
  }

  void Update()
  {

  }
}





