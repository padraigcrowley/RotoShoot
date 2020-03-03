using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy02_0002 : Mr1.EnemyBehaviour02
{
  public bool startedOnPath = false;
  [SerializeField] private GameObject enemyMissile;


  protected override void Start()
  {
    base.Start();
    //Debug.Log("Enemy02_0002 START method");
    //InvokeRepeating("FireMissileAtPlayerPos", 3, 5);
  }
  protected override  void Update()
  {
    base.Update();
    //Debug.Log("Enemy02_0002 update method");
  }

  public override void DoMovement(float initialSpeed, Mr1.FollowType followType)
  {
    if (!startedOnPath)
    {
      transform.FollowPath(wayPointPathName, initialSpeed, Mr1.FollowType.Loop).Log(true);
      startedOnPath = true;
    }
  }

  //private void FireMissileAtPlayerPos()
  //{
  //  GameObject firedBullet = Instantiate(enemyMissile, transform.position, transform.rotation);
  //  Debug.Log("FireMissileAtPlayerPos()");
  //}

  public override void StopMovement()
  {
    transform.StopFollowing();
    startedOnPath = false;
  }

  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.2f; // scale slightly up to show they've been shot
  }

}
