using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PowerUpSmartBomb : PowerUp
{
  private float durationSeconds;
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;

  protected override void PowerUpPayload()
  {
    //LevelManager.Instance.KillActiveEnemies();
    //above now done in TriggerPowerUpSmartBomb
    
    UIManager.Instance.ShowTriggerSmartBombButton();
    base.PowerUpPayload();
  }

  //protected override void OnEnable()
  //{
  //  base.OnEnable();
  //}

  //protected override void Update()
  //{
  //  if (powerUpState == PowerUpState.IsCollected)
  //  {
  //    durationSeconds -= Time.deltaTime;
  //    if (durationSeconds < 0)
  //    {
  //      PowerUpHasExpired();
  //    }
  //  }
  //  base.Update();
  //}
  //protected override void PowerUpHasExpired()
  //{
  //  base.PowerUpHasExpired();
  //}
}
