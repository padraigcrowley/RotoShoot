using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS; //simple waypoints

public class SpinningMineBehaviour : ExtendedBehaviour
{

  enum EnemyState { DOING_NOTHING, WAITING_TO_SPAWN, SPAWNING, SPINNING_IN_STARTED, SPINNING_IN_IN_PROGRESS, SPINNING_IN_COMPLETED, FIRING, NOT_FIRING, SPINNING_OUT_STARTED, SPINNING_OUT_IN_PROGRESS, SPINNING_OUT_COMPLETED, DYING_TEMPORARILY, DEAD_TEMPORARILY, DYING_FULLY, DEAD_FULLY }
  EnemyState enemyState;
  private Renderer spriteMaterial, spriteRenderer;
  private float minX = -3.22f, maxX = 3.2f, minY = -2f, maxY = 8f; //(topleft: -3.22, 8.0) (bottomright: 3.2, -2.0)
  private float xDelta = 2f, yDelta = 2f;
  public SWS.PathManager waypointPath;
  public splineMove splineMoveScript;
  private int numBurstFires = 0;
  public int numBurstFiresBeforePause;
  public float waitTimeBeforeFirstSpawn, waitTimeBetweenSpawns;

  float spinningMineMaxHealth = 100;  //todo - hard coded health?!?!
  float spinningMineCurrentHealth;
  public float hpMultiplierFromSpawner;
  public float speedMultiplierFromSpawner;
  private float rotationSpeed = 200f;

  public GameObject spinningMineMissile;
  private Quaternion rotation;
  private GameObject spinningMineMissilesParentPool;
  public float angle;
  private List<Transform> ShootingPointTransforms;
  private Transform[] ShootingPointTransformsArray;
  private CapsuleCollider[] ShootingPointCollidersArray;
  private bool waitingToRespawn = true, waitingBeforeFirstSpawn = true;
  private bool firstTime = true;

  public UltimateStatusBar SpinningMineStatusBar;

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
    enemyState = EnemyState.DOING_NOTHING;
    transform.position = new Vector2((UnityEngine.Random.Range(-3.2f, 3.2f)), (UnityEngine.Random.Range(2f, 8f)));
    spinningMineMissilesParentPool = new GameObject("spinningMineMissilesParentPoolObject");

