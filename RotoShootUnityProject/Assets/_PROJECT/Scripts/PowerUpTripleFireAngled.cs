using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTripleFireAngled : PowerUp
{
  
  //private static float durationSeconds = 0;
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    GameplayManager.Instance.tripleFirePowerupRemainingDuration += GameController.Instance.powerupDuration;
    
    base.PowerUpPayload();
  }
  //protected override void OnEnable()
  //{
  //  base.OnEnable();
  //}

  //protected override void Update()
  //{
  //  //if (powerUpState == PowerUpState.IsCollected)
  //  //{
  //  //  durationSeconds -= Time.deltaTime;
  //  //  //print($"In {gameObject.ToString()} DURATION  = {durationSeconds}");
  //  //  if (durationSeconds < 0)
  //  //  {
  //  //    PowerUpHasExpired();
  //  //  }
      
  //  //}
  //  base.Update();
  //}
  //protected override void PowerUpHasExpired()
  //{
  //  GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.STRAIGHT_SINGLE;
  //  base.PowerUpHasExpired();
  //}
    
}
