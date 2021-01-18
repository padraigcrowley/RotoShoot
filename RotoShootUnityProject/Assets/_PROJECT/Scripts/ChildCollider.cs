using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildCollider : MonoBehaviour
{
  public BossBehaviour01 parentScript;
  private void Start()
  {
    parentScript = GetComponentInParent<BossBehaviour01>();
  }
  void OnTriggerEnter(Collider other)
  {
    //transform.parent.gameObject.GetComponent<BossBehaviour01>().OnChildTriggerEntered(other, transform.position);
    parentScript.OnChildTriggerEntered(other, transform.gameObject.tag);
  }
  private void onEggRaised()
  {
    parentScript.eggIsMoving = false;
  }

  private void onEggLowered()
  {
    parentScript.eggIsMoving = false;
  }
}