using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  private float durationSeconds;
  public float currentPlayerShipFireRateIncrease = 3.0f;
  
  public GameObject playerShip;
    
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    base.PowerUpPayload();
  }

  protected override void Start()
  {
    durationSeconds = GameplayManager.Instance.powerupDurationSeconds;
    playerShip = GameObject.FindWithTag("Player");
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
    //playerShield.SetActive(false);
    base.PowerUpHasExpired();
  }
}
