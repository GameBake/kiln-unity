#if UNITY_ANDROID
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Kiln
{
    public class AndroidException : Exception {
        private static readonly string DefaultMessage = "Java Exception.";

        public AndroidException() : base(DefaultMessage) { }
        public AndroidException(string message) : base(message) { }
        public AndroidException(string message, System.Exception innerException) : base(message, innerException) { }    
    }

    public enum AdType  {
        INTERSTITIAL, 
        REWARDED,
        BANNER
    }

    public class DummyAd 
    {
        public string PlacementID { get; set; }

        public AdType AdType { get; set; }

        public bool RewardUser { get; set; }

    }

    public class Configuration 
    {
        public List<DummyAd> DummyAds { get; set; }
    }

    public interface IKilnObjectWrapper {

        AndroidJavaObject JavaInst { set;  }
    }

    public class AndroidRewardedAdResponse : IKilnObjectWrapper, IRewardedAdResponse {

        private AndroidJavaObject javaInst;

        public AndroidJavaObject JavaInst { set => javaInst = value; }

        public string GetPlacementID() {
            return javaInst.Call<string>("getPlacementID");
        }
        public bool GetWithReward()
        {
            return javaInst.Call<bool>("getWithReward");
        }
    }

    public class AndroidPlayer : IKilnObjectWrapper, IPlayer
    {
        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }

        public string GetId()
        {
            return javaInst.Call<string>("getId");
        }
        public string GetName()
        {
            return javaInst.Call<string>("getName");
        }
        public string GetPhotoURL()
        {
            return javaInst.Call<string>("getPhotoURL");
        }
    }

    public class AndroidLeaderboardEntry : IKilnObjectWrapper, ILeaderboardEntry
    {
        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }
        
        public double GetScore()
        {
            return javaInst.Call<double>("getScore");
        }
        public int GetRank()
        {
            return javaInst.Call<int>("getRank");
        }

        public IPlayer GetPlayer()
        {
            AndroidPlayer player = new AndroidPlayer();
            player.JavaInst = javaInst.Call<AndroidJavaObject>("getPlayer");

            return player;
        }

        new public string ToString()
        {
            return $"Player {GetPlayer().GetName()} - score: {GetScore()}, rank: {GetRank()}";
        }

    }

    public class AndroidProduct : IKilnObjectWrapper, IProduct
    {
        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }
        
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

        public ProductType GetProductType()
        {
            return javaInst.Call<ProductType>("getProductType");
        }

        new public string ToString()
        {
            return javaInst.Call<string>("toString");
        }
    }
    
    public class AndroidPurchase : IKilnObjectWrapper, IPurchase {

        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }

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

        new public string ToString()
        {
            return javaInst.Call<string>("toString");
        }    

    }

    public class AndroidAnalyticEvent : AndroidJavaProxy, IAnalyticEvent 
    {
        public string Category { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public AndroidAnalyticEvent() : base("io.gamebake.kiln.types.AnalyticEvent") {}
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

    public class Bridge {
        private AndroidJavaObject kiln;

        class Callback<T> : AndroidJavaProxy {

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

            public Callback() : base("io.gamebake.kiln.Callback") {}

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
                taskCompletionSource.SetException(new AndroidException(exception.Call<string>("toString")));
            }

        }

        class ListCallback<T> : Callback<List<T>> {

            new public void onSuccess(object result) 
            {
                Debug.Log("Callback onSuccess");

                List<T> outList = new List<T>();
                int size = ((AndroidJavaObject)result).Call<int>("size");

                System.Type type = wrapper.GetType();
                for (int i = 0; i < size; i++)
                {
                    T instance = (T)System.Activator.CreateInstance(type);
                    ((IKilnObjectWrapper)instance).JavaInst = ((AndroidJavaObject)result).Call<AndroidJavaObject>("get", i);
                    outList.Add(instance);
                }

                Tcs.SetResult(outList);
            }


        }

        class PurchaseSettings : AndroidJavaProxy
        {
            public PurchaseSettings() : base("io.gamebake.kiln.types.PurchaseSettings") {}

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
             // string AD_UNIT_ID = "ad_unit_id";

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            // AndroidJavaObject configBuilder = new AndroidJavaObject("io.gamebake.kiln.KilnConfiguration$Builder", context, AD_UNIT_ID);

            kiln = new AndroidJavaObject("io.gamebake.kiln.Kiln", context);

            var aTcs = new TaskCompletionSource<object>();

            kiln.Call("init", new Callback<object>(){
                Tcs = aTcs
            });

            return aTcs.Task;
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
        /// problem loading the Task will fail with an exception <see cref="Kiln.Exception"/>
        /// (you can check <see cref="SupportsInterstitialAds"/> previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task LoadInterstitialAd(string identifier)  
        {
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("loadInterstitialAd", identifier, new Callback<object>(){
                Tcs = aTcs
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It shows the interstitial ad. If the platform doesn't support interstitial ads or there's any problem the 
        /// Task will get an exception <see cref="Kiln.Exception"/>
        /// (check <see cref="SupportsInterstitialAds"/>). Remember to call <see cref="LoadInterstitialAd"/> previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task ShowInterstitialAd(string identifier) 
        {
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("showInterstitialAd", identifier, new Callback<object>(){
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
        /// problem loading the Task will fail with an exception <see cref="Kiln.Exception"/>
        /// (you can check <see cref="SupportsRewardedAds"/> previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task LoadRewardedAd(string identifier)  
        {
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("loadRewardedAd", identifier, new Callback<object>(){
                Tcs = aTcs
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It shows the rewarded ad. If the platform doesn't support rewarded ads or there's any problem the 
        /// Task will get an exception <see cref="Kiln.Exception"/>
        /// (check <see cref="SupportsRewardedAds"/>). Remember to call <see cref="LoadRewardedAd"/> previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task<IRewardedAdResponse> ShowRewardedAd(string identifier) 
        {
            var aTcs = new TaskCompletionSource<IRewardedAdResponse>();
            Callback<IRewardedAdResponse> callback = new Callback<IRewardedAdResponse>()
            {
                Tcs = aTcs
            };
            callback.Wrapper = new AndroidRewardedAdResponse();
            kiln.Call("showRewardedAd", identifier, callback);
            return aTcs.Task;
        }

        /// <summary>
        /// Use this to check if the underlying platform supports banner ads
        /// </summary>
        /// <returns>boolean true if it's supported, false otherwise</returns>
        public bool SupportsBannerAds() {
            return kiln.Call<bool>("supportsBannerAds");
        }

        /// <summary>
        /// It loads a banner ad. If the current platform doesn't support rewarded ads or there's a 
        /// problem loading the Task will fail with an exception <see cref="Kiln.Exception"/>
        /// (you can check <see cref="SupportsBannerAds"/> previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <param name="position">Where in the screen to position the loaded ad. See <see cref="Kiln.BannerPosition"/></param>
        /// <param name="maxSize">The maximum size of the banner to load. See <see cref="Kiln.BannerSize"/>.</param>
        /// <returns>Task</returns>
        public Task LoadBannerAd(string identifier, BannerPosition position, BannerSize maxSize = BannerSize.Width320Height50)
        {
            var width = maxSize.Width();
            var height = maxSize.Height();
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("loadBannerAd", identifier, width, height, (int) position, new Callback<object>(){
                Tcs = aTcs
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It shows the rewarded ad. If the platform doesn't support rewarded ads or there's any problem the 
        /// Task will get an exception <see cref="Kiln.Exception"/>
        /// (check <see cref="SupportsRewardedAds"/>). Remember to call <see cref="LoadRewardedAd"/> previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task ShowBannerAd(string identifier)
        {
            var aTcs = new TaskCompletionSource<object>();
            Callback<object> callback = new Callback<object>()
            {
                Tcs = aTcs
            };
            kiln.Call("showBannerAd", identifier, callback);
            return aTcs.Task;
        }

         /// <summary>
        /// It hides the banner ad. If the platform doesn't support banner ads or there's any problem the 
        /// Task will get an exception <see cref="Kiln.Exception"/>
        /// (check <see cref="SupportsBannerAds"/>). Remember to call <see cref="LoadBannerAd"/> previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task HideBannerAd(string identifier)
        {
            var aTcs = new TaskCompletionSource<object>();
            Callback<object> callback = new Callback<object>()
            {
                Tcs = aTcs
            };
            kiln.Call("hideBannerAd", identifier, callback);
            return aTcs.Task;
        }

        /// <summary>
        /// It destroys the banner ad. If the platform doesn't support banner ads or there's any problem the 
        /// Task will get an exception <see cref="Kiln.Exception"/>
        /// (check <see cref="SupportsBannerAds"/>). Remember to call <see cref="LoadBannerAd"/> previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task DestroyBannerAd(string identifier)
        {
            var aTcs = new TaskCompletionSource<object>();
            Callback<object> callback = new Callback<object>()
            {
                Tcs = aTcs
            };
            kiln.Call("destroyBannerAd", identifier, callback);
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
        /// will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="score">score to set</param>
        /// <param name="data">data optional. If the platform supports it, additional data to set.</param>
        /// <returns>Task</returns>
        public Task SetUserScore(string leaderboardId, double score, object data) 
        {
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("setUserScore", leaderboardId, score, data, new Callback<object>() {
                Tcs = aTcs
            });
            return aTcs.Task;        
        }

        /// <summary>
        /// It retrieves the user score. If the platform doesn't support leaderboards the Task  
        /// will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="id">the user identifier</param>
        /// <returns>Task</returns>
        public Task<ILeaderboardEntry> GetUserScore(string id)
        {
            var aTcs = new TaskCompletionSource<ILeaderboardEntry>();
            kiln.Call("getUserScore", id, new Callback<ILeaderboardEntry>() {
                Tcs = aTcs,
                Wrapper = new AndroidLeaderboardEntry()
            });
            return aTcs.Task;        
        }

        /// <summary>
        /// It gets leaderboard scores for all players. If the platform doesn't support leaderboards the Task  
        /// will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="id">the leaderboard identifier</param>
        /// <param name="count">number of entries to retrieve. default to 10 if unspecified</param>
        /// <param name="offset">The offset from the top of the leaderboard that entries will be fetched from. default 0 if not specified</param>
        /// <returns></returns>
        public Task<List<ILeaderboardEntry>> GetScores(string id, int count, int offset)
        {
            var aTcs = new TaskCompletionSource<List<ILeaderboardEntry>>();
            
            kiln.Call("getScores", id, count, offset, new ListCallback<ILeaderboardEntry>() {
                Tcs = aTcs,
                Wrapper = new AndroidLeaderboardEntry()
            });

            return aTcs.Task;
        }

        /// <summary>
        /// Check If the current platform supports a native leaderboards ui
        /// </summary>
        /// <returns><c>true</c> if platform supports native leaderboards ui, false otherwise</returns>
        public bool SupportsPlatformLeaderboardUI() 
        {
            return kiln.Call<bool>("supportsPlatformLeaderboardUI");
        }

        /// <summary>
        /// Shows native leaderboard ui if supported. Otherwise return <see cref="Kiln.Exception"/>
        /// </summary>
        /// <returns>Task</returns>
        public Task ShowPlatformLeaderboardUI() 
        {
            var aTcs = new TaskCompletionSource<object>();

            kiln.Call("showLeaderboardUI", new Callback<object>() {
                Tcs = aTcs
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
        /// purchases the Task will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <returns>Task</returns>
        public Task<List<IProduct>> GetAvailableProducts() {
            var aTcs = new TaskCompletionSource<List<IProduct>>();
            kiln.Call("getAvailableProducts", new ListCallback<IProduct>() {
                Tcs = aTcs,
                Wrapper = new AndroidProduct()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// Retrieves the list of available products to be purchased. If the platform doesn't support
        /// purchases the Task will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="ids">List of identifiers to retrieve desired products</param>
        /// <returns></returns>
        public Task<List<IProduct>> GetAvailableProducts(List<string> ids) {
            var aTcs = new TaskCompletionSource<List<IProduct>>();

            AndroidJavaObject arrayList = new AndroidJavaObject("java.util.ArrayList");

            foreach (var item in ids)
            {
                arrayList.Call<bool>("add", new AndroidJavaObject("java.lang.String", item));
            }

            kiln.Call("getAvailableProducts", arrayList, new ListCallback<IProduct>() {
                Tcs = aTcs,
                Wrapper = new AndroidProduct()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It gets the list of purchases still unconsumed. If the platform doesn't support 
        /// purchases the Task will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <returns>Task</returns>
        public Task<List<IPurchase>> GetPurchases()
        {
            var aTcs = new TaskCompletionSource<List<IPurchase>>();
            kiln.Call("getPurchases", new ListCallback<IPurchase>() {
                Tcs = aTcs,
                Wrapper = new AndroidPurchase()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// Purchase of a product. If the platform doesn't support purchases 
        /// the Task will get an exception <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="productID">id to refer the product to be purchased</param>
        /// <param name="payload">additional data to send with the purchase</param>
        /// <returns></returns>
        public Task<IPurchase> PurchaseProduct(string productID, string payload) 
        {
            var aTcs = new TaskCompletionSource<IPurchase>();
            PurchaseSettings settings = new PurchaseSettings() { 
                ProductID = productID,
                DeveloperPayload = payload
            };
            kiln.Call("purchaseProduct", settings, new Callback<IPurchase>() {
                Tcs = aTcs,
                Wrapper = new AndroidPurchase()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It consumes a product already purchased. If the platform doesn't support purchases 
        /// the Task will get an exception <see cref="Kiln.Exception">
        /// </summary>
        /// <param name="purchaseToken">the product token</param>
        /// <returns>Task</returns>
        public Task ConsumePurchasedProduct(string purchaseToken)
        {
            var aTcs = new TaskCompletionSource<object>();
            kiln.Call("consumePurchasedProduct", purchaseToken, new Callback<object>(){
                Tcs = aTcs
            });
            return aTcs.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="evt"></param>
        public void SubmitAnalyticsEvent(string evt)
        {
            kiln.Call("submitAnalyticsEvent", evt);
        }

    }
}
#endif