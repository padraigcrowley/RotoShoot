using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
  public void LoadSpecificLevel(int level)
  {
    GameController.Instance.currentLevel = level;
    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";
    //if (!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    { 
      SceneManager.LoadScene("BaseGameScene"); 
    }
    SceneManager.LoadScene(levelName+level.ToString(), LoadSceneMode.Additive);

    //SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName + level.ToString()));
    // Ouput the name of the active Scene
    // See now that the name is updated
    //Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);

  }

  public void LoadNextLevel()
  {
    //// TODO: This needs to be expanded to handle > 0009 levels
    //string levelName = "Level000";

    ////unload the current levelscene, if loaded
    //if (SceneManager.GetSceneByName(levelName + GameController.Instance.currentLevel.ToString()).isLoaded)
    //{
    //  SceneManager.UnloadSceneAsync(levelName + GameController.Instance.currentLevel.ToString());
    //}
    
    
    //Check if the base scene is already loaded, if not, load it.
    if(!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    {
      SceneManager.LoadScene("BaseGameScene", LoadSceneMode.Additive);
    }

    ////set the level scene to the next level, load it
    //GameController.Instance.currentLevel++;
    //SceneManager.LoadScene(levelName + GameController.Instance.currentLevel.ToString(), LoadSceneMode.Additive);
  }


}
