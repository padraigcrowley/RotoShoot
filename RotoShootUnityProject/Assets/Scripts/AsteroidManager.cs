using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
  private GameObject asteroids1Instance, asteroids2Instance;
  public GameObject asteroids1Prefab, asteroids2Prefab;
  private float travelSpeed = 4f;

  private bool asteroidsCreated = false;
  private List<GameObject> asteroid1ChildrenObjects = new List<GameObject>();
  private List<GameObject> asteroid2ChildrenObjects = new List<GameObject>();
  private Animator asteroids1RotationAnimator, asteroids2RotationAnimator;
  private SpriteRenderer asteroid1Sprite, asteroid2Sprite;

  private float timeBetweenAsteroidShower = 30f;

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
      Vector2 pos1 = new Vector2(1.28f, 4.0f + (i * 2));
      Vector2 pos2 = new Vector2(3.84f, 5.0f + (i * 2));

      int rnd = UnityEngine.Random.Range(0, 2);
      print($"RND was {rnd}");

      //if (rnd == 0)
      {
        asteroids1Instance = SimplePool.Spawn(asteroids1Prefab, pos1, transform.rotation, transform);
        asteroids2Instance = SimplePool.Spawn(asteroids2Prefab, pos2, transform.rotation, transform);
      }
      //else
      {
        //asteroids1Instance = SimplePool.Spawn(asteroids2Prefab, pos, transform.rotation, transform);
      }
      asteroid1ChildrenObjects.Add(asteroids1Instance);
      asteroid2ChildrenObjects.Add(asteroids2Instance);
    }
    
    asteroidsCreated = true;
  }

  private void AnimateAsteroids()
  {
    int i = 0;
    foreach (GameObject childObj in asteroid1ChildrenObjects)
    {
      asteroids1RotationAnimator = childObj.GetComponent<Animator>();
      asteroid1Sprite = childObj.GetComponent<SpriteRenderer>();
      asteroid1Sprite.transform.localScale *= Random.Range(.75f, 1.1f);

      if (i == 0)
      {
        asteroids1RotationAnimator.speed = .5f ;
      }
      if (i == 1)
      { 
        asteroid1Sprite.flipX = true;
      asteroids1RotationAnimator.speed = .75f;

      }
      if (i == 2)
      { 
        asteroid1Sprite.flipY = true;
        asteroids1RotationAnimator.speed = 1f;
      }
      if (i == 3)
      {
        asteroid1Sprite.flipX = true;
        asteroid1Sprite.flipY = true;
        asteroids1RotationAnimator.speed = 1.5f;
      }
      asteroids1RotationAnimator.Play("asteroids_1", -1, Random.Range(0f, 1f)); 

      i++;
    }
    
    i = 0;

    foreach (GameObject childObj in asteroid2ChildrenObjects)
    {
      asteroids2RotationAnimator = childObj.GetComponent<Animator>();
      asteroid2Sprite = childObj.GetComponent<SpriteRenderer>();
      asteroid2Sprite.transform.localScale *= Random.Range(.8f, 1.2f);

      if (i == 0)
      {
        asteroids2RotationAnimator.speed = .5f;
      }
      if (i == 1)
      {
        asteroid2Sprite.flipX = true;
        asteroids2RotationAnimator.speed = .75f;

      }
      if (i == 2)
      {
        asteroid2Sprite.flipY = true;
        asteroids2RotationAnimator.speed = 1f;
      }
      if (i == 3)
      {
        asteroid2Sprite.flipX = true;
        asteroid2Sprite.flipY = true;
        asteroids2RotationAnimator.speed = 1.5f;
      }
      asteroids2RotationAnimator.Play("asteroids_2", -1, Random.Range(0f, 1f)); 
      i++;
    }
  }

  private void MoveAsteroids()
  {
    foreach (GameObject childObj in asteroid1ChildrenObjects)
    {
      childObj.transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime;
    }
    foreach (GameObject childObj in asteroid2ChildrenObjects)
    {
      childObj.transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime * 1.5f;
    }
  }
}