#if UNITY_ANDROID
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Kiln
{
    public class Exception : System.Exception {
        private static readonly string DefaultMessage = "Java Exception.";

        public Exception() : base(DefaultMessage) { }
        public Exception(string message) : base(message) { }
        public Exception(string message, System.Exception innerException): base(message, innerException) { }    
    }

    public enum AdType  {
        INTERSTITIAL, 
        REWARDED
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

    public class RewardedAdResponse: IKilnObjectWrapper {

        private AndroidJavaObject javaInst;

        public AndroidJavaObject JavaInst { set => javaInst = value; }

#if UNITY_EDITOR
        private bool _rewardUser = false;
        public bool RewardUser { set { _rewardUser = value; } }
#endif

        public string getPlacementID() {
            return javaInst.Call<string>("getPlacementID");
        }
        public bool getWithReward()
        {
#if ANDROID_DEVICE
            return javaInst.Call<bool>("getWithReward");
#endif
            
            return _rewardUser;
        }

    }

    public class Player: IKilnObjectWrapper
    {
        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }

#if UNITY_EDITOR
        private string _id;
        public string ID { set { _id = value;  } }
        private string _name;
        public string Name { set { _name = value; } }
        private string _photoURL;
        public string PhotoURL { set { _photoURL = value; } }
#endif

        public string GetId()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getId");
#endif
            return _id;
        }
        public string GetName()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getName");
#endif
            return _name;
        }
        public string GetPhotoURL()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getPhotoURL");
#endif
            return _photoURL;
        }
    }

    public class LeaderboardEntry : IKilnObjectWrapper
    {
        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }
        
#if UNITY_EDITOR
        private double _score;
        public double Score { set { _score = value; } }
        private int _rank;
        public int Rank { set { _rank = value; } }
        private Player _player;
        public Player Player { set { _player = value; } }
#endif

        public double GetScore()
        {
#if ANDROID_DEVICE
            return javaInst.Call<double>("getScore");
#endif
            return _score;
        }
        public int GetRank()
        {
#if ANDROID_DEVICE
            return javaInst.Call<int>("getRank");
#endif
            return _rank;
        }

        public Player GetPlayer()
        {
#if ANDROID_DEVICE
            return javaInst.Call<int>("getPlayer");
#endif
            return _player;
        }

        new public string ToString()
        {
            return $"Player {GetPlayer().GetName()} - score: {GetScore()}, rank: {GetRank()}";
        }

    }

    public class Product : IKilnObjectWrapper
    {
        
        public enum ProductType
        {
            CONSUMABLE, NON_CONSUMABLE
        }

        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }
        
#if UNITY_EDITOR
        private string _id;
        public string ID { set { _id = value; } }
        private string _price;
        public string Price { set { _price = value; } }
        // TODO: This isn't supported on the Java side ?
        private ProductType _type;
        public ProductType Type { set { _type = value; } }
#endif

        public string GetTitle()
        {
            return javaInst.Call<string>("getTitle");
        }

        public string GetProductID()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getProductID");
#endif
            return _id;
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
#if ANDROID_DEVICE
            return javaInst.Call<string>("getPrice");
#endif
            return _price;
        }

        public string GetPriceCurrencyCode()
        {
            return javaInst.Call<string>("getPriceCurrencyCode");
        }

        public ProductType GetProductType()
        {
#if ANDROID_DEVICE
            // TODO: This was added by me (Bruno). Gotta see what's up on the other side.
            return javaInst.Call<string>("getProductType");
#endif
            return _type;
        }

        new public string ToString()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("toString");
#endif
            return $"-----\nID: {GetProductID()}\nPrice: {GetPrice()}\nType: {GetProductType()}\n";
        }    

    }
    public class Purchase: IKilnObjectWrapper {

        private AndroidJavaObject javaInst;
        public AndroidJavaObject JavaInst { set => javaInst = value; }

#if UNITY_EDITOR
        private string _productID;
        public string ProductID { set { _productID = value; } }
        private string _purchaseToken;
        public string PurchaseToken { set { _purchaseToken = value; } }
        private string _developerPayload;
        public string DeveloperPayload { set { _developerPayload= value; } }
#endif


        public string GetDeveloperPayload()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getDeveloperPayload");
#endif
            return _developerPayload;
        }    
        public string GetPaymentID()
        {
            return javaInst.Call<string>("getPaymentID");
        }    

        public string GetProductID()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getProductID");
#endif
            return _productID;
        }    

        public string GetPurchaseTime()
        {
            return javaInst.Call<string>("getPurchaseTime");
        }    

        public string GetPurchaseToken()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("getPurchaseToken");
#endif
            return _purchaseToken;
        }    

        public string GetSignedRequest()
        {
            return javaInst.Call<string>("getSignedRequest");
        }

        new public string ToString()
        {
#if ANDROID_DEVICE
            return javaInst.Call<string>("toString");
#endif
            return $"-----\nProduct ID: {_productID}\nPurchase Token: {_purchaseToken}\nDeveloper Payload: {_developerPayload}\n";
        }    

    }

    public class AnalyticEvent : AndroidJavaProxy 
    {
        public string Category { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public AnalyticEvent() : base("io.gamebake.kiln.types.AnalyticEvent") {}
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

        class Callback<T>: AndroidJavaProxy {

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

            public Callback(): base("io.gamebake.kiln.Callback") {}

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
                taskCompletionSource.SetException(new Exception(exception.Call<string>("toString")));
            }

        }

        class ListCallback<T>: Callback<List<T>> {

            new public void onSuccess(object result) 
            {
                Debug.Log("Callback onSuccess");

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

        class PurchaseSettings: AndroidJavaProxy
        {
            public PurchaseSettings(): base("io.gamebake.kiln.types.PurchaseSettings") {}

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

        public Task Init(Configuration configuration) 

        {
            string AD_UNIT_ID = "ad_unit_id";

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // getBaseContext or getApplicationContext
            // AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            AndroidJavaObject configBuilder = new AndroidJavaObject("io.gamebake.kiln.KilnConfiguration$Builder", context, AD_UNIT_ID);

            if (configuration != null && configuration.DummyAds.Count > 0) 
            {
                AndroidJavaClass javaEnum = new AndroidJavaClass("io.gamebake.kiln.types.AdType");
                AndroidJavaObject javaEnumInterstitial = javaEnum.GetStatic<AndroidJavaObject>("Interstitial");
                AndroidJavaObject javaEnumRewarded = javaEnum.GetStatic<AndroidJavaObject>("Rewarded");
                AndroidJavaObject javaEnumSel;
                AndroidJavaObject arrayList = new AndroidJavaObject("java.util.ArrayList");

                foreach (var item in configuration.DummyAds)
                {
                    if (item.AdType == (AdType)javaEnumInterstitial.Call<int>("ordinal")) {
                        javaEnumSel = javaEnumInterstitial;
                    } else {
                        javaEnumSel = javaEnumRewarded;
                    }
                    AndroidJavaObject javaDummyAdd = new AndroidJavaObject("io.gamebake.kiln.types.DummyAd", item.PlacementID, javaEnumSel, item.RewardUser);
                    arrayList.Call<bool>("add", javaDummyAdd);
                }
                configBuilder.Call<AndroidJavaObject>("withDummyAds", arrayList);            
            }

            AndroidJavaObject config = configBuilder.Call<AndroidJavaObject>("build");

            kiln = new AndroidJavaObject("io.gamebake.kiln.Kiln", config);

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
        /// problem loading the Task will fail with an exception <c>KilnException</c> 
        /// (you can check supportsInterstitialAds previously).
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
        /// Task will get an exception <c>KilnException</c> 
        /// (check supportsInterstitialAds). Remember to call loadInterstitialAd previously.
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
        /// problem loading the Task will fail with an exception <c>KilnException</c> 
        /// (you can check supportsRewardedAds previously).
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
        /// Task will get an exception <c>KilnException</c> 
        /// (check supportsRewardedAds). Remember to call loadRewardedAd previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public Task<RewardedAdResponse> ShowRewardedAd(string identifier) 
        {
            var aTcs = new TaskCompletionSource<RewardedAdResponse>();
            Callback<RewardedAdResponse> callback = new Callback<RewardedAdResponse>()
            {
                Tcs = aTcs
            };
            callback.Wrapper = new RewardedAdResponse();
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
            kiln.Call("setUserScore", score, data, new Callback<object>() {
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
        public Task<LeaderboardEntry> GetUserScore(string id)
        {
            var aTcs = new TaskCompletionSource<LeaderboardEntry>();
            kiln.Call("getUserScore", id, new Callback<LeaderboardEntry>() {
                Tcs = aTcs,
                Wrapper = new LeaderboardEntry()
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
        public Task<List<LeaderboardEntry>> GetScores(int count, int offset, string id)
        {
            var aTcs = new TaskCompletionSource<List<LeaderboardEntry>>();
            kiln.Call("getScores",  count, offset, id, new ListCallback<LeaderboardEntry>() {
                Tcs = aTcs,
                Wrapper = new LeaderboardEntry()
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
        /// Shows native leaderboard ui if supported. Otherwise return Kiln.Exception
        /// </summary>
        /// <returns>Task</returns>
        public Task ShowPlatformLeaderboardUI() 
        {
            var aTcs = new TaskCompletionSource<object>();

            // TODO: Do the actual implementatiuon
            // kiln.Call("showPlatformLeaderboardUI", new Callback<object>() {
            //     Tcs = aTcs
            // });

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
        public Task<List<Product>> GetAvailableProducts() {
            var aTcs = new TaskCompletionSource<List<Product>>();
            kiln.Call("getAvailableProducts", new ListCallback<Product>() {
                Tcs = aTcs,
                Wrapper = new Product()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// Retrieves the list of available products to be purchased. If the platform doesn't support
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="ids">List of identifiers to retrieve desired products</param>
        /// <returns></returns>
        public Task<List<Product>> GetAvailableProducts(List<string> ids) {
            var aTcs = new TaskCompletionSource<List<Product>>();

            AndroidJavaObject arrayList = new AndroidJavaObject("java.util.ArrayList");

            foreach (var item in ids)
            {
                arrayList.Call<bool>("add", new AndroidJavaObject("java.lang.String", item));
            }

            kiln.Call("getAvailableProducts", arrayList, new ListCallback<Product>() {
                Tcs = aTcs,
                Wrapper = new Product()
            });
            return aTcs.Task;
        }

        /// <summary>
        /// It gets the list of products already purchased but still unconsumed. If the platform doesn't support 
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <returns>Task</returns>
        public Task<List<Purchase>> GetPurchasedProducts()
        {
            var aTcs = new TaskCompletionSource<List<Purchase>>();
            kiln.Call("getPurchasedProducts", new ListCallback<Purchase>() {
                Tcs = aTcs,
                Wrapper = new Purchase()
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
        public Task<Purchase> PurchaseProduct(string productID, string payload) 
        {
            var aTcs = new TaskCompletionSource<Purchase>();
            PurchaseSettings settings = new PurchaseSettings() { 
                ProductID = productID,
                DeveloperPayload = payload
            };
            kiln.Call("purchaseProduct", settings, new Callback<Purchase>() {
                Tcs = aTcs,
                Wrapper = new Purchase()
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
            kiln.Call("consumePurchasedProduct", purchaseToken, new Callback<object>(){
                Tcs = aTcs
            });
            return aTcs.Task;
        }

        /// <summary>
        /// </summary>
        /// <param name="evt"></param>
        public void SubmitAnalyticsEvent(AnalyticEvent evt) 
        {
            kiln.Call("submitAnalyticsEvent", evt);
        }

    }
}
#endif