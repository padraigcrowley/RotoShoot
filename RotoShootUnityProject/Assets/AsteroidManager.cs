﻿using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
  private GameObject asteroids1Instance, asteroids2Instance;
  public GameObject asteroids1Prefab, asteroids2Prefab;
  private float travelSpeed = .1f;

  private bool asteroidsCreated = false;
  private List<GameObject> asteroidChildrenObjects = new List<GameObject>();
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
      Vector2 pos = new Vector2(1.28f, 4.0f + (i * 2));

      int rnd = UnityEngine.Random.Range(0, 2);
      print($"RND was {rnd}");

      //if (rnd == 0)
      {
        asteroids1Instance = SimplePool.Spawn(asteroids2Prefab, pos, transform.rotation, transform);
      }
      //else
      {
        //asteroids1Instance = SimplePool.Spawn(asteroids2Prefab, pos, transform.rotation, transform);
      }
      asteroidChildrenObjects.Add(asteroids1Instance);
    }
    asteroidsCreated = true;
  }

  private void AnimateAsteroids()
  {
    int i = 0;
    foreach (GameObject childObj in asteroidChildrenObjects)
    {

      asteroid1Sprite = childObj.GetComponent<SpriteRenderer>();
      if (i == 1)
        asteroid1Sprite.flipX = true;
      if (i == 2)
        asteroid1Sprite.flipY = true;
      if (i == 3)
      {
        asteroid1Sprite.flipX = true;
        asteroid1Sprite.flipY = true;
      }
      asteroid1Sprite.transform.localScale *= Random.Range(.5f, 1.5f);
      
      asteroids1RotationAnimator = childObj.GetComponent<Animator>();
      //asteroids1RotationAnimator.Play("asteroids_1", -1, Random.Range(0f, 1f));
      asteroids1RotationAnimator.speed = 1+i;
      asteroids1RotationAnimator.Play("asteroids_2", -1, Random.Range(0f, 1f)); //awful hack!!

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