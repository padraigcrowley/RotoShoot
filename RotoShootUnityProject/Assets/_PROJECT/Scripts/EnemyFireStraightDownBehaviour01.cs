using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireStraightDownBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;
  private EnemyBehaviour02 eb;
  private Quaternion rotation;
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
      //print($"{this.gameObject.name}: calling FireMissileAtPlayerPos()");
      FireMissileAtPlayerPos();
      LevelManager.Instance.readyToFireAtPlayer = false;
      LevelManager.Instance.currentTimeBetweenFiringAtPlayer = LevelManager.Instance.TIME_BETWEEN_FIRING_AT_PLAYER;//todo: magic number
    }
  } 

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;

    Vector2 direction = GameplayManager.Instance.playerShipPos - transform.position;  //direction is a vector2 containing the (x,y) distance from the player ship to the firing gameobject (the enemy position)
    
    float angle = -90f;
    rotation.eulerAngles = new Vector3(-angle, 90, 0);

    var adjustedPos = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
    firedBullet = SimplePool.Spawn(enemyMissile, adjustedPos, Quaternion.identity, enemyMissilesParentPool.transform);
    firedBullet.transform.localRotation = rotation; //v.important line!!!

  }
}
