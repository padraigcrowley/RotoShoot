using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PowerUpSmartBomb : PowerUp
{
  private float durationSeconds;
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;

  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    //GameController.Instance.starCoinCount++;
    //UIManager.Instance.starCoinCountText.text = GameController.Instance.starCoinCount.ToString();
    LevelManager.Instance.KillActiveEnemies();
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
