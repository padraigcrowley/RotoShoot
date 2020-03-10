using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_Test : ExtendedBehaviour
{
  // Start is called before the first frame update
  public GameObject muzzleVFX;


    void Start()
    {

    

  }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      //psChild.Stop();
      print($"muzzleVFX.SetActive(false);");
      Instantiate(muzzleVFX, transform.position, transform.rotation);

    }
  }
}
