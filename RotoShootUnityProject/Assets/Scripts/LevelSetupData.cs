using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetupData : ScriptableObject
{

  public int levelNumber = 0;
  
  [System.Serializable]
  public struct enemySpawnPointData
  {
    public GameObject enemyPrefab;
    public Vector2 startPos;
    public float speedMultiplier, hpMultiplier;
  }

  public enemySpawnPointData[] levelEnemySpawnPointData;
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 

  public Vector3 PlayerShipPos;
  public int lccEnemyKills;
  public int lccSurviveTime;

}
