///
/// not actually used anymore...
/// 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

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
  }

 
  public  void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;

    Vector2 direction = GameplayManager.Instance.playerShipPos - transform.position;  //direction is a vector2 containing the (x,y) distance from the player ship to the firing gameobject (the enemy position)
    
    float angle = -90f;
    rotation.eulerAngles = new Vector3(-angle, 90, 0);

    var adjustedPos = new Vector3(transform.position.x, transform.position.y /*- 1f*/, transform.position.z);
    firedBullet = SimplePool.Spawn(enemyMissile, adjustedPos, Quaternion.identity, enemyMissilesParentPool.transform);
    firedBullet.transform.localRotation = rotation; //v.important line!!!

    if (eb is Enemy02_0003)
    {
      print("Played 02_01");
      MasterAudio.PlaySound("EnemyMissile02_01");
    }
    else
    {
      print("Played 03_01");
      MasterAudio.PlaySound("EnemyMissile03_01");
    }
  }
}
