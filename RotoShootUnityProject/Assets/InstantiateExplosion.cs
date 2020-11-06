using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateExplosion : MonoBehaviour
{
  public GameObject atmosExplosion;
  public GameObject atmosExplosionInstance;
  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      atmosExplosionInstance = SimplePool.Spawn(atmosExplosion, transform.position, transform.rotation);
    }
    }
}
