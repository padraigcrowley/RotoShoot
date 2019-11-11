using UnityEngine;
using UnityEditor;

public class EnemySpawnPointDataAsset
{
  [MenuItem("Assets/Create/EnemySpawnPointData")]
  public static void CreateAsset()
  {
    ScriptableObjectUtility.CreateAsset<EnemySpawnPointData>();
  }
}