using UnityEngine;
using DG.Tweening;
using System;

namespace Mr1
{
  public abstract class EnemyBehaviour02 : ExtendedBehaviour
  //https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
  {
    protected float speed; //"protected" to allow abstract sub-class to access it
    private float hp;
    public float speedMultiplierFromSpawner = 1f;
    public float hpMultiplierFromSpawner = 1f;
    private float respawnWaitDelay = 2.0f; //default value unless overridden by derived class
    public string wayPointPathName;

    protected float startPosX, startPosY, startPosZ;
    private float startScaleX, startScaleY, startScaleZ;

    protected float initialHP, initialSpeed; //"protected" to allow abstract sub-class to access it
    private bool enemyHitByPlayerMissile;

    private GameObject missileObject;
    private SpriteRenderer enemySpriteRenderer;
    private CircleCollider2D enemyCircleCollider;
    private Vector3 upDirection;

    public enum EnemyState { ALIVE, TEMPORARILY_DEAD, WAITING_TO_RESPAWN, INVINCIBLE, FULLY_DEAD, HIT_BY_PLAYER_MISSILE, HIT_BY_PLAYER_SHIP, HIT_BY_ATMOSPHERE }

    public EnemyState enemyState;
    public bool respawnWaitOver;
    private bool startedWaiting;

    public GameObject[] availablePowerUps;
    public GameObject powerUpInstance;

    public PlayerShip playerShip;

    virtual public float GetRespawnWaitDelay() => respawnWaitDelay;

    public abstract void ReactToNonLethalPlayerMissileHit(); //each enemy variant has to implement their own

    public abstract void DoMovement(float initialSpeed, FollowType followType);

    public abstract void StopMovement();

    protected virtual void Start()
    {
      InitialSetup();
    }

    protected virtual void Update()
    {
      if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
      {
        switch (enemyState)
        {
          case EnemyState.ALIVE:
            {
              DoMovement(initialSpeed, FollowType.Loop);
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
              //    respawnWaitOver = true;
              //    //Debug.Log(respawnWaitDelay + " respawnWaitOver seconds passed");
              //  });
              //  startedWaiting = true;
              //}
              if (respawnWaitOver)
                Respawn();
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

    private void InitialSetup()
    {
      //todo - move this out to GameplayManager or to sub-class or enemy prefab??
      speed = 1.0f;
      hp = 1f;
      initialSpeed = speed * speedMultiplierFromSpawner; // todo: should these be set in this class, not MyGameplayManager??
      initialHP = hp * hpMultiplierFromSpawner;
      speed = initialSpeed;
      hp = initialHP;

      enemyHitByPlayerMissile = false;
      respawnWaitOver = false;
      startedWaiting = false;

      enemySpriteRenderer = GetComponent<SpriteRenderer>();
      enemyCircleCollider = GetComponent<CircleCollider2D>();

      startPosX = transform.position.x;
      startPosY = transform.position.y;
      startPosZ = transform.position.z;
      startScaleX = transform.localScale.x;
      startScaleY = transform.localScale.y;
      startScaleZ = transform.localScale.z;

      enemyState = EnemyState.WAITING_TO_RESPAWN;

      playerShip = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerShip>();
      upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
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
      print(($"totalEnemyKillCount={GameplayManager.Instance.totalEnemyKillCount} DROPPED POWERUP!"));
      powerUpInstance = SimplePool.Spawn(availablePowerUps[UnityEngine.Random.Range(0, availablePowerUps.Length)], transform.position, transform.rotation);
    }

    private void TemporarilyDie()
    {
      LevelManager.Instance.numEnemyKillsInLevel++;
      GameplayManager.Instance.totalEnemyKillCount++;
      //print($"totalEnemyKillCount {GameplayManager.Instance.totalEnemyKillCount}");
      if (GameplayManager.Instance.totalEnemyKillCount % GameplayManager.Instance.enemyKillPowerUpDropFrequency == 0)
      {
        DropPowerUp();
      }
      enemySpriteRenderer.enabled = false;
      enemyCircleCollider.enabled = false;
      respawnWaitOver = false;
      startedWaiting = false;

      StopMovement();
      enemyState = EnemyState.WAITING_TO_RESPAWN;
    }

    private void Respawn()
    {
      hp = initialHP; //reset health and position
      transform.position = new Vector3(startPosX, startPosY, startPosZ);
      transform.localScale = new Vector3(startScaleX, startScaleX, startScaleX); // reset its scale back to original scale
      enemySpriteRenderer.enabled = true;
      enemyCircleCollider.enabled = true;

      enemyState = EnemyState.ALIVE;
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnTriggerExit2D(Collider2D collision)
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
}