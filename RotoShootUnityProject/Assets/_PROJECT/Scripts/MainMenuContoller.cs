
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;

public class MainMenuContoller : MonoBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject PlayButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;


  void Start()
  {
    //See the "performance" section from: https://www.textanimator.febucci.com/docs/troubleshooting/#editor
    Febucci.UI.Core.TAnimBuilder.InitializeGlobalDatabase();

    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
		TransitionHelper.TransitionIn(LogoImageTransitions);
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

  void Update()
  {

  }
}





