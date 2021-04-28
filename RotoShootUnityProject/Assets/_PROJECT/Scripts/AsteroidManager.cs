using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : ExtendedBehaviour
{
  private GameObject asteroids1Instance, asteroids2Instance;
  public GameObject asteroids1Prefab, asteroids2Prefab;
  private float travelSpeed = 10f;

  private bool asteroidsCanMove = false;
  private List<GameObject> asteroid1ChildrenObjects = new List<GameObject>();
  private List<GameObject> asteroid1ChildrenObjectsVariant = new List<GameObject>();
  private List<GameObject> asteroid2ChildrenObjects = new List<GameObject>();
  private Animator asteroids1RotationAnimator, asteroids2RotationAnimator;
  private SpriteRenderer asteroid1Sprite, asteroid2Sprite;

  private float timeToNextAsteroidShower;
  public int numAsteroidLanes = 2;
  const int NUM_ASTEROIDS_IN_LANE = 6;
  const int NUM_LANES_OF_ASTEROIDS = 3;

  private float startingHeight = 13f;

  private void Start()
  {
    if (LevelManager.Instance.timeBetweenAsteroidShower <= 0) //it's been set to 0 in level setup => no asteroidshower in this level
      return; 
    timeToNextAsteroidShower = LevelManager.Instance.timeBetweenAsteroidShower;
    if (timeToNextAsteroidShower > 0)
    {
      CreateAsteroids();
      GenerateAsteroidStartPositions();
      AnimateAsteroids();
    }
  }

  // Update is called once per frame
  private void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (LevelManager.Instance.timeBetweenAsteroidShower <= 0) //it's been set to 0 in level setup => no asteroidshower in this level
        return;
      timeToNextAsteroidShower -= Time.deltaTime;
      if (timeToNextAsteroidShower <= 0)
      {
        GenerateAsteroidStartPositions();
        asteroidsCanMove = true;
        timeToNextAsteroidShower = LevelManager.Instance.timeBetweenAsteroidShower;
      }
      //if ((Input.GetKeyDown(KeyCode.Space)) && (asteroidsCreated == false))
      //{
      //}
    }
  }

  private void FixedUpdate()
  {
    //if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (asteroidsCanMove)
      {
        MoveAsteroids();
      }
    }
  }

  private void GenerateAsteroidStartPositions()
  {
    List<int> asteroidLanes = new List<int>(NUM_LANES_OF_ASTEROIDS);
    asteroidLanes = GetRandomNumbers(NUM_LANES_OF_ASTEROIDS);

    int i = 0;
    foreach (GameObject childObj in asteroid1ChildrenObjects)
    {
      //childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[UnityEngine.Random.Range(0, 4)].x, 14.0f + (i * 2));
      childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[asteroidLanes[0]].x, 14.0f + (i * 2));
      i++;
    }
    i = 0;
    foreach (GameObject childObj in asteroid2ChildrenObjects)
    {
      //childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[UnityEngine.Random.Range(0, 4)].x, 15.0f + (i * 2));
      childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[asteroidLanes[1]].x, 15.0f + (i * 2));
      i++;
    }
    i = 0;
    foreach (GameObject childObj in asteroid1ChildrenObjectsVariant)
    {
      //childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[UnityEngine.Random.Range(0, 4)].x, 15.0f + (i * 2));
      childObj.transform.position = new Vector2(GameplayManager.Instance.shipLanes[asteroidLanes[2]].x, 15.0f + (i * 2));
      i++;
    }

    //  for (int i = 0; i < 4; i++)
    //{
    //   pos1[i] = new Vector2(GameplayManager.Instance.shipLanes[UnityEngine.Random.Range(0, 4)].x, 14.0f + (i * 2));
    //   pos2[i] = new Vector2(GameplayManager.Instance.shipLanes[UnityEngine.Random.Range(0, 4)].x, 15.0f + (i * 2));
    //}
  }

  // generate 'unique' i.e. non-repeating, random numbers
  public static List<int> GetRandomNumbers(int count)
  {
    List<int> randomNumbers = new List<int>();

    for (int i = 0; i < count; i++)
    {
      int number;

      do number = UnityEngine.Random.Range(0, 4);
      while (randomNumbers.Contains(number));

      randomNumbers.Add(number);
    }
    
    return randomNumbers;
  }
  private void CreateAsteroids()
  {
    
    for (int i = 0; i < NUM_ASTEROIDS_IN_LANE; i++)
    {
      int rnd = UnityEngine.Random.Range(0, 2);
      //print($"RND was {rnd}");

      asteroids1Instance = SimplePool.Spawn(asteroids1Prefab, new Vector2(15,0), transform.rotation, transform);
      asteroid1ChildrenObjects.Add(asteroids1Instance);
      
      asteroids1Instance = SimplePool.Spawn(asteroids1Prefab, new Vector2(15,0), transform.rotation, transform);
      asteroid1ChildrenObjectsVariant.Add(asteroids1Instance);

      asteroids2Instance = SimplePool.Spawn(asteroids2Prefab, new Vector2(15, 0), transform.rotation, transform);
      asteroid2ChildrenObjects.Add(asteroids2Instance);
    }
       
  }

  private void AnimateAsteroids()
  {
    int i = 0;
    foreach (GameObject childObj in asteroid1ChildrenObjects)
    {
      asteroids1RotationAnimator = childObj.GetComponent<Animator>();
      asteroid1Sprite = childObj.GetComponent<SpriteRenderer>();
      asteroid1Sprite.transform.localScale *= Random.Range(.5f, 1.7f);

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

    foreach (GameObject childObj in asteroid1ChildrenObjectsVariant)
    {
      asteroids1RotationAnimator = childObj.GetComponent<Animator>();
      asteroid1Sprite = childObj.GetComponent<SpriteRenderer>();
      asteroid1Sprite.transform.localScale *= Random.Range(.75f, 1.1f);

      if (i == 0)
      {
        asteroids1RotationAnimator.speed = .5f;
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
    //drop the 3 lanes of asteroids
    foreach (GameObject childObj in asteroid1ChildrenObjects)
    {
      if(childObj.transform.position.y > -11) // don't move it if it's off the bottom of the screen
        childObj.transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime;
    }

    Wait(.5f, () => {
      foreach (GameObject childObj in asteroid1ChildrenObjectsVariant)
      {
        if (childObj.transform.position.y > -11) // don't move it if it's off the bottom of the screen
          childObj.transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime * 1.25f;
      }
    });

    Wait(.4f, () => {
      foreach (GameObject childObj in asteroid2ChildrenObjects)
      {
        if (childObj.transform.position.y > -11) // don't move it if it's off the bottom of the screen
          childObj.transform.position -= transform.up * travelSpeed * Time.fixedDeltaTime * 1.5f;
      }
    });


  }
}