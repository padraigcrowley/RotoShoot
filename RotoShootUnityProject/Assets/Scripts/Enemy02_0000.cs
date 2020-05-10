using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy type that flys into centre of screen (or wherever playership is), towards player ship
/// </summary>

public class Enemy02_0000 : Mr1.EnemyBehaviour02
{
  protected override void Start()
  {
    base.Start();
    Vector3 dir = GameplayManager.Instance.playerShipPos - transform.position;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

  }
  public override void DoMovement(float initialSpeed, Mr1.FollowType followType)
  {
    
    float step = speed * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, step);

  }

  public override void StopMovement()
  {
    return;
  }

  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.1f; // scale slightly up to show they've been shot
  }
}
