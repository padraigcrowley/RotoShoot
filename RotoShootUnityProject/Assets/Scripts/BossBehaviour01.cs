using SWS; //simple waypoints
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

  public GameObject bossMissile, bossEgg;
  private Quaternion rotation;
  private GameObject bossMissilesParentPool;
  private bool fireAtPlayerPos = true;

  public Animator bossEggAnimator;
  private bool waiting = false;
  public bool eggIsMoving = false;
  private bool damageFXReady = true;

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

    damageFXReady = true;

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
          boss01State = BossState.BOSS_INVULNERABLE;
          break;
        }
      case BossState.BOSS_INVULNERABLE:
        {
          if (!waiting)
          {
            StartCoroutine(DelayedLowerEgg());
          }
          break;
        }
      case BossState.BOSS_VULNERABLE:
        {
          if (!waiting)
          {
            StartCoroutine(DelayedRaiseEgg());
          }
          break;
        }
      default:
        break;
    }
    
  }

  
  private IEnumerator DelayedLowerEgg()
  {
    waiting = true;
    yield return new WaitForSeconds(5f);

    CancelInvoke();
    bossEggAnimator.Play("Boss01EggLower"); 
    eggIsMoving = true;
    while (eggIsMoving)
    {
      yield return null;
    }
      
    InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0, .75f);
    //InvokeRepeating(nameof(this.FireMissileStraightDown), 1, 2f);
    
    boss01State = BossState.BOSS_VULNERABLE;
    waiting = false;
  }

  private IEnumerator DelayedRaiseEgg()
  {
    waiting = true;
    yield return new WaitForSeconds(5f);
    CancelInvoke();
    bossEggAnimator.Play("Boss01EggRaise");
    eggIsMoving = true;
    while (eggIsMoving)
    {
      yield return null;
    }
    
    //InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0, 1f);
    InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0f, .5f);
    InvokeRepeating(nameof(this.FireMissileStraightDown), .25f, .5f);

    boss01State = BossState.BOSS_INVULNERABLE;
    waiting = false;
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

    firedBullet = SimplePool.Spawn(bossMissile, bossEgg.transform.position, Quaternion.identity, bossMissilesParentPool.transform);
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
      if (damageFXReady)
      {
        damageFXReady = false;
        StartCoroutine(BossTakesDamageEffect(.25f)); //note: the param is half the overall duration
        StartCoroutine(DamageFXCooldown(.5f));
      }

    }
  }

  IEnumerator BossTakesDamageEffect(float halfDuration)
  {
    float elapsedTime = 0f;
    float currentVal;
    
    while (elapsedTime <= halfDuration) //from normal to red
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / halfDuration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }

    elapsedTime = 0f;
    while (elapsedTime <= halfDuration) //from red back to normal
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / halfDuration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();

      // just to make sure it goes back down to zero
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          sr.material.SetFloat("_InnerOutlineAlpha", 0f);
        }
      }
    }
  }

  public IEnumerator DamageFXCooldown(float cooldownTime)
  {
    yield return new WaitForSeconds(cooldownTime);
    damageFXReady = true;
  }

}