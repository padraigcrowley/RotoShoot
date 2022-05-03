using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core;
using DarkTonic.MasterAudio;

public class PlayerShip : ExtendedBehaviour
{
  Animator PlayerShipGFXAnim;
//  public Animator playerShieldAnimController;
  private float nextActionTime = 0.0f;

  public GameObject playerShield;

  private int currentShipLane = 1; // the lane number = the array index
  
  public Transform playerShipFrontTurret, playerShipLeftTurret, playerShipRightTurret;
  [SerializeField] private Renderer shipSpriteRenderer, playerShipExhaustSpriteRenderer;
  [SerializeField] private GameObject playerMissilePrefab;
  public float playerShipTweenMoveSpeed = .25f;
  public float playerShipTweenRotateSpeed = .25f;
  public bool PlayerShipIntroAnimCompleted = false;
  public bool PlayerShipOutroAnimCompleted = false;
  private bool PlayerShipIntroAnimPlaying = false;
  private bool PlayerShipOutroAnimPlaying = false;
  private bool playerShipMoving = false;
  public CameraShake camShakeScript;
  public GameObject playerShipMissilesParentPool;
  //public bool invulnerable = false;

  public GameObject PlayerShipDamageLarge;
  private GameObject playerMissileInstance, playerMissileLeftTurretInstance, playerMissileRightTurretInstance;
  Quaternion uniqueprojectileRotation = Quaternion.identity;
  Quaternion uniqueprojectileRotationLeftTurret = Quaternion.identity;
  Quaternion uniqueprojectileRotationRightTurret = Quaternion.identity;
  private List<GameObject> playerDeathFXObjects = new List<GameObject>();

  public UltimateStatusBar playerStatusBar;
  public UltimateStatusBar PlayerHPWorldSpaceStatusBar;
  public GameObject playerDeathFX;

  public GameObject deathExplosion;
  private GameObject deathExplosionInstance;
  private Collider shipCollider;
  void Start()
  {

    /* see EnemyFireAtPlayerBehaviour01.cs
     * "angle" is a float, and it's:
     * 90 when you're aiming/firing directly above you, 
     * -90 when firing directly below, 
     * 0 to the right and
     * 180 (-180) to the left 
     * "angle" is the first (x) parameter in the 3 lines below, the Y is always 90 when using UniqueProjectiles in 2D 
     Although... note, ANGLE is inverted here because the player ship is facing down. the enemymissilefiring stuff is facing down*/
    uniqueprojectileRotation.eulerAngles = new Vector3(-90, 90, 0);  
    uniqueprojectileRotationLeftTurret.eulerAngles = new Vector3(-110, 90, 0);  
    uniqueprojectileRotationRightTurret.eulerAngles = new Vector3(-70, 90, 0);  

    transform.position = GameplayManager.Instance.playerShipPos;
    gameObject.SetActive(true);
    PlayerShipGFXAnim = GetComponentInChildren<Animator>();
    shipCollider = GetComponent<Collider>();
    //shipSpriteRenderer.GetComponentInChildren<Renderer>();
    playerShipMissilesParentPool = new GameObject("PlayerShipMissilesParentPoolObject");
    shipCollider.enabled = false;
  }

