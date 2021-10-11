using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class RecoilTest : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject spriteObj;
  private GameObject spawnedObj;
  public float tspeed = .025f;
  public float ttrans = -.1f;


  void Start()
  {
    //spawnedObj = SimplePool.Spawn(spriteObj, transform.position, transform.rotation);

  }

  // Update is called once per frame
  void Update()
  {
    //print(DOTween.PlayingTweens());

    if (Input.GetKeyDown(KeyCode.D))
    {
      transform.DOMoveY(ttrans, tspeed).SetLoops(2, LoopType.Yoyo);
      //SimplePool.Despawn(spawnedObj);

    }

    if (Input.GetKeyDown(KeyCode.S))
    {
      //spawnedObj = SimplePool.Spawn(spriteObj, transform.position, transform.rotation);
    }
  }
}
