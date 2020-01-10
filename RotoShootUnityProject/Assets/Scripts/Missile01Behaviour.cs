using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile01Behaviour : MonoBehaviour
{

  private Transform playerShipBarrelTip;
  private Vector3 upDirection;

  // Start is called before the first frame update
  void Start()
  {
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
  }

  // Update is called once per frame
  void Update()
  {

    this.transform.position += upDirection  * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.deltaTime;

    //if ((this.transform.position.x > 7) || (this.transform.position.x < -7) || (this.transform.position.y > 12) || (this.transform.position.y < -12))
    //{
    //  Destroy(gameObject);
    //}
  }
}
