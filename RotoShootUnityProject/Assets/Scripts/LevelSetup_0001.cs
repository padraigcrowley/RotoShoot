using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup_0001 : MonoBehaviour
{
  //public GameObject [] LevelEnemyPrefabs;

  private void Awake()
  {
    GameplayManager.Instance.playerShipPos = new Vector3(0f, -6f, .1f); // set the player ship position for this level.
  }

  void Start()
  {
    LevelManager.Instance.LevelCompletionCriteria.Add("EnemyKills", 10);
    LevelManager.Instance.LevelCompletionCriteria.Add("SurviveForXSeconds", 20);
    //LevelManager.Instance.LevelEnemyPrefabs = LevelEnemyPrefabs;

    //list which angles [0] - [N] you don't want to allow the player ship to rotate to.
    LevelManager.Instance.blockedPlayerShipRotationAngles.Clear();
    LevelManager.Instance.blockedPlayerShipRotationAngles.Add(180);

    // Array of EnemySpawnPoint objects (pos, speedMult, HPMult)
    /**LevelManager.Instance.spawnPoints = new EnemySpawnPoint[]
    {
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (7.5f,-14),   1f,    2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-7.5f,-14),  1.5f,  2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (8,2),        1f,    2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-8,2),       1.5f,  2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-10,-6),     1.25f, 2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (12,-6),      1.25f, 2),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (0,25),       5f,    2)
    };    **/
  }

  private void Update()
  {
    
  }

}
