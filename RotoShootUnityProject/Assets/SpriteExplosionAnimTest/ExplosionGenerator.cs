// thanks to https://www.youtube.com/watch?v=N73EWquTGSY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGenerator : ExtendedBehaviour
{
  public Animator boomAnim;  
  
  //Start() not needed because OnEnable()is called in initialization anyway, and we use OnEnable(), not Start() because we're pooling and despawning the object.

  void OnEnable()
  {
    //sr = GetComponent<SpriteRenderer>();
    //sr.enabled = false;
    //print("ALSO BOOM!");
    boomAnim.Play("fireexplosion");
    Wait(3, () => {
      gameObject.Despawn();
    });
  }
    
}
