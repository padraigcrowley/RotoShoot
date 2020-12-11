using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpRapidFireSingle : PowerUp
{
  public float durationSeconds;
  public float currentPlayerShipFireRateIncrease = 3.0f;
  protected override void PowerUpPayload()
  {
    
    //do stuff specific to this PU//todo
    GameplayManager.Instance.currentPlayerShipFireRate /= currentPlayerShipFireRateIncrease;
    GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.RAPID_FIRE_SINGLE;
    base.PowerUpPayload();
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
    GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.STRAIGHT_SINGLE;
    GameplayManager.Instance.currentPlayerShipFireRate *= currentPlayerShipFireRateIncrease;
    base.PowerUpHasExpired();
  }
}
