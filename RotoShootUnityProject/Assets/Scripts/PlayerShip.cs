using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core;

public class PlayerShip : MonoBehaviour
{
  Animator PlayerShipGFXAnim;
  private float nextActionTime = 0.0f;
  
  private int currentShipLane = 1; // the lane number = the array index
  
  public Transform playerShipFrontTurret, playerShipLeftTurret, playerShipRightTurret;
  [SerializeField] private Renderer shipSpriteRenderer;
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

  void Start()
  {
    //camShakeScript = GetComponent<CameraShake>();
    //if (camShakeScript == null)    
    //  Debug.LogWarning("CameraShake returned NULL");

    transform.position = GameplayManager.Instance.playerShipPos;
    gameObject.SetActive(true);
    PlayerShipGFXAnim = GetComponentInChildren<Animator>();
    shipSpriteRenderer.GetComponentInChildren<Renderer>();
    playerShipMissilesParentPool = new GameObject("PlayerShipMissilesParentPoolObject");

  }

  void Update()
  {

    if (PlayerShipIntroAnimCompleted == true)
      PlayerShipIntroAninCompletedEvent();

    switch (GameplayManager.Instance.currentGameState)
    {
      case GameplayManager.GameState.LEVEL_INTRO_IN_PROGRESS:
        if (PlayerShipIntroAnimPlaying == false)
        {
          transform.rotation = Quaternion.identity;
 
          PlayerShipIntroAnimPlaying = true;
          shipSpriteRenderer.enabled = true;
          PlayerShipGFXAnim.Play("PlayerShipIntro", -1, 0f);
        }
        else if (PlayerShipIntroAnimCompleted == true)
        {
          PlayerShipIntroAnimPlaying = false;
          // not needed?  PlayerShipGFXAnim.StopPlayback();// ("PlayerShipIntro", -1, 0f);
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_IN_PROGRESS;
          //print("gamestate is now set to LEVEL_IN_PROGRESS! ");
        }
        break;

      case GameplayManager.GameState.LEVEL_IN_PROGRESS:
        PlayerShipIntroAnimCompleted = false;
        ProcessInputQueue();
        
        if ((Time.time > nextActionTime)  && (playerShipMoving == false))
        {
          nextActionTime = Time.time + GameplayManager.Instance.currentPlayerShipFireRate;
          CreatePlayerBullets();
        }
        break;

      case GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS:
        if (PlayerShipOutroAnimPlaying == false)
        {
          PlayerShipOutroAnimPlaying = true;

          //TODO: below make sure the angle is of the ship graphic, not the parent object
          //float angle = 0f - this.gameObject.transform.eulerAngles.z;
          //print("Angle to reach zero: " + angle);
          //StartCoroutine(RotatePlayerShip(this.gameObject, new Vector3(0, 0, angle), .2f));
          //transform.rotation = Quaternion.identity; // reset to face upwards, back to its original rotation.

          PlayerShipGFXAnim.Play("PlayerShipOutro", -1, 0f);
        }
        else if (PlayerShipOutroAnimCompleted == true)
        {
          PlayerShipOutroAnimPlaying = false;
          GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_COMPLETE;
        }
        break;
      case GameplayManager.GameState.LEVEL_COMPLETE:
          //transform.rotation = Quaternion.identity; // reset to face upwards, back to its original rotation.
          shipSpriteRenderer.gameObject.GetComponent<Renderer>().enabled = false;
          PlayerShipIntroAnimCompleted = false;
          
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

      if (GameplayManager.Instance.levelControlType == 1)
      {
        StartCoroutine(RotatePlayerShip(this.gameObject, new Vector3(0, 0, angleToRotate), GameplayManager.Instance.currentPlayerShipRotationDuration));
      }
      else if (GameplayManager.Instance.levelControlType == 2)
      {
        if ((angleToRotate < 0) && (currentShipLane + 1 < GameplayManager.Instance.shipLanes.Length))
        {
          StartCoroutine(MovePlayerShip(GameplayManager.Instance.shipLanes[currentShipLane + 1]));
          //print($"Current Ship lane: {currentShipLane}");
        }
        else if ((angleToRotate > 0) && (currentShipLane - 1 >= 0))
        {
          StartCoroutine(MovePlayerShip(GameplayManager.Instance.shipLanes[currentShipLane - 1]));
          //print($"Current Ship lane: {currentShipLane}");
        }
      }
    }
  }

  private void CreatePlayerBullets()
  {
    switch(GameplayManager.Instance.currentPlayerFiringState)
    {
      case GameplayManager.PlayerFiringState.STRAIGHT_SINGLE:
      case GameplayManager.PlayerFiringState.RAPID_FIRE_SINGLE:
        SimplePool.Spawn(playerMissilePrefab, playerShipFrontTurret.position, playerShipFrontTurret.rotation,playerShipMissilesParentPool.transform);
        break;
      case GameplayManager.PlayerFiringState.ANGLED_TRIPLE:
        SimplePool.Spawn(playerMissilePrefab, playerShipFrontTurret.position, playerShipFrontTurret.rotation, playerShipMissilesParentPool.transform);
        SimplePool.Spawn(playerMissilePrefab, playerShipLeftTurret.position, playerShipLeftTurret.rotation, playerShipMissilesParentPool.transform);
        SimplePool.Spawn(playerMissilePrefab, playerShipRightTurret.position, playerShipRightTurret.rotation, playerShipMissilesParentPool.transform);
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

    if (playerShipMoving)      yield break;
    
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
  
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if ((collision.gameObject.tag.Equals("Enemy01")) || (collision.gameObject.tag.Equals("EnemyMissile")))
      {
        print($"collision between this {transform.position} and other {collision.gameObject.transform.position}");
        DoCameraShake();
      }
    }
  }
  void DoCameraShake()
  {
    camShakeScript.CameraShakeOnPlayerHit();
  }

}
