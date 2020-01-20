using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUniqueProjectiles : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject effectToSpawn;
  

  void Start()
    {
      GameObject vfx;
      //vfx = Instantiate(effectToSpawn);
    vfx = Instantiate(effectToSpawn, new Vector3(0f,0f,0f), Quaternion.identity);
    //vfx.transform.localRotation = ;

  }

    // Update is called once per frame
    void Update()
    {
        
    }
}
