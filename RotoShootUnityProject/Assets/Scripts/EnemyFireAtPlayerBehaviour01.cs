using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireAtPlayerBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;
  private Mr1.EnemyBehaviour02 eb;

  void Start()
  {
    eb = GetComponent<Mr1.EnemyBehaviour02>();
		//InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 3, 5);
  }

  // Update is called once per frame
  void Update()
  {
    if(LevelManager.Instance.readyToFireAtPlayer == true)
    {
      FireMissileAtPlayerPos();
      LevelManager.Instance.readyToFireAtPlayer = false;
      LevelManager.Instance.timeBetweenFiringAtPlayer = 3.0f;//todo: magic number
    }
  } 

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;

    if(eb.enemyState == Mr1.EnemyBehaviour02.EnemyState.ALIVE)
      //Dont shoot if the y pos is almost same as playership
      if (transform.position.y - 3 > GameplayManager.Instance.playerShipPos.y)
      {
        //firedBullet = Instantiate(enemyMissile, transform.position, transform.rotation);
        firedBullet = SimplePool.Spawn(enemyMissile, transform.position, transform.rotation);
        //Debug.Log("FireMissileAtPlayerPos()");
      }

  }
}
