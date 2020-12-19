using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_shoot_at_player : MonoBehaviour
{

  [SerializeField] private GameObject enemyMissile;

  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.E))
      FireMissileAtPlayerPos();

  }

  private void FireMissileAtPlayerPos()
  {
    GameObject firedBullet;

    firedBullet = SimplePool.Spawn(enemyMissile, transform.position, transform.rotation );

    //Quaternion thisRotation = Quaternion.identity;
    //thisRotation.eulerAngles = new Vector3(0, -90, 0);
    //firedBullet = SimplePool.Spawn(enemyMissile, transform.position, thisRotation);




  }
}
