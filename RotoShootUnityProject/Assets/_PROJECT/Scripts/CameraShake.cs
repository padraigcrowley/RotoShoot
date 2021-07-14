using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
  private Camera myCamera;
  private static bool isShaking = false;
 
  private void Awake()
  {
    myCamera = FindObjectOfType<Camera>();
    isShaking = false;
  }

  public void CameraShakeOnPlayerHit()
  {
    if (!isShaking)
    {
      isShaking = true;
      Vector3 shakeVector = new Vector3(0.4f, 0.4f, 0);
      myCamera.transform.DOShakePosition(.5f, shakeVector, 30, 0f, false, true).OnComplete(finishedShaking);
    }
    //else
      //print("Soz, still shaking");
  }

  private void finishedShaking()
  {
    isShaking = false;
  }


}
