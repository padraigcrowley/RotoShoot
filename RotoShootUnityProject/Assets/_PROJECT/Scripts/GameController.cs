using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
  public const int NUM_UNIQUE_LEVELS = 10;
  public int currentLevelPlaying = 1;
  public int currentLevelBackground = 1;
  
  public int highestLevelPlayed = 1;
  
/// <summary>
/// initial starting values for Player / Ship
/// </summary>
  public int starCoinCount = 0;
  public int maxPlayerHPLevel = 1;
  public int playerMissileDamageLevel = 1;
  public int shieldDurationLevel = 1;
  public int powerupDurationLevel = 1;
  public float maxPlayerHP = 1;
  public float playerMissileDamage = 1;
  public float shieldDuration = 1;
  public float powerupDuration = 1;
  public float musicVolume = .8f;
  public float soundsVolume = .8f;


  public ES3Spreadsheet levelStatsSpreadsheet = new ES3Spreadsheet();
 public ES3Spreadsheet playerStatsSpreadsheet = new ES3Spreadsheet();

  void Awake()
  {
    highestLevelPlayed = ES3.Load("highestLevelPlayed", 1);
    starCoinCount = ES3.Load("starCoinCount", starCoinCount);
		maxPlayerHPLevel = ES3.Load("maxPlayerHPLevel", maxPlayerHPLevel);
		playerMissileDamageLevel = ES3.Load("playerMissileDamageLevel", playerMissileDamageLevel);
		shieldDurationLevel = ES3.Load("shieldDurationLevel", shieldDurationLevel);
		powerupDurationLevel = ES3.Load("powerupDurationLevel", powerupDurationLevel);
    musicVolume = ES3.Load("musicVolume", 0.8f);
    soundsVolume = ES3.Load("soundsVolume", 0.8f);
		currentLevelPlaying = highestLevelPlayed;
    GetStatsSpreadsheetData();
    GetPlayerStatsSpreadsheetData();
  }

  void Start()
  {
    LoadSpecificLevel(currentLevelPlaying);
  }

  private void GetPlayerStatsSpreadsheetData()
  {
    maxPlayerHP = GetSheetStatValue(playerStatsSpreadsheet, "maxPlayerHP", maxPlayerHPLevel);
    playerMissileDamage = GetSheetStatValue(playerStatsSpreadsheet, "playerMissileDamage", playerMissileDamageLevel);
    shieldDuration = GetSheetStatValue(playerStatsSpreadsheet, "shieldDuration", shieldDurationLevel); 
     powerupDuration = GetSheetStatValue(playerStatsSpreadsheet, "powerupDuration", powerupDurationLevel);
  }

    private void GetStatsSpreadsheetData()
	{
    print("ES3Settings.defaultSettings.path = " + ES3Settings.defaultSettings.path);

    // Create an ES3Settings object to set the storage location to Resources.
    var settings = new ES3Settings();
    settings.location = ES3.Location.Resources;
    print("settings.location = " + settings.location);

    levelStatsSpreadsheet = new ES3Spreadsheet();
    levelStatsSpreadsheet.Load("mySheet.csv", settings);
    playerStatsSpreadsheet = new ES3Spreadsheet();
    playerStatsSpreadsheet.Load("PlayerStatsSheet.csv", settings);

    print($"StatsSheet has  { levelStatsSpreadsheet.ColumnCount } columns, { levelStatsSpreadsheet.RowCount } rows");
    print($"PlayerStatSheet has  { playerStatsSpreadsheet.ColumnCount } columns, { playerStatsSpreadsheet.RowCount } rows");
  }

  public float GetSheetStatValue(ES3Spreadsheet spreadSheet, string TextID, int LevelNumber)
  {
    for (int row = 0; row < spreadSheet.RowCount; row++)
    {
      for (int col = 0; col < spreadSheet.ColumnCount; col++)
        {
        string cellContent = spreadSheet.GetCell<string>(col, row);
        if (cellContent.Contains(TextID))
        {
          return spreadSheet.GetCell<float>(col, row + LevelNumber);
        }
      }
    }
    return -1; //error
  }

  public void LoadSpecificLevelAndBaseGame(int level)
  {
    level = level % NUM_UNIQUE_LEVELS;
    if (level == 0)
      level = NUM_UNIQUE_LEVELS;

    LoadSpecificLevel(level % NUM_UNIQUE_LEVELS);
    LoadBaseGame();
  }
	public void LoadSpecificLevel(int level)
  {
    //GameController.Instance.currentLevelPlaying = level;
    level = level % NUM_UNIQUE_LEVELS;
    if (level == 0)
      level = NUM_UNIQUE_LEVELS;

    // handles up to 999 levels
    string levelName = "";
    if (level > 0 && level < 10)
      levelName = "Level000";
    else if (level  >= 10 && level < 100)
      levelName = "Level00";
    else if (level >= 100 && level < 1000)
      levelName = "Level0";

    //load it if it's not already loaded
    if (!SceneManager.GetSceneByName(levelName + level.ToString()).isLoaded)
    {
      SceneManager.LoadScene(levelName + level.ToString(), LoadSceneMode.Additive);
      currentLevelBackground = level;
    }
  }

  public void UnloadSpecificLevel(int level)
	{

   level = level % NUM_UNIQUE_LEVELS;
   if (level == 0)
      level = NUM_UNIQUE_LEVELS;

    // handles up to 999 levels
    string levelName = "";
    if (level > 0 && level < 10)
      levelName = "Level000";
    else if (level >= 10 && level < 100)
      levelName = "Level00";
    else if (level >= 100 && level < 1000)
      levelName = "Level0";

    //unload the current levelscene, if loaded
    if (SceneManager.GetSceneByName(levelName + level.ToString()).isLoaded)
    {
      SceneManager.UnloadSceneAsync(levelName + level.ToString());
      //currentLevelBackground = 0;
    }
  }

  public void LoadBaseGame()
  {
    //Check if the base scene is already loaded, if not, load it.
    if (!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    {
      SceneManager.LoadSceneAsync ("BaseGameScene", LoadSceneMode.Additive);
    }
  }
}
