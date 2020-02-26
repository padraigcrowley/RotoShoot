using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy02_0002 : Mr1.EnemyBehaviour02
{
  public bool startedOnPath = false;
  public override void DoMovement(float initialSpeed, Mr1.FollowType followType)
  {
    if (!startedOnPath)
    {
      transform.FollowPath(wayPointPathName, initialSpeed, Mr1.FollowType.Loop).Log(true);
      startedOnPath = true;
    }
  }

  public override void StopMovement()
  {
    transform.StopFollowing();
    startedOnPath = false;
  }

  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 2f; // scale slightly up to show they've been shot
  }

}
