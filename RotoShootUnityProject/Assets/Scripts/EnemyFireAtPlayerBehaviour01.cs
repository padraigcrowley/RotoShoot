using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireAtPlayerBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;

  void Start()
  {
    InvokeRepeating("FireMissileAtPlayerPos", 3, 5);
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;
    //Dont shoot if the y pos is almost same as playership
    if (transform.position.y - 3 > GameplayManager.Instance.playerShipPos.y)
    {
      firedBullet = Instantiate(enemyMissile, transform.position, transform.rotation);
      Debug.Log("FireMissileAtPlayerPos()");
    }
  }
}
