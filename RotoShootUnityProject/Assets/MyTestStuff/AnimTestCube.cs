using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTestCube : ExtendedBehaviour
{
  public Animator cubeAnimator;
  // Start is called before the first frame update
  void Start()
    {
    cubeAnimator = GetComponentInChildren<Animator>();
    //Wait(5, () => {
    //  Debug.Log("5 seconds is lost forever");
    //  cubeAnimator.Play("AnimTestCube_MoveUp");
    //});

  }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      cubeAnimator.Play("AnimTestCube_MoveUp");
    }
  }
}
