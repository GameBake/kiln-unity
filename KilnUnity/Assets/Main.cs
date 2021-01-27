using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    private KilnBridge kiln;

    // Start is called before the first frame update
    void Start()
    {
        kiln = new KilnBridge();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public async void InitButtonPressed() 
    {
        Debug.Log("Init button pressed!");
        try
        {
            var config = new KilnConfiguration();

            config.DummyAds = new List<KilnDummyAd> { 
                new KilnDummyAd() 
                {
                    PlacementID="ABC001", 
                    RewardUser=true, 
                    AdType=KilnAdType.Rewarded
                }, 
                new KilnDummyAd() 
                {
                    PlacementID="ABC002",
                    RewardUser=false,
                    AdType=KilnAdType.Interstitial
                }
            };

            await kiln.Init(config);   

            Debug.Log("Initialized"); 
            await kiln.LoadInterstitialAd("ABC002");
            await kiln.ShowInterstitialAd("ABC002");
            // await kiln.LoadRewardedAd("lalalala");
            // KilnRewardedAdResponse response = await kiln.ShowRewardedAd("lalalala");
            // Debug.Log(response.getPlacementID());
            // await kiln.SetUserScore(123, null);
            // KilnScore score = await kiln.GetUserScore(null);
            // Debug.Log(score.GetScore());
            // List<KilnScore> scores = await kiln.GetScores(10, 0, "lalala");
            // foreach(KilnScore ascore in scores) {
            //     Debug.Log(ascore.ToString());
            // }
            // List<KilnProduct> items = await kiln.GetAvailableProducts();
            // foreach(KilnProduct item in items) {
            //     Debug.Log(item.ToString());
            // }
            // List<KilnProduct> items = await kiln.GetAvailableProducts(new List<string>{"SKU010", "SKU050"});
            // foreach(KilnProduct item in items) {
            //     Debug.Log(item.ToString());
            // }
            // List<KilnPurchase> items = await kiln.GetPurchasedProducts();
            // foreach(KilnPurchase item in items) {
            //     Debug.Log(item.ToString());
            // }
            //
            // KilnPurchase purchase = await kiln.PurchaseProduct("SKU010", null);

            // List<KilnPurchase> items = await kiln.GetPurchasedProducts();
            // foreach(KilnPurchase item in items) {
            //     Debug.Log(item.ToString());
            // }
            // Debug.Log("###############################################");
            // await kiln.ConsumePurchasedProduct(purchase.GetPurchaseToken());

            // items = await kiln.GetPurchasedProducts();
            // foreach(KilnPurchase item in items) {
            //     Debug.Log(item.ToString());
            // }


        }
        catch (KilnException ex) 
        {
            Debug.Log("Kiln Exception: " + ex.Message);
        }
        catch (System.Exception ex)
        {
            Debug.Log("System Exception: " + ex);
        }
        return;
    }
}
