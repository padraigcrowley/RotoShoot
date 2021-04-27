/*--------------------------------------------
 *
 *    https://gamedev.stackexchange.com/questions/110958/what-is-the-proper-way-to-handle-data-between-scenes
 * 
 -----------------------------------------*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BootManager : Singleton<BootManager>
{

  void Awake()
  {
    //DontDestroyOnLoad(transform.gameObject); - Needed anymore????
    Application.targetFrameRate = 60;
  }

  void Start()
  {
    Debug.Log("The next word is <color=red>red</color>");
    print("The next word is <color=blue>blue</color>");
    //Load first game scene (probably main menu)
    //SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive); // TODO: Does this need to be additive?
    SceneManager.LoadScene("MainMenu");
  }
  
}