﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHealth : PowerUp
{
  private float durationSeconds;
  public int HPincrease = 25;
  protected override void PowerUpPayload()
  {

    //do stuff specific to this PU//todo
    GameplayManager.Instance.currentPlayerHP += HPincrease;
    UltimateStatusBar.UpdateStatus("playerStatusBar", GameplayManager.Instance.currentPlayerHP, GameplayManager.Instance.MAX_PLAYER_HP);
    UltimateStatusBar.UpdateStatus("PlayerHPWorldSpaceStatusBar", GameplayManager.Instance.currentPlayerHP, GameplayManager.Instance.MAX_PLAYER_HP);

    base.PowerUpPayload();
  }

  protected override void Start()
  {

    base.Start();
  }

  protected override void Update()
  {
    //if (powerUpState == PowerUpState.IsCollected)
    //{
    //  durationSeconds -= Time.deltaTime;
    //  if (durationSeconds < 0)
    //  {
    //    PowerUpHasExpired();
    //  }
    //}
    base.Update();
  }
  protected override void PowerUpHasExpired()
  {

    base.PowerUpHasExpired();
  }
}
