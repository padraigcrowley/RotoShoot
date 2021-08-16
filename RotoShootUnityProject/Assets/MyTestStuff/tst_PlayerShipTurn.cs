using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class tst_PlayerShipTurn : MonoBehaviour
{
  public Animator RedShipTurning;
  Animator PlayerShipGFXAnim;
  private float nextActionTime = 0.0f;

  private int currentShipLane = 1; // the lane number = the array index
  public Vector2[] shipLanes = new[] { new Vector2(-3.85f, -6f), new Vector2(-1.29f, -6f), new Vector2(1.29f, -6f), new Vector2(3.84f, -6f) };

  public Transform playerShipFrontTurret, playerShipLeftTurret, playerShipRightTurret;
  [SerializeField] private Renderer shipSpriteRenderer;
  [SerializeField] private GameObject playerMissilePrefab;
  public float playerShipTweenMoveSpeed = 10f;
  public float playerShipTweenRotateSpeed = .25f;
  public bool PlayerShipIntroAnimCompleted = false;
  public bool PlayerShipOutroAnimCompleted = false;
  private bool PlayerShipIntroAnimPlaying = false;
  private bool PlayerShipOutroAnimPlaying = false;
  private bool playerShipMoving = false;
  [HideInInspector] public Vector3 playerShipPos;
  public CameraShake camShakeScript;
  public GameObject playerShipMissilesParentPool;

  // Start is called before the first frame update
  void Start()
    {
        
    }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.A))
    {
      RedShipTurning.Play("RedPlayerShipTurnLeft");
      StartCoroutine(MovePlayerShip(shipLanes[currentShipLane - 1]));
      currentShipLane--;
    }

    if (Input.GetKeyDown(KeyCode.D)) 
    {
      Flip();
      StartCoroutine(MovePlayerShip(shipLanes[currentShipLane + 1]));
      RedShipTurning.Play("RedPlayerShipTurnLeft");
      currentShipLane++;
    }
  }
  void Flip()
  {
  //https://stackoverflow.com/questions/26568542/flipping-a-2d-sprite-animation-in-unity-2d
    // Switch the way the player is labelled as facing
    //facingRight = !facingRight;

    // Multiply the player's x local scale by -1
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }

  IEnumerator MovePlayerShip(Vector2 newPos)
  {
    float speed = 2.5f;
    float step = speed * Time.deltaTime; // calculate distance to move
    float oldX = transform.position.x;

    Tween myTween;

    if (playerShipMoving) yield break;

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

    playerShipPos = newPos;
    playerShipMoving = false;
    //print($"PlayerShipMoving: {playerShipMoving }");
    ////(oldX < newPos.x) ? currentShipLane+=1 : currentShipLane-=1;
    if (oldX < newPos.x)
      currentShipLane++;
    else
      currentShipLane--;
  }

}
