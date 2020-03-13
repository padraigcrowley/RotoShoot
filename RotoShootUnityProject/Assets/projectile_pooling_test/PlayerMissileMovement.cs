using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMissileMovement : ExtendedBehaviour
{
  private Vector3 upDirection;
  public float speed = 5;
  public GameObject vfxMuzzleFlash;

  void Start()
  {
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
    EditorApplication.isPaused = true;
    SimplePool.Spawn(vfxMuzzleFlash, transform.position, transform.rotation);

    if (vfxMuzzleFlash != null)
    {
      var muzzleVFX = SimplePool.Spawn(vfxMuzzleFlash, transform.position, Quaternion.identity);
      muzzleVFX.transform.forward = gameObject.transform.forward;// + offset;
      var ps = muzzleVFX.GetComponent<ParticleSystem>();
      if (ps != null)
      {
        Wait(ps.main.duration, () => {
          SimplePool.Despawn(muzzleVFX);
        });
      }

      else
      {
        var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        Wait(.3f, () => {
          print($"Waited {psChild.main.duration} before Despawn");
          SimplePool.Despawn(muzzleVFX);
        });
      }
    }
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
