using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class PowerUpHealth : PowerUp
{
  private float durationSeconds;
  public int HPincrease = 99999; // basically, refill the players health
	public RectTransform playerHPWorldspaceCanvasRect;
	private Tween pulseTween;
	protected override void PowerUpPayload()
  {
    //do stuff specific to this PU//todo
    playerShip.ChangeShipHP(HPincrease);
    base.PowerUpPayload();
  }

  protected override void PickupEffects()
	{
		if (playerHPWorldspaceCanvasRect == null)
		{
			playerHPWorldspaceCanvasRect = playerShip.gameObject.GetComponentInChildren<RectTransform>();
		}
		pulseTween = playerHPWorldspaceCanvasRect.DOScale(1.5f, 0.2f).SetLoops(4, LoopType.Yoyo);
		base.PickupEffects();
		MasterAudio.PlaySound("playership_collect_powerup");
	}


	protected override void Start()
	{

			
		base.Start();
			

	}

	//protected override void Update()
	//{
	//  //if (powerUpState == PowerUpState.IsCollected)
	//  //{
	//  //  durationSeconds -= Time.deltaTime;
	//  //  if (durationSeconds < 0)
	//  //  {
	//  //    PowerUpHasExpired();
	//  //  }
	//  //}
	//  base.Update();
	//}



	//protected override void PowerUpHasExpired()
	//{

	//  base.PowerUpHasExpired();
	//}
}
