using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_0002 : EnemyBehaviour
{
  private float knockBackAmount = 50f;

  public override void ReactToPlayerMissileHit(GameObject missile)
  {
    transform.localScale *= 2f; // scale slightly up to show they've been shot

    // Move our position a step closer to the target.
    float step = knockBackAmount * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, -step);
  }
}
