using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy type that follows waypoint path
/// </summary>

public class Enemy02_0002 : EnemyBehaviour02
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

  public override void DoMovement(float initialSpeed)
  {
    if (!startedOnPath)
    {
      splineMoveScript.StartMove();
      startedOnPath = true;
    }
  }

  public override void StopMovement()
  {
    
    startedOnPath = false;
  }

  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.2f; // scale slightly up to show they've been shot
  }

}
