using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup_0002 : MonoBehaviour
{
  public GameObject[] LevelEnemyPrefabs;

  private void Awake()
  {
    GameplayManager.Instance.playerShipPos = new Vector3(0f, 0f, .1f); // set the player ship position for this level
  }

  void Start()
  {
    LevelManager.Instance.LevelCompletionCriteria.Add("EnemyKills", 10);
    LevelManager.Instance.LevelCompletionCriteria.Add("SurviveForXSeconds", 15);
    LevelManager.Instance.LevelEnemyPrefabs = LevelEnemyPrefabs;

    //list which angles [0] - [N] you don't want to allow the player ship to rotate to
    LevelManager.Instance.blockedPlayerShipRotationAngles.Clear();
    //LevelManager.Instance.blockedPlayerShipRotationAngles.Add(180);

    //MyGameplayManager.Instance.playerShipPos = Vector3.zero; // set the player ship position for this level


    // Array of EnemySpawnPoint objects (pos, speedMult, HPMult)
    LevelManager.Instance.spawnPoints = new EnemySpawnPoint[]
    {
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (10,-10), 5f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-10,-10), 3f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (8,0), 4f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-9,0), 4f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-10,10), 2.25f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (12,12), 3f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (0,20), 5f, 1),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (0,-15), 5f, 1)
    };
  }

  private void Update()
  {

  }

}
