
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using TMPro;

public class MainMenuContoller : MonoBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject PlayButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;

  public Image prevLevelButtonImage;
  public Image nextLevelButtonImage;
  public Image logoImage;
  private Material logoImageMaterial;
  public TMP_Text selectedLevelText;

  void Start()
  {
    //See the "performance" section from: https://www.textanimator.febucci.com/docs/troubleshooting/#editor
    Febucci.UI.Core.TAnimBuilder.InitializeGlobalDatabase();

    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
		TransitionHelper.TransitionIn(LogoImageTransitions);
    logoImageMaterial = logoImage.material;
  }
  public void MainMenuStartButtonTransitionOut()
  {
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    selectedLevelText.text = "LEVEL " + GameController.Instance.currentLevelPlaying.ToString();
    SetCorrectPrevNextLevelAlphaButtons();
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

  public void DoLogoTransitionOut()
  {
    var logoTransitionOut = new Move(logoGameObject, startPosition: logoGameObject.transform.localPosition , endPosition: new Vector3(0, 1620, 0), duration: .6f, delay:0f ,tweenType: TransitionHelper.TweenType.easeInBack , coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
    logoTransitionOut.Start();
  }

  public void MainMenuPlayButtonTransitionOut()
  {
    DoLogoTransitionOut();
    TransitionHelper.TransitionOut(PlayButtonTransitions);
  }
  public void MainMenuUpgradesButtonTransitionOut()
  {
    DoLogoTransitionOut();
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





