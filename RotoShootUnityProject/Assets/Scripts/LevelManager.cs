using JetBrains.Annotations;
using Mr1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
  [HideInInspector] public int numEnemyKillsInLevel = 0;
  public float TIME_BETWEEN_FIRING_AT_PLAYER = 5f; //todo: magic number
  [HideInInspector] public float currentTimeBetweenFiringAtPlayer; //todo: magic number

  public Dictionary<string, int> LevelCompletionCriteria = new Dictionary<string, int>();
  private bool lccMet; //levelcompletioncriteria
  public LevelSetupData levelSetupData;
  public float levelPlayTimeElapsed;
  public float verticalDistBetweenEnemies = 2.0f; //todo: magic number
  public float horizontalDistBetweenEnemies = 2.0f; //todo: magic number
  public bool readyToFireAtPlayer = false; 
  private List<GameObject> enemyWaves = new List<GameObject>();

  public GameObject enemyMissilesParentPool;

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
    enemyMissilesParentPool = new GameObject("enemyMissilesParentPoolObject");
    enemyMissilesParentPool.tag = "enemyMissilesParentPoolObject";

    numEnemyKillsInLevel = 0;
    currentTimeBetweenFiringAtPlayer = TIME_BETWEEN_FIRING_AT_PLAYER;

    if (levelSetupData.blockedPlayerShipRotationAngles.Length != 0)
      GameplayManager.Instance.blockedPlayerShipRotationAngles = levelSetupData.blockedPlayerShipRotationAngles;

    InitializeLCC();

    int index = 0;
    foreach (EnemySpawnPointData sp in levelSetupData.levelEnemySpawnPointData)
    {
      var waveParentObject = new GameObject("EnemyWave_" + index);
      waveParentObject.transform.position = new Vector2(sp.startPos.x, sp.startPos.y);
      enemyWaves.Add(waveParentObject);

      verticalDistBetweenEnemies = sp.verticalDistBetweenEnemies;
      horizontalDistBetweenEnemies = sp.horizontalDistBetweenEnemies;

      for (int i = 0; i < sp.numEnemiesInWave; i++)
      {
        GameObject enemy = Instantiate(sp.enemyPrefab, new Vector3(sp.startPos.x + (i * horizontalDistBetweenEnemies), sp.startPos.y + (i * verticalDistBetweenEnemies)), Quaternion.identity, waveParentObject.transform);
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
    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      //print($"NumActiveWaves = { GetNumActiveWaves()}");
      //if there are less than the number of maxwaves, activate another (random) wave
      if (GetNumActiveWaves() < levelSetupData.numMaxActiveWaves)
      {
        foreach (Transform enemyShipObjectTransform in enemyWaves[Random.Range(0, enemyWaves.Count)].transform)
        {
          enemyShipObjectTransform.gameObject.GetComponent<EnemyBehaviour02>().respawnWaitOver = true;
        }
      }

      if (readyToFireAtPlayer == false)
      {
        currentTimeBetweenFiringAtPlayer -= Time.deltaTime;
        if (currentTimeBetweenFiringAtPlayer <= 0f)
        {
          //print($"readyToFireAtPlayer = true");
          readyToFireAtPlayer = true; // this is set back to false in Update() of EnemyFireAtPlayerBehaviour01.cs
        }
      }

      CheckLCC(); // check LevelCompletionCriteria
    }
  }

  /// <summary>
  /// loop through each of the enemywave parent gameobjects in the list, if any of the parent's children's state is ALIVE, increnent the number of active waves.
  /// </summary> 
  /// <returns>numActiveWaves</returns>
  private int GetNumActiveWaves()
  {
    int numActiveWaves = 0;
    bool activeWave = false;
    foreach (GameObject enemyWaveParentObject in enemyWaves)
    {
      activeWave = false;
      foreach (Transform enemyShipObjectTransform in enemyWaveParentObject.transform)
      {
        if (enemyShipObjectTransform.gameObject.GetComponent<EnemyBehaviour02>().enemyState == EnemyBehaviour02.EnemyState.ALIVE)
        {
          activeWave = true;
        }
      }
      if (activeWave)
        numActiveWaves++;
    }
    return numActiveWaves;
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
      else // level completion criteria = true
      {
        print("Level Completion Criteria TRUE!, Next Level is:" + (GameManagerX.Instance.currentLevel + 1));
        GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS;
      }
    }    

  }

}
