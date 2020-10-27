using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTripleFireAngled : PowerUp
{
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    GameplayManager.Instance.currentPlayerFiringState = GameplayManager.PlayerFiringState.ANGLED_TRIPLE;
    base.PowerUpPayload();
  }
  
  
  // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
