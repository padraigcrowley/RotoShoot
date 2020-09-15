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
  private ParticleSystem[] muzzleParticleSystems;
  private GameObject muzzleVFX;
  private bool collided;

  private Vector3 upDirection;

  void Start()
  {
    print("-----------ProjectileMoveScript Start()-----------");
  }

  void OnEnable() //instead of Start() because of object pooling system
  {
    print("-----------ProjectileMoveScript OnEnable()-----------");
    collided = false;
    upDirection = GameObject.FindGameObjectWithTag("Player").transform.up;

    muzzleVFX = ObjectPooler.SharedInstance.GetPooledObject("PlayerMuzzleFlash");

    if (muzzleVFX != null)
    {
      muzzleParticleSystems = muzzleVFX.transform.GetComponentsInChildren<ParticleSystem>(); 
      muzzleVFX.SetActive(true);
    
      foreach (ParticleSystem ps in muzzleParticleSystems)
      {
        ps.transform.position = new Vector3(GameplayManager.Instance.playerShipPos.x, GameplayManager.Instance.playerShipPos.y + 0.8f, GameplayManager.Instance.playerShipPos.z);
        ps.transform.rotation = Quaternion.identity;
        ps.transform.forward = gameObject.transform.forward;
        ps.Play();        
      }
         
      if (shotSFX != null && GetComponent<AudioSource>())
      {
        GetComponent<AudioSource>().PlayOneShot(shotSFX);
      }
    }
  }

	void FixedUpdate () 
    {
		
		this.transform.position += upDirection * GameplayManager.Instance.currentPlayerMissileSpeedMultiplier * Time.deltaTime;
    
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
    if (muzzleVFX != null) 
      muzzleVFX.SetActive(false);
    //gameObject.SetActive(false);
    SimplePool.Despawn(gameObject);
  }
}
