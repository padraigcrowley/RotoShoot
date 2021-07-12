
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;

public class MainMenuContoller : MonoBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject LogoImageTransitions;
  public GameObject logoGameObject;


  // Start is called before the first frame update
  void Start()
  {
    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(LogoImageTransitions);
  }
  public void MainMenuStartButtonTransitionOut()
  {
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    //TransitionHelper.TransitionIn(LogoImageTransitions);
  }
  public void MainMenuUpgradesButtonTransitionOut()
  {
    var logoTransitionOut = new Move(logoGameObject, endPosition:new Vector3(0,1520,0), duration:.6f, tweenType: TransitionHelper.TweenType.easeInBack, coordinateSpace: BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses.TransitionStep.CoordinateSpaceType.AnchoredPosition );
    logoTransitionOut.Start();

    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    //TransitionHelper.TransitionIn(LogoImageTransitions);
  }

  // Update is called once per frame
  void Update()
  {

  }
}





