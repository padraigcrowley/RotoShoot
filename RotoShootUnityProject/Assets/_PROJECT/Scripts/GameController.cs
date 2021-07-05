using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
  public int currentLevel = 0;
  public int highestLevelPlayed = 1;
  public int weapon = 0;
  void Awake()
  {
    DontDestroyOnLoad(transform.gameObject);
  }

  void Start()
  {
    // TODO: this needs expanding to detect and load the latest level played by the player, or if they can choose to play earlier levels, etc.
    currentLevel = 1;
    LoadSpecificLevel(currentLevel);
  }
  public void LoadSpecificLevelAndBaseGame(int level)
  {
    
    LoadSpecificLevel(level);
    LoadBaseGame();

  }
	public void LoadSpecificLevel(int level)
  {
    GameController.Instance.currentLevel = level;

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
    }
  }

  public void UnloadSpecificLevel(int level)
	{
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
    }
  }

  public void LoadBaseGame()
  {
    //Check if the base scene is already loaded, if not, load it.
    if (!SceneManager.GetSceneByName("BaseGameScene").isLoaded)
    {
      SceneManager.LoadScene("BaseGameScene", LoadSceneMode.Additive);
    }
  }
}
