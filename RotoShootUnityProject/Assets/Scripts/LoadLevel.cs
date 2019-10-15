using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
  public void LoadSpecificLevel(int level)
  {
    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";
    SceneManager.LoadScene("BaseGameScene");
    SceneManager.LoadScene(levelName+level.ToString(), LoadSceneMode.Additive);
    
  }

  public void LoadNextLevel()
  {
    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";
    GameManagerX.Instance.currentLevel++;

    SceneManager.LoadScene("BaseGameScene");
    SceneManager.LoadScene(levelName + GameManagerX.Instance.currentLevel.ToString(), LoadSceneMode.Additive);
  }


}
