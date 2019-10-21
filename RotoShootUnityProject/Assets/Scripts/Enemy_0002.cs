using UnityEngine;

public class Enemy_0002 : EnemyBehaviour
{
  private float knockBackAmount = 99f;


  public override void ReactToNonLethalPlayerMissileHit()
  {
    transform.localScale *= 1.1f; // scale slightly up to show they've been shot

    // Move our position a step closer to the target.
    float step = knockBackAmount * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, -step);

  }
}
