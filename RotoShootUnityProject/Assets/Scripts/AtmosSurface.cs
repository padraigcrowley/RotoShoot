using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosSurface : ExtendedBehaviour
{
  public GameObject atmosExplosion;
  public GameObject atmosExplosionInstance;
  // Start is called before the first frame update
  GameObject parentPool;

  public CameraShake camShakeScript;

  void Start()
  {
     parentPool = new GameObject("ExplosionsParentPoolObject");
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (collision.gameObject.tag.Equals("Enemy01"))
      {
        atmosExplosionInstance = SimplePool.Spawn(atmosExplosion, collision.transform.position, collision.transform.rotation, parentPool.transform);
        camShakeScript.CameraShakeOnPlayerHit();



      }
    }
  }
}
