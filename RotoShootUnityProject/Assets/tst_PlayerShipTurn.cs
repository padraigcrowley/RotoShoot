using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tst_PlayerShipTurn : MonoBehaviour
{
  public Animator RedShipTurning;
  
  // Start is called before the first frame update
    void Start()
    {
        
    }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.A))
    {
      RedShipTurning.Play("RedPlayerShipTurnLeft");
    }

    if (Input.GetKeyDown(KeyCode.D)) 
    {
      Flip();
      RedShipTurning.Play("RedPlayerShipTurnLeft");
    }
  }
  void Flip()
  {
  //https://stackoverflow.com/questions/26568542/flipping-a-2d-sprite-animation-in-unity-2d
    // Switch the way the player is labelled as facing
    //facingRight = !facingRight;

    // Multiply the player's x local scale by -1
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }

}
