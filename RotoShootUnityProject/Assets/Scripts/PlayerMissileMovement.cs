using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : MissileMovement
{
    protected override void Start()
  {
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up; 
    base.Start();
    print("---PlayerMissilemovement Start()---");
  }
  
   protected override void FixedUpdate()
  {
    base.FixedUpdate();
    if (!collided)
      this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.fixedDeltaTime;
  }
}
