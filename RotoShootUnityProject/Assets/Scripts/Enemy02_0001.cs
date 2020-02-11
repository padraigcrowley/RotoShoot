using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy02_0001 : Mr1.EnemyBehaviour02
{
  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.1f; // scale slightly up to show they've been shot
  }
}
