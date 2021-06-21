using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy type that follows waypoint path
/// </summary>

public class Enemy02_0003 : EnemyBehaviour02
{
  public bool startedOnPath = false;

  //private Renderer spriteMaterial;
  protected override void Start()
  {

    //spriteMaterial = GetComponent<Renderer>();
    base.Start();
    //Debug.Log("Enemy02_0002 START method");
    //InvokeRepeating("FireMissileAtPlayerPos", 3, 5);
  }
  protected override  void Update()
  {
    base.Update();
    //Debug.Log("Enemy02_0002 update method");
  }

  public override void DoMovement()
  {
    if (!startedOnPath)
    {
      splineMoveScript.StartMove();
      startedOnPath = true;
    }
  }

  public override void StopMovement()
  {
    splineMoveScript.Stop();
    startedOnPath = false;
  }

  protected override void DoExplode()
  {
    deathExplosionInstance = SimplePool.Spawn(deathExplosion, this.transform.position, this.transform.rotation, enemyExplosionsPool.transform);
    int randScaleFlip = UnityEngine.Random.Range(0, 4);// not scaleflipped, scaledFlippedX, scaledFlippedY, scaledFlippedXandY
    switch (randScaleFlip)
    {
      case (0):
        deathExplosionInstance.transform.localScale = new Vector3(1f, 1f, 1f);
        break;
      case (1):
        deathExplosionInstance.transform.localScale = new Vector3(-1f, .9f, 1f);
        break;
      case (2):
        deathExplosionInstance.transform.localScale = new Vector3(1.1f, -1.1f, 1f);
        break;
      case (3):
        deathExplosionInstance.transform.localScale = new Vector3(-.9f, -.9f, 1f);
        break;
      default:
        break;
    }
  }
  public override void ReactToNonLethalPlayerMissileHit()
  {
    StartCoroutine(DoHitEffect());
    //transform.localScale *= 1.2f; // scale slightly up to show they've been shot
  }

  IEnumerator DoHitEffect()
  {
    float duration = .2f;
    float elapsedTime = 0f;
    float currentEffectBlendVal;
    float startEffectBlendVal = 0;
    float endEffectBlendVal = .4f;

    while (elapsedTime <= duration)
    {
      currentEffectBlendVal = Mathf.Lerp(startEffectBlendVal, endEffectBlendVal, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_HitEffectBlend", currentEffectBlendVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    while (elapsedTime <= duration)
    {
      currentEffectBlendVal = Mathf.Lerp(endEffectBlendVal, startEffectBlendVal, (elapsedTime / duration));
      spriteMaterial.material.SetFloat("_HitEffectBlend", currentEffectBlendVal);
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    spriteMaterial.material.SetFloat("_HitEffectBlend", startEffectBlendVal); // just to make sure it ends up back at its initial/start value
  }



}
