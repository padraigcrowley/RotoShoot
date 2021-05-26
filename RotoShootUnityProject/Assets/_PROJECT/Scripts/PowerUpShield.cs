using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  public GameObject playerShieldPrefab;

  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    Instantiate(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity,playerShip.transform);
   
    
    base.PowerUpPayload();
  }

  //protected override void Start()
  //{
   

  //  base.Start();
  //}

  //protected override void Update()
  //{
  //  //if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
  //  //{
  //  //  Destroy(gameObject);
  //  //}
    
   
  //  base.Update();
  //}
  //protected override void PowerUpHasExpired()
  //{
  //  SimplePool.Despawn(playerShieldPrefabInstance);
   
  //  base.PowerUpHasExpired();
  //}


 

  }
