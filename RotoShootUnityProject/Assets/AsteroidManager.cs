using UnityEngine;
using System.Collections.Generic;

public class AsteroidManager : MonoBehaviour
{
  private GameObject asteroids1Instance, asteroids2Instance;
  public GameObject asteroids1Prefab, asteroids2Prefab;
  private float travelSpeed = 0.5f;

  private bool asteroidsCreated = false;
  private List<GameObject> asteroidChildrenObjects = new List<GameObject>();
  private Animator asteroids1RotationAnimator;
  private SpriteRenderer asteroidSprite;

  private void Start()
  {
  }

  // Update is called once per frame
  private void Update()
  {
    if ((Input.GetKeyDown(KeyCode.Space)) && (asteroidsCreated == false))
    {
      CreateAsteroids();
      AnimateAsteroids();
    }
  }

  private void FixedUpdate()
  {
    if (asteroidsCreated)
    {
      MoveAsteroids();
    }
  }

  private void CreateAsteroids()
  {
    for (int i = 0; i < 4; i++)
    {
      Vector2 pos = new Vector2(1.28f,i*2);
      asteroids1Instance = SimplePool.Spawn(asteroids1Prefab, pos, transform.rotation, transform);
      asteroidChildrenObjects.Add(asteroids1Instance);
    }
    asteroidsCreated = true;
  }

  private void AnimateAsteroids()
  {
    int i = 0;
    foreach (GameObject childObj in asteroidChildrenObjects)
    {

      asteroids1RotationAnimator = childObj.GetComponent<Animator>();
      asteroids1RotationAnimator.Play("asteroids_1", -1, Random.Range(0f, 1f));
      asteroidSprite = childObj.GetComponent<SpriteRenderer>();
      if (i == 1)
        asteroidSprite.flipX = true;
      if (i == 2)
        asteroidSprite.flipY = true;
      if (i == 3)
      {
        asteroidSprite.flipX = true;
        asteroidSprite.flipY = true;
      }
      i++;
    }
  }

  private void MoveAsteroids()
  {
    foreach (GameObject childObj in asteroidChildrenObjects)
    {
      transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime;
    }
  }
}