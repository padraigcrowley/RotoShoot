using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundary : MonoBehaviour
{
  //private void OnTriggerEnter(Collider co)
  //  void OnCollisionEnter(Collision co) //this works providing neither are triggers and the missile is not kinematic -
  //private void OnTriggerEnter(Collider co)
  //{
  //  print($"Collision entered with {co.gameObject.tag}");

  //}
  private void OnTriggerEnter(Collider co)
  {
    print($"Collision entered with {co.gameObject.tag}");

  }
}
