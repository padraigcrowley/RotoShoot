using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnPointData
{
  public GameObject enemyPrefab;
  public Vector2 startPos;
  public float speedMultiplier, hpMultiplier;
  public ScriptableObject WayPointPath;

}