    splineMoveScript = GetComponent<splineMove>();
    if (splineMoveScript != null)
    {
      splineMoveScript.pathContainer = waypointPath;
      splineMoveScript.speed *= this.speedMultiplierFromSpawner;
    }
    spinningMineMaxHealth *= hpMultiplierFromSpawner;
    spinningMineCurrentHealth = spinningMineMaxHealth;
    UltimateStatusBar.UpdateStatus("SpinningMineStatusBar", spinningMineCurrentHealth, spinningMineMaxHealth);
  }

  IEnumerator DoHitEffect()
  {
    float duration = .2f;
    float elapsedTime = 0f;
    float currentEffectBlendVal;
    float startEffectBlendVal = 0;
    float endEffectBlendVal = .4f;

    while (elapsedTime <= duration)
    {
      currentEffectBlendVal = Mathf.Lerp(startEffectBlendVal, endEffectBlendVal, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_HitEffectBlend", currentEffectBlendVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    while (elapsedTime <= duration)
    {
      currentEffectBlendVal = Mathf.Lerp(endEffectBlendVal, startEffectBlendVal, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_HitEffectBlend", currentEffectBlendVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    spriteMaterial.material.SetFloat("_HitEffectBlend", startEffectBlendVal); // just to make sure it ends up back at its initial/start value
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
    //print("Got Here 0");
    spriteMaterial.material.SetFloat("_TwistUvAmount", 3.14f);
    spriteMaterial.material.SetFloat("_BlurIntensity", 0f);
    if (firstTime)
    {
      splineMoveScript.StartMove();
      firstTime = false;
    }
    InvokeRepeating("RepeatBurstFire", 0f, 10f);
    enemyState = EnemyState.SPINNING_IN_COMPLETED;

    foreach (CapsuleCollider collider in ShootingPointCollidersArray)
    {
      collider.enabled = true;
    }

    
    SpinningMineStatusBar.EnableStatusBar();
    UltimateStatusBar.UpdateStatus("SpinningMineStatusBar", spinningMineCurrentHealth, spinningMineMaxHealth);
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
    //print("Got Here 1");
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
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
    {
      Destroy(gameObject);
      return;
    }
    //if (numBurstFires >= numBurstFiresBeforePause)
    //{
    //  enemyState = EnemyState.SPINNING_OUT_STARTED;
    //  numBurstFires = 0;
    //}
    if (spinningMineCurrentHealth <= 0)
    {
      enemyState = EnemyState.SPINNING_OUT_STARTED;
      //UltimateStatusBar.UpdateStatus("SpinningMineStatusBar", spinningMineCurrentHealth, spinningMineMaxHealth);
    }

    switch (enemyState)
    {
      case EnemyState.DOING_NOTHING:
        if (waitingBeforeFirstSpawn == true)
        {
          waitingBeforeFirstSpawn = false;
          Wait(waitTimeBeforeFirstSpawn, () =>
          {
            enemyState = EnemyState.SPAWNING;
            //print($"Waited {waitTimeBeforeFirstSpawn} to first spawn");
          });
        }
        break;
      case EnemyState.WAITING_TO_SPAWN:
        if (waitingToRespawn == true)
        {
          waitingToRespawn = false;
          Wait(waitTimeBetweenSpawns, () =>
          {
            enemyState = EnemyState.SPAWNING;
            //print($"Waited {waitTimeBetweenSpawns} to re-spawn");
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
        
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        break;
      case EnemyState.SPINNING_OUT_STARTED:
        SpinningMineStatusBar.DisableStatusBar();
        spinningMineCurrentHealth = spinningMineMaxHealth;
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        CancelInvoke();
        StartCoroutine(SpinOutEffect(1f));
        enemyState = EnemyState.SPINNING_OUT_IN_PROGRESS;
        break;
      case EnemyState.SPINNING_OUT_IN_PROGRESS:
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        break;
      case EnemyState.SPINNING_OUT_COMPLETED:
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        StartCoroutine(AlphaFadeTo(0f, .50f));
        waitingToRespawn = true;
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
    StartCoroutine(BurstFire(5, .025f, 4, 1f));
  }
  IEnumerator BurstFire(int numShots, float timeBetweenShots, int numTimesToRepeat, float intervalBetweenBursts)
  {
    for (int repeat = 0; repeat < numTimesToRepeat; repeat++)
    {
      for (int i = 0; i < numShots; i++)
      {
        FireMissile(false);
        yield return new WaitForSeconds(timeBetweenShots);
      }
      yield return new WaitForSeconds(intervalBetweenBursts);
    }
    numBurstFires++;
    //print($"numBurstFires++ {numBurstFires}/{numBurstFiresBeforePause}");
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

    foreach (Transform tr in ShootingPointTransformsArray)
    {
      if (tr != this.transform) //Note that parent Transform ALSO gets returned from GetComponentsInChildren(), so need to check it and skip it
      {
        rotation.eulerAngles = new Vector3(-tr.transform.eulerAngles.z, 90, 0);
        firedBullet = SimplePool.Spawn(spinningMineMissile, tr.transform.position, Quaternion.identity, spinningMineMissilesParentPool.transform);
        firedBullet.transform.localRotation = rotation; //v.important line!!!
      }
    }

  }


  private void OnTriggerEnter(Collider collision)
  {
    if (collision.gameObject.tag.Equals("PlayerMissile") && (enemyState == EnemyState.SPINNING_IN_COMPLETED) )
    {
      StartCoroutine(DoHitEffect());
      spinningMineCurrentHealth -= 10;  //todo - hard coded value?!?!
      UltimateStatusBar.UpdateStatus("SpinningMineStatusBar", spinningMineCurrentHealth, spinningMineMaxHealth);
    }

  }
}


/// DOING_NOTHING, WAITING_TO_SPAWN, SPAWNING, SPINNING_IN_STARTED, SPINNING_IN_IN_PROGRESS, SPINNING_IN_COMPLETED, FIRING, NOT_FIRING, SPINNING_OUT_STARTED, SPINNING_OUT_IN_PROGRESS, SPINNING_OUT_COMPLETED
/// 
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