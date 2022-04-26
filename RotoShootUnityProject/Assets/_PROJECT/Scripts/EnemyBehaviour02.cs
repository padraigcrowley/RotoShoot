using UnityEngine;
using DG.Tweening;
using System;
using SWS; //simple waypoints
using System.Collections;
using DarkTonic.MasterAudio;

public abstract class EnemyBehaviour02 : ExtendedBehaviour
//https://answers.unity.com/questions/379440/a-simple-wait-function-without-coroutine-c.html
{
  protected float speed; //"protected" to allow abstract sub-class to access it
  protected float hp;
  public float speedMultiplierFromSpawner = 1f;
  public float hpMultiplierFromSpawner = 1f;
  //private float respawnWaitDelay = 6.0f; //default value unless overridden by derived class - no longer used, handled in LeveManager

  public float startPosX, startPosY, startPosZ = 0f;
  private float startScaleX, startScaleY, startScaleZ;

  protected float initialHP, initialSpeed; //"protected" to allow abstract sub-class to access it
  private bool enemyHitByPlayerMissile;

  public bool ignoreHueShift;

  private GameObject missileObject;
  private SpriteRenderer enemySpriteRenderer;
  private CapsuleCollider enemyCapsuleCollider;
  protected Vector3 upDirection;

  protected Renderer spriteMaterial;
  public GameObject deathExplosion;
  public GameObject deathExplosionInstance;
  public GameObject enemyExplosionsPool;

  public enum EnemyState {  ALIVE, TEMPORARILY_DEAD, WAITING_TO_RESPAWN, INVINCIBLE, FULLY_DYING, FULLY_DEAD, HIT_BY_PLAYER_MISSILE, HIT_BY_PLAYER_SHIP, 
                            HIT_BY_PLAYER_SHIELD, HIT_BY_ATMOSPHERE }

  public EnemyState enemyState;
  public bool waveRespawnWaitOver;
  private bool timeBetweenSpawnPassed = true;
  private bool startedWaiting;
  public float timeBetweenSpawn;

  private bool burnFadeEffectComplete = true;

  public GameObject[] availablePowerUps;
  public GameObject powerUpInstance;

  public PlayerShip playerShip;

  public SWS.PathManager waypointPath;
  protected splineMove splineMoveScript;
  private Tween tw1, tw2, tw3;
  bool previouslyDidAHitEffectTween = false;

  public EnemyFireAtPlayerBehaviour01 enemyFireAtPlayerBehaviour01;

  public UltimateStatusBar Enemy02HPWorldSpaceStatusBar;


  // virtual public float GetRespawnWaitDelay() => respawnWaitDelay;

  public virtual void ReactToPlayerMissileHit()
  {
    if (previouslyDidAHitEffectTween)
    {
      if (tw3.IsActive())
      {
        if (!tw3.IsPlaying())
          DoHitEffect();
      }
			else
			{
        DoHitEffect();
      }
    }
		else
		{
      DoHitEffect();
      previouslyDidAHitEffectTween = true;
    }
     

  }
  private void DoHitEffect()
  {
    float duration = .05f;
    float hitEffectBlendValue = 1f;
    //enemySpriteMaterial.material.SetFloat("_HsvBright", currHueSatBoostValue);
    tw3 = spriteMaterial.material.DOFloat(hitEffectBlendValue, "_HitEffectBlend", duration).SetEase(Ease.OutQuart).SetLoops(2, LoopType.Yoyo);
    
  }
  
  private void TempHueSaturationBoost()
  {
    float duration = .1f;
    float finishHueSatBoostValue = 2.0f;
    //enemySpriteMaterial.material.SetFloat("_HsvBright", currHueSatBoostValue);
    tw1 = spriteMaterial.material.DOFloat(finishHueSatBoostValue, "_HsvBright", duration).SetEase(Ease.OutQuart).SetLoops(2, LoopType.Yoyo);
    tw2 = spriteMaterial.material.DOFloat(finishHueSatBoostValue, "_HsvSaturation", duration).SetEase(Ease.OutQuart).SetLoops(2, LoopType.Yoyo);
  }
  public abstract void DoMovement();

  public abstract void StopMovement();

  protected virtual void Start()
  {
    InitialSetup();
  }
  private void InitialSetup()
  {
    //todo - move this out to GameplayManager or to sub-class or enemy prefab??
        
    hp = LevelManager.Instance.LevelStats["EnemyHP"];
    speed = LevelManager.Instance.LevelStats["EnemySpeed"];
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
    enemyExplosionsPool = GameObject.FindGameObjectWithTag("EnemyExplosionsPool");

    splineMoveScript = GetComponent<splineMove>();
    if (splineMoveScript != null)
    {
      splineMoveScript.pathContainer = waypointPath;
      splineMoveScript.speed = this.speed;
      //splineMoveScript.loopType = splineMove.LoopType.loop;
      //splineMoveScript.closeLoop = true;
    }
    //else
    //{
    //  Debug.LogError($"ERROR! no splineMove component found on {this.name}");
    //}

    spriteMaterial.material.SetFloat("_FadeAmount", 0f);
   



  }

  protected virtual void Update()
  {
		

		if ((GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS) ||
				(GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DYING) ||
				(GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DIED) ||
        (GameplayManager.Instance.currentGameState == GameplayManager.GameState.WAITING_FOR_PLAYERDIED_BUTTONS) ||
        (GameplayManager.Instance.currentGameState == GameplayManager.GameState.EXITING_LEVEL) ||
        (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS))
		//if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS) 
		{

      if (Input.GetKeyDown(KeyCode.S))
      {
        TemporarilyDie();
      }


      switch (enemyState)
      {
        case EnemyState.ALIVE:
          {
            enemyCapsuleCollider.enabled = true;
            enemySpriteRenderer.enabled = true;
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
            ReactToPlayerMissileHit();
            HandleDamage();

            break;
          }
        case EnemyState.HIT_BY_PLAYER_SHIP:
        case EnemyState.HIT_BY_PLAYER_SHIELD:
          {
            //if ((!GameplayManager.Instance.playerShipInvulnerable) && (!GameplayManager.Instance.playerShieldVisible))
            //{
            //  playerShip.ChangeShipHP(-20);
            //}
            TemporarilyDie();
            break;
          }
        case EnemyState.HIT_BY_ATMOSPHERE:
          {
            //playerShip.ChangeShipHP(-10);
            TemporarilyDie();
            break;
          }
        case EnemyState.WAITING_TO_RESPAWN:
          {
            enemyCapsuleCollider.enabled = false;
            if (burnFadeEffectComplete == true)
              enemySpriteRenderer.enabled = false;
            else
              break;
            if ((waveRespawnWaitOver))// && (burnFadeEffectComplete == true))
            //if (waveRespawnWaitOver)
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
        case EnemyState.FULLY_DYING:
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
      if (enemyState == EnemyState.FULLY_DEAD)
        return;
      if (enemyState == EnemyState.ALIVE) // if the enemy is still active, alive, visible on-screen - do the fade-out dissolve, then destroy
      {
        enemyState = EnemyState.FULLY_DYING;
        FullyDie();
        return;
      }
      if ((enemyState == EnemyState.TEMPORARILY_DEAD) || (enemyState == EnemyState.WAITING_TO_RESPAWN) || (enemyState == EnemyState.HIT_BY_ATMOSPHERE))
      {
        enemyState = EnemyState.FULLY_DEAD;
        Destroy(gameObject);
        return;
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
    //hp -= GameplayManager.Instance.PlayerMissileDamage;
    hp -= GameController.Instance.playerMissileDamage;

    Enemy02HPWorldSpaceStatusBar.UpdateStatus(hp, initialHP);

    if (hp <= 0) //lethal hit
    {
      enemyState = EnemyState.TEMPORARILY_DEAD;
    }
    else //non-lethal hit
    {
      enemyState = EnemyState.ALIVE;  //??
      Enemy02HPWorldSpaceStatusBar.EnableStatusBar();
      Enemy02HPWorldSpaceStatusBar.UpdateStatus(hp, initialHP);
    }
  }

  private void DropPowerUp()
  {
    int index = UnityEngine.Random.Range(0, availablePowerUps.Length);
    //index = 2;
    powerUpInstance = SimplePool.Spawn(availablePowerUps[index], transform.position, transform.rotation);
  }

  public void TemporarilyDie()
  {
    MasterAudio.PlaySound("EnemyDeath01");

    Wait(.05f, () => {
      //Debug.Log("5 seconds is lost forever");
      StopMovement();
    });

    Enemy02HPWorldSpaceStatusBar.DisableStatusBar();

    if (enemyState == EnemyState.HIT_BY_PLAYER_SHIELD)
    { 
      StartCoroutine(DoBurnFadeEffect(.75f, 0f, 1f));
      DoExplode();

      LevelManager.Instance.numEnemyKillsInLevel++;
      UIManager.Instance.CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
      GameplayManager.Instance.totalEnemyKillCount++;
    }
    else if(enemyState != EnemyState.HIT_BY_ATMOSPHERE)
		{

      StartCoroutine(DoBurnFadeEffect(.75f, 0f, 1f));
      DoExplode();

      LevelManager.Instance.numEnemyKillsInLevel++;
      GameplayManager.Instance.totalEnemyKillCount++;
      UIManager.Instance.CurrentEnemyKillCount.text = LevelManager.Instance.numEnemyKillsInLevel.ToString();
      if (GameplayManager.Instance.totalEnemyKillCount % GameplayManager.Instance.enemyKillPowerUpDropFrequency == 0)
      {
        DropPowerUp();
      }
		}

    enemyCapsuleCollider.enabled = false;
    waveRespawnWaitOver = false;
    startedWaiting = false;

    enemyState = EnemyState.WAITING_TO_RESPAWN;
  }

  protected virtual void DoExplode()
	{
    deathExplosionInstance = SimplePool.Spawn(deathExplosion, this.transform.position, this.transform.rotation, enemyExplosionsPool.transform);
    int randScaleFlip = UnityEngine.Random.Range(0, 4);// not scaleflipped, scaledFlippedX, scaledFlippedY, scaledFlippedXandY
    switch (randScaleFlip)
    {
      case (0):
        deathExplosionInstance.transform.localScale = new Vector3(.5f, .5f, 1f);
        break;
      case (1):
        deathExplosionInstance.transform.localScale = new Vector3(-.5f, .4f, 1f);
        break;
      case (2):
        deathExplosionInstance.transform.localScale = new Vector3(.6f, -.6f, 1f);
        break;
      case (3):
        deathExplosionInstance.transform.localScale = new Vector3(-.4f, -.4f, 1f);
        break;
      default:
        break;
    }
  }
  
  private void FullyDie()
  {
    StartCoroutine(DoBurnFadeEffect(.75f, 0f, 1f));
    StopMovement();
    enemyState = EnemyState.FULLY_DEAD;
    Destroy(gameObject);
  }

  IEnumerator DoBurnFadeEffect(float duration, float startVal, float endVal)
  {
    float elapsedTime = 0f;
    float currentVal;

    burnFadeEffectComplete = false;

    while (elapsedTime <= duration) //from normal to red
    {
      currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_FadeAmount", currentVal);
      elapsedTime += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    //enemySpriteRenderer.enabled = false;
    burnFadeEffectComplete = true;
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

  private void ResetPositionToTop()
	{
    transform.position = new Vector3(startPosX, startPosY, startPosZ);
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
      else if (collision.gameObject.tag.Equals("PlayerShield"))
      {
        enemyState = EnemyState.HIT_BY_PLAYER_SHIELD;
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
      if ( (collision.gameObject.CompareTag("BoundaryBottom")))
      {
        ResetPositionToTop(); // don't kill them, or reset their HP, just pop them back to the top.
        //enemyState = EnemyState.HIT_BY_ATMOSPHERE;
      }
    }
  }
}
