using UnityEngine;
using System.Collections;
using System;

public class ExtendedBehaviour : MonoBehaviour
//used in EnemyBehaviour.cs - See: https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html

{

  /*
   USEAGE:
    Wait(5, () => {
    Debug.Log("5 seconds is lost forever");
  });
  */
    

  public void Wait(float seconds, Action action)
  {
    StartCoroutine(_wait(seconds, action));
  }
  IEnumerator _wait(float time, Action callback)
  {
    yield return new WaitForSeconds(time);
    callback();
  }


  

}