using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : ExtendedBehaviour
{
  private const float DESPAWN_DELAY_TIME = 0.5F; //not 100% sure what this should be

  private Vector3 upDirection;
  //public float speed = 5;
  public GameObject MuzzleFlashPrefab,HitFXPrefab;
  private GameObject muzzleVFX,hitVFX;
  private bool despawnTriggered = false, hitFXTriggered = false;
  private bool readyToDespawn = false;
  private ParticleSystem ps;
  private bool collided;
  private ParticleSystem[] psChildren;


  private void Start()
  {
    print("---Start()---");
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
    //ps = this.GetComponent<ParticleSystem>();
    //psChildren = transform.GetComponentsInChildren<ParticleSystem>();
    //print($"PS= {ps}");
    
  }

  void OnEnable()
  {
    print("---OnEnable()---");
    //EditorApplication.isPaused = true;
    despawnTriggered = false;
    hitFXTriggered = false;
    readyToDespawn = false;
    collided = false;
    transform.localScale = new Vector3(1f, 1f, 1f);

    DoMuzzleFlash();
   
  }
  
  void FixedUpdate()
  {
    if (!collided)
      this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.deltaTime;
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

  private void OnTriggerEnter2D(Collider2D co)
  {
    if (co.gameObject.tag != "Boundary" && co.gameObject.tag != "EnemyMissile")
    {
      Vector3 colPos = co.gameObject.transform.position;

      if ((HitFXPrefab != null) && (!hitFXTriggered))
      {
        collided = true;
        hitFXTriggered = true;
        hitVFX = SimplePool.Spawn(HitFXPrefab, transform.position, Quaternion.identity);
        hitVFX.transform.forward = gameObject.transform.forward;// + offset;
        transform.localScale = new Vector3(.001f, .001f, .001f);// urgh, pretty hacky way to stop the missile projectile bullet being "drawn". 
        //foreach (ParticleSystem psChild in psChildren)
        //  psChild.Stop();
      }

      Wait(DESPAWN_DELAY_TIME, () =>
      {
        SimplePool.Despawn(muzzleVFX);
        SimplePool.Despawn(hitVFX);
        SimplePool.Despawn(gameObject);
      });
    }

    if (co.gameObject.tag == "Boundary")
    {
      Wait(DESPAWN_DELAY_TIME, () =>
      {
        SimplePool.Despawn(muzzleVFX);
        //SimplePool.Despawn(hitVFX);
        SimplePool.Despawn(gameObject);
      });
    }

  }
}
