using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Kiln;

public class Main : MonoBehaviour
{
    private Bridge kiln;

    // Start is called before the first frame update
    void Start()
    {
        kiln = new Bridge();
        
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
            var config = new Configuration();

            config.DummyAds = new List<DummyAd> { 
                new DummyAd() 
                {
                    PlacementID="ABC001", 
                    RewardUser=true, 
                    AdType=AdType.REWARDED
                }, 
                new DummyAd() 
                {
                    PlacementID="ABC002",
                    RewardUser=false,
                    AdType=AdType.INTERSTITIAL
                }
            };

            await kiln.Init(config);   

            Debug.Log("Initialized"); 
            // await kiln.LoadInterstitialAd("ABC002");
            // await kiln.ShowInterstitialAd("ABC002");
            // await kiln.LoadRewardedAd("lalalala");
            // RewardedAdResponse response = await kiln.ShowRewardedAd("lalalala");
            // Debug.Log(response.getPlacementID());
            // await kiln.SetUserScore(123, null);
            // LeaderboardEntry score = await kiln.GetUserScore(null);
            // Debug.Log(score.getScore());
            // List<LeaderboardEntry> scores = await kiln.GetScores(10, 0, "lalala");
            // foreach(LeaderboardEntry score in scores) {
            //     Debug.Log(score.ToString());
            // }
            // List<Product> items = await kiln.GetAvailableProducts();
            // foreach(Product item in items) {
            //     Debug.Log(item.ToString());
            // }
            // List<Purchase> items = await kiln.GetPurchasedProducts();
            // foreach(Purchase item in items) {
            //     Debug.Log(item.ToString());
            // }
            // List<Product> items = await kiln.GetAvailableProducts(new List<string>{"SKU010", "SKU050"});
            // foreach(Product item in items) {
            //     Debug.Log(item.ToString());
            // }
            //
            // Purchase purchase = await kiln.PurchaseProduct("SKU010", null);

            // List<Purchase> items = await kiln.GetPurchasedProducts();
            // foreach(Purchase item in items) {
            //     Debug.Log(item.ToString());
            // }
            // Debug.Log("###############################################");
            // await kiln.ConsumePurchasedProduct(purchase.GetPurchaseToken());

            // items = await kiln.GetPurchasedProducts();
            // foreach(Purchase item in items) {
            //     Debug.Log(item.ToString());
            // }
        }
        catch (Kiln.Exception ex) 
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
