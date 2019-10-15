using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_0001 : EnemyBehaviour
{
  public override void ReactToPlayerMissileHit(GameObject missile)
  {
    transform.localScale *= 2f; // scale slightly up to show they've been shot
  }
}
