using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine; 

public class KilnException : System.Exception {
    private static readonly string DefaultMessage = "Java Exception.";

    public KilnException() : base(DefaultMessage) { }
    public KilnException(string message) : base(message) { }
    public KilnException(string message, System.Exception innerException)
    : base(message, innerException) { }    
}

public enum KilnPlatform {
    Development
}

public interface IKilnObjectWrapper {

    AndroidJavaObject JavaInst { set;  }
}

public class KilnRewardedAdResponse: IKilnObjectWrapper {

    private AndroidJavaObject javaInst;

    public AndroidJavaObject JavaInst { set => javaInst = value;  }

    public string getPlacementID() {
        return javaInst.Call<string>("getPlacementID");
    }
    public bool getWithReward() {
        return javaInst.Call<bool>("getWithReward");
    }

}

public class KilnScore: IKilnObjectWrapper {

    private AndroidJavaObject javaInst;
    public AndroidJavaObject JavaInst { set => javaInst = value;  }

    public double GetScore() {
        return javaInst.Call<double>("getScore");
    }
    public int GetRank() {
        return javaInst.Call<int>("getRank");
    }

    new public string ToString() {
        return "score:" + GetScore() + ", rank:" + GetRank();
    }

}

public class KilnProduct: IKilnObjectWrapper {

    private AndroidJavaObject javaInst;
    public AndroidJavaObject JavaInst { set => javaInst = value;  }

    public string GetTitle()
    {
        return javaInst.Call<string>("getTitle");
    }

    public string GetProductID()
    {
        return javaInst.Call<string>("getProductID");
    }

    public string GetDescription()
    {
        return javaInst.Call<string>("getDescription");
    }

    public string GetImageURI()
    {
        return javaInst.Call<string>("getImageURI");
    }

    public string GetPrice()
    {
        return javaInst.Call<string>("getPrice");
    }

    public string GetPriceCurrencyCode()
    {
        return javaInst.Call<string>("getPriceCurrencyCode");
    }

    new public string ToString() {
        return javaInst.Call<string>("toString");
    }    

}
public class KilnPurchase: IKilnObjectWrapper {

    private AndroidJavaObject javaInst;
    public AndroidJavaObject JavaInst { set => javaInst = value;  }


    public string GetDeveloperPayload()
    {
        return javaInst.Call<string>("getDeveloperPayload");
    }    
    public string GetPaymentID()
    {
        return javaInst.Call<string>("getPaymentID");
    }    

    public string GetProductID()
    {
        return javaInst.Call<string>("getProductID");
    }    

    public string GetPurchaseTime()
    {
        return javaInst.Call<string>("getPurchaseTime");
    }    

    public string GetPurchaseToken()
    {
        return javaInst.Call<string>("getPurchaseToken");
    }    

    public string GetSignedRequest()
    {
        return javaInst.Call<string>("getSignedRequest");
    }    

    new public string ToString() {
        return javaInst.Call<string>("toString");
    }    

}

public class KilnAnalyticEvent : AndroidJavaProxy 
{
    public string Category { get; set; }

    public string Action { get; set; }

    public string Label { get; set; }

    public string Value { get; set; }

    public KilnAnalyticEvent() : base("io.gamebake.kiln.types.AnalyticEvent") {}
    public string getCategory() {
        return Category;
    }
    public string getAction() {
        return Action;
    }
    public string getLabel() {
        return Label;
    }
    public string getValue() {
        return Value;
    }

}

public class KilnBridge {
	private AndroidJavaObject kiln;

    class KilnCallback<T>: AndroidJavaProxy {

        private TaskCompletionSource<T> taskCompletionSource;

        protected IKilnObjectWrapper wrapper;

        public TaskCompletionSource<T> Tcs 
        {
            get {
                return taskCompletionSource;
            }
            set {
                taskCompletionSource = value;
            }
        }

        public IKilnObjectWrapper Wrapper 
        {
            get {
                return wrapper;
            }
            set {
                wrapper = value;
            }
        }

        public KilnCallback(): base("io.gamebake.kiln.Callback") {}

        public void onSuccess(object result) 
        {
            Debug.Log("KilnCallback onSuccess");
            if (wrapper != null) {
                wrapper.JavaInst = (AndroidJavaObject)result;
                taskCompletionSource.SetResult((T)wrapper);
            }
            else {
                taskCompletionSource.SetResult((T)result);
            }
        }

        public void onFailure(AndroidJavaObject exception) 
        {
            Debug.Log("KilnCallback onFailure");
            taskCompletionSource.SetException(new KilnException(exception.ToString()));
        }

    }

    class KilnListCallback<T>: KilnCallback<List<T>> {

        new public void onSuccess(object result) 
        {
            Debug.Log("KilnCallback onSuccess");

            List<T> outList = new List<T>();
            int size = ((AndroidJavaObject)result).Call<int>("size");

            for (int i = 0; i < size; i++)
            {
                T instance = (T)System.Activator.CreateInstance(typeof(T));
                ((IKilnObjectWrapper)instance).JavaInst = ((AndroidJavaObject)result).Call<AndroidJavaObject>("get", i);
                outList.Add(instance);
            }
            Tcs.SetResult(outList);
        }


    }

    class KilnPurchaseSettings: AndroidJavaProxy
    {
        public KilnPurchaseSettings(): base("io.gamebake.kiln.types.PurchaseSettings") {}

        private string productID;
        private string developerPayload;

        public string ProductID 
        {
            set {
                productID = value;
            }
        }

        public string DeveloperPayload 
        {
            set {
                developerPayload = value;
            }
        }

        public string getProductID() {
            return productID;
        }

        public string getDeveloperPayload() {
            return developerPayload;
        }

    }

    public Task Init() 
    {
        string AD_UNIT_ID = "ad_unit_id";

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // getBaseContext or getApplicationContext
        // AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaObject configBuilder = new AndroidJavaObject("io.gamebake.kiln.KilnConfiguration$Builder", context, AD_UNIT_ID);

        AndroidJavaObject config = configBuilder.Call<AndroidJavaObject>("build");

        kiln = new AndroidJavaObject("io.gamebake.kiln.Kiln", config);

        var aTcs = new TaskCompletionSource<object>();

        kiln.Call("init", new KilnCallback<object>(){
            Tcs = aTcs
        });

        return aTcs.Task;
    }

    /// <summary>
    /// method <c>platformAvailable</c> to check platform for a custom setting/configuration/option.
    /// </summary>
    /// <returns>the platform available</returns>
    public KilnPlatform PlatformAvailable() 
    {
        AndroidJavaClass kClass = new AndroidJavaClass("io.gamebake.kiln.Kiln");
        AndroidJavaObject platformEnum = kClass.CallStatic<AndroidJavaObject>("platformAvailable");
        return (KilnPlatform)platformEnum.Call<int>("ordinal");
    }

    /// <summary>
    /// Use this to check if the underlying platform supports interstitial ads
    /// </summary>
    /// <returns>boolean true if it's supported, false otherwise</returns>
    public bool SupportsInterstitialAds() 
    {
        return kiln.Call<bool>("supportsInterstitialAds");
    }

    /// <summary>
    /// It loads an interstitial ad. If the current platform doesn't support interstitial ads or there's a 
    /// problem loading the Task will fail with an exception <c>KilnException</c> 
    /// (you can check supportsInterstitialAds previously).
    /// </summary>
    /// <param name="identifier">the ad identifier</param>
    /// <returns>Task</returns>
    public Task LoadInterstitialAd(string identifier)  
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("loadInterstitialAd", identifier, new KilnCallback<object>(){
            Tcs = aTcs
        });
        return aTcs.Task;
    }

    /// <summary>
    /// It shows the interstitial ad. If the platform doesn't support interstitial ads or there's any problem the 
    /// Task will get an exception <c>KilnException</c> 
    /// (check supportsInterstitialAds). Remember to call loadInterstitialAd previously.
    /// </summary>
    /// <param name="identifier">the ad identifier</param>
    /// <returns>Task</returns>
    public Task ShowInterstitialAd(string identifier) 
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("showInterstitialAd", identifier, new KilnCallback<object>(){
            Tcs = aTcs
        });
        return aTcs.Task;
    }

    /// <summary>
    /// Use this to check if the underlying platform supports rewarded ads
    /// </summary>
    /// <returns>boolean true if it's supported, false otherwise</returns>
    public bool SupportsRewardedAds() {
        return kiln.Call<bool>("supportsRewardedAds");
    }

    /// <summary>
    /// It loads a rewarded ad. If the current platform doesn't support rewarded ads or there's a 
    /// problem loading the Task will fail with an exception <c>KilnException</c> 
    /// (you can check supportsRewardedAds previously).
    /// </summary>
    /// <param name="identifier">the ad identifier</param>
    /// <returns>Task</returns>
    public Task LoadRewardedAd(string identifier)  
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("loadRewardedAd", identifier, new KilnCallback<object>(){
            Tcs = aTcs
        });
        return aTcs.Task;
    }

    /// <summary>
    /// It shows the rewarded ad. If the platform doesn't support rewarded ads or there's any problem the 
    /// Task will get an exception <c>KilnException</c> 
    /// (check supportsRewardedAds). Remember to call loadRewardedAd previously.
    /// </summary>
    /// <param name="identifier">the ad identifier</param>
    /// <returns>Task</returns>
    public Task<KilnRewardedAdResponse> ShowRewardedAd(string identifier) 
    {
        var aTcs = new TaskCompletionSource<KilnRewardedAdResponse>();
        KilnCallback<KilnRewardedAdResponse> callback = new KilnCallback<KilnRewardedAdResponse>()
        {
            Tcs = aTcs
        };
        callback.Wrapper = new KilnRewardedAdResponse();
        kiln.Call("showRewardedAd", identifier, callback);
        return aTcs.Task;
    }

    /// <summary>
    /// Check If the current platform supports leaderboards
    /// </summary>
    /// <returns><c>true</c> if platform supports leaderboards, false otherwise</returns>
    public bool SupportsLeaderboards() 
    {
        return kiln.Call<bool>("supportsLeaderboards");
    }

    /// <summary>
    /// It sets the user score. If the platform doesn't support leaderboards the Task  
    /// will get an exception <c>KilnException</c>
    /// </summary>
    /// <param name="score">score to set</param>
    /// <param name="data">data optional. If the platform supports it, additional data to set.</param>
    /// <returns>Task</returns>
    public Task SetUserScore(double score, object data) 
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("setUserScore", score, data, new KilnCallback<object>() {
            Tcs = aTcs
        });
        return aTcs.Task;        
    }

    /// <summary>
    /// It retrieves the user score. If the platform doesn't support leaderboards the Task  
    /// will get an exception <c>KilnException</c>
    /// </summary>
    /// <param name="id">the user identifier</param>
    /// <returns>Task</returns>
    public Task<KilnScore> GetUserScore(string id)
    {
        var aTcs = new TaskCompletionSource<KilnScore>();
        kiln.Call("getUserScore", id, new KilnCallback<KilnScore>() {
            Tcs = aTcs,
            Wrapper = new KilnScore()
        });
        return aTcs.Task;        
    }

    /// <summary>
    /// It gets leaderboard scores for all players. If the platform doesn't support leaderboards the Task  
    /// will get an exception <c>KilnException</c>
    /// </summary>
    /// <param name="count">number of entries to retrieve. default to 10 if unspecified</param>
    /// <param name="offset">The offset from the top of the leaderboard that entries will be fetched from. default 0 if not specified</param>
    /// <param name="id">the leaderboard identifier</param>
    /// <returns></returns>
    public Task<List<KilnScore>> GetScores(int count, int offset, string id)
    {
        var aTcs = new TaskCompletionSource<List<KilnScore>>();
        kiln.Call("getScores",  count, offset, id, new KilnListCallback<KilnScore>() {
            Tcs = aTcs,
            Wrapper = new KilnScore()
        });
        return aTcs.Task;
    }

    /// <summary>
    /// It checks if the current platform supports in app purchases
    /// </summary>
    /// <returns>true if supported, false otherwise</returns>
    public bool SupportsIAP() {
        return kiln.Call<bool>("supportsIAP");
    }

    /// <summary>
    /// Retrieves the list of available products to be purchased. If the platform doesn't support
    /// purchases the Task will get an exception <c>KilnException</c>
    /// </summary>
    /// <returns>Task</returns>
    public Task<List<KilnProduct>> GetAvailableProducts() {
        var aTcs = new TaskCompletionSource<List<KilnProduct>>();
        kiln.Call("getAvailableProducts", new KilnListCallback<KilnProduct>() {
            Tcs = aTcs,
            Wrapper = new KilnProduct()
        });
        return aTcs.Task;
    }

    /// <summary>
    /// It gets the list of products already purchased but still unconsumed. If the platform doesn't support 
    /// purchases the Task will get an exception <c>KilnException</c>
    /// </summary>
    /// <returns>Task</returns>
    public Task<List<KilnPurchase>> GetPurchasedProducts()
    {
        var aTcs = new TaskCompletionSource<List<KilnPurchase>>();
        kiln.Call("getPurchasedProducts", new KilnListCallback<KilnPurchase>() {
            Tcs = aTcs,
            Wrapper = new KilnPurchase()
        });
        return aTcs.Task;
    }

    /// <summary>
    /// Purchase of a product. If the platform doesn't support purchases 
    /// the Task will get an exception <c>KilnException</c>
    /// </summary>
    /// <param name="productID">id to refer the product to be purchased</param>
    /// <param name="payload">additional data to send with the purchase</param>
    /// <returns></returns>
    public Task<KilnPurchase> PurchaseProduct(string productID, string payload) 
    {
        var aTcs = new TaskCompletionSource<KilnPurchase>();
        KilnPurchaseSettings settings = new KilnPurchaseSettings() { 
            ProductID = productID,
            DeveloperPayload = payload
        };
        kiln.Call("purchaseProduct", settings, new KilnCallback<KilnPurchase>() {
            Tcs = aTcs,
            Wrapper = new KilnPurchase()
        });
        return aTcs.Task;
    }

    /// <summary>
    /// It consumes a product already purchased. If the platform doesn't support purchases 
    /// the Task will get an exception <c>KilnException</c>
    /// </summary>
    /// <param name="purchaseToken">the product token</param>
    /// <returns>Task</returns>
    public Task ConsumePurchasedProduct(string purchaseToken)
    {
        var aTcs = new TaskCompletionSource<object>();
        kiln.Call("consumePurchasedProduct", purchaseToken, new KilnCallback<object>(){
            Tcs = aTcs
        });
        return aTcs.Task;
    }

    public void SubmitAnalyticsEvent(KilnAnalyticEvent evt) 
    {
        kiln.Call("submitAnalyticsEvent", evt);
    }

}