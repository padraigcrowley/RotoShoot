using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy type that starts at top of screen, flys straight down lane to bottom of screen.
/// </summary>

public class Enemy02_0001 : EnemyBehaviour02
{
  protected override void Start()
  {
    base.Start();
    var HPMultiplier = 4.0f; //this particular enemy should be 4 times tougher than normal ones
    initialHP *= HPMultiplier;
    hp = initialHP;
  }
  public override void DoMovement()
  {
    float step = speed * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y-20f), step);
    
  }

  public override void StopMovement()
  {
    return;
  }

  //public override void ReactToPlayerMissileHit()
  //{
  //  transform.localScale *= 1.1f; // scale slightly up to show they've been shot
  //}
}