  void Update()
  {
    //if (Input.GetKeyDown(KeyCode.S))
    //{
    //  print($"Trying to play shield anim");
    //  PlayerShield.SetActive(true);
    //}

    if (PlayerShipIntroAnimCompleted == true)
      PlayerShipIntroAninCompletedEvent();

    switch (GameplayManager.Instance.currentGameState)
    {
      case GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS:

        shipCollider.enabled = false;
        GameplayManager.Instance.playerShipInvulnerable = true;

        //PlayerShipGFXAnim.Play("PlayerShipExhaust");
        if (PlayerShipIntroAnimPlaying == false)
        {
          transform.rotation = Quaternion.identity;
 
          PlayerShipIntroAnimPlaying = true;
          shipSpriteRenderer.enabled = true;
          playerShipExhaustSpriteRenderer.enabled = true;
          //PlayerShipGFXAnim.Play("PlayerShipIntro", -1, 0f);
          PlayerShipGFXAnim.Play("PlayerShipIntro",0);
        PlayerShipGFXAnim.Play("PlayerShipExhaust",1);
        }
        else if (PlayerShipIntroAnimCompleted == true)
        {
          PlayerShipIntroAnimPlaying = false;
          PlayerShipIntroAnimCompleted = false;
          playerShipExhaustSpriteRenderer.enabled = false;
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_IN_PROGRESS;
          GameplayManager.Instance.playerShipInvulnerable = false;

          PlayerHPWorldSpaceStatusBar.EnableStatusBar();
          //UltimateStatusBar.UpdateStatus("PlayerHPWorldSpaceStatusBar", 0f, GameplayManager.Instance.maxPlayerHPLevel);
          GameplayManager.Instance.currentPlayerHP = GameController.Instance.maxPlayerHP;
          UltimateStatusBar.UpdateStatus("PlayerHPWorldSpaceStatusBar", GameplayManager.Instance.currentPlayerHP, GameController.Instance.maxPlayerHP);
          
          if (playerDeathFXObjects.Count != 0) //despawn the player death explosions from the previous playerdeath. slightly weird place to do it, I know but enough time has passed...
					{
            foreach (GameObject go in playerDeathFXObjects)
              if (go!=null)
                SimplePool.Despawn(go);
					}
        }
        break;

      case GameplayManager.GameState.LEVEL_IN_PROGRESS:
        
        PlayerShipIntroAnimCompleted = false;
        shipCollider.enabled = true;
        
        ProcessInputQueue();

        if (GameplayManager.Instance.tripleFirePowerupRemainingDuration >= 0)
				{
          GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.ANGLED_TRIPLE;
          GameplayManager.Instance.tripleFirePowerupRemainingDuration -= Time.deltaTime;
          //print($"In {gameObject.ToString()} DURATION  = {GameplayManager.Instance.tripleFirePowerupRemainingDuration}");
        }
        else
				{
          GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.STRAIGHT_SINGLE;
        }

        if ((Time.time > nextActionTime))  //&& (playerShipMoving == false))
        {
          nextActionTime = Time.time + GameplayManager.Instance.currentPlayerShipFireRate;
          if(GameplayManager.Instance.playerShipFiring)
            CreatePlayerBullets();
        }
        break;

      case GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS:
        if (PlayerShipOutroAnimPlaying == false)
        {
          PlayerHPWorldSpaceStatusBar.DisableStatusBar();
          PlayerShipOutroAnimPlaying = true;
          playerShipExhaustSpriteRenderer.enabled = true;

          //TODO: below make sure the angle is of the ship graphic, not the parent object
          //float angle = 0f - this.gameObject.transform.eulerAngles.z;
          //print("Angle to reach zero: " + angle);
          //StartCoroutine(RotatePlayerShip(this.gameObject, new Vector3(0, 0, angle), .2f));
          //transform.rotation = Quaternion.identity; // reset to face upwards, back to its original rotation.

          PlayerShipGFXAnim.Play("PlayerShipOutro", -1, 0f);
        }
        else if (PlayerShipOutroAnimCompleted == true)
        {
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_COMPLETE;
          PlayerShipOutroAnimPlaying = false;
          PlayerShipOutroAnimCompleted = false;
          shipSpriteRenderer.gameObject.GetComponent<Renderer>().enabled = false;
          shipCollider.enabled = false;
          playerShipExhaustSpriteRenderer.enabled = false;
        }
        break;
      case GameplayManager.GameState.PLAYER_DIED:
        break;
      case GameplayManager.GameState.LEVEL_COMPLETE:
          //transform.rotation = Quaternion.identity; // reset to face upwards, back to its original rotation.
          shipSpriteRenderer.gameObject.GetComponent<Renderer>().enabled = false;
          PlayerShipIntroAnimCompleted = false;
          PlayerShipIntroAnimPlaying = false;
          shipCollider.enabled = false;
        break;
      
      default:
        break;
    }
  }

