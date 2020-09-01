using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyMissileMovement : MissileMovement
{
  
  private Vector3 destinationPos;
  private Vector3 movementVector = Vector2.zero;
  public float speed;

  protected override void OnEnable()
  {
    movementVector = (GameplayManager.Instance.playerShipPos - transform.position).normalized * speed;
    base.OnEnable();
    //print("---PlayerMissilemovement OnEnable()---");
  }
  private void OnTriggerEnter2D(Collider2D co)
  {
    //print($"Collision entered with {co.gameObject.tag}");
    if ((!co.gameObject.CompareTag("PlayerMissile")) && (co.gameObject.CompareTag("Player")))
    {
      Vector3 colPos = co.gameObject.transform.position;

      if ((HitFXPrefab != null) && (!hitFXTriggered))
      {
        collided = true;
        hitFXTriggered = true;
        hitVFX = SimplePool.Spawn(HitFXPrefab, transform.position, Quaternion.identity);
        hitVFX.transform.forward = gameObject.transform.forward;// + offset;
        transform.localScale = new Vector3(.001f, .001f, .001f);// urgh, pretty hacky way to stop the missile projectile bullet being "drawn". Because can't SetActive(false) the missile object cos that will kill this script as well?

        foreach (GameObject childObj in projectileChildrenObjects)
        {
          if(childObj!=null)
            childObj.SetActive(false);
        }
        //trailObj.SetActive(false);
      }

      Wait(DESPAWN_DELAY_TIME, () =>
      {
        SimplePool.Despawn(muzzleVFX);
        SimplePool.Despawn(hitVFX);
        SimplePool.Despawn(gameObject);
      });
    }
  }
  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    if (!collided)
      transform.position += movementVector * Time.fixedDeltaTime;
  }
}
