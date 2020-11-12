using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireAtPlayerBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;
  private Mr1.EnemyBehaviour02 eb;

  public GameObject enemyMissilesParentPool;

  void Start()
  {
    eb = GetComponent<Mr1.EnemyBehaviour02>();
    enemyMissilesParentPool = new GameObject("enemyMissilesParentPoolObject");
    //InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 3, 5);
  }

  // Update is called once per frame
  void Update()
  {
    //Dont shoot if the y pos is almost same as playership
    if ((LevelManager.Instance.readyToFireAtPlayer == true) && (eb.enemyState == Mr1.EnemyBehaviour02.EnemyState.ALIVE) && (transform.position.y - 3 > GameplayManager.Instance.playerShipPos.y))
    {
      FireMissileAtPlayerPos();
      LevelManager.Instance.readyToFireAtPlayer = false;
      LevelManager.Instance.currentTimeBetweenFiringAtPlayer = LevelManager.Instance.TIME_BETWEEN_FIRING_AT_PLAYER;//todo: magic number
    }
  } 

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;
      
        //firedBullet = Instantiate(enemyMissile, transform.position, transform.rotation);
        firedBullet = SimplePool.Spawn(enemyMissile, transform.position, transform.rotation, enemyMissilesParentPool.transform);
        //Debug.Log("FireMissileAtPlayerPos()");      
  }
}
