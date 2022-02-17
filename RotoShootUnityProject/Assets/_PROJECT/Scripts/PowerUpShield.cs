using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  public GameObject playerShieldPrefab;
  public GameObject TriggerShieldButton;

  protected override void PowerUpPayload()
  {
    //Instantiate(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity,playerShip.transform);
    //print("PLAYER SHIELD INSTANTIATED!!!");

    UIManager.Instance.ShowTriggerShieldButton();
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
