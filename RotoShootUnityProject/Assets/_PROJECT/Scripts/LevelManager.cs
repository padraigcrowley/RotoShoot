﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
  [HideInInspector] public int numEnemyKillsInLevel = 0;
   public float TIME_BETWEEN_FIRING_AT_PLAYER = .2f; //todo: magic number
   public float currentTimeBetweenFiringAtPlayer; //todo: magic number

  public Dictionary<string, int> LevelCompletionCriteria = new Dictionary<string, int>();
  private bool lccMet; //levelcompletioncriteria
  public LevelSetupData [] levelSetupDataArray;
  public LevelSetupData levelSetupData;

  public float levelPlayTimeElapsed;
  public float verticalDistBetweenEnemies = 2.0f; //todo: magic number
  public float horizontalDistBetweenEnemies = 2.0f; //todo: magic number
  public bool readyToFireAtPlayer = false;


  // a list of lists of EnemyBehaviour02 scripts - for each enemy in each wave
  List<List<EnemyBehaviour02>> enemyWavesParentBehaviourScripts = new List<List<EnemyBehaviour02>>();
  
  private List<GameObject> enemyWaves = new List<GameObject>();

  public GameObject enemyMissilesParentPool, bossMissilesParentPool;
  public float timeBetweenAsteroidShower;
  private bool currentlyActivatingWave = false;
    SWS.PathManager pathInstance = null;
  public bool bossHasBeenKilled = false;

  public SpinningMineBehaviour spinningMine;
  private GameObject spinningMineInstance;
  private BossBehaviour01 bossScript;

  public GameObject enemyWavesParentGroupObject;

  private void Awake()
  {
    //GameplayManager.Instance.playerShipPos = levelSetupData.PlayerShipPos;
    //GameplayManager.Instance.levelControlType = levelSetupData.levelControlType;
    //GameplayManager.Instance.shipLanes = levelSetupData.shipLanes;
    //timeBetweenAsteroidShower = levelSetupData.timeBetweenAsteroidShower;
  }

	private void Start() // only the stuff that ever gets set up once across all levels here . Th eindividual actual levels setup per level is in InitialiseLevel()
	{
    enemyMissilesParentPool = new GameObject("enemyMissilesParentPoolObject");
    enemyMissilesParentPool.tag = "enemyMissilesParentPoolObject";

  }

	public void InitialiseLevel()
  {

    enemyWavesParentBehaviourScripts.Clear();

    levelSetupData = levelSetupDataArray[GameController.Instance.currentLevel - 1]; // get the relevant scriptable object setup file from the array.

    GameplayManager.Instance.playerShipPos = levelSetupData.PlayerShipPos;
    GameplayManager.Instance.levelControlType = levelSetupData.levelControlType;
    GameplayManager.Instance.shipLanes = levelSetupData.shipLanes;
    timeBetweenAsteroidShower = levelSetupData.timeBetweenAsteroidShower;

    InitializeLCC();

    if (levelSetupData.lccEnemyKills != -1)
    {
      SetupEnemies();
      StartCoroutine(UIManager.Instance.DoMissionStartLCCText(2f, 2f, $"KILL\n{levelSetupData.lccEnemyKills}\nENEMIES!"));
    }
    if (levelSetupData.lccKillBoss)
    {
      SetupBoss();
      StartCoroutine(UIManager.Instance.DoMissionStartLCCText(2f, 2f, "KILL\nTHE\nBOSS!"));

    }
    if (levelSetupData.spinningMineSpawnPointData.levelHasSpinningMine)
    {
      SetupSpinningMine();
    }

    numEnemyKillsInLevel = 0;
    currentTimeBetweenFiringAtPlayer = TIME_BETWEEN_FIRING_AT_PLAYER;

  }

  void InitializeLCC()
  {
    lccMet = false;

    //empty the lcc dictionary to clean it out
    LevelCompletionCriteria.Clear();
    
    //add the level completion criterias to the lcc dictionary
    if (levelSetupData.lccEnemyKills != -1) //checking if it's -1 beacase 0 could be a valid LCC number for enemykills
      LevelCompletionCriteria.Add("EnemyKills", levelSetupData.lccEnemyKills);
    if (levelSetupData.lccSurviveTime != -1)
      LevelCompletionCriteria.Add("SurviveTime", levelSetupData.lccSurviveTime);
    if (levelSetupData.lccKillBoss == true)
      LevelCompletionCriteria.Add("KillBoss", 1);

    //process the particular level completion criteria(s)
    foreach (string lccString in LevelCompletionCriteria.Keys)
    {
      if (lccString == "Enemy Kills")
      {
        UIManager.Instance.RequiredEnemyKillCount.text = "/" + LevelCompletionCriteria[lccString].ToString();
      }
    }
  }

  void SetupEnemies()
  {
    
        
    int index = 0;
    foreach (EnemySpawnPointData sp in levelSetupData.levelEnemySpawnPointData)
    {
      List<EnemyBehaviour02> enemyWavesChildrenBehaviourScripts = new List<EnemyBehaviour02>();

      var waveParentObject = new GameObject("EnemyWave_" + index);
      waveParentObject.transform.parent = enemyWavesParentGroupObject.transform; // keep all the enemy waveparentobjects together in the hierarchy under EnemyWavesParentGroupObject

      waveParentObject.transform.position = new Vector2(sp.startPos.x, sp.startPos.y);
      enemyWaves.Add(waveParentObject);

      verticalDistBetweenEnemies = sp.verticalDistBetweenEnemies;
      horizontalDistBetweenEnemies = sp.horizontalDistBetweenEnemies;

      if (sp.waypointPath != null)
      {
        pathInstance = Instantiate(sp.waypointPath, new Vector3(0f, 0f, 0f), Quaternion.identity);
        
      }

      for (int i = 0; i < sp.numEnemiesInWave; i++)
      {
        Vector3 startingPosition = new Vector3(sp.startPos.x + (i * horizontalDistBetweenEnemies), sp.startPos.y + (i * verticalDistBetweenEnemies));
        GameObject enemy = Instantiate(sp.enemyPrefab, startingPosition, Quaternion.identity, waveParentObject.transform);
        enemy.name = "Wave" + index + " Enemy" + i;
        EnemyBehaviour02 enemyScript = enemy.GetComponent<EnemyBehaviour02>();
        enemyWavesChildrenBehaviourScripts.Add(enemyScript);// add each of the enemies in the wave's behaviour scripts to a list
        enemyScript.startPosX = startingPosition.x;
        enemyScript.startPosY = startingPosition.y;
        enemyScript.speedMultiplierFromSpawner = sp.speedMultiplier;
        enemyScript.hpMultiplierFromSpawner = sp.hpMultiplier;
        enemyScript.timeBetweenSpawn = sp.timeBetweenSpawn;
        
        

        if (pathInstance != null)
        {
          //print($"In LevelManager, sp.waypointPath: {sp.waypointPath}");
          enemyScript.waypointPath = pathInstance;
        }
      }
      enemyWavesParentBehaviourScripts.Add(enemyWavesChildrenBehaviourScripts);
      index++;
    }
  }

  void SetupBoss()
  {
    bossMissilesParentPool = new GameObject("bossMissilesParentPoolObject");
    bossMissilesParentPool.tag = "bossMissilesParentPoolObject";

    GameObject boss = Instantiate(levelSetupData.bossSpawnPointData.bossPrefab, new Vector2(levelSetupData.bossSpawnPointData.startPos.x, levelSetupData.bossSpawnPointData.startPos.y), Quaternion.identity);
    
    bossScript = boss.GetComponent<BossBehaviour01>();

    if (levelSetupData.bossSpawnPointData.waypointPath != null)
    {
      bossScript.startPosX = levelSetupData.bossSpawnPointData.startPos.x;
      bossScript.startPosY = levelSetupData.bossSpawnPointData.startPos.y;
      bossScript.speedMultiplierFromSpawner = levelSetupData.bossSpawnPointData.speedMultiplier;
      bossScript.hpMultiplierFromSpawner = levelSetupData.bossSpawnPointData.hpMultiplier;

      pathInstance = Instantiate(levelSetupData.bossSpawnPointData.waypointPath, Vector3.zero, Quaternion.identity);
      bossScript.waypointPath = pathInstance;
      
    }

  }

  void SetupSpinningMine()
  {
    
    spinningMineInstance = Instantiate(levelSetupData.spinningMineSpawnPointData.spinningMinePrefab);

    SpinningMineBehaviour spinningMineScript = spinningMineInstance.GetComponent<SpinningMineBehaviour>();

    if (levelSetupData.spinningMineSpawnPointData.waypointPath != null)
    {
      spinningMineScript.waitTimeBeforeFirstSpawn   = levelSetupData.spinningMineSpawnPointData.waitTimeBeforeFirstSpawn;
      spinningMineScript.waitTimeBetweenSpawns      = levelSetupData.spinningMineSpawnPointData.waitTimeBetweenSpawns;
      spinningMineScript.speedMultiplierFromSpawner = levelSetupData.spinningMineSpawnPointData.speedMultiplier;
      spinningMineScript.hpMultiplierFromSpawner    = levelSetupData.spinningMineSpawnPointData.hpMultiplier;
      spinningMineScript.numBurstFiresBeforePause = levelSetupData.spinningMineSpawnPointData.numBurstFiresBeforePause;

      pathInstance = Instantiate(levelSetupData.spinningMineSpawnPointData.waypointPath, Vector3.zero, Quaternion.identity);
      spinningMineScript.waypointPath = pathInstance;

    }

  }

  void Update()
  {
    //if (Input.GetKeyDown(KeyCode.S))
    //spinningMineInstance = Instantiate(spinningMine);

    if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_IN_PROGRESS)
    {
      if (levelSetupData.lccEnemyKills != -1)
      {
        //print($"NumActiveWaves = { GetNumActiveWaves()}");
        //if there are less than the number of maxwaves, activate another (random) wave
        int numActiveWaves = GetNumActiveWaves();
        if ((numActiveWaves < levelSetupData.numMaxActiveWaves) && (currentlyActivatingWave == false))
        {
          StartCoroutine(ActivateEnemyWave());
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
      }
      
      CheckLCC(); // check LevelCompletionCriteria  
    }
    else if (GameplayManager.Instance.currentGameState == GameplayManager.GameState.LEVEL_COMPLETE)
    {
      DestroyChildrenWaves(enemyWavesParentGroupObject.transform);
    }

  }
  IEnumerator ActivateEnemyWave()
  {
    currentlyActivatingWave = true;
    var enemyWave = enemyWaves[Random.Range(0, enemyWaves.Count)];
    var enemyWavesChildrenBehaviourScripts = enemyWavesParentBehaviourScripts[Random.Range(0, enemyWavesParentBehaviourScripts.Count)];

    foreach (EnemyBehaviour02 enemyBehaviourScript in enemyWavesChildrenBehaviourScripts)
    {
      yield return new WaitForSeconds(enemyBehaviourScript.timeBetweenSpawn);
      enemyBehaviourScript.waveRespawnWaitOver = true;
    }
    currentlyActivatingWave = false;
  }


  /// <summary>
  /// loop through each of the enemywave's behaviour scripts, if any of the parent's children's state is ALIVE, increnent the number of active waves.
  /// </summary> 
  /// <returns>numActiveWaves</returns>
  private int GetNumActiveWaves()
  {
    int numActiveWaves = 0;
    bool activeWave = false;

    foreach (List<EnemyBehaviour02>enemyWaveParentBehaviourScripts in enemyWavesParentBehaviourScripts)
    {
      activeWave = false;
      foreach(EnemyBehaviour02 enemyBehaviourScript in enemyWaveParentBehaviourScripts)
      {
        if(enemyBehaviourScript.enemyState != EnemyBehaviour02.EnemyState.WAITING_TO_RESPAWN)
        {
          activeWave = true;
          break;// we've found at least 1 active enemy in that wave, so no need to check any further - break;
        }
      }
      if (activeWave)
        numActiveWaves++;
    }
    return numActiveWaves;
  }

  /// <summary>
  /// Calls GameObject.Destroy on all children of transform. and immediately detaches the children
  /// from transform so after this call tranform.childCount is zero.
  /// </summary>
  public void DestroyChildrenWaves(Transform transform) //http://forum.unity3d.com/threads/deleting-all-chidlren-of-an-object.92827/#post-2058407
  {
    for (int i = transform.childCount - 1; i >= 0; --i)
    {
      GameObject.Destroy(transform.GetChild(i).gameObject);
    }
    transform.DetachChildren();
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
            case "KillBoss":
              if (!bossHasBeenKilled)
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
        print("Level Completion Criteria TRUE!, Next Level is:" + (GameController.Instance.currentLevel + 1));
        
        GameplayManager.Instance.currentGameState = GameplayManager.GameState.LEVEL_OUTRO_IN_PROGRESS;
      }
    }    

  }

}
