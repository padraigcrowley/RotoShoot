using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenRotArrow : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    transform.position = GameplayManager.Instance.playerShipPos;
    //print("PlayerShipRedGreenArrowObj.transform.position " + transform.position);

  }

  // Update is called once per frame
  void Update()
  {

  }
}
