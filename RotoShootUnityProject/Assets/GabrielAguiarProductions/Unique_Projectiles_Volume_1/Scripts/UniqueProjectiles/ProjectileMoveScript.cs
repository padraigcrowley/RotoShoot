/// Just for the player missile, separate script for EnemyMissile

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoveScript : ExtendedBehaviour {

	public float speed;
	[Tooltip("From 0% to 100%")]
	public float accuracy;
	public float fireRate;
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public AudioClip shotSFX;
	public AudioClip hitSFX;
	public List<GameObject> trails;

	private float speedRandomness;
	private Vector3 offset;
	private bool collided;
	private Rigidbody2D rb;

  private Vector3 upDirection;

  void OnEnable () //instead of Start() because of object pooling system
  { 
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;
    collided = false;

    rb = GetComponent <Rigidbody2D> ();

		//used to create a radius for the accuracy and have a very unique randomness
		if (accuracy != 100) {
			accuracy = 1 - (accuracy / 100);

			for (int i = 0; i < 2; i++) {
				var val = 1 * Random.Range (-accuracy, accuracy);
				var index = Random.Range (0, 2);
				if (i == 0) {
					if (index == 0)
						offset = new Vector3 (0, -val, 0);
					else
						offset = new Vector3 (0, val, 0);
				} else {
					if (index == 0)
						offset = new Vector3 (0, offset.y, -val);
					else
						offset = new Vector3 (0, offset.y, val);
				}
			}
		}

    //GameObject muzzlePrefab = ObjectPooler.SharedInstance.GetPooledObject("EnemyMuzzleFlash");
    //if (playerMissile != null)
    //{
    //  playerMissile.transform.position = barrelTip.transform.position;
    //  playerMissile.transform.rotation = barrelTip.transform.rotation;
    //  playerMissile.SetActive(true);
    //}

    GameObject muzzleVFX = ObjectPooler.SharedInstance.GetPooledObject("EnemyMuzzleFlash");
    if (muzzleVFX != null)
    {
      //var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
      muzzleVFX.transform.position = new Vector3 (GameplayManager.Instance.playerShipPos.x, GameplayManager.Instance.playerShipPos.y+0.8f, GameplayManager.Instance.playerShipPos.z) ;
      muzzleVFX.transform.rotation = Quaternion.identity;
      muzzleVFX.transform.forward = gameObject.transform.forward + offset;
      muzzleVFX.SetActive(true);
      
      {
        var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        psChild.Play();
        //muzzleVFX.SetActive(false);
        //Destroy(muzzleVFX, psChild.main.duration);
        Wait(1.5f, () =>
        {
          //psChild.Stop();
          print($"muzzleVFX.SetActive(false);");
          muzzleVFX.SetActive(false);
        });
      }
		}

		if (shotSFX != null && GetComponent<AudioSource>()) {
			GetComponent<AudioSource> ().PlayOneShot (shotSFX);
		}
	}

	void FixedUpdate () {
		//if (speed != 0 && rb != null)
		//	rb.position += (transform.forward + offset) * (speed * Time.deltaTime);

		this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.deltaTime;


		//if (speed != 0 && rb != null)
		//	rb.position += (transform.up + offset) * (speed * Time.fixedDeltaTime);

		//if (speed != 0 && rb != null)
		//rb.position += (upDirection + offset)  * (speed * Time.deltaTime);
	}


  private void OnTriggerEnter2D(Collider2D co)
  {
    //print($"OnTriggerEnter2D in ProjectileMoveScript - {co.gameObject.tag}");

    if (!collided && (co.gameObject.tag != "EnemyMissile"))
    {
      collided = true;

      if (shotSFX != null && GetComponent<AudioSource>())
      {
        GetComponent<AudioSource>().PlayOneShot(hitSFX);
      }
      //// Commented out by PC, don't think it's needed due to pooling.
      //if (trails.Count > 0) 
      //{
      //	for (int i = 0; i < trails.Count; i++)
      //	{
      //		trails[i].transform.parent = null;
      //		var ps = trails[i].GetComponent<ParticleSystem>();
      //		if (ps != null) // never seems to do this block of code?
      //       {
      //			ps.Stop();
      //		  Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
      //		}
      //	}
      //}


      if (co.gameObject.tag != "Boundary" && co.gameObject.tag != "EnemyMissile")
      {
        speed = 0;

        Vector3 pos = co.gameObject.transform.position;

        if (hitPrefab != null)
        {
          var hitVFX = Instantiate(hitPrefab, pos, Quaternion.identity) as GameObject;

          var ps = hitVFX.GetComponent<ParticleSystem>();
          if (ps == null)
          {
            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(hitVFX, psChild.main.duration);
          }
          else
            Destroy(hitVFX, ps.main.duration);
        }
      }
    
      StartCoroutine(DestroyParticle(0f));

    }
	}

	public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		
		yield return new WaitForSeconds (waitTime);
    //Destroy (gameObject);
    gameObject.SetActive(false);
	}
}
