using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MySaveTest : MonoBehaviour
{
  public ES3Spreadsheet TestSheet;
  ES3Spreadsheet sheet = new ES3Spreadsheet();
  //public TMP_Text testNumber01Text;
  //public TMP_Text testNumber02Text;

  public Dictionary<string, float> LevelStats = new Dictionary<string, float>();

  int testNumber01 = 1, testNumber02 = 2;
  void Start()
  {
    int num = 1;
    print("ES3Settings.defaultSettings.path = " + ES3Settings.defaultSettings.path);

    // Create an ES3Settings object to set the storage location to Resources.
    var settings = new ES3Settings();
    settings.location = ES3.Location.Resources;
    print("settings.location = " + settings.location);

    sheet = new ES3Spreadsheet();
    sheet.Load("mySheet.csv", settings);
    print($"Sheet has  { sheet.ColumnCount } columns, { sheet.RowCount } rows");

    //print($"BaseEnemyCollisionDamage = {(GetSheetStatValue("BaseEnemyCollisionDamage"))}");
    //print ($"EnemyHP at level 10 = {(GetSheetStatValue("EnemyHP", 10))}");
    //int HP = (int)GetSheetStatValue("EnemyHP", 10);
    //print($"EnemyRateOfFireMax at level 10 = {(GetSheetStatValue("EnemyRateOfFireMax", 10))}");
    //print($"EnemyRateOfFireMax at level 100 = {(GetSheetStatValue("EnemyRateOfFireMax", 100))}");
    //print($"EnemySpeed at level 4 = {(GetSheetStatValue("EnemySpeed", 4))}");

    GetLevelStats(1);
    GetLevelStats(100);
    foreach (string statName in LevelStats.Keys)
    {
      print($"Stats for level 1:  {statName} = {LevelStats[statName] }");

    }
  }
  private void GetLevelStats(int levelNum)
  {
    LevelStats.Clear();
		for (int i = 0; i < sheet.ColumnCount; i++)
		{
      string statName = sheet.GetCell<string>(i, 0);
      float statValue = GetSheetStatValue(statName, levelNum);
      //print($"Stat:");
      LevelStats.Add(statName, statValue);

    }

  }

  float GetSheetStatValue(string TextID, int LevelNumber)
  {
    for (int col = 0; col < sheet.ColumnCount; col++)
    {
      for (int row = 0; row < sheet.RowCount; row++)
      {
        string cellContent = sheet.GetCell<string>(col, row);
        if (cellContent == TextID)
				{
          Debug.Log($"At col:{col} row:{row} TextID: {cellContent}");
          return sheet.GetCell<float>(col, row + LevelNumber);
				}
      }
    }
    return -1; //error
  }

  float GetSheetStatValue(string TextID)
  {
    for (int col = 0; col < sheet.ColumnCount; col++)
    {
      for (int row = 0; row < sheet.RowCount; row++)
      {
        string cellContent = sheet.GetCell<string>(col, row);
        if (cellContent == TextID)
        {
          Debug.Log($"At col:{col} row:{row} TextID: {cellContent}");
          return sheet.GetCell<float>(col, row + 1);
        }
      }
    }
    return -1; //error
  }

  // Update is called once per frame
  void Update()
  {
    //if (Input.GetKeyDown(KeyCode.S))
    //{
    //  ES3.Save("myInt1", testNumber01);
    //  ES3.Save("myInt2", testNumber02);
    //}
    //if (Input.GetKeyDown(KeyCode.L))
    //{
    //  testNumber01 = ES3.Load("myInt1", 1);
    //  testNumber02 = ES3.Load("myInt2", 1);
    //}
    //if (Input.GetKeyDown(KeyCode.A))
    //{
    //  testNumber01--;
    //  testNumber02 -= 10;
    //}
    //if (Input.GetKeyDown(KeyCode.D))
    //{
    //  testNumber01++;
    //  testNumber02 += 10;
    //}

    //testNumber01Text.text = testNumber01.ToString();
    //testNumber02Text.text = testNumber02.ToString();



  }
}