  private void ProcessInputQueue()
  {
    if ((GameplayManager.Instance.mouseClickQueue.Count != 0) && (!GameplayManager.Instance.playerShipRotating) && !playerShipMoving)
    {
      //print("Prev Ship Rotation: " + this.gameObject.transform.eulerAngles);
      //Queue dbgQueue = MyGameplayManager.Instance.mouseClickQueue;

      float angleToRotate = 0f;
      if ((GameplayManager.Instance.levelControlType == 2) && (playerShipMoving))
        return;
      
      angleToRotate = (float)GameplayManager.Instance.mouseClickQueue.Dequeue();

      //print("this.gameObject.transform.eulerAngles.z + angleToRotate = " + (this.gameObject.transform.eulerAngles.z + angleToRotate));

      foreach (int angle in GameplayManager.Instance.blockedPlayerShipRotationAngles)
      {
        if ((Mathf.Round(this.gameObject.transform.eulerAngles.z + angleToRotate)) == angle)
        {
          return; // don't do the rotate, exit the method
        }
      }

      if ((GameplayManager.Instance.levelControlType == 1) && (GameplayManager.Instance.playerShipMovementAllowed))
      {
        StartCoroutine(RotatePlayerShip(this.gameObject, new Vector3(0, 0, angleToRotate), GameplayManager.Instance.currentPlayerShipRotationDuration));
      }
      else if ((GameplayManager.Instance.levelControlType == 2) && (GameplayManager.Instance.playerShipMovementAllowed))
      {
        if ((angleToRotate < 0) && (currentShipLane + 1 < GameplayManager.Instance.shipLanes.Length)) //right
        {
          StartCoroutine(MovePlayerShip(GameplayManager.Instance.shipLanes[currentShipLane + 1]));
          PlayerShipGFXAnim.Play("PlayerShipRightTurn");
        }
        else if ((angleToRotate > 0) && (currentShipLane - 1 >= 0))
        {
          StartCoroutine(MovePlayerShip(GameplayManager.Instance.shipLanes[currentShipLane - 1])); //left
          PlayerShipGFXAnim.Play("PlayerShipLeftTurn");
        }
      }
    }
  }

  public IEnumerator DoPlayerDeath()
  {
    //print("In Player Death");
    MasterAudio.PlaySound("PlayerShipHitFatal");
    int numExplosions = 4;
    float timeBetweenExplosions = .3f;
    Vector3 explosionPos;

    for (int i = 0; i < numExplosions; i++)
    {
      explosionPos = new Vector3(transform.position.x + Random.Range(-.5f, .5f),
                                  transform.position.y + Random.Range(-.5f, .5f),
                                  transform.position.z);

      deathExplosionInstance = SimplePool.Spawn(deathExplosion, explosionPos, transform.rotation);
      GameObject newParticleEffect = SimplePool.Spawn(playerDeathFX, explosionPos, transform.rotation, transform) as GameObject;
      playerDeathFXObjects.Add(deathExplosionInstance);
      playerDeathFXObjects.Add(newParticleEffect);

      //change the sorting order so the explosion is sorted over the boss sprite. the explosion sorting is set to default layer as that works best for enemy deaths - the enemy death effect doesn't get too obscured by the explosion. And yeah, I know I'm doing a GetComponent here, but it's on boss death so shouldn't matter if it's slow...
      deathExplosionInstance.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "VFX_OverPlayerShip";
      yield return new WaitForSeconds(timeBetweenExplosions);

    }
    
    GameplayManager.Instance.currentGameState = GameplayManager.GameState.PLAYER_DIED;
    shipSpriteRenderer.enabled = false;
    //playerStatusBar.DisableStatusBar();
    PlayerHPWorldSpaceStatusBar.DisableStatusBar();
}

