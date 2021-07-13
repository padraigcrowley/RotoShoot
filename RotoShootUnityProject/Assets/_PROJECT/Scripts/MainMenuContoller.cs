
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;

public class MainMenuContoller : MonoBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject PlayButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;

  public Image logoImage;
  private Material logoImageMaterial;
  

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
    
  }

  public void DoLogoTransitionOut()
  {
    var logoTransitionOut = new Move(logoGameObject, startPosition: logoGameObject.transform.localPosition , endPosition: new Vector3(0, 1520, 0), duration: .6f, delay:0f ,tweenType: TransitionHelper.TweenType.easeInBack , coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition);
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

  IEnumerator LogoShine(float duration = .5f, float interval = 5f, float delay = 0f)
	{
    float elapsedTime = 0f;
    float currentVal;
    if (delay > 0f) yield return new WaitForSeconds(delay);

    var waithandle = new WaitForSeconds(interval);
    while (true)  //https://forum.unity.com/threads/invokerepeating-or-coroutine.875965/
    {
      elapsedTime = 0f;
      while (elapsedTime <= duration) //from normal to red
      {
        //sr.material.SetFloat("_ChromAberrAmount", 0f);
        currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
        logoImageMaterial.SetFloat("_ShineLocation", currentVal);
        elapsedTime += Time.deltaTime;

        //yield return null;
        yield return new WaitForEndOfFrame();
      }
      elapsedTime = 0f;
      while (elapsedTime <= duration) //from normal to red
      {
        //sr.material.SetFloat("_ChromAberrAmount", 0f);
        currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / duration));
        logoImageMaterial.SetFloat("_ShineLocation", currentVal);
        elapsedTime += Time.deltaTime;

        //yield return null;
        yield return new WaitForEndOfFrame();
      }
    }
  }

  void Update()
  {

  }
}





