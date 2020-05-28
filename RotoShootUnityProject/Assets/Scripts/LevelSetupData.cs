using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetupData : ScriptableObject
{
  public int levelNumber = 0;
  public int levelControlType = 2;
  public float numMaxActiveWaves;
  public float minTimeBetweenWaves, maxTimeBetweenWaves;

  public EnemySpawnPointData[] levelEnemySpawnPointData;
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 

  public Vector3 PlayerShipPos;
  public int lccEnemyKills;
  public int lccSurviveTime;

  public Vector2[] shipLanes;

}