  private void CreatePlayerBullets()
  {
    MasterAudio.PlaySound("ShipMissile");
    switch (GameplayManager.Instance.currentPlayerFiringState)
    {
      case GameplayManager.PlayerFiringState.STRAIGHT_SINGLE:
      case GameplayManager.PlayerFiringState.RAPID_FIRE_SINGLE:
        playerMissileInstance = SimplePool.Spawn(playerMissilePrefab, playerShipFrontTurret.position, Quaternion.identity, playerShipMissilesParentPool.transform);
        playerMissileInstance.transform.localRotation = uniqueprojectileRotation; //v.important line!!!
        break;
      case GameplayManager.PlayerFiringState.ANGLED_TRIPLE:
        playerMissileInstance = SimplePool.Spawn(playerMissilePrefab, playerShipFrontTurret.position, uniqueprojectileRotation, playerShipMissilesParentPool.transform);
        playerMissileInstance.transform.localRotation = uniqueprojectileRotation; //v.important line!!!

        playerMissileLeftTurretInstance = SimplePool.Spawn(playerMissilePrefab, playerShipLeftTurret.position, uniqueprojectileRotationLeftTurret, playerShipMissilesParentPool.transform);
        playerMissileLeftTurretInstance.transform.localRotation = uniqueprojectileRotationLeftTurret; //v.important line!!!

        playerMissileRightTurretInstance = SimplePool.Spawn(playerMissilePrefab, playerShipRightTurret.position, uniqueprojectileRotationRightTurret, playerShipMissilesParentPool.transform);
        playerMissileRightTurretInstance.transform.localRotation = uniqueprojectileRotationRightTurret; //v.important line!!!
        
        break;
      default:
        break;
    }
  }

  IEnumerator MovePlayerShip(Vector2 newPos)
  {
    float speed = 2.5f;
    float step = speed * Time.deltaTime; // calculate distance to move
    float oldX = transform.position.x;

    Tween myTween;

    if (playerShipMoving)      yield break; // if playership is already moving, return, do nothing.
    
    //validate the possible move before it's made
    if (oldX < newPos.x) // don't go past either boundary
    {
      if (currentShipLane + 1 > 3) yield break;
    }
    else
      if (currentShipLane - 1 < 0) yield break;

    playerShipMoving = true;
    //print($"PlayerShipMoving: {playerShipMoving }");

    //while (transform.position.x != newPos.x)
    //{
    //  transform.position = Vector3.MoveTowards(transform.position, newPos, step);
    //  //Debug.Log($"CurrX: {transform.position.x} DestX: {newPos.x}");
    //  yield return null;
    //}

    myTween = transform.DOMove(new Vector3(newPos.x, newPos.y, 0), playerShipTweenMoveSpeed).SetEase(Ease.OutQuad);
    //myTween = transform.DOMove(new Vector3(newPos.x, newPos.y, 0), .25f).SetEase(Ease.OutQuad);

    yield return myTween.WaitForCompletion();
    // This log will happen after the tween has completed
    //Debug.Log("Move Tween completed!");
    //print($"New ship pos: {newPos}");

    GameplayManager.Instance.playerShipPos = newPos;
    playerShipMoving = false;
    //print($"PlayerShipMoving: {playerShipMoving }");
    ////(oldX < newPos.x) ? currentShipLane+=1 : currentShipLane-=1;
    if (oldX < newPos.x)
      currentShipLane++;
    else
      currentShipLane--;
  }

  IEnumerator RotatePlayerShip(GameObject gameObjectToMove, Vector3 eulerAngles, float duration)  //https://stackoverflow.com/questions/37586407/rotate-gameobject-over-time/37588536
  {
    if (GameplayManager.Instance.playerShipRotating)
    {
      yield break;
    }
    GameplayManager.Instance.playerShipRotating = true;
   
    Vector3 newRot = gameObjectToMove.transform.eulerAngles + eulerAngles;
    Vector3 currentRot = gameObjectToMove.transform.eulerAngles;
    //print("PrevRot: " + currentRot + " NewRot: " + newRot);

    float counter = 0;

    //http://dotween.demigiant.com/documentation.php
    Tween myTween = transform.DORotate( eulerAngles,  .35f, RotateMode.WorldAxisAdd ).SetEase(Ease.InOutSine);
    yield return myTween.WaitForCompletion();
    // This log will happen after the tween has completed
    //Debug.Log("Rotate Tween completed!");

    //while (counter < duration)
    //{
    //  counter += Time.deltaTime;
    //  gameObjectToMove.transform.eulerAngles = Vector3.Lerp(currentRot, newRot, counter / duration);
    //  yield return null;
    //}
    GameplayManager.Instance.playerShipRotating = false;
    
  }

  

  public void PlayerShipIntroAninStartedEvent()
  {
    // needed any more?
    // PlayerShipIntroAninCompleted = false;

  }
  public void PlayerShipIntroAninCompletedEvent()
  {
    //PlayerShipIntroAnimCompleted = true;

  }
  
  private void OnTriggerEnter(Collider collision)
  {
    if((!GameplayManager.Instance.playerShipInvulnerable) && (!GameplayManager.Instance.playerShieldVisible))
    {
      if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
      {
        if ((collision.gameObject.tag.Equals("Enemy01")) || (collision.gameObject.tag.Equals("EnemyMissile")) || (collision.gameObject.tag.Equals("EnemyMissileMedium")) || (collision.gameObject.tag.Equals("EnemyMissileSmall")) || (collision.gameObject.tag.Equals("EnemyMissileLarge")))
        {
          //GameObject newParticleEffect = GameObject.Instantiate(PlayerShipDamageLarge, transform.position, PlayerShipDamageLarge.transform.rotation, transform ) as GameObject;
          GameObject newParticleEffect = SimplePool.Spawn(PlayerShipDamageLarge, collision.gameObject.transform.position, PlayerShipDamageLarge.transform.rotation, transform) as GameObject;

          Wait(2, () =>
          {
            //Debug.Log("Despawn(newParticleEffect)");
            SimplePool.Despawn(newParticleEffect);
          });

          //print($"collision between this {transform.position} and other {collision.gameObject.transform.position}");
          DoCameraShake();
          if ((collision.gameObject.tag.Equals("Enemy01")) || (collision.gameObject.tag.Equals("EnemyMissile")))
            ChangeShipHP(-(int)LevelManager.Instance.LevelStats["EnemyMissileDamage"]);
          else if (collision.gameObject.tag.Equals("EnemyMissileLarge"))
            ChangeShipHP(-(int)LevelManager.Instance.LevelStats["EnemyMissileDamage"] * 10);
          else if (collision.gameObject.tag.Equals("EnemyMissileMedium"))
            ChangeShipHP(-(int)(LevelManager.Instance.LevelStats["EnemyMissileDamage"] * 1.5f));
          else if (collision.gameObject.tag.Equals("EnemyMissileSmall"))
            ChangeShipHP(-(int)LevelManager.Instance.LevelStats["EnemyMissileDamage"] / 3);


        }

        if ((collision.gameObject.tag.Equals("Asteroid")))
        {
          GameObject newParticleEffect = SimplePool.Spawn(PlayerShipDamageLarge, collision.gameObject.transform.position, PlayerShipDamageLarge.transform.rotation, transform) as GameObject;

          Wait(2, () =>
          {
            //Debug.Log("Despawn(newParticleEffect)");
            SimplePool.Despawn(newParticleEffect);
          });
          //print($"collision between this {transform.position} and asteroid {collision.gameObject.transform.position}");
          DoCameraShake();
          ChangeShipHP(-(int)LevelManager.Instance.LevelStats["EnemyMissileDamage"]);
        }
      }
    }
  }

  //could be increment or decrement;
  public void ChangeShipHP(int hpChange)
  {
    if ((GameplayManager.Instance.currentPlayerHP + hpChange) > GameController.Instance.maxPlayerHP)
      GameplayManager.Instance.currentPlayerHP = GameController.Instance.maxPlayerHP; 
    else
      GameplayManager.Instance.currentPlayerHP += hpChange; //looks weird but negattive numbers are passed in for a decrease in HP
    
    //UltimateStatusBar.UpdateStatus("playerStatusBar", GameplayManager.Instance.currentPlayerHP, GameplayManager.Instance.maxPlayerHP);
    UltimateStatusBar.UpdateStatus("PlayerHPWorldSpaceStatusBar", GameplayManager.Instance.currentPlayerHP, GameController.Instance.maxPlayerHP);
    if(hpChange < 0)
      MasterAudio.PlaySound("PlayerShipHitNonFatal");
  }

 
  void DoCameraShake()
  {
    camShakeScript.CameraShakeOnPlayerHit();
  }

}
