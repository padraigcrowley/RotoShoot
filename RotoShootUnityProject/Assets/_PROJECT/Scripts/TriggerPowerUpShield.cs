using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPowerUpShield : MonoBehaviour
{
  public GameObject playerShieldPrefab;
  public GameObject playerShip;
  public void DoTriggerPowerUpShield()
	{
    Instantiate(playerShieldPrefab, playerShip.gameObject.transform.position, Quaternion.identity, playerShip.transform);
  }
}
