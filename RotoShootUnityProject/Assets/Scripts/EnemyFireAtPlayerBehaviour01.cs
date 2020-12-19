using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireAtPlayerBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;
  private EnemyBehaviour02 eb;

  private GameObject enemyMissilesParentPool;

  void Start()
  {
    eb = GetComponent<EnemyBehaviour02>();
    enemyMissilesParentPool = GameObject.FindWithTag("enemyMissilesParentPoolObject");
    if (enemyMissilesParentPool == null)
      Debug.LogWarning("enemyMissilesParentPoolObject not found!");
    //enemyMissilesParentPool = new GameObject("enemyMissilesParentPoolObject");
    //InvokeRepeating(nameof(this.FireMissileAtPlayerPos), 3, 5);
  }

  // Update is called once per frame
  void Update()
  {
    //Dont shoot if the y pos is almost same as playership
    if ((LevelManager.Instance.readyToFireAtPlayer == true) && (eb.enemyState == EnemyBehaviour02.EnemyState.ALIVE) && (transform.position.y - 3 > GameplayManager.Instance.playerShipPos.y))
    {
      FireMissileAtPlayerPos();
      LevelManager.Instance.readyToFireAtPlayer = false;
      LevelManager.Instance.currentTimeBetweenFiringAtPlayer = LevelManager.Instance.TIME_BETWEEN_FIRING_AT_PLAYER;//todo: magic number
    }
  } 

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;

    //Quaternion thisRotation = Quaternion.identity;
    //thisRotation.eulerAngles = new Vector3(90, 0, 0); // !!! the UniqueProjectiles Pack's projectiles need rotating  if used in 2D games / 2D mode !!!
    //firedBullet = SimplePool.Spawn(enemyMissile, transform.position, thisRotation, enemyMissilesParentPool.transform);
    
    
    firedBullet = SimplePool.Spawn(enemyMissile, transform.position, transform.rotation, enemyMissilesParentPool.transform);

  }
}
