using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy type that starts at top of screen, flys straight down lane to bottom of screen.
/// </summary>

public class Enemy02_0001 : Mr1.EnemyBehaviour02
{

  public override void DoMovement(float initialSpeed, Mr1.FollowType followType)
  {
    float step = initialSpeed * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y-20f), step);
    
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
