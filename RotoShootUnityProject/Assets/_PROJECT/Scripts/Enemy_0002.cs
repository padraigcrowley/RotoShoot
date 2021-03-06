﻿using UnityEngine;

public class Enemy_0002 : EnemyBehaviour
{
  private float knockBackAmount = 50f;
  private float thisrespawnWaitDelay = 4.0f;

  override public float GetRespawnWaitDelay()
  {
    return thisrespawnWaitDelay;
  }

  override public void ReactToNonLethalPlayerMissileHit()
  {
     // knock the enemy back
    float step = knockBackAmount * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, -step);
  }
}