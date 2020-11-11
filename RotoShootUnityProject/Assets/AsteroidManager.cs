using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
  private GameObject asteroids1Instance, asteroids2Instance;
  public GameObject asteroids1Prefab, asteroids2Prefab;
  // Start is called before the first frame update
  void Start()
    {
        
    }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      asteroids1Instance = SimplePool.Spawn(asteroids1Prefab, transform.position, transform.rotation);
    }
  }
}
