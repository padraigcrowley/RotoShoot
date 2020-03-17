using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : ExtendedBehaviour
{
  // Start is called before the first frame update
  //public GameObject myVFX;
  public GameObject vfxProjectile;
 
  private ParticleSystem psChild0;
  private ParticleSystem [] psChildren;

  void Start()
    {
    
    //GameObject ParticleGameobject = Instantiate(myVFX, transform.position, transform.rotation);
    //psChildren = ParticleGameobject.transform.GetComponentsInChildren<ParticleSystem>();
   
  }

  // Update is called once per frame
  void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      SimplePool.Spawn(vfxProjectile, transform.position, transform.rotation);
      
      //psChild.Stop();
      //print($"muzzleVFX.SetActive(false);");
      //muzzleVFX.SetActive(true);

      //var psChild0 = myVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
      //var psChild0 = muzzleVFX.transform.GetComponent<ParticleSystem>();
      //print("PSCHILD = " + psChild0);
      //ParticleSystem.EmissionModule module0 = psChild0.emission;
      //module0.enabled = true;

      //foreach (ParticleSystem ps in psChildren)
      //{
      //  ps.Play(true);
      //  print("ps.isEmitting " + ps.isEmitting);
      //}

      //print ("psChild0.isEmitting " + psChild0.isEmitting);

      //var ps = transform.GetComponent<ParticleSystem>();
      //ps.Play();
      //print("ps.isEmitting " + ps.isEmitting);
    }
   }
}
