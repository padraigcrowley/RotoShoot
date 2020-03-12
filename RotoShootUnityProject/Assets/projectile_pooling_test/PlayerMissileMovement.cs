using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileMovement : MonoBehaviour
{
  private Vector3 upDirection;
  public float speed = 5;
  public GameObject vfxMuzzleFlash;

  void Start()
  {
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
    SimplePool.Spawn(vfxMuzzleFlash, transform.position, transform.rotation);
  }

  void FixedUpdate()
  {
    this.transform.position += upDirection * speed * Time.deltaTime;
  }

  // Update is called once per frame
  void Update()
  {
    if (transform.position.y >= 6f)
    {
      SimplePool.Despawn(gameObject);
    }
  }
}
