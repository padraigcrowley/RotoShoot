using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MySaveTest : MonoBehaviour
{
  public ES3Spreadsheet TestSheet;
  //public TMP_Text testNumber01Text;
  //public TMP_Text testNumber02Text;

  int testNumber01 = 1, testNumber02 = 2;
  // Start is called before the first frame update
  void Start()
  {
    int num = 1;
    print("ES3Settings.defaultSettings.path = " + ES3Settings.defaultSettings.path);

    // Create an ES3Settings object to set the storage location to Resources.
    var settings = new ES3Settings();
    settings.location = ES3.Location.Resources;
    print("settings.location = " + settings.location);

    //var sheet = new ES3Spreadsheet();
    //// Add data to cells in the spreadsheet.
    //for (int col = 0; col < 10; col++)
    //  for (int row = 0; row < 8; row++)
    //  {
    //    sheet.SetCell(col, row, num++);
    //    //sheet.SetCell(col, row, "someData");
    //  }

    //sheet.Save("Assets/Resources/mySheet02.csv", settings);

    var sheet2 = new ES3Spreadsheet();
    //sheet.Load("TestSheet_01.csv");
    sheet2.Load("mySheet.csv", settings);
    // Output the first row of the spreadsheet to console.
    for (int col = 0; col < sheet2.ColumnCount; col++)
      Debug.Log(sheet2.GetCell<int>(col, 0));

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