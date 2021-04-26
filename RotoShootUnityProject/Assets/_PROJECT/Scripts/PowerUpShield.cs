using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  private float durationSeconds;
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;
    
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    playerShieldPrefabInstance = SimplePool.Spawn(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity,playerShip.transform);
    playerShip.invulnerable = true;
    base.PowerUpPayload();
  }

  protected override void Start()
  {
    durationSeconds = GameplayManager.Instance.playerShieldPowerupDurationSeconds;
    base.Start();
  }

  protected override void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
    {
      Destroy(gameObject);
    }
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
    SimplePool.Despawn(playerShieldPrefabInstance);
    playerShip.invulnerable = false;
    base.PowerUpHasExpired();
  }
}
