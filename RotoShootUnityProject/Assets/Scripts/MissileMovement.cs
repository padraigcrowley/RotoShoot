using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMovement : ExtendedBehaviour
{
  protected const float DESPAWN_DELAY_TIME = 0.5F; //not 100% sure what this should be

  protected Vector3 upDirection;
  //public float speed = 5;
  public GameObject MuzzleFlashPrefab, HitFXPrefab;
  protected  GameObject muzzleVFX;
  protected GameObject hitVFX;
  private bool despawnTriggered = false;
  protected bool hitFXTriggered = false;
  private bool readyToDespawn = false;
  private ParticleSystem ps;
  protected bool collided;
  private GameObject trailObj;
  protected List<GameObject> projectileChildrenObjects = new List<GameObject>();

  virtual protected void Start()
    {
    //print("---Missilemovement Start()---");

    //manually have to turn the child objects off/on after collisions coz otherwise e.g. the trail's quick position change makes it glitch
    foreach (Transform childTransform in this.transform)
    {     
      if (childTransform != null)
      {
        projectileChildrenObjects.Add(childTransform.gameObject);     
      }
    }
  }

  virtual protected void OnEnable()
  {
    //print("---OnEnable()---");
    //EditorApplication.isPaused = true;
    despawnTriggered = false;
    hitFXTriggered = false;
    readyToDespawn = false;
    collided = false;
    transform.localScale = new Vector3(1f, 1f, 1f);
    foreach (GameObject childObj in projectileChildrenObjects)
    {
      if(childObj!=null)
      childObj.SetActive(true);
    }
    //if (trailObj != null)
     // trailObj.SetActive(true);
    DoMuzzleFlash();

  }

  protected  virtual void FixedUpdate()
  {
    
  }

  // Update is called once per frame
  void Update()
    {
        
    }


  

  private void OnTriggerExit2D(Collider2D co)
  {
    //print($"Collision exited with {co.gameObject.tag}");
    if (co.gameObject.CompareTag("Boundary"))
    {
      foreach (GameObject childObj in projectileChildrenObjects)
      {
        if (childObj != null)
          childObj.SetActive(false);
      }
      //trailObj.SetActive(false);

      Wait(DESPAWN_DELAY_TIME, () =>
      {
        SimplePool.Despawn(muzzleVFX);
        //SimplePool.Despawn(hitVFX);
        SimplePool.Despawn(gameObject);
      });
    }
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
}
