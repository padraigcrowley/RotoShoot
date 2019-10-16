using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBehaviour : ExtendedBehaviour
//https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
{

  private float speed;
  private float hp;
  public float speedMultiplierFromSpawner = 1f;
  public float hpMultiplierFromSpawner = 1f;

  private float startPosX, startPosY, startPosZ;
  private float startScaleX, startScaleY, startScaleZ;

  private float initialHP, initialSpeed;
  private bool readyToRespawn;

  //----------------------
  public abstract void ReactToPlayerMissileHit();

  void Start()
  {
    //todo - move this out to GameplayManager or to sub-class or prefab??
    speed = .25f;
    hp = 1f;
    initialSpeed = speed * speedMultiplierFromSpawner; // should these be set in this class, not MyGameplayManager??
    initialHP = hp * hpMultiplierFromSpawner;
    speed = initialSpeed;
    hp = initialHP;

    readyToRespawn = true;

    // rotate enemy to face player ship https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
    Vector3 dir = GameplayManager.Instance.playerShipPos - transform.position;
    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
    startPosX = transform.position.x;
    startPosY = transform.position.y;
    startPosZ = transform.position.z;
    startScaleX = transform.localScale.x;
    startScaleY = transform.localScale.y;
    startScaleZ = transform.localScale.z;
  }

  void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      // Move our position a step closer to the target.
      float step = speed * Time.deltaTime; // calculate distance to move
      transform.position = Vector3.MoveTowards(transform.position, GameplayManager.Instance.playerShipPos, step);
    }
    else if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.GAME_OVER_SCREEN)
    {
      hp = initialHP; //reset health and position
      speed = initialSpeed;
      transform.position = new Vector3(startPosX, startPosY, startPosZ);
      transform.localScale = new Vector3(startScaleX, startScaleX, startScaleX); // reset its scale  
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (collision.gameObject.tag.Equals("PlayerMissile"))
      {
        if (hp <= 1) //lethal hit
        {
          Destroy(collision.gameObject);//destroy the missile object
          LevelManager.Instance.numEnemyKillsInLevel++;

          //https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
          Wait(5, () => {
            Debug.Log("5 seconds is lost forever");
          });

          if (!readyToRespawn)
          {
            hp = initialHP; //reset health and position
            transform.position = new Vector3(startPosX, startPosY, startPosZ);
            transform.localScale = new Vector3(startScaleX, startScaleX, startScaleX); // reset its scale back to original scale
          }
          
        }
        else //non-lethal hit
        {
          Destroy(collision.gameObject);//destroy the missile object
          hp--;
          ReactToPlayerMissileHit();
        }
      }
      else if (collision.gameObject.tag.Equals("Player"))
      {
        GameplayManager.Instance.currentPlayerHP--;
        hp = initialHP; //reset health and position
        transform.position = new Vector3(startPosX, startPosY, startPosZ);
        transform.localScale = new Vector3(1f, 1f, 1f); // reset its scale back to 1
      }
    }
  }

  
  
}
