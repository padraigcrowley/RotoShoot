﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS; //simple waypoints

public class SpinningMineBehaviour : ExtendedBehaviour
{

  enum EnemyState { WAITING_TO_SPAWN, SPAWNING, SPINNING_IN_STARTED, SPINNING_IN_IN_PROGRESS, SPINNING_IN_COMPLETED, FIRING, NOT_FIRING, SPINNING_OUT_STARTED, SPINNING_OUT_IN_PROGRESS, SPINNING_OUT_COMPLETED }
  EnemyState enemyState;
  private Renderer spriteMaterial, spriteRenderer;
  private float minX = -3.22f, maxX = 3.2f, minY = -2f, maxY = 8f; //(topleft: -3.22, 8.0) (bottomright: 3.2, -2.0)
  private float xDelta = 2f, yDelta = 2f;
  public SWS.PathManager waypointPath;
  public splineMove splineMoveScript;
  public int numBurstFires = 0, numBurstFiresBeforePause = 4;

  public GameObject spinningMineMissile;
  private Quaternion rotation;
  private GameObject spinningMineMissilesParentPool;
  public float angle;
  private List<Transform> ShootingPointTransforms;
  private Transform [] ShootingPointTransformsArray;
  private CapsuleCollider[] ShootingPointCollidersArray;
  private bool waiting = false;
  private bool firstTime = true;

  private void Awake()
  {
    spriteMaterial = GetComponent<Renderer>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    ShootingPointTransformsArray = (GetComponentsInChildren<Transform>());
    ShootingPointCollidersArray = (GetComponentsInChildren<CapsuleCollider>());

    //Note that parent Transform ALSO gets returned from GetComponentsInChildren, so need to do the following LINQ weirdness (from one of the answers here: https://forum.unity.com/threads/getcomponentsinchildren-not-parent-and-children.222009/#post-2955910 )
    //ShootingPointTransforms.AddRange(GetComponentsInChildren<Transform>().Where(x => x != this.transform));
  }


  void Start()
  {
    spriteRenderer.material.color = new Color(1, 1, 1, 0);
    spriteMaterial.material.SetFloat("_TwistUvAmount", 1f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 100f);
    enemyState = EnemyState.SPAWNING;
    transform.position = new Vector2((UnityEngine.Random.Range(-3.2f, 3.2f)), (UnityEngine.Random.Range(2f, 8f)));
    spinningMineMissilesParentPool = new GameObject("spinningMineMissilesParentPoolObject");
  }
  IEnumerator AlphaFadeTo(float aValue, float aTime)
  {
    float alpha = spriteRenderer.material.color.a;
    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
    {
      Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
      spriteRenderer.material.color = newColor;
      yield return null;
    }
  }
  IEnumerator SpinInEffect(float duration)
  {
    float elapsedTime = 0f;
    float currentTwistVal, currentBlurVal;
    while (elapsedTime <= duration)
    {
      currentTwistVal = Mathf.Lerp(1f, 3.14f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_TwistUvAmount", currentTwistVal);
      currentBlurVal = Mathf.Lerp(100f, 0f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_BlurIntensity", currentBlurVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    print("Got Here 0");
    spriteMaterial.material.SetFloat("_TwistUvAmount", 3.14f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 0f);
    if (firstTime)
    {
      splineMoveScript.StartMove();
      firstTime = false;
    }
    InvokeRepeating("RepeatBurstFire",0f,2f);
    enemyState = EnemyState.SPINNING_IN_COMPLETED;

    foreach (CapsuleCollider collider in ShootingPointCollidersArray)
    {
      collider.enabled = true;
    }
  }
  
  IEnumerator SpinOutEffect(float duration)
  {
    float elapsedTime = 0f;
    float currentTwistVal, currentBlurVal;
    while (elapsedTime <= duration)
    {
      currentTwistVal = Mathf.Lerp(3.14f, 1f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_TwistUvAmount", currentTwistVal);
      currentBlurVal = Mathf.Lerp(0f, 100f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_BlurIntensity", currentBlurVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    print("Got Here 1");
    spriteMaterial.material.SetFloat("_TwistUvAmount", 1f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 100f);
    enemyState = EnemyState.SPINNING_OUT_COMPLETED;
    //splineMoveScript.Stop();
    foreach (CapsuleCollider collider in ShootingPointCollidersArray)
    {
      collider.enabled = false;
    }

  }


    void Update()
  {

    if (numBurstFires >= numBurstFiresBeforePause)
    {
      enemyState = EnemyState.SPINNING_OUT_STARTED;
      numBurstFires = 0;
    }
    
    switch (enemyState)
    {
      case EnemyState.WAITING_TO_SPAWN:
        if (waiting == true)
        {
          waiting = false;
          Wait(10, () => {
            //Debug.Log("2 seconds is lost forever");
            enemyState = EnemyState.SPAWNING;
          });
        }
        break;
      case EnemyState.SPAWNING:
        //if (Input.GetKeyDown(KeyCode.S))
        {
          Spawn();
        }
        break;
      case EnemyState.SPINNING_IN_STARTED:
        StartCoroutine(AlphaFadeTo(1.0f, .5f));
        StartCoroutine(SpinInEffect(2f));
        enemyState = EnemyState.SPINNING_IN_IN_PROGRESS;
        break;
      case EnemyState.SPINNING_IN_IN_PROGRESS:
        break;
      case EnemyState.SPINNING_IN_COMPLETED:
        transform.Rotate(new Vector3(0, 0, 40 * Time.deltaTime));
        break;
      case EnemyState.SPINNING_OUT_STARTED:
        transform.Rotate(new Vector3(0, 0, 40 * Time.deltaTime));
        CancelInvoke();
        StartCoroutine(SpinOutEffect(1f));
        enemyState = EnemyState.SPINNING_OUT_IN_PROGRESS;
        break;
      case EnemyState.SPINNING_OUT_IN_PROGRESS:
        transform.Rotate(new Vector3(0, 0, 40 * Time.deltaTime));
        break;
      case EnemyState.SPINNING_OUT_COMPLETED:
        transform.Rotate(new Vector3(0, 0, 40 * Time.deltaTime));
        StartCoroutine(AlphaFadeTo(0f, .50f));
        waiting = true;
        enemyState = EnemyState.WAITING_TO_SPAWN;
        break;
      default:
        break;
    }
  }
  void Spawn()
  {
    enemyState = EnemyState.SPINNING_IN_STARTED;
  }

  void RepeatBurstFire()
  {
    StartCoroutine(BurstFire(15, .05f));
    numBurstFires++;
  }
  IEnumerator BurstFire(int numShots, float timeBetweenShots)
  {
    for (int i = 0; i < numShots; i++)
    {
      FireMissile(false);
      yield return new WaitForSeconds(timeBetweenShots);
    }
    //yield return new WaitForSeconds(timeBetweenBursts);
  }

    public void FireMissile(bool fireAtPlayerPos)
  {
    GameObject firedBullet;

      //angle  = -90f;
      //angle = transform.rotation.z * Mathf.Rad2Deg; //angle = -90f;

    /* angle is a float, and it's 90 when you're aiming/firing directly above you, 
     * -90 when firing directly below, 
     * 0 to the right and
     * 180 (-180) to the left */
    if (angle > 180)
      angle -= 360;

    //shoot missile 90degrees to the right of (i.e. perpindicular to) the rotation direction of the transform
    //rotation.eulerAngles = new Vector3( -ShootingPointTransforms[1].transform.eulerAngles.z, 90, 0); 
    
    //shoot missile in the rotation direction of the transform
    //rotation.eulerAngles = new Vector3( (-90 - transform.eulerAngles.z), 90, 0); 
    
    //firedBullet = SimplePool.Spawn(spinningMineMissile, ShootingPointTransforms[1].transform.position, Quaternion.identity, spinningMineMissilesParentPool.transform);
    
    //firedBullet.transform.localRotation = rotation; //v.important line!!!

    foreach( Transform tr in ShootingPointTransformsArray) 
    {
      if (tr != this.transform) //Note that parent Transform ALSO gets returned from GetComponentsInChildren(), so need to check it and skip it
      {
        rotation.eulerAngles = new Vector3(-tr.transform.eulerAngles.z, 90, 0);
        firedBullet = SimplePool.Spawn(spinningMineMissile, tr.transform.position, Quaternion.identity, spinningMineMissilesParentPool.transform);
        firedBullet.transform.localRotation = rotation; //v.important line!!!
      }
    }

  }

}


/* Define a set of screen valid to appear within (topleft: -3.22, 8.0) (bottomright: 3.2, -2.0)
 * set the 5 shooting points
 * set the collision on the min body of the mine
 * set collision on the 5 shooting ponits
 * choose a vfx type of missile it shoots
 * add a worldspace healthbar to it
 * add successful hitFX
 * 
 * set it up to be triggered by the levelmanager
 * 
 * while (alive)
 * {
 *  "randomly" appear within a set of screen bounds
 *   use shader to 'spin' shader into life
 *    when spin shader ends, start rotating and firing from the 5 shooting points - do this for x seconds
 *    use shader to 'spin' shader out of life
 *  }
 *        
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */