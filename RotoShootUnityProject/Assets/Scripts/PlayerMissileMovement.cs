using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : MissileMovement
{
    protected override void Start()
  {
    //upDirection = GameObject.FindGameObjectWithTag("PlayerShipFrontTurret").transform.up;
    upDirection = this.transform.up;
    base.Start();
    //print("---PlayerMissilemovement Start()---");
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    //upDirection = GameObject.FindGameObjectWithTag("PlayerShipFrontTurret").transform.up;
    upDirection = this.transform.up;
  }

   void FixedUpdate()
  {
    if (!collided)
      //this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.fixedDeltaTime;
      //this.transform.position += upDirection * 10.0f * Time.fixedDeltaTime;
      this.transform.position += this.transform.forward * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.fixedDeltaTime;
    //print($"this.transform.position {this.transform.gameObject.name } = {this.transform.position}");
      
  }
  private void OnTriggerEnter(Collider co)
  {
    //print($"Collision entered with {co.gameObject.tag}");
    if ((!co.gameObject.CompareTag("EnemyMissile")) && ((co.gameObject.CompareTag("Enemy01")) || (co.gameObject.CompareTag("BossInvulnerable")) || (co.gameObject.CompareTag("BossVulnerable"))))
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
