using UnityEngine;
using UnityEditor;

public class LevelSetupDataAsset
{
  [MenuItem("Assets/Create/LevelSetupData")]
  public static void CreateAsset()
  {
    ScriptableObjectUtility.CreateAsset<LevelSetupData>();
  }
}