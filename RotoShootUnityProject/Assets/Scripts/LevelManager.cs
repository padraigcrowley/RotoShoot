using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
  [HideInInspector] public int numEnemyKillsInLevel = 0;
  public Dictionary<string, int> LevelCompletionCriteria = new Dictionary<string, int>();
  private bool lccMet; //levelcompletioncriteria
  public EnemySpawnPoint[] spawnPoints;
  public  GameObject[] LevelEnemies;
  public GameObject [] LevelEnemyPrefabs;
  public float levelPlayTimeElapsed;
  public List<int> blockedPlayerShipRotationAngles = new List<int>(); // rotation angles that will be blocked. 
  void Start()
  {
    //process the particular level completion criteria(s)
    foreach (string lccString in LevelCompletionCriteria.Keys)
    {
      print(lccString + ": " + LevelCompletionCriteria[lccString]);
      if (lccString == "EnemyKills")
        UIManager.Instance.RequiredEnemyKillCount.text = "/"+LevelCompletionCriteria[lccString].ToString();
    }

    numEnemyKillsInLevel = 0;
    LevelEnemies = new GameObject[spawnPoints.Length];//create as many gameobjects as array elements
    lccMet = false;     

    int index = 0;
    foreach (EnemySpawnPoint sp in spawnPoints)
    {
      LevelEnemies[index] = Instantiate(sp.enemyPrefab, sp.startPos, Quaternion.identity) as GameObject;
      LevelEnemies[index].GetComponent<EnemyBehaviour>().speedMultiplierFromSpawner = sp.speedMultiplier;
      LevelEnemies[index].GetComponent<EnemyBehaviour>().hpMultiplierFromSpawner = sp.hpMultiplier;
      index++;
    }
  }

  void Update()
  {
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      levelPlayTimeElapsed += Time.deltaTime;
      if (!lccMet)
      {
        lccMet = true;
        foreach (string lccString in LevelCompletionCriteria.Keys)
        {
          switch (lccString)
          {
            case "EnemyKills":
              if (numEnemyKillsInLevel < LevelCompletionCriteria[lccString])
              {
                lccMet = false;
              }
              break;
            case "SurviveForXSeconds":
              if (levelPlayTimeElapsed < LevelCompletionCriteria[lccString])
              {
                lccMet = false;
              }
              break;
            default:
              //print("Default case");
              break;
          }
        }
      }
      else
      {        
        GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS;
      }
    }    
  }
}
