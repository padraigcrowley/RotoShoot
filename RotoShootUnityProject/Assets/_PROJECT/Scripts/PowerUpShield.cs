using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp
{
  private float durationSeconds, timeoutWarningFlashThreshold = 2.0f,  timeoutWarningFlashDuration = 0.2f;
  public GameObject playerShieldPrefab, playerShieldPrefabInstance;
  private SpriteRenderer playerShieldSpriteRenderer;
  private bool shieldVisible = true;
  bool currentlyFlashing = false;
    
  protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    //playerShield.SetActive(true);
    playerShieldPrefabInstance = SimplePool.Spawn(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity,playerShip.transform);
    playerShieldSpriteRenderer = playerShieldPrefabInstance.GetComponent<SpriteRenderer>();
    playerShieldSpriteRenderer.enabled = true;
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
    //if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS)
    //{
    //  Destroy(gameObject);
    //}
    
    if (powerUpState == PowerUpState.IsCollected)
    {
      durationSeconds -= Time.deltaTime;
      
      if ((durationSeconds < timeoutWarningFlashThreshold)  && (currentlyFlashing == false))
      {
        currentlyFlashing = true;
        StartCoroutine(ToggleSpriteOffOn());
      }

      if ((durationSeconds <= 0) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_COMPLETE) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DYING) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DIED)|| (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS))
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
    playerShieldSpriteRenderer.enabled = false;
    base.PowerUpHasExpired();
  }


  private IEnumerator ToggleSpriteOffOn()
  {
    
    while (true)
    {
      playerShieldSpriteRenderer.enabled = false;
      yield return new WaitForSeconds(timeoutWarningFlashDuration);
      playerShieldSpriteRenderer.enabled = true;
      yield return new WaitForSeconds(timeoutWarningFlashDuration);
    }

  }

  }
