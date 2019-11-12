using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPointData : ScriptableObject
{
  [System.Serializable]
  public struct enemySpawnPointData
  {
    public GameObject enemyPrefab;
    public Vector2 startPos;
    public float speedMultiplier, hpMultiplier;
  }

  public enemySpawnPointData[] levelEnemySpawnPointData;





}
