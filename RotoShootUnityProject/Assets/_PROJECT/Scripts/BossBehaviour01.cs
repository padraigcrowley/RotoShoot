using SWS; //simple waypoints
using System.Collections;
using UnityEngine;
using DG.Tweening;

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
  private bool firstTimeAppearance = true;

  public Animator bossEggAnimator;
  private bool waiting = false;
  public bool eggIsMoving = false;
  private bool damageFXReady = true;
  private Tween pulseTween;

  public UltimateStatusBar statusBar;
  private Canvas HealthBarCanvas;
  public float bossMaxHealth = 100, bossCurrentHealth;

  enum BossState { BOSS_INTRO_IN_PROGRESS, BOSS_INTRO_COMPLETED, BOSS_IN_PROGRESS, BOSS_OUTRO_IN_PROGRESS, BOSS_VULNERABLE, BOSS_INVULNERABLE, BOSS_FIRING, BOSS_NOT_FIRING, BOSS_LOWERING_EGG, BOSS_RAISING_EGG }

  BossState boss01State;

  public GameObject bossDamageFX;
  private void Awake()
  {
    HealthBarCanvas = statusBar.GetComponentInParent<Canvas>();
    HealthBarCanvas.enabled = false;
  }

  void Start()
  {
    
    transform.position = new Vector3(startPosX, startPosY, 0f);
    bossMaxHealth *= hpMultiplierFromSpawner;
    bossCurrentHealth = 0;// we'll set off a coroutine to fill up the healthbar later.
    UltimateStatusBar.UpdateStatus("BossStatusBar", "BossHealthBar", bossCurrentHealth, bossMaxHealth);
    //bossCurrentHealth = bossMaxHealth;

    GameplayManager.Instance.playerShipFiring = false;
    GameplayManager.Instance.playerShipMovementAllowed = false;

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

  void Update()
  {
    //if (Input.GetKeyDown(KeyCode.S))
    //{
    //  fireAtPlayerPos = true;
    //  FireMissile(fireAtPlayerPos);
    //}

    switch (boss01State)
    {
      case BossState.BOSS_INTRO_IN_PROGRESS:
        break;
      case BossState.BOSS_INTRO_COMPLETED:
        {
          HealthBarCanvas.enabled = true;
          pulseTween = transform.DOScale(1.1f, 0.5f).SetLoops(50, LoopType.Yoyo );
          StartCoroutine(FillHealthBar(5f,bossMaxHealth));
          
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
  private IEnumerator FillHealthBar(float duration, float newHealthLevel)
  {
    float elapsedTime = 0f;
    //float currentVal;
    float bossStartHealth = bossCurrentHealth;

    while (elapsedTime <= duration) 
    {
      bossCurrentHealth = Mathf.Lerp(bossStartHealth, newHealthLevel, (elapsedTime / duration));
      elapsedTime += Time.deltaTime;
      //bossCurrentHealth -= 10;
      //print($"bossCurrHealth: {bossCurrentHealth}");
      UltimateStatusBar.UpdateStatus("BossStatusBar", "BossHealthBar", bossCurrentHealth, bossMaxHealth);
      yield return null;
      //yield return new WaitForEndOfFrame();
    }
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

    //this is just to turn the Orb sprite on, but i'm too lazy so I just set them set all to enable (even tho some are are already enabled :-)
    foreach (Renderer sr in bossSpriteMaterials)
    {
      sr.enabled = true;
    }

    boss01State = BossState.BOSS_INTRO_COMPLETED;
  }

  private IEnumerator DelayedLowerEgg()
  {
    waiting = true;
    yield return new WaitForSeconds(5f);

    if (firstTimeAppearance)
    {
      pulseTween.Pause();
      splineMoveScript.StartMove();
      GameplayManager.Instance.playerShipFiring = true;
      GameplayManager.Instance.playerShipMovementAllowed = true;
      firstTimeAppearance = false;
    }

    CancelInvoke(); //stop firing at player
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
    
    //calculate how much to add back onto healthbar - half of what health the boss DOESN'T have (maxHealth - currentHealth)
    float amountToRaiseTo = bossCurrentHealth + ((bossMaxHealth - bossCurrentHealth) / 2.0f);
    print("StartCoroutine(FillHealthBar)");
    pulseTween.Restart();
    StartCoroutine(FillHealthBar(2.5f, amountToRaiseTo)); 
    
    CancelInvoke();//stop firing at player
    bossEggAnimator.Play("Boss01EggRaise");
    eggIsMoving = true;
    while (eggIsMoving)
    {
      yield return null;
    }
    pulseTween.Pause();

    //InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0, 1f);
    InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 0f, .5f);
    InvokeRepeating(nameof(this.FireMissileStraightDown), .25f, .5f);

    boss01State = BossState.BOSS_INVULNERABLE;
    waiting = false;
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

  //custom method, gets called by ChildCollider.cs
  public void OnChildTriggerEntered(Collider other, string childTag)
  {
    if (other.gameObject.CompareTag("PlayerMissile") && childTag.Equals("BossVulnerable"))
    {
      GameObject newParticleEffect = SimplePool.Spawn(bossDamageFX, other.gameObject.transform.position, bossDamageFX.transform.rotation, transform) as GameObject;
      Wait(2, () => {
        SimplePool.Despawn(newParticleEffect);
      });
      if (damageFXReady)
      {
        damageFXReady = false;
        StartCoroutine(BossTakesDamageEffect(.25f)); //note: the param is half the overall duration
        StartCoroutine(DamageFXCooldown(.5f));
      }
      bossCurrentHealth -= 10;
      UltimateStatusBar.UpdateStatus("BossStatusBar", "BossHealthBar", bossCurrentHealth, bossMaxHealth);
    }
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

}