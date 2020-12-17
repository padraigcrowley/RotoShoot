using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUniqueProjectiles : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject  playerMissilePrefab, spawnInstance;
  private Vector3 shootingPos;

  void Start()
  {
    shootingPos = new Vector3(0f, -8f, 0f);

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.S))
      SpawnVFX();
    if (Input.GetKeyDown(KeyCode.A))
      shootingPos.x -= .5f;
    if (Input.GetKeyDown(KeyCode.D))
      shootingPos.x += .5f;
  }

  public void SpawnVFX()
  {
    Quaternion thisRotation = Quaternion.identity;
    thisRotation.eulerAngles = new Vector3(-90, 0, 0); // !!! the UniqueProjectiles Pack's projectiles need rotating 90 if used in 2D games / 2D mode !!!
    spawnInstance = SimplePool.Spawn(playerMissilePrefab, shootingPos, thisRotation);
    //spawnInstance = SimplePool.Spawn(playerMissilePrefab, shootingPos, Quaternion.identity);

    GameObject vfx;
    //spawnInstance = SimplePool.Spawn(playerMissilePrefab, shootingPos, Quaternion.identity);
    //vfx = Instantiate(effectToSpawn);
    //vfx = Instantiate(effectToSpawn, shootingPos, Quaternion.identity);
    //vfx.transform.localRotation = ;


  }

  
}