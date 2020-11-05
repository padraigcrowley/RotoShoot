using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGenerator : MonoBehaviour
{
  public Animator boom;  
  // Start is called before the first frame update
    void Start()
    {
        
    }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown("x"))
    {
      print("BOOM!");
      boom.Play("fireexplosion");
    }
  }
}
