using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace my_SpaceshipExample
{

  public class SBGameManager : MonoBehaviour
  {
    float maxHealth = 50f, health;
    //public UltimateStatusBar statusBar;

    // Start is called before the first frame update
    void Start()
    {
      health = maxHealth;
      //UltimateStatusBar.UpdateStatus("myStatusBar", health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.A))
      {
        health--;
        UltimateStatusBar.UpdateStatus("myStatusBar",  health, maxHealth);
        //UltimateStatusBar.UpdateStatus("Player", currentHealth, maxHealth, "Health");
      }
      if (Input.GetKeyDown(KeyCode.D))
      {
        health++;
        UltimateStatusBar.UpdateStatus("myStatusBar", health, maxHealth);
      }
    }
  }
}
