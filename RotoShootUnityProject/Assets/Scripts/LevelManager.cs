using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
  [HideInInspector] public int numEnemyKillsInLevel = 0;
  public Dictionary<string, int> LevelCompletionCriteria = new Dictionary<string, int>();
  private bool lccMet; //levelcompletioncriteria
  public LevelSetupData levelSetupData;
  public float levelPlayTimeElapsed;
  public float verticalDistBetweenEnemies = 2.0f;


  private void Awake()
  {
    GameplayManager.Instance.playerShipPos = levelSetupData.PlayerShipPos;
    GameplayManager.Instance.levelControlType = levelSetupData.levelControlType;
    GameplayManager.Instance.shipLanes = levelSetupData.shipLanes;
  }

  void InitializeLCC()
  {
    lccMet = false;
    //add the level completion criterias to the lcc dictionary
    if (levelSetupData.lccEnemyKills != -1)
      LevelCompletionCriteria.Add("EnemyKills", levelSetupData.lccEnemyKills);
    if (levelSetupData.lccSurviveTime != -1)
      LevelCompletionCriteria.Add("SurviveTime", levelSetupData.lccSurviveTime);

    //process the particular level completion criteria(s)
    foreach (string lccString in LevelCompletionCriteria.Keys)
    {
      if (lccString == "Enemy Kills")
      {
        UIManager.Instance.RequiredEnemyKillCount.text = "/" + LevelCompletionCriteria[lccString].ToString();
      }
    }
  }
  void Start()
  {    
    numEnemyKillsInLevel = 0;
        
    if (levelSetupData.blockedPlayerShipRotationAngles.Length != 0)
      GameplayManager.Instance.blockedPlayerShipRotationAngles = levelSetupData.blockedPlayerShipRotationAngles;

    InitializeLCC();

    int index = 0;
    foreach (EnemySpawnPointData sp in levelSetupData.levelEnemySpawnPointData)
    {
      //for (int i = 0; i < sp.numEnemiesInWave; i++)
      //{
      //  levelEnemies[index] = Instantiate(sp.enemyPrefab, new Vector3(sp.startPos.x, sp.startPos.y+1), Quaternion.identity) as GameObject;
      //}
      //levelEnemies[index].GetComponent<Mr1.EnemyBehaviour02>().speedMultiplierFromSpawner = sp.speedMultiplier;
      //levelEnemies[index].GetComponent<Mr1.EnemyBehaviour02>().hpMultiplierFromSpawner = sp.hpMultiplier;
      //if (sp.WayPointPath != null)
      //  levelEnemies[index].GetComponent<Mr1.EnemyBehaviour02>().wayPointPathName = sp.WayPointPath.pathName;
      //index++;
      GameObject enemy = new GameObject();
      for (int i = 0; i < sp.numEnemiesInWave; i++)
      {
        enemy = Instantiate(sp.enemyPrefab, new Vector3(sp.startPos.x, sp.startPos.y + (i*verticalDistBetweenEnemies)), Quaternion.identity) as GameObject;
        enemy.GetComponent<Mr1.EnemyBehaviour02>().speedMultiplierFromSpawner = sp.speedMultiplier;
        enemy.GetComponent<Mr1.EnemyBehaviour02>().hpMultiplierFromSpawner = sp.hpMultiplier;
        if (sp.WayPointPath != null)
          enemy.GetComponent<Mr1.EnemyBehaviour02>().wayPointPathName = sp.WayPointPath.pathName;
      }
      index++;
    }
  }

  void Update()
  {
    CheckLCC(); // check LevelCompletionCriteria
  }

  
  void CheckLCC() // check LevelCompletionCriteria
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
