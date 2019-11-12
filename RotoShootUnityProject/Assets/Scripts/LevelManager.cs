﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
  [HideInInspector] public int numEnemyKillsInLevel = 0;
  public Dictionary<string, int> LevelCompletionCriteria = new Dictionary<string, int>();
  private bool lccMet; //levelcompletioncriteria
  public LevelSetupData levelSetupData;
  
  public  GameObject[] levelEnemies;
  
  public float levelPlayTimeElapsed;
  //public List<int> blockedPlayerShipRotationAngles = new List<int>(); // rotation angles that will be blocked. 
  public int[] blockedPlayerShipRotationAngles;// rotation angles that will be blocked. 

  private void Awake()
  {
    GameplayManager.Instance.playerShipPos = levelSetupData.PlayerShipPos;
  }

  void Start()
  {
    
    lccMet = false;
    numEnemyKillsInLevel = 0;

    
    //blockedPlayerShipRotationAngles = new int[levelSetupData.blockedPlayerShipRotationAngles.Length];
    blockedPlayerShipRotationAngles = levelSetupData.blockedPlayerShipRotationAngles;

    //add the level completion criterias to the lcc dictionary
    if (levelSetupData.lccEnemyKills!=0)
      LevelCompletionCriteria.Add("EnemyKills", levelSetupData.lccEnemyKills);
    if (levelSetupData.lccSurviveTime != 0)
      LevelCompletionCriteria.Add("SurviveTime", levelSetupData.lccSurviveTime);

    //process the particular level completion criteria(s)
    foreach (string lccString in LevelCompletionCriteria.Keys)
    {
      if (lccString == "EnemyKills")
      {
        UIManager.Instance.RequiredEnemyKillCount.text = "/" + LevelCompletionCriteria[lccString].ToString();
      }
    }
    
    
    print(levelSetupData.levelEnemySpawnPointData.Length);
    levelEnemies = new GameObject[levelSetupData.levelEnemySpawnPointData.Length];
    //create as many gameobjects as array elements      
    
    int index = 0;
    foreach (LevelSetupData.enemySpawnPointData sp in levelSetupData.levelEnemySpawnPointData)
    {
      levelEnemies[index] = Instantiate(sp.enemyPrefab, sp.startPos, Quaternion.identity) as GameObject;
      levelEnemies[index].GetComponent<EnemyBehaviour>().speedMultiplierFromSpawner = sp.speedMultiplier;
      levelEnemies[index].GetComponent<EnemyBehaviour>().hpMultiplierFromSpawner = sp.hpMultiplier;
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
            case "SurviveTime":
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
