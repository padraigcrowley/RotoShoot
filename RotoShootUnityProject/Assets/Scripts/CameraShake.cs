using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
  private Camera myCamera;
  private void Awake()
  {
    myCamera = FindObjectOfType<Camera>();
  }

  public void CameraShakeOnPlayerHit()
  {
    Vector3 shakeVector = new Vector3(0.2f, 0.2f, 0);
    myCamera.transform.DOShakePosition(.5f, shakeVector, 30, 10f, false, true);
  }
}
