using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetupData : ScriptableObject
{
  public int levelNumber = 0;
  public int lccEnemyKills;
  public int lccSurviveTime;
  public bool lccKillBoss = false;
  [Space(10)]
  public int numMaxActiveWaves;
  public float minTimeBetweenWaves, maxTimeBetweenWaves;
  [Space(10)]
  [Header("ENEMY WAVES DATA")]
  public EnemySpawnPointData[] levelEnemySpawnPointData;
  [Space(10)]
  [Header("BOSS DATA")]
  public BossSpawnPointData bossSpawnPointData;
  [Space(10)]
  public Vector3 PlayerShipPos;
  public Vector2[] shipLanes;
  [Space(10)]
  public float timeBetweenAsteroidShower = 30f;
  [Space(10)]
  public int levelControlType = 2;
  

}

[System.Serializable]
public class EnemySpawnPointData
{
  public GameObject enemyPrefab;
  public Vector2 startPos;
  public float verticalDistBetweenEnemies = 2.0f, horizontalDistBetweenEnemies = 0.0f;
  public float speedMultiplier, hpMultiplier;
  public SWS.PathManager waypointPath;
  public float numEnemiesInWave = 1;
  public float timeBetweenSpawn = .5f;

}
[System.Serializable]
public class BossSpawnPointData
{
  public GameObject bossPrefab;
  public Vector2 startPos;
  public float speedMultiplier, hpMultiplier;
  public SWS.PathManager waypointPath;
}