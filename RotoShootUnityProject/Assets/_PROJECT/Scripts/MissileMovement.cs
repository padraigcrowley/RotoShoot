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
  
  protected List<GameObject> projectileChildrenObjects = new List<GameObject>();

  protected Component[] trailRenderers;
  private TrailRenderer trailRenderer;
  protected   BoxCollider colliderComponent;

  private void Awake()
  {
    trailRenderers = GetComponentsInChildren<TrailRenderer>();
    colliderComponent = GetComponent<BoxCollider>();

    
  }

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
    despawnTriggered = false;
    hitFXTriggered = false;
    readyToDespawn = false;
    collided = false;
    if (colliderComponent != null)
      colliderComponent.enabled = true;

    if (trailRenderers != null)
    {
      foreach (TrailRenderer trailRenderer in trailRenderers)
      {
        if (trailRenderer != null)
          trailRenderer.Clear();
      }
    }

    transform.localScale = new Vector3(1f, 1f, 1f);
    foreach (GameObject childObj in projectileChildrenObjects)
    {
      if(childObj!=null)
      childObj.SetActive(true);
    }
    DoMuzzleFlash();
  }

  
  private void DoMuzzleFlash()
  {
    if (MuzzleFlashPrefab != null)
    {
      muzzleVFX = SimplePool.Spawn(MuzzleFlashPrefab, transform.position, Quaternion.identity, transform.parent);
      //muzzleVFX.transform.forward = gameObject.transform.forward;// + offset;
    }
  }
}
