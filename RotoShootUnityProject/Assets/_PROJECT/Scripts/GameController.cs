using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController: Singleton<GameController>
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
    currentLevel = 0;

    // TODO: This needs to be expanded to handle > 0009 levels
    string levelName = "Level000";

    //unload the current levelscene, if loaded
    if (SceneManager.GetSceneByName(levelName + currentLevel.ToString()).isLoaded)
    {
      SceneManager.UnloadSceneAsync(levelName + Instance.currentLevel.ToString());
    }
    //set the level scene to the next level, load it
    currentLevel++;
    SceneManager.LoadScene(levelName + currentLevel.ToString(), LoadSceneMode.Additive);

  }

  // Update is called once per frame
  void Update()
  {

  }
}
