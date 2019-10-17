using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_0001 : EnemyBehaviour
{
  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.1f; // scale slightly up to show they've been shot
  }
}
