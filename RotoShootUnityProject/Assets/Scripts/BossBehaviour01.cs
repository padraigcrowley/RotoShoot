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
    if (bossAppeared)
    {
      splineMoveScript.StartMove();
      bossAppeared = false; // just to make it stop executing the startmove() again
    }

  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    List<Collider2D> collisions = new List<Collider2D>();

  int numCollisionContacts = -1;
    
    ///Collider2D[] contacts = new Collider2D[5];
    numCollisionContacts = collider.GetContacts(collisions);
    if (numCollisionContacts == 2)
    {
      print($"numCollisionContacts = {numCollisionContacts}");
    }

    foreach (Collider2D collision in collisions)
    {
      if (collision.gameObject.CompareTag("BossVulnerable") && collider.gameObject.CompareTag("PlayerMissile"))
      {
        print("HIT BOSS ORB!");
        StartCoroutine(BossTakesDamageEffect(.5f));
      }
      //if (collision.gameObject.CompareTag("BossInvulnerable")&& collider.gameObject.CompareTag("PlayerMissile")))
      //print("HIT BOSS BODY!");
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