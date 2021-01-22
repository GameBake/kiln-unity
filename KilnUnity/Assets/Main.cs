using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Kiln;

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
            await kiln.Init();   
            Debug.Log("Initialized"); 
            Debug.Log("Platform is " + kiln.PlatformAvailable());
            // await kiln.LoadInterstitialAd("lalalala");
            // await kiln.ShowInterstitialAd("lalalala");
            // await kiln.LoadRewardedAd("lalalala");
            // KilnRewardedAdResponse response = await kiln.ShowRewardedAd("lalalala");
            // Debug.Log(response.getPlacementID());
            // await kiln.SetUserScore(123, null);
            // KilnScore score = await kiln.GetUserScore(null);
            // Debug.Log(score.getScore());
            // List<KilnScore> scores = await kiln.GetScores(10, 0, "lalala");
            // foreach(KilnScore score in scores) {
            //     Debug.Log(score.ToString());
            // }
            // List<KilnProduct> items = await kiln.GetAvailableProducts();
            // foreach(KilnProduct item in items) {
            //     Debug.Log(item.ToString());
            // }
            // List<KilnPurchase> items = await kiln.GetPurchasedProducts();
            // foreach(KilnPurchase item in items) {
            //     Debug.Log(item.ToString());
            // }

            KilnPurchase purchase = await kiln.PurchaseProduct("SKU010", null);

            List<KilnPurchase> items = await kiln.GetPurchasedProducts();
            foreach(KilnPurchase item in items) {
                Debug.Log(item.ToString());
            }
            Debug.Log("###############################################");
            await kiln.ConsumePurchasedProduct(purchase.GetPurchaseToken());

            items = await kiln.GetPurchasedProducts();
            foreach(KilnPurchase item in items) {
                Debug.Log(item.ToString());
            }


        }
        catch (KilnException ex) 
        {
            Debug.Log("Kiln Exception: " + ex);
        }
        catch (System.Exception ex)
        {
            Debug.Log("System Exception: " + ex);
        }
        return;
    }
}
