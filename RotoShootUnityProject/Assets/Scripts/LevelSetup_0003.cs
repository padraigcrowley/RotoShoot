using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup_0003 : MonoBehaviour
{
  public GameObject[] LevelEnemyPrefabs;

  private void Awake()
  {
    GameplayManager.Instance.playerShipPos = new Vector3(0f, 0f, .1f); // set the player ship position for this level
  }

  void Start()
  {
    LevelManager.Instance.LevelCompletionCriteria.Add("EnemyKills", 20);
    LevelManager.Instance.LevelCompletionCriteria.Add("SurviveForXSeconds", 15);
    LevelManager.Instance.LevelEnemyPrefabs = LevelEnemyPrefabs;

    //list which angles [0] - [N] you don't want to allow the player ship to rotate to
    LevelManager.Instance.blockedPlayerShipRotationAngles.Clear();
    
    // Array of EnemySpawnPoint objects (pos, speedMult, HPMult)
    /**LevelManager.Instance.spawnPoints = new EnemySpawnPoint[]
    {
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (10,-10),   3f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[1], new Vector2 (-10,-10),  2f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (8,0),      1f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[1], new Vector2 (-9,0),     3f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (-10,10),   2f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[1], new Vector2 (12,12),    1f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[0], new Vector2 (0,20),     1f,   3),
      new EnemySpawnPoint(LevelEnemyPrefabs[1], new Vector2 (0,-15),    2f,   3)
    };**/
  }

  private void Update()
  {

  }

}
