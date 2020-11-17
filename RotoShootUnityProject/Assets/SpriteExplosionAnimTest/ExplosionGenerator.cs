﻿// thanks to https://www.youtube.com/watch?v=N73EWquTGSY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGenerator : ExtendedBehaviour
{
  public Animator boomAnim;  
  //public SpriteRenderer sr;
  // Start is called before the first frame update
    void Start()
    {
    //sr = GetComponent<SpriteRenderer>();
    //sr.enabled = false;
    //print("ALSO BOOM!");
    boomAnim.Play("fireexplosion");
    Wait(3, () => {
      gameObject.Despawn();
    });
  }

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

  // Update is called once per frame
  //void Update()
  //{

  //  if (Input.GetKeyDown(KeyCode.Space))
  //  {
  //    //sr.enabled = true;
  //    //print("ALSO BOOM!");
  //    //boomAnim.Play("fireexplosion");

  //  }
  //}
}