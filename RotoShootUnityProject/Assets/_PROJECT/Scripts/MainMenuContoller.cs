
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeautifulTransitions.Scripts.Transitions;

public class MainMenuContoller : MonoBehaviour
{
  public GameObject MainMenuButtonTransitions;
  public GameObject LogoImageTransitions;


  // Start is called before the first frame update
  void Start()
  {
    TransitionHelper.TransitionIn(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(LogoImageTransitions);
  }
  public void MainMenuStartButtonTransitionOut()
  {
    TransitionHelper.TransitionOut(MainMenuButtonTransitions);
    TransitionHelper.TransitionIn(LogoImageTransitions);
  }

  // Update is called once per frame
  void Update()
  {

  }
}





