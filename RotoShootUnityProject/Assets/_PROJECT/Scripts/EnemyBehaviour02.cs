using UnityEngine;
using DG.Tweening;
using System;
using SWS; //simple waypoints
using System.Collections;

public abstract class EnemyBehaviour02 : ExtendedBehaviour
//https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
{
  protected float speed; //"protected" to allow abstract sub-class to access it
  private float hp;
  public float speedMultiplierFromSpawner = 1f;
  public float hpMultiplierFromSpawner = 1f;
  private float respawnWaitDelay = 2.0f; //default value unless overridden by derived class

  public float startPosX, startPosY, startPosZ = 0f;
  private float startScaleX, startScaleY, startScaleZ;

  protected float initialHP, initialSpeed; //"protected" to allow abstract sub-class to access it
  private bool enemyHitByPlayerMissile;

  private GameObject missileObject;
  private SpriteRenderer enemySpriteRenderer;
  private CapsuleCollider enemyCapsuleCollider;
  protected Vector3 upDirection;

  private Renderer spriteMaterial;
  public GameObject deathExplosion;
  public GameObject deathExplosionInstance;

  public enum EnemyState { ALIVE, TEMPORARILY_DEAD, WAITING_TO_RESPAWN, INVINCIBLE, FULLY_DEAD, HIT_BY_PLAYER_MISSILE, HIT_BY_PLAYER_SHIP, HIT_BY_ATMOSPHERE }

  public EnemyState enemyState;
  public bool waveRespawnWaitOver;
  private bool timeBetweenSpawnPassed = true;
  private bool startedWaiting;
  public float timeBetweenSpawn;


  public GameObject[] availablePowerUps;
  public GameObject powerUpInstance;

  public PlayerShip playerShip;

  public SWS.PathManager waypointPath;
  protected splineMove splineMoveScript;

  virtual public float GetRespawnWaitDelay() => respawnWaitDelay;

  public abstract void ReactToNonLethalPlayerMissileHit(); //each enemy variant has to implement their own

  public abstract void DoMovement();

  public abstract void StopMovement();

  protected virtual void Start()
  {
    InitialSetup();
  }
  private void InitialSetup()
  {
    //todo - move this out to GameplayManager or to sub-class or enemy prefab??

    hp = 1f;
    speed = 1f;
    speed *= speedMultiplierFromSpawner;
    initialHP = hp * hpMultiplierFromSpawner;
    hp = initialHP;

    enemyHitByPlayerMissile = false;
    waveRespawnWaitOver = false;
    startedWaiting = false;

    enemySpriteRenderer = GetComponent<SpriteRenderer>();
    spriteMaterial = GetComponent<Renderer>();
    enemyCapsuleCollider = GetComponent<CapsuleCollider>();

    transform.position = new Vector2(startPosX, startPosY);

    //startPosZ = transform.position.z;
    startScaleX = transform.localScale.x;
    startScaleY = transform.localScale.y;
    startScaleZ = transform.localScale.z;

    enemyState = EnemyState.WAITING_TO_RESPAWN;

    playerShip = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShip>();
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;

    splineMoveScript = GetComponent<splineMove>();
    if (splineMoveScript != null)
    {
      splineMoveScript.pathContainer = waypointPath;
      splineMoveScript.speed = this.speed;
      splineMoveScript.loopType = splineMove.LoopType.loop;
      splineMoveScript.closeLoop = true;
    }


    //if (TryGetComponent<splineMove>(out splineMove splineMoveScript))
    //{
    //  splineMoveScript.pathContainer = waypointPath;
    //  splineMoveScript.speed = this.speed;
    //}
    //else
    //{
    //  Debug.LogError($"ERROR! no splineMove component found on {this.name}");
    //}
  }

  protected virtual void Update()
  {
    if (Input.GetKeyDown(KeyCode.S))
    {
      TemporarilyDie();
    }

    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      switch (enemyState)
      {
        case EnemyState.ALIVE:
          {
            DoMovement();
            break;
          }
        case EnemyState.TEMPORARILY_DEAD:
          {
            TemporarilyDie();
            break;
          }
        case EnemyState.HIT_BY_PLAYER_MISSILE:
          {
            //Destroy(missileObject);//destroy the missile object - should the missileObject itself be doing this or at least pass a message back to it?
            //missileObject.SetActive(false);
            HandleDamage();

            break;
          }
        case EnemyState.HIT_BY_PLAYER_SHIP:
          {
            playerShip.ChangeShipHP(-20);
            TemporarilyDie();
            break;
          }
        case EnemyState.HIT_BY_ATMOSPHERE:
          {
            playerShip.ChangeShipHP(-10);
            TemporarilyDie();
            break;
          }
        case EnemyState.WAITING_TO_RESPAWN:
          {
            ////https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
            //if (!startedWaiting)
            //{
            //  Wait(GetRespawnWaitDelay(), () =>
            //  {
            //    waveRespawnWaitOver = true;
            //    //Debug.Log(respawnWaitDelay + " waveRespawnWaitOver seconds passed");
            //  });
            //  startedWaiting = true;
            //}
            if (waveRespawnWaitOver)
            {
              //if (timeBetweenSpawnPassed)
              {
                Respawn();
              }
            }
            break;
          }
        case EnemyState.INVINCIBLE:
          {
            break;
          }
        case EnemyState.FULLY_DEAD:
          {
            break;
          }
        default:
          {
            break;
          }
      }
    }
    else if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
    {
      Destroy(gameObject);//todo, instead of just destroy, add some fancy sprite shader effect here, then destroy after delay.
    }
    else if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.GAME_OVER_SCREEN)
    {
      hp = initialHP; //reset health and position
      speed = initialSpeed;
      transform.position = new Vector3(startPosX, startPosY, startPosZ);
      transform.localScale = new Vector3(startScaleX, startScaleX, startScaleX); // reset its scale
    }
  }


  private void HandleDamage()
  {
    if (hp <= 1) //lethal hit
    {
      enemyState = EnemyState.TEMPORARILY_DEAD;
    }
    else //non-lethal hit
    {
      hp--;
      ReactToNonLethalPlayerMissileHit();
      enemyState = EnemyState.ALIVE;
    }
  }

  private void DropPowerUp()
  {
    //print(($"totalEnemyKillCount={GameplayManager.Instance.totalEnemyKillCount} DROPPED POWERUP!"));
    powerUpInstance = SimplePool.Spawn(availablePowerUps[UnityEngine.Random.Range(0, availablePowerUps.Length)], transform.position, transform.rotation);
  }

  private void TemporarilyDie()
  {
    deathExplosionInstance = SimplePool.Spawn(deathExplosion, this.transform.position, this.transform.rotation/*, parentPool.transform*/);

    int randScaleFlip = UnityEngine.Random.Range(0, 4);// not scaleflipped, scaledFlippedX, scaledFlippedY, scaledFlippedXandY
    switch(randScaleFlip)
    {
      case (0):
        deathExplosionInstance.transform.localScale = new Vector3(.5f, .5f, 1f);
        break;
      case (1):
        deathExplosionInstance.transform.localScale = new Vector3(-.6f, .4f, 1f);
        break;
      case (2):
        deathExplosionInstance.transform.localScale = new Vector3(.5f, -.6f, 1f);
        break;
      case (3):
        deathExplosionInstance.transform.localScale = new Vector3(-.4f, -.4f, 1f);
        break;
      default:
        break;
    }

    StartCoroutine(DoFadeEffect(.5f, 0f, 1f));
    StopMovement();
    LevelManager.Instance.numEnemyKillsInLevel++;
    GameplayManager.Instance.totalEnemyKillCount++;
    //print($"totalEnemyKillCount {GameplayManager.Instance.totalEnemyKillCount}");
    if (GameplayManager.Instance.totalEnemyKillCount % GameplayManager.Instance.enemyKillPowerUpDropFrequency == 0)
    {
      DropPowerUp();
    }

    enemyCapsuleCollider.enabled = false;
    waveRespawnWaitOver = false;
    startedWaiting = false;

    enemyState = EnemyState.WAITING_TO_RESPAWN;
  }

  IEnumerator DoFadeEffect(float duration, float startVal, float endVal)
  {
    float elapsedTime = 0f;
    float currentVal;

    while (elapsedTime <= duration) //from normal to red
    {
      currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_FadeAmount", currentVal);
      elapsedTime += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    //enemySpriteRenderer.enabled = false;
  }

  private void Respawn()
  {
    //print ("in Respawn()");
    //Wait(timeBetweenSpawn, () => 
    //{
    //  timeBetweenSpawnPassed = true;
    //});

    if (timeBetweenSpawnPassed)
    {
      //print($"waited for timeBetweenSpawn ({timeBetweenSpawn}) seconds");
      spriteMaterial.material.SetFloat("_FadeAmount", 0f);
      hp = initialHP; //reset health and position
      transform.position = new Vector3(startPosX, startPosY, startPosZ);
      transform.localScale = new Vector3(startScaleX, startScaleX, startScaleX); // reset its scale back to original scale
      enemySpriteRenderer.enabled = true;
      enemyCapsuleCollider.enabled = true;
      enemyState = EnemyState.ALIVE;
      //timeBetweenSpawnPassed = false;
    }
  }

  private void OnTriggerEnter(Collider collision)
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (collision.gameObject.tag.Equals("PlayerMissile"))
      {
        missileObject = collision.gameObject;
        enemyState = EnemyState.HIT_BY_PLAYER_MISSILE;
      }
      else if (collision.gameObject.tag.Equals("Player"))
      {
        enemyState = EnemyState.HIT_BY_PLAYER_SHIP;
      }
      else if (collision.gameObject.tag.Equals("Atmosphere"))
      {
        enemyState = EnemyState.HIT_BY_ATMOSPHERE;
      }
    }
  }

  private void OnTriggerExit(Collider collision)
  {
    if (GameplayManager.Instance.levelControlType != 1) // this doean't apply to circular enemies moving towards centre
    {
      if (collision.gameObject.name == "Bottom Boundary")
      {
        enemyState = EnemyState.TEMPORARILY_DEAD;
        //DOTween.Kill(myTweenID);
      }
    }
  }
}
