using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif

public class AdsIAPController : MonoBehaviour
{
  public bool isReady;
  const int FREE_ADS_STARCOIN_AMOUNT = 10;
  public MainMenuContoller MainMenuControllerScript;

  // Start is called before the first frame update
  void Start()
    {
    Advertising.Initialize();
    InAppPurchasing.InitializePurchasing();
  }


  public void ShowRewardedAd()
  {

    isReady = Advertising.IsRewardedAdReady();

    // Show it if it's ready
    if (isReady)
    {
      Advertising.ShowRewardedAd();
      print("Showing ad");
    }
    else
    {
      print("Not Ready");
    }
  }

  void OnEnable()
  {
    Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
    Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;

    //InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
    //InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
  }

  void OnDisable()
  {
    Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
    Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;

    //InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
    //InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
  }

  void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement placement)
  {
    Debug.LogWarning("Rewarded ad has completed. The user should be rewarded now.");
    GameController.Instance.starCoinCount += FREE_ADS_STARCOIN_AMOUNT;
    ES3.Save("starCoinCount", GameController.Instance.starCoinCount);
    MainMenuControllerScript.UpdateUpgradesMenuStatsText();
  }

  // Event handler called when a rewarded ad has been skipped
  void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement placement)
  {
    Debug.LogWarning("Rewarded ad was skipped. The user should NOT be rewarded.");
  }

  // Update is called once per frame
  void Update()
    {
        
    }
}
