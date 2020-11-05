using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
  public void LoadSpecificLevel(int level)
  {
    GameManagerX.Instance.currentLevel = level;
    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";
    //if (!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    { 
      SceneManager.LoadScene("BaseGameScene"); 
    }
    SceneManager.LoadScene(levelName+level.ToString(), LoadSceneMode.Additive);
    
  }

  public void LoadNextLevel()
  {
    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";

    //unload the current levelscene, if loaded
    if (SceneManager.GetSceneByName(levelName + GameManagerX.Instance.currentLevel.ToString()).isLoaded)
    {
      SceneManager.UnloadSceneAsync(levelName + GameManagerX.Instance.currentLevel.ToString());
    }
    
    
    //Check if the base scene is already loaded, if not, load it.
    if(!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    {
      SceneManager.LoadScene("BaseGameScene");
    }
    
    //set the level scene to the next level, load it
    GameManagerX.Instance.currentLevel++;
    SceneManager.LoadScene(levelName + GameManagerX.Instance.currentLevel.ToString(), LoadSceneMode.Additive);
  }


}
