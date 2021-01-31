using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyMissileMovement : MissileMovement
{
  public float speed;
  public HealthBar healthBar;

  protected override void OnEnable()
  {

    base.OnEnable();
    //print("---PlayerMissilemovement OnEnable()---");
  }
  private void OnTriggerEnter(Collider co)
  {
    //print($"Collision entered with {co.gameObject.tag}");
    if ((!co.gameObject.CompareTag("PlayerMissile")) && (co.gameObject.CompareTag("Player")))
    {
      Vector3 colPos = co.gameObject.transform.position;

      if ((HitFXPrefab != null) && (!hitFXTriggered))
      {
        collided = true;
        hitFXTriggered = true;
        hitVFX = SimplePool.Spawn(HitFXPrefab, transform.position, Quaternion.identity, transform.parent);
        hitVFX.transform.forward = gameObject.transform.forward;// + offset;
        transform.localScale = new Vector3(.001f, .001f, .001f);// urgh, pretty hacky way to stop the missile projectile bullet being "drawn". Because can't SetActive(false) the missile object cos that will kill this script as well?

        foreach (GameObject childObj in projectileChildrenObjects)
        {
          if (childObj != null)
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
  void FixedUpdate()
  {

    if (!collided)
      transform.position += transform.forward * speed * Time.fixedDeltaTime;
    //transform.position += movementVector * Time.fixedDeltaTime;

  }

  void OnTriggerExit(Collider co)
  {
    if ( co.gameObject.CompareTag("BoundaryBottom") || co.gameObject.CompareTag("BoundaryRight") || co.gameObject.CompareTag("BoundaryLeft") || co.gameObject.CompareTag("BoundaryTop"))
    {
      collided = true; // let FixedUpdate know to stop moving it upwards the screen.
      transform.localScale = new Vector3(.001f, .001f, .001f);// urgh, pretty hacky way to stop the missile projectile bullet being "drawn". Because can't SetActive(false) the missile object cos that will kill this script as well?

      foreach (GameObject childObj in projectileChildrenObjects)
      {
        if (childObj != null)
          childObj.SetActive(false);
      }

      Wait(DESPAWN_DELAY_TIME, () =>
      {
        SimplePool.Despawn(muzzleVFX);
        //SimplePool.Despawn(hitVFX);
        SimplePool.Despawn(gameObject);
      });
    }
  }
}
