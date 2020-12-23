using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS; //simple waypoints

public class BossBehaviour01 : MonoBehaviour
{
  public SWS.PathManager waypointPath;
  protected splineMove splineMoveScript;
  private Renderer[] bossSpriteMaterials;
  private bool bossAppeared = false;
  public float startPosX, startPosY, startPosZ;
  public float bossHP = 1;
  public float hpMultiplierFromSpawner;
  public float speedMultiplierFromSpawner;

  public GameObject bossMissile, bossOrb;
  private Quaternion rotation;
  private GameObject bossMissilesParentPool;
  private bool fireAtPlayerPos = true;

  // Start is called before the first frame update
  void Start()
  {
    transform.position = new Vector3(startPosX, startPosY, 0f);
    bossHP *= hpMultiplierFromSpawner;

    bossSpriteMaterials = GetComponentsInChildren<Renderer>();
    StartCoroutine(BossAppearEffect(5f));

    splineMoveScript = GetComponent<splineMove>();
    if (splineMoveScript != null)
    {
      splineMoveScript.pathContainer = waypointPath;
      splineMoveScript.speed *= this.speedMultiplierFromSpawner;
    }

    bossMissilesParentPool = new GameObject("bossMissilesParentPoolObject");
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
    foreach (Renderer sr in bossSpriteMaterials)
    {
      sr.enabled = true;
    }
    bossAppeared = true;
  }
  // Update is called once per frame
  void Update()
  {

    if (Input.GetKeyDown(KeyCode.S))
    {
      fireAtPlayerPos = false;
      FireMissile(fireAtPlayerPos);
    }

    if (bossAppeared)
    {
      splineMoveScript.StartMove();
      bossAppeared = false; // just to make it stop executing the startmove() again
    }

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
    
    firedBullet = SimplePool.Spawn(bossMissile, bossOrb.transform.position, Quaternion.identity, bossMissilesParentPool.transform);
    firedBullet.transform.localRotation = rotation; //v.important line!!!

  }

  void OnTriggerEnter(Collider collider)
  {
    List<Collider> collisions = new List<Collider>();

  int numCollisionContacts = -1;
    
    ///Collider2D[] contacts = new Collider2D[5];
   //numCollisionContacts = collider.GetCon
    if (numCollisionContacts == 2)
    {
      print($"numCollisionContacts = {numCollisionContacts}");
    }

    foreach (Collider collision in collisions)
    {
      // ***TODO - ADD A CHECK IT@S NOT COLLIDING AGAINST "BOUNDARY" !!
      if (collision.gameObject.CompareTag("BossVulnerable") && collider.gameObject.CompareTag("PlayerMissile"))
      {
        print("HIT BOSS ORB!");
        StartCoroutine(BossTakesDamageEffect(.5f));
      }
      //if (collision.gameObject.CompareTag("BossInvulnerable")&& collider.gameObject.CompareTag("PlayerMissile")))
      //print("HIT BOSS BODY!");
    }
  }

  public void OnChildTriggerEntered(Collider other, string childTag)
  {
    if (other.gameObject.CompareTag("PlayerMissile") && childTag.Equals("BossVulnerable"))
    {
      print("HIT BOSS ORB!");
      StartCoroutine(BossTakesDamageEffect(.25f));
    }
  }

  IEnumerator BossTakesDamageEffect(float duration)
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
          currentVal = Mathf.Lerp(0f, 1f, (elapsedTime / duration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }

    elapsedTime = 0f;
    while (elapsedTime <= duration)
    {
      foreach (Renderer sr in bossSpriteMaterials)
      {
        if (sr != null)
        {
          //sr.material.SetFloat("_ChromAberrAmount", 0f);
          currentVal = Mathf.Lerp(1f, 0f, (elapsedTime / duration));
          sr.material.SetFloat("_InnerOutlineAlpha", currentVal);
          elapsedTime += Time.deltaTime;
        }
      }
      //yield return null;
      yield return new WaitForEndOfFrame();
    }
  }

}