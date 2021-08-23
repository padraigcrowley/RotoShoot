using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USB_TestController : MonoBehaviour
{
    // Start is called before the first frame update
  public int currHealth,maxHealth   = 1000;
  public UltimateStatusBar SpinningMineStatusBar;
  void Start()
    {
    currHealth = maxHealth;
    UltimateStatusBar.UpdateStatus("SpinningMine2StatusBar", currHealth, maxHealth);
  }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.S))
    {
      currHealth -= 100;
      UltimateStatusBar.UpdateStatus("SpinningMine2StatusBar", currHealth, maxHealth);
    }
    if (Input.GetKeyDown(KeyCode.D))
    {
      currHealth += 100;
      UltimateStatusBar.UpdateStatus("SpinningMine2StatusBar", currHealth, maxHealth);
    }
  }
}
