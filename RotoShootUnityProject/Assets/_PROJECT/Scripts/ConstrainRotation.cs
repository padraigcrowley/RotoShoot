using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// https://answers.unity.com/questions/176235/how-to-freeze-child-gameobject-from-rotation.html
/// 
/// </summary>


public class ConstrainRotation : MonoBehaviour
{

  private Quaternion iniRot;
 
 void Start()
  {
    iniRot = transform.rotation;
  }

  void LateUpdate()
  {
    transform.rotation = iniRot;
  }

}
