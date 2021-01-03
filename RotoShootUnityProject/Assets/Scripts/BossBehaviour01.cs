﻿using SWS; //simple waypoints
using System.Collections;
using UnityEngine;

public class BossBehaviour01 : ExtendedBehaviour
{
  public SWS.PathManager waypointPath;
  protected splineMove splineMoveScript;
  private Renderer[] bossSpriteMaterials;
  public float startPosX, startPosY, startPosZ;
  public float bossHP = 1;
  public float hpMultiplierFromSpawner;
  public float speedMultiplierFromSpawner;

  public GameObject bossMissile, bossOrb;
  private Quaternion rotation;
  private GameObject bossMissilesParentPool;
  private bool fireAtPlayerPos = true;

  public Animator bossEggAnimator;
  private bool waiting = false;
  enum BossState { BOSS_INTRO_IN_PROGRESS, BOSS_INTRO_COMPLETED, BOSS_IN_PROGRESS, BOSS_OUTRO_IN_PROGRESS, BOSS_VULNERABLE, BOSS_INVULNERABLE, BOSS_FIRING, BOSS_NOT_FIRING, BOSS_LOWERING_EGG, BOSS_RAISING_EGG }

  BossState boss01State;

  // Start is called before the first frame update
  void Start()
  {
    transform.position = new Vector3(startPosX, startPosY, 0f);
    bossHP *= hpMultiplierFromSpawner;

    bossEggAnimator = GetComponentInChildren<Animator>();
    bossSpriteMaterials = GetComponentsInChildren<Renderer>();


    splineMoveScript = GetComponent<splineMove>();
    if (splineMoveScript != null)
    {
      splineMoveScript.pathContainer = waypointPath;
      splineMoveScript.speed *= this.speedMultiplierFromSpawner;
    }

    bossMissilesParentPool = new GameObject("bossMissilesParentPoolObject");

    boss01State = BossState.BOSS_INTRO_IN_PROGRESS;
    StartCoroutine(BossAppearEffect(5f));

    //Wait(5, () =>
    //{
    //  Debug.Log("5 seconds is lost forever");
    //  bossEggAnimator.Play("Boss01EggLower");
    //});

    //InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 4, 2f);
    //InvokeRepeating(nameof(this.FireMissileStraightDown), 5, 2f);
  }

  IEnumerator BossAppearEffect(float duration)
  {
    float elapsedTime = 0f;
    float currentVal;
    while (elapsedTime <= duration)
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / duration));
          sr.material.SetFloat("_ChromAberrAmount", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }

    yield return new WaitForSeconds(.3f);
    foreach (Renderer sr in bossSpriteMaterials)
    {
      sr.enabled = true;
    }

    boss01State = BossState.BOSS_INTRO_COMPLETED;
  }
  
  void Update()
  {

    //if (Input.GetKeyDown(KeyCode.S))
    //{
    //  fireAtPlayerPos = true;
    //  FireMissile(fireAtPlayerPos);
    //}


    //NOTE TO ME - the bosses missiles are hitting up/left/right border, causing them to expire before launching towards player

    switch (boss01State)
    {
      case BossState.BOSS_INTRO_IN_PROGRESS:
        break;
      case BossState.BOSS_INTRO_COMPLETED:
        {
          splineMoveScript.StartMove();
          boss01State = BossState.BOSS_NOT_FIRING;
          break;
        }
      case BossState.BOSS_NOT_FIRING:
        {
          if (!waiting)
          {
            
            print("in BOSS_NOT_FIRING 1");
            StartCoroutine(DelayedStartFiring());
            print("in BOSS_NOT_FIRING 2");
          }
          break;
        }
      case BossState.BOSS_FIRING:
        {
          StartCoroutine(DelayedStopFiring());
          break;
        }
      default:
        break;
    }


    //if (bossAppeared)
    //{
    //  splineMoveScript.StartMove();
    //  bossAppeared = false; // just to make it stop executing the startmove() again
    //}

  }

  private IEnumerator DelayedStartFiring()
  {
    waiting = true;
    print($"just b4 the yield");
    yield return new WaitForSeconds(5f);
    boss01State = BossState.BOSS_FIRING;
    print($"Calling Invoke");
    InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0, 2f);
    InvokeRepeating(nameof(this.FireMissileStraightDown), 1, 2f);
    waiting = false;
  }

  private IEnumerator DelayedStopFiring()
  {
    yield return new WaitForSeconds(5f);
    boss01State = BossState.BOSS_NOT_FIRING;
    CancelInvoke();
  }



  void FireMissileAtPlayerPos()
  {
    FireMissile(true);
  }
  void FireMissileStraightDown()
  {
    FireMissile(false);
  }
  void FireMissile(bool fireAtPlayerPos)
  {
    GameObject firedBullet;
    float angle;

    Vector2 direction = GameplayManager.Instance.playerShipPos - transform.position;  //direction is a vector2 containing the (x,y) distance from the player ship to the firing gameobject (the enemy position)
    if (fireAtPlayerPos)
      angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    else
      angle = -90f;

    /* angle is a float, and it's 90 when you're aiming/firing directly above you, 
     * -90 when firing directly below, 
     * 0 to the right and
     * 180 (-180) to the left */
    if (angle > 180) angle -= 360;
    rotation.eulerAngles = new Vector3(-angle, 90, 0); // use different values to lock on different axis

    firedBullet = SimplePool.Spawn(bossMissile, bossOrb.transform.position, Quaternion.identity, bossMissilesParentPool.transform);
    firedBullet.transform.localRotation = rotation; //v.important line!!!

  }

  //void OnTriggerEnter(Collider collider)
  //{
  //  List<Collider> collisions = new List<Collider>();

  //int numCollisionContacts = -1;

  //  ///Collider2D[] contacts = new Collider2D[5];
  // //numCollisionContacts = collider.GetCon
  //  if (numCollisionContacts == 2)
  //  {
  //    print($"numCollisionContacts = {numCollisionContacts}");
  //  }

  //  foreach (Collider collision in collisions)
  //  {
  //    // ***TODO - ADD A CHECK IT@S NOT COLLIDING AGAINST "BOUNDARY" !!
  //    if (collision.gameObject.CompareTag("BossVulnerable") && collider.gameObject.CompareTag("PlayerMissile"))
  //    {
  //      print("HIT BOSS ORB in OnTriggerEnter!");
  //      StartCoroutine(BossTakesDamageEffect(.5f));
  //    }
  //    //if (collision.gameObject.CompareTag("BossInvulnerable")&& collider.gameObject.CompareTag("PlayerMissile")))
  //    //print("HIT BOSS BODY!");
  //  }
  //}

  public void OnChildTriggerEntered(Collider other, string childTag)
  {
    if (other.gameObject.CompareTag("PlayerMissile") && childTag.Equals("BossVulnerable"))
    {
      //print("HIT BOSS ORB in OnChildTriggerEntered!");
      StartCoroutine(BossTakesDamageEffect(.25f));
    }
  }

  IEnumerator BossTakesDamageEffect(float duration)
  {
    float elapsedTime = 0f;
    float currentVal;
    while (elapsedTime <= duration) //from normal to red
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }

    elapsedTime = 0f;
    while (elapsedTime <= duration) //from red back to normal
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / duration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }
  }

}