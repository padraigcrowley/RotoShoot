using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTripleFireAngled : PowerUp
{
  
  private float durationSeconds;
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    
    GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.ANGLED_TRIPLE;
    base.PowerUpPayload();
  }



  protected override void OnEnable()
  {
    durationSeconds = GameplayManager.Instance.powerupDurationSeconds;
    base.OnEnable();
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
    base.PowerUpHasExpired();
  }
    
}
