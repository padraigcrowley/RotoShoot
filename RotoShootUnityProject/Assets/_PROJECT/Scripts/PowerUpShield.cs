using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  private float durationSeconds;
  public float currentPlayerShipFireRateIncrease = 3.0f;
  
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;
    
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    base.PowerUpPayload();
    playerShieldPrefabInstance = SimplePool.Spawn(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity,playerShip.transform);
  }

  protected override void Start()
  {
    durationSeconds = GameplayManager.Instance.powerupDurationSeconds;
    base.Start();
  }

  protected override void Update()
  {
    if (powerUpState == PowerUpState.IsCollected)
    {
      durationSeconds -= Time.deltaTime;
      if (durationSeconds < 0)
      {
        PowerUpHasExpired();
      }
    }
    base.Update();
  }
  protected override void PowerUpHasExpired()
  {
    SimplePool.Despawn(playerShieldPrefabInstance);
    base.PowerUpHasExpired();
  }
}
