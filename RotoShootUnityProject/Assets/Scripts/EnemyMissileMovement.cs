using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyMissileMovement : MissileMovement
{
  
  private Vector3 destinationPos;
  private Vector3 movementVector = Vector2.zero;
  public float speed=10;

  protected override void Start()
  {
    movementVector = (GameplayManager.Instance.playerShipPos - transform.position).normalized * speed;
    base.Start();
    print("---PlayerMissilemovement Start()---");
  }
  
  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    if (!collided)
      transform.position += movementVector * Time.fixedDeltaTime;
  }
}
