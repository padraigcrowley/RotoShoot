using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenuUIManager : MonoBehaviour
{
  public Slider playerShipRotationDurationSlider;
  public Text playerShipRotationDurationSliderTextValue;
  public Slider playerShipFireRateSlider;
  public Text playerShipFireRateSliderTextValue;
  public Slider maxEnemyHPSlider;
  public Text maxEnemyHPSliderTextValue;

  public Image DebugMenuPanel;
  private bool isDebugMenuPanelHidden;

  private bool paused = false;

  void Start()
  {
    DebugMenuPanel.gameObject.SetActive(false);
    isDebugMenuPanelHidden = true;

    playerShipRotationDurationSliderTextValue.text = GameplayManager.Instance.currentPlayerShipRotationDuration.ToString();
    playerShipRotationDurationSlider.SetValueWithoutNotify(GameplayManager.Instance.currentPlayerShipRotationDuration);
    playerShipFireRateSliderTextValue.text = GameplayManager.Instance.currentPlayerShipFireRate.ToString();
    playerShipFireRateSlider.SetValueWithoutNotify(GameplayManager.Instance.currentPlayerShipFireRate);
    maxEnemyHPSliderTextValue.text = GameplayManager.Instance.maxEnemy0001HP.ToString();
    maxEnemyHPSlider.SetValueWithoutNotify(GameplayManager.Instance.maxEnemy0001HP);
    /*
    playerShipRotationDurationSliderTextValue.text = MyGameplayManager.Instance.playerShipRotationDuration.ToString();
    playerShipRotationDurationSlider.value = MyGameplayManager.Instance.playerShipRotationDuration;
    playerShipFireRateSliderTextValue.text = MyGameplayManager.Instance.playerShipFireRate.ToString();
    playerShipFireRateSlider.value = MyGameplayManager.Instance.playerShipFireRate;
    maxEnemyHPSliderTextValue.text = MyGameplayManager.Instance.maxEnemy01HP.ToString();
    maxEnemyHPSlider.value = MyGameplayManager.Instance.maxEnemy01HP;*/
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void AdjustPlayerShipRotationDuration()
  {
    GameplayManager.Instance.currentPlayerShipRotationDuration = playerShipRotationDurationSlider.value;
    playerShipRotationDurationSliderTextValue.text = GameplayManager.Instance.currentPlayerShipRotationDuration.ToString();
    //PlayerPrefs.SetFloat("musicVolume", PlayerStats.MusicVolume);
    Debug.Log("PlayerShipRotationDurationSlider = " + GameplayManager.Instance.currentPlayerShipRotationDuration);
    
  }

  public void AdjustPlayerShipFireRate()
  {
    GameplayManager.Instance.currentPlayerShipFireRate = playerShipFireRateSlider.value;
    playerShipFireRateSliderTextValue.text = GameplayManager.Instance.currentPlayerShipFireRate.ToString();
    //PlayerPrefs.SetFloat("musicVolume", PlayerStats.MusicVolume);
    Debug.Log("playerShipFireRate = " + GameplayManager.Instance.currentPlayerShipFireRate);
  }

  public void AdjustMaxEnemy01HP()
  {
    GameplayManager.Instance.maxEnemy0001HP = (int)maxEnemyHPSlider.value;
    maxEnemyHPSliderTextValue.text = GameplayManager.Instance.maxEnemy0001HP.ToString();
    //PlayerPrefs.SetFloat("musicVolume", PlayerStats.MusicVolume);
    //Debug.Log("enemyHPSlider = " + enemyHP);
  }

  public void ToggleDebugMenu()
  {
    if (isDebugMenuPanelHidden)
    {
      DebugMenuPanel.gameObject.SetActive(true);
      isDebugMenuPanelHidden = false;
      paused = true;
    }
    else
    {
      DebugMenuPanel.gameObject.SetActive(false);
      isDebugMenuPanelHidden = true;
      paused = false;
    }
  }
}
