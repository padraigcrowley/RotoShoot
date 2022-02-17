using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPowerUpSmartBomb : MonoBehaviour
{
  public void DoTriggerPowerUpSmartBomb()
  {
    LevelManager.Instance.KillActiveEnemies();
    UIManager.Instance.HideTriggerSmartBombButton();
  }
}
