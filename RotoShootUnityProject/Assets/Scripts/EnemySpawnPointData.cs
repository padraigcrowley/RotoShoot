using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnPointData
{
  public GameObject enemyPrefab;
  public Vector2 startPos;
  public float verticalDistBetweenEnemies = 2.0f, horizontalDistBetweenEnemies = 0.0f;
  public float speedMultiplier, hpMultiplier;
  public Mr1.PathData WayPointPath;
  //public string wayPointPathName;
  public float numEnemiesInWave = 1;
  public float timeBetweenSpawn = .5f;

}
