using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenRotArrow : MonoBehaviour
{
  void Start()
  {
    transform.position = GameplayManager.Instance.playerShipPos;
    //print("PlayerShipRedGreenArrowObj.transform.position " + transform.position);

  }

  void Update()
  {

  }
}
