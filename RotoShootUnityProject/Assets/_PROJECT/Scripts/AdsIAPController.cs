using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
#if EM_UIAP
using UnityEngine.Purchasing;
#endif

public class AdsIAPController : Singleton<AdsIAPController>
{
  const int FREE_ADS_STARCOIN_AMOUNT = 10;
  public string[] iapIDs = { "iap_coins_0199", "iap_coins_0899", "iap_coins_4999" };
  public int[] iapCoinAmounts = { 50,475,5050 };
  
  public bool isReady;
  public IAPProduct[] iaps;
  private static int iapBeingBought = 0; // 0, 1 or 2
  public Dictionary<string, string> iapsLocalizedPrices = new Dictionary<string, string>(); //<productIDstring, LocalizedPriceString>
  public MainMenuContoller MainMenuControllerScript;

  public TMPro.TMP_Text starCoinCountText;
  public TMPro.TMP_Text getMoreCoinsPanelStarCoinCountText;
  void Start()
  {
    StartCoroutine(DelayInitialization());
  }

  private IEnumerator DelayInitialization()
  {
    yield return new WaitForSeconds(4);

    Advertising.Initialize();
    InAppPurchasing.InitializePurchasing();
    IAPProduct[] iaps = InAppPurchasing.GetAllIAPProducts();
    foreach (IAPProduct iap in iaps)
    {
      Debug.LogWarning("--------------------rabbitbadger----------------------------------------");
      Debug.LogWarning("Product name: " + iap.Name);
      Debug.LogWarning("Product ID: " + iap.Id);
      Debug.LogWarning("Product price: " + iap.Price );
#if EM_UIAP
      // EM_IAPConstants.Sample_Product is the generated name constant of a product named "Sample Product"
      Product sampleProduct = InAppPurchasing.GetProduct(iap.Id);
      Debug.LogWarning("IAP Metadata localizedPriceString: " + sampleProduct.metadata.localizedPriceString.ToString());
      iapsLocalizedPrices.Add(iap.Id, sampleProduct.metadata.localizedPriceString.ToString());
#endif
    }
  }

  public void BuyIAP(int iapNumber) // 0, 1 or 2
  {
    iapBeingBought = iapNumber;
    InAppPurchasing.Purchase(iapIDs[iapNumber]);

  }

  public void ShowRewardedAd()
  {

    isReady = Advertising.IsRewardedAdReady();

    // Show it if it's ready
    if (isReady)
    {
      Advertising.ShowRewardedAd();
      //print("Showing ad");
    }
    else
    {
      //print("Not Ready");
    }
  }

  void OnEnable()
  {
    Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
    Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;

    Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;

    InAppPurchasing.PurchaseCompleted += PurchaseCompletedHandler;
    InAppPurchasing.PurchaseFailed += PurchaseFailedHandler;
  }

  void OnDisable()
  {
    Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
    Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;

    Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;

    InAppPurchasing.PurchaseCompleted -= PurchaseCompletedHandler;
    InAppPurchasing.PurchaseFailed -= PurchaseFailedHandler;
  }
  void PurchaseCompletedHandler(IAPProduct product)
  {

    GetSampleProduct(product.Name);
    GameController.Instance.starCoinCount += iapCoinAmounts[iapBeingBought];
    starCoinCountText.text = ("" + GameController.Instance.starCoinCount);
    getMoreCoinsPanelStarCoinCountText.text = ("" + GameController.Instance.starCoinCount);

  }
  void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement placement)
  {
    Debug.Log("Interstitial ad has been closed.");
  }

  public void GetSampleProduct(string productID)
  {
#if EM_UIAP
    // EM_IAPConstants.Sample_Product is the generated name constant of a product named "Sample Product"
    Product sampleProduct = InAppPurchasing.GetProduct(productID);

    if (sampleProduct != null)
    {

      ProductMetadata data = InAppPurchasing.GetProductLocalizedData(productID);
      if (data != null)
      {
        Debug.LogWarning("Localized title: " + data.localizedTitle);
        Debug.LogWarning("Localized description: " + data.localizedDescription);
        Debug.LogWarning("Localized price string: " + data.localizedPriceString);
      }


      if (sampleProduct.hasReceipt)
      {
        Debug.LogWarning("Receipt: " + sampleProduct.receipt);
      }

    }
#endif
  }

  // Failed purchase handler
  void PurchaseFailedHandler(IAPProduct product, string failureReason)
  {
    Debug.LogWarning("The purchase of product " + product.Name + " has failed with reason: " + failureReason);
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

  
}
