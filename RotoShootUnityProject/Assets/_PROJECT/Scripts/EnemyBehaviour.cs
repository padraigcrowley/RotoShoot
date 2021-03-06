﻿using UnityEngine;

public abstract class EnemyBehaviour : ExtendedBehaviour
//https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
{
  private float speed;
  private float hp;
  public float speedMultiplierFromSpawner = 1f;
  public float hpMultiplierFromSpawner = 1f;
  private  float respawnWaitDelay = 2.0f; //defauly value unless overridden by derived class

  private float startPosX, startPosY, startPosZ;
  private float startScaleX, startScaleY, startScaleZ;

  private float initialHP, initialSpeed;
  private bool enemyHitByPlayerMissile;

  private GameObject missileObject;
  private SpriteRenderer enemySpriteRenderer;
  private CircleCollider2D enemyCircleCollider;

  private enum EnemyState { ALIVE, TEMPORARILY_DEAD, WAITING_TO_RESPAWN, INVINCIBLE, FULLY_DEAD, HIT_BY_PLAYER_MISSILE, HIT_BY_PLAYER_SHIP }

  private EnemyState enemyState;
  private bool respawnWaitOver;
  private bool startedWaiting;

  virtual public float GetRespawnWaitDelay() => respawnWaitDelay;
 
  public abstract void ReactToNonLethalPlayerMissileHit(); //each enemy variant has to implement their own 

  private void Start()
  {
    //todo - move this out to GameplayManager or to sub-class or enemy prefab??
    speed = .25f;
    hp = 1f;
    initialSpeed = speed * speedMultiplierFromSpawner; // todo: should these be set in this class, not MyGameplayManager??
    initialHP = hp * hpMultiplierFromSpawner;
    speed = initialSpeed;
    hp = initialHP;

    enemyState = EnemyState.ALIVE;
    enemyHitByPlayerMissile = false;
    respawnWaitOver = false;
    startedWaiting = false;

    // rotate enemy to face player ship https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
    Vector3 dir = GameplayManager.Instance.playerShipPos - transform.position;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    enemySpriteRenderer = GetComponent<SpriteRenderer>();
    enemyCircleCollider = GetComponent<CircleCollider2D>();

    startPosX = transform.position.x;
    startPosY = transform.position.y;
    startPosZ = transform.position.z;
    startScaleX = transform.localScale.x;
    startScaleY = transform.localScale.y;
    startScaleZ = transform.localScale.z;
  }

  private void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      switch (enemyState)
      {
        case EnemyState.ALIVE:
          {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, step);
            break;
          }
        case EnemyState.TEMPORARILY_DEAD:
          {
            TemporarilyDie();
            break;
          }
        case EnemyState.HIT_BY_PLAYER_MISSILE:
          {
            Destroy(missileObject);//destroy the missile object - should the missileObject itself be doing this or at least pass a message back to it?
            HandleDamage();

            break;
          }
        case EnemyState.HIT_BY_PLAYER_SHIP:
          {
            GameplayManager.Instance.currentPlayerHP--;
            TemporarilyDie();
            //hp = initialHP; //reset health and position
            //transform.position = new Vector3(startPosX, startPosY, startPosZ);
            //transform.localScale = new Vector3(1f, 1f, 1f); // reset its scale back to 1
            ////TODO: THEN SET STATE TO WHAT??
            break;
          }
        case EnemyState.WAITING_TO_RESPAWN:
          {
            //https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
            if (!startedWaiting)
            {
              Wait(GetRespawnWaitDelay(), () =>
              {
                respawnWaitOver = true;
                //Debug.Log(respawnWaitDelay + " seconds is lost forever");
              });
              startedWaiting = true;
            }
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
      LevelManager.Instance.numEnemyKillsInLevel++;
      enemyState = EnemyState.TEMPORARILY_DEAD;
    }
    else //non-lethal hit
    {
      hp--;
      ReactToNonLethalPlayerMissileHit();
      enemyState = EnemyState.ALIVE;
    }
  }

  private void TemporarilyDie()
  {
    enemySpriteRenderer.enabled = false;
    enemyCircleCollider.enabled = false;
    respawnWaitOver = false;
    startedWaiting = false;
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
    }
  }
}