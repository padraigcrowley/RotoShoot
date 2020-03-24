using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : MissileMovement
{
  


  private void Start()
  {
   
  }
  void OnEnable()
  {
    
   
  }
  
  void FixedUpdate()
  {
    
  }



  // Update is called once per frame
  //void Update()
  //{
  //  if (transform.position.y >= 6f)
  //  {
  //    if ((HitFXPrefab != null) && (!hitFXTriggered))
  //    {
  //      collided = true;
  //      hitFXTriggered = true;
  //      hitVFX = SimplePool.Spawn(HitFXPrefab, transform.position, Quaternion.identity);
  //      hitVFX.transform.forward = gameObject.transform.forward;// + offset;
  //      transform.localScale = new Vector3(.001f, .001f, .001f);// urgh, pretty hacky way to stop the missile projectile bullet being "drawn". 
  //    }

  //    Wait(DESPAWN_DELAY_TIME, () =>
  //    {
  //      if (!despawnTriggered)
  //      {
  //        despawnTriggered = true;
  //        SimplePool.Despawn(muzzleVFX);
  //        SimplePool.Despawn(hitVFX);
  //        SimplePool.Despawn(gameObject);
  //      }
  //    });
  //  }
  //}

  
}
