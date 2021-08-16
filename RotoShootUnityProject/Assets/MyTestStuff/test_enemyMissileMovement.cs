using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_enemyMissileMovement : MonoBehaviour
{

	public bool rotate = false;
	//public float rotateAmount = 45;
	public bool bounce = false;
	public float bounceForce = 10;
	public float speed;
	[Tooltip("From 0% to 100%")]
	public float accuracy;
	public float fireRate;
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public List<GameObject> trails;

	private Vector3 startPos;
	private float speedRandomness;
	private Vector3 offset;
	private bool collided;
	private Rigidbody rb;
	private RotateToMouseScript rotateToMouse;
	private GameObject target;

	void Start()
	{
		startPos = transform.position;
		rb = GetComponent<Rigidbody>();
		if (muzzlePrefab != null)
		{
			var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVFX.transform.forward = gameObject.transform.forward + offset;
			var ps = muzzleVFX.GetComponent<ParticleSystem>();
			if (ps != null)
				Destroy(muzzleVFX, ps.main.duration);
			else
			{
				var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy(muzzleVFX, psChild.main.duration);
			}
		}
	}
	void FixedUpdate()
	{
		if (target != null)
			rotateToMouse.RotateToMouse(gameObject, target.transform.position);
		
		if (speed != 0 && rb != null)
			transform.position += (transform.forward + offset) * (speed * Time.deltaTime);
	}
}