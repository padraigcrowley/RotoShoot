using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetupData : ScriptableObject
{
  [Header("Hi there!")]
  public int levelNumber = 0;
  public int numMaxActiveWaves;
  public float minTimeBetweenWaves, maxTimeBetweenWaves;
  [Space(10)]
  public EnemySpawnPointData[] levelEnemySpawnPointData;
  [Space(10)]
  public Vector3 PlayerShipPos;
  public Vector2[] shipLanes;
  [Space(10)]
  public float timeBetweenAsteroidShower = 30f;
  [Space(10)]
  public int lccEnemyKills;
  public int lccSurviveTime;
  [Space(10)]
  public int levelControlType = 2;
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 

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