using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy02_0002 : Mr1.EnemyBehaviour02
{

  public override void DoMovement(float initialSpeed, Mr1.FollowType followType)
  {
    transform.FollowPath(wayPointPathName, initialSpeed, Mr1.FollowType.Loop).Log(true);
  }

  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 2f; // scale slightly up to show they've been shot
  }


}
