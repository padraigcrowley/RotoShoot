using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerX : Singleton<GameManagerX>
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
  }

  // Update is called once per frame
  void Update()
  {

  }
}
