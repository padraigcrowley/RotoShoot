using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PowerUp : ExtendedBehaviour
{
  public string powerUpName;
  [Tooltip("Tick true for power ups that are instant use, eg a health addition that has no delay before expiring")]
  public bool expiresImmediately;
  public bool pulseWhileFalling;
  public GameObject specialEffect;
  public AudioClip soundEffect;
  public float travelSpeed;

  public Animator dissolveAnim;

  protected PlayerShip playerShip;
  protected SpriteRenderer spriteRenderer;

  private Tween pulseTween;

  protected enum PowerUpState
  {
    InAttractMode,
    IsCollected,
    IsExpiring
  }

  protected PowerUpState powerUpState;

  protected virtual void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

	private void Start()
	{
    if (pulseWhileFalling)
    {
      pulseTween = transform.DOScale(1.5f, 0.3f).SetLoops(-1, LoopType.Yoyo);
    }
  }

	protected virtual void OnEnable()
  {
    spriteRenderer.enabled = true;
    if (pulseWhileFalling)
    {
      if (pulseTween.IsActive())
      {
        if (!pulseTween.IsPlaying())
        {
          pulseTween.Play();
          //print("Tween wasn't playing, kicking it off again.");
        }
        else
        {
          //print("Tween was playing, no need to kick off again...");
        }
      }
    }
    powerUpState = PowerUpState.InAttractMode;
  }
    
  /// <summary>
  /// 3D support
  /// </summary>
  protected virtual void OnTriggerEnter(Collider other)
  {
    if ((other.tag == "Player") && (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS))
    {
      //print($"PowerUp Collected!");
      PowerUpCollected(other.gameObject);
    }
    else if ((other.gameObject.CompareTag("Atmosphere")) || (other.gameObject.CompareTag("BoundaryBottom")))
    {
      //print($"Collision entered with Atmos! ");
      //if (pulseWhileFalling)
      //  pulseTween.Kill();
      //travelSpeed /= 2.0f;
      //dissolveAnim.Play("PowerUpDissolve");

      if (pulseWhileFalling)
      {
        if (pulseTween.IsPlaying())
        {
          //print("caling KILL!");
          pulseTween.Pause();
        }
      }
      SimplePool.Despawn(gameObject);
      //Destroy(gameObject, 1);
    }
  }

  protected virtual void PowerUpCollected(GameObject gameObjectCollectingPowerUp)
  {
    // We only care if we've been collected by the player
    //if (gameObjectCollectingPowerUp.tag != "Player")
    //{
    //  return;
    //}

    // We only care if we've not been collected before
    if (powerUpState == PowerUpState.IsCollected || powerUpState == PowerUpState.IsExpiring)
    {
      return;
    }
    powerUpState = PowerUpState.IsCollected;
    //if (pulseWhileFalling) 
    //  pulseTween.Kill();

    // We must have been collected by a player, store handle to player for later use      
    playerShip = gameObjectCollectingPowerUp.GetComponent<PlayerShip>();

    // We move the power up game object to be under the player that collect it, this isn't essential for functionality 
    // presented so far, but it is neater in the gameObject hierarchy
    gameObject.transform.parent = playerShip.gameObject.transform;
    gameObject.transform.position = playerShip.gameObject.transform.position;

    // Collection effects
    PickupEffects();

    // Payload      
    PowerUpPayload();

    // Send message to any listeners
    foreach (GameObject go in EventSystemListeners.main.listeners)
    {
      ExecuteEvents.Execute<IPowerUpEvents>(go, null, (x, y) => x.OnPowerUpCollected(this, playerShip));
    }

    // Now the power up visuals can go away
    spriteRenderer.enabled = false;
    //SimplePool.Despawn(gameObject);
  }

  protected virtual void PickupEffects()
  {
    //print("POWERUP EFFECTS!"); //todo
    //  if (specialEffect != null)
  //  {
  //    Instantiate(specialEffect, transform.position, transform.rotation, transform);
  //  }

  //  if (soundEffect != null)
  //  {
  //    MainGameController.main.PlaySound(soundEffect);
  //  }
  }

  protected virtual void PowerUpPayload()
  {
    //Debug.Log("Power Up collected, issuing payload for: " + gameObject.name);
    
    // If we're instant use we also expire self immediately
    if (expiresImmediately) // e.g. an "instant" effect, like HP increase.
    {
      PowerUpHasExpired();
    }
  }

  protected virtual void PowerUpHasExpired()
  {
    if (powerUpState == PowerUpState.IsExpiring)
    {
      return;
    }
    powerUpState = PowerUpState.IsExpiring;

    // Send message to any listeners
    foreach (GameObject go in EventSystemListeners.main.listeners)
    {
      ExecuteEvents.Execute<IPowerUpEvents>(go, null, (x, y) => x.OnPowerUpExpired(this, playerShip));
    }
    //Debug.Log("Power Up has expired, removing after a delay for: " + gameObject.name);
    //DestroySelfAfterDelay(); - /// not using this coz I'm pooling/reusing the powerups
    if (pulseWhileFalling)
    {
      if (pulseTween.IsPlaying())
      {
        pulseTween.Pause();
      }
    }
    SimplePool.Despawn(gameObject);

  }

  protected virtual void DestroySelfAfterDelay()
  {
    // Arbitrary delay of some seconds to allow particle, audio is all done
    // TODO could tighten this and inspect the sfx? Hard to know how many, as subclasses could have spawned their own
    Destroy(gameObject, 10f);
  }

  protected virtual void DespawnSelfAfterDelay(float delay)
	{
    Wait(delay, () =>
    {
      if (pulseWhileFalling)
      {
        if (pulseTween.IsPlaying())
        {
          pulseTween.Pause();
        }
      }
      SimplePool.Despawn(gameObject);
    });
  }



  protected virtual void Update()
  {
    if((GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS) && (powerUpState == PowerUpState.InAttractMode))
		{
      if (pulseWhileFalling)
      {
        if (pulseTween.IsPlaying())
        {
          pulseTween.Pause();
        }
      }
			travelSpeed /= 2.0f;
			dissolveAnim.Play("PowerUpDissolve");
      //Destroy(gameObject, 1);
      DespawnSelfAfterDelay(5f);
      powerUpState = PowerUpState.IsExpiring;
    }

    if(powerUpState == PowerUpState.InAttractMode)
      DoMovement();
  }

  public void DoMovement()
  {
    float step = travelSpeed * Time.deltaTime; // calculate distance to move
    transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 20f), step);

  }
}

