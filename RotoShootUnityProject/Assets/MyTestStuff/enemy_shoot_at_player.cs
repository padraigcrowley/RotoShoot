using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_shoot_at_player : MonoBehaviour
{

  [SerializeField] private GameObject enemyMissile;
  private Quaternion rotation;

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

    Vector2 direction = new Vector3(0,-8,0) - transform.position; // a vector2 containing the (x,y) distance of the mouse cursor from the firing gameobject (the sphere)
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    if (angle > 180) angle -= 360;
    rotation.eulerAngles = new Vector3(-angle, 90, 0); // use different values to lock on different axis
    transform.rotation = rotation;


    firedBullet = SimplePool.Spawn(enemyMissile, transform.position, Quaternion.identity );
    firedBullet.transform.localRotation = rotation;

    //Quaternion thisRotation = Quaternion.identity;
    //thisRotation.eulerAngles = new Vector3(0, -90, 0);
    //firedBullet = SimplePool.Spawn(enemyMissile, transform.position, thisRotation);




  }
}
