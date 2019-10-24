using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint 
{
  public GameObject enemyPrefab;
  public Vector2 startPos;
  public float speedMultiplier, hpMultiplier;

  public EnemySpawnPoint(GameObject a, Vector2 b, float c, float d)
  {
    enemyPrefab = a;
    startPos = b;
    speedMultiplier = c; // possibly redundant? speed is in the enemybehaviour class (tho we do pass a multiplier)
    hpMultiplier = d;
  }


}
