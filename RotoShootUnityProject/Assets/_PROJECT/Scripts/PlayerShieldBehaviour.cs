using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;


public class PlayerShieldBehaviour : ExtendedBehaviour
{

  private float durationSeconds, timeoutWarningFlashThreshold = 2.0f, timeoutWarningFlashDuration = 0.2f;
  public PlayerShip playerShip;
  private SpriteRenderer playerShieldSpriteRenderer;
 
  bool currentlyFlashing = false;

  // Start is called before the first frame update
  void Start()
  {
    GameObject go = GameObject.FindGameObjectWithTag("Player");
    playerShip = go.GetComponent<PlayerShip>();
    GameplayManager.Instance.playerShipInvulnerable = true;
    GameplayManager.Instance.playerShieldVisible = true;

    playerShieldSpriteRenderer = GetComponent<SpriteRenderer>();
    //playerShieldSpriteRenderer.enabled = true;

    durationSeconds = GameController.Instance.shieldDuration;

    MasterAudio.PlaySound("player_shield_active_01");


  }

  // Update is called once per frame
  void Update()
  {

    durationSeconds -= Time.deltaTime;

    if ((durationSeconds < timeoutWarningFlashThreshold) && (currentlyFlashing == false))
    {
      currentlyFlashing = true;
      StartCoroutine(ToggleSpriteOffOn());
    }

    if ((durationSeconds <= 0) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_COMPLETE) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DYING) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.PLAYER_DIED) || (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS))
    {
      GameplayManager.Instance.playerShipInvulnerable = false;
      GameplayManager.Instance.playerShieldVisible = false;
      
      MasterAudio.FadeOutAllOfSound("player_shield_active_01", .1f);
      Wait(.12f, () => {
        Destroy(gameObject);
      });
    }


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

  //private void OnTriggerEnter(Collider collision)
  //{
  //  if ((collision.gameObject.tag.Equals("EnemyMissile")) || (collision.gameObject.tag.Equals("Enemy01")) || (collision.gameObject.tag.Equals("Asteroid")))
  //  {

  //  }

  //}
}
