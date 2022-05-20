using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class EnemyFireAtPlayerBehaviour01 : MonoBehaviour
{
  [SerializeField] private GameObject enemyMissile;
  private EnemyBehaviour02 eb;
  private Quaternion rotation;
  private GameObject enemyMissilesParentPool;
  public bool straightDown = true; //fires straightdown by default
  

  void Start()
  {
    eb = GetComponent<EnemyBehaviour02>();
    enemyMissilesParentPool = GameObject.FindWithTag("enemyMissilesParentPoolObject");
    if (enemyMissilesParentPool == null)
      Debug.LogWarning("enemyMissilesParentPoolObject not found!");
    
    
  }

  
  public void FireMissileAtPlayer()
  {
    GameObject firedBullet;
    float angle;

    Vector2 direction = GameplayManager.Instance.playerShipPos - transform.position;  //direction is a vector2 containing the (x,y) distance from the player ship to the firing gameobject (the enemy position)

    if (!straightDown)
    {
       angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      /* angle is a float, and it's 90 when you're aiming/firing directly above you, 
       * -90 when firing directly below, 
       * 0 to the right and
       * 180 (-180) to the left */
      if (angle > 180)
        angle -= 360;
    }
		else
		{
       angle = -90f;
    }
    rotation.eulerAngles = new Vector3(-angle, 90, 0); // use different values to lock on different axis
    
    var adjustedPos = new Vector3(transform.position.x, transform.position.y /*- 1f*/, transform.position.z);
    firedBullet = SimplePool.Spawn(enemyMissile, adjustedPos, Quaternion.identity, enemyMissilesParentPool.transform);
    firedBullet.transform.localRotation = rotation; //v.important line!!!

    if (eb is Enemy02_0003)
    {
      print("Played 02_01");
      MasterAudio.PlaySound("EnemyMissile02_01");
    }
    else if (eb is Enemy02_0001)
    {
      print("Played 03_01");
      MasterAudio.PlaySound("EnemyMissile03_01");
    }
    else 
    {
      MasterAudio.PlaySound("EnemyMissile01");
      print("Played 01");
    }

  }
}
