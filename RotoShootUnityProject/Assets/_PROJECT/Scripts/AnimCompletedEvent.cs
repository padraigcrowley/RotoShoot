using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCompletedEvent : MonoBehaviour
{

  public GameObject PlayerShip;

  public void PlayerShipIntroAnimCompletedEvent()
  {
    //print("Player Intro AnimCompleteEventTriggered!");
    PlayerShip.GetComponent<PlayerShip>().PlayerShipIntroAnimCompleted = true;
    //PlayerShipGreenRotArrowObj.SetActive(true);
  }

  public void PlayerShipOutroAnimCompletedEvent()
  {
    //print("Player Intro AnimCompleteEventTriggered!");
    PlayerShip.GetComponent<PlayerShip>().PlayerShipOutroAnimCompleted = true;
    //PlayerShipGreenRotArrowObj.SetActive(true);
  }
}
