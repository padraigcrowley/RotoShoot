using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHealth : PowerUp
{
  private float durationSeconds;
  public float currentPlayerShipFireRateIncrease = 3.0f;
  protected override void PowerUpPayload()
  {
    
    //do stuff specific to this PU//todo
    
    base.PowerUpPayload();
  }

  protected override void Start()
  {
    
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
    
    base.PowerUpHasExpired();
  }
}
