using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TweenDespawnTest : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject spriteObj;
  private GameObject spawnedObj;
  

  void Start()
  {
    //spawnedObj = SimplePool.Spawn(spriteObj, transform.position, transform.rotation);
    
  }

  // Update is called once per frame
  void Update()
  {
    print(DOTween.PlayingTweens());
    
    if (Input.GetKeyDown(KeyCode.D))
    {
      SimplePool.Despawn(spawnedObj);

    }

    if (Input.GetKeyDown(KeyCode.S))
    {
      spawnedObj = SimplePool.Spawn(spriteObj, transform.position, transform.rotation);
    }
  }
}
