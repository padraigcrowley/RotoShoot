using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUniqueProjectiles : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject effectToSpawn, playerMissilePrefab;


  void Start()
  {
    

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.D))
      SpawnVFX();
  }

  public void SpawnVFX()
  {
    GameObject vfx;
    //vfx = Instantiate(effectToSpawn);
    
    SimplePool.Spawn(playerMissilePrefab, new Vector3(0f, -8f, 0f), Quaternion.identity);
    //vfx = Instantiate(effectToSpawn, new Vector3(0f, -8f, 0f), Quaternion.identity);
    
    //vfx.transform.localRotation = ;
  }

}
