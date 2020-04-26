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
    //this.transform.position += upDirection * 10 * Time.fixedDeltaTime;
  }
  private void OnTriggerEnter2D(Collider2D co)
  {
    print($"Collision entered with {co.gameObject.tag}");
    if ((!co.gameObject.CompareTag("EnemyMissile")) && (co.gameObject.CompareTag("Enemy01")))
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
}
