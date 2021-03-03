using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpStarCoin : PowerUp
{
  private float durationSeconds;
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;
    
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    GameplayManager.Instance.starCoinCount++;
    UIManager.Instance.starCoinCountText.text = GameplayManager.Instance.starCoinCount.ToString();
    base.PowerUpPayload();
  }

  protected override void Start()
  {
    base.Start();
  }

  protected override void Update()
  {
    if (powerUpState == PowerUpState.IsCollected)
    {
      durationSeconds -= Time.deltaTime;
      if (durationSeconds < 0)
      {
        PowerUpHasExpired();
      }
    }
    base.Update();
  }
  protected override void PowerUpHasExpired()
  {
    base.PowerUpHasExpired();
  }
}
