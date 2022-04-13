using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPowerUpSmartBomb : MonoBehaviour
{
  public CameraFlashDamage cameraFlashDamage;

  public void DoTriggerPowerUpSmartBomb()
  {
    LevelManager.Instance.KillActiveEnemies();
    UIManager.Instance.HideTriggerSmartBombButton();
    cameraFlashDamage.doFlashAnim();
  }
}
