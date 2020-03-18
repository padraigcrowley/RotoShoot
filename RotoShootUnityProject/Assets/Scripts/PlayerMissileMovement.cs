using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : ExtendedBehaviour
{
  private const float DESPAWN_DELAY_TIME = 0.3F; //not 100% sure what this should be

  private Vector3 upDirection;
  //public float speed = 5;
  public GameObject MuzzleFlashPrefab,HitFXPrefab;
  private GameObject muzzleVFX,hitVFX;
  private bool despawnTriggered = false, hitFXTriggered = false;

  private void Start()
  {
    print("---Start()---");
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
    ParticleSystem ps = this.GetComponent<ParticleSystem>();
    print($"PS= {ps}");
  }

  void OnEnable()
  {
    print("---OnEnable()---");
    //EditorApplication.isPaused = true;
    despawnTriggered = false;
    hitFXTriggered = false;

    DoMuzzleFlash();
   
  }
  private void DoMuzzleFlash()
  {
    if (MuzzleFlashPrefab != null)
    {
      muzzleVFX = SimplePool.Spawn(MuzzleFlashPrefab, transform.position, Quaternion.identity);
      muzzleVFX.transform.forward = gameObject.transform.forward;// + offset;

      //var ps = muzzleVFX.GetComponent<ParticleSystem>();
      //if (ps != null)
      //{
      //  Wait(ps.main.duration, () => {
      //    SimplePool.Despawn(muzzleVFX);
      //  });
      //}

      //else
      //{
      //  var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
      //  Wait(.3f, () => {
      //    //print($"Waited {psChild.main.duration} before Despawn");
      //    //SimplePool.Despawn(muzzleVFX);
      //  });
      //}
    }
  }

  void FixedUpdate()
  {
    this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.deltaTime;
  }

  // Update is called once per frame
  void Update()
  {
    if (transform.position.y >= 6f)
    {
      if ((HitFXPrefab != null) && (!hitFXTriggered))
      {
        hitFXTriggered = true;
        hitVFX = SimplePool.Spawn(HitFXPrefab, transform.position, Quaternion.identity);
        hitVFX.transform.forward = gameObject.transform.forward;// + offset;
      }
      
      Wait(DESPAWN_DELAY_TIME, () =>
      {
        if (!despawnTriggered)
        {
          despawnTriggered = true;
          SimplePool.Despawn(muzzleVFX);
          SimplePool.Despawn(hitVFX);
          SimplePool.Despawn(gameObject);
        }
      });
    }
  }
}
