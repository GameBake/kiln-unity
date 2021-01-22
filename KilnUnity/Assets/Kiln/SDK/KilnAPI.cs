#if !UNITY_EDITOR && UNITY
    #define ANDROID_DEVICE
#endif

using System.Threading.Tasks;
using UnityEngine;

namespace Kiln
{
    public class API
    {
#if ANDROID_DEVICE
        private static KilnBridge _bridge;
        public static KilnBridge Bridge
        {
            get
            {
                if (_bridge == null)
                {
                    _bridge = new KilnBridge();
                }

                return _bridge;
            }
        }
#endif

#if UNITY_EDITOR
        private static Settings _settings;
        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<Settings>("KilnSettings");

                    if (_settings == null)
                    {
                        _settings = new Settings();
                    }
                }

                return _settings;
            }
        }

        private static InterstitialAdController _interstitialAdPrefab;
        public static InterstitialAdController InterstitialAdPrefab
        {
            get
            {
                if (_interstitialAdPrefab == null)
                {
                    _interstitialAdPrefab = Resources.Load<InterstitialAdController>("KilnInterstitialAd");

                    if (_interstitialAdPrefab == null)
                    {
                        throw new System.Exception("Kiln Interstitial Ad Prefab Missing");
                    }
                }

                return _interstitialAdPrefab;
            }
        }
        
        private static RewardedAdController _rewardedAdPrefab;
        public static RewardedAdController RewardedAdPrefab
        {
            get
            {
                if (_rewardedAdPrefab == null)
                {
                    _rewardedAdPrefab = Resources.Load<RewardedAdController>("KilnRewardedAd");

                    if (_rewardedAdPrefab == null)
                    {
                        throw new System.Exception("Kiln Rewarded Ad Prefab Missing");
                    }
                }

                return _rewardedAdPrefab;
            }
        }

        private static bool _initialized = false;
        private static bool _rewardedAdLoaded = false;
        private static bool _interstitialAdLoaded = false;

        /// <summary>
        /// Throw an exception if SDK is not initialized
        /// </summary>
        private static void CheckInitialized()
        {
            if (!_initialized)
            {
                throw new KilnException("Kiln is not initialized.");
            }
        }
#endif

        public static Task Init()
        {
#if ANDROID_DEVICE
            return Bridge.Init();
#endif
            var aTcs = new TaskCompletionSource<object>();

            aTcs.SetResult(0);

            _initialized = true;

            return aTcs.Task;
        }

        /// <summary>
        /// method <c>platformAvailable</c> to check platform for a custom setting/configuration/option.
        /// </summary>
        /// <returns>the platform available</returns>
        public static KilnPlatform PlatformAvailable()
        {
#if ANDROID_DEVICE
            return Bridge.PlatformAvailable();
#endif
            CheckInitialized();

            return KilnPlatform.Development;
        }

        /// <summary>
        /// Use this to check if the underlying platform supports interstitial ads
        /// </summary>
        /// <returns>boolean true if it's supported, false otherwise</returns>
        public static bool SupportsInterstitialAds()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsInterstitialAds();
#endif
            CheckInitialized();

            return Settings.SupportsInterstitialAds;
        }

        /// <summary>
        /// It loads an interstitial ad. If the current platform doesn't support interstitial ads or there's a 
        /// problem loading the Task will fail with an exception <c>KilnException</c> 
        /// (you can check supportsInterstitialAds previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task LoadInterstitialAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.LoadInterstitialAd(identifier);
#endif
            CheckInitialized();

            var aTcs = new TaskCompletionSource<object>();

            aTcs.SetResult(0);

            _interstitialAdLoaded = true;

            return aTcs.Task;
        }

        /// <summary>
        /// It shows the interstitial ad. If the platform doesn't support interstitial ads or there's any problem the 
        /// Task will get an exception <c>KilnException</c> 
        /// (check supportsInterstitialAds). Remember to call loadInterstitialAd previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task ShowInterstitialAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.ShowInterstitialAd(identifier);
#endif
            CheckInitialized();

            var aTcs = new TaskCompletionSource<object>();

            aTcs.SetResult(0);

            if (!_interstitialAdLoaded)
            {
                // TODO: How does this behave on the backend ? Raising a KilnException if no ad is loaded ?
            }
            else
            {
                MonoBehaviour.Instantiate(InterstitialAdPrefab);
                _interstitialAdLoaded = false;
            }

            // TODO: This should wait until the ad instance is done with processing before returning.
            return aTcs.Task;
        }

        /// <summary>
        /// Use this to check if the underlying platform supports rewarded ads
        /// </summary>
        /// <returns>boolean true if it's supported, false otherwise</returns>
        public static bool SupportsRewardedAds() {
#if ANDROID_DEVICE
            return Bridge.SupportsRewardedAds();
#endif
            CheckInitialized();

            return Settings.SupportsRewardedAds;
        }

        /// <summary>
        /// It loads a rewarded ad. If the current platform doesn't support rewarded ads or there's a 
        /// problem loading the Task will fail with an exception <c>KilnException</c> 
        /// (you can check supportsRewardedAds previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task LoadRewardedAd(string identifier)  
        {
#if ANDROID_DEVICE
            return Bridge.LoadRewardedAd(identifier);
#endif
            CheckInitialized();

            var aTcs = new TaskCompletionSource<object>();

            aTcs.SetResult(0);

            _rewardedAdLoaded = true;

            return aTcs.Task;
        }

        /// <summary>
        /// It shows the rewarded ad. If the platform doesn't support rewarded ads or there's any problem the 
        /// Task will get an exception <c>KilnException</c> 
        /// (check supportsRewardedAds). Remember to call loadRewardedAd previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task<KilnRewardedAdResponse> ShowRewardedAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.ShowRewardedAd(identifier);
#endif
            CheckInitialized();

            var aTcs = new TaskCompletionSource<KilnRewardedAdResponse>();

            // aTcs.SetResult(0);

            if (!_rewardedAdLoaded)
            {
                // TODO: How does this behave on the backend ? Raising a KilnException if no ad is loaded ?
            }
            else
            {
                MonoBehaviour.Instantiate(RewardedAdPrefab);
                _rewardedAdLoaded = false;
            }

            // TODO: This should wait until the ad instance is done with processing before returning.
            return aTcs.Task;

            // var aTcs = new TaskCompletionSource<KilnRewardedAdResponse>();
            // KilnCallback<KilnRewardedAdResponse> callback = new KilnCallback<KilnRewardedAdResponse>()
            // {
            //     Tcs = aTcs
            // };
            // callback.Wrapper = new KilnRewardedAdResponse();
            // kiln.Call("showRewardedAd", identifier, callback);
            // return aTcs.Task;
        }

        /// <summary>
        /// Check If the current platform supports leaderboards
        /// </summary>
        /// <returns><c>true</c> if platform supports leaderboards, false otherwise</returns>
        public static bool SupportsLeaderboards() 
        {
#if ANDROID_DEVICE
            return Bridge.SupportsLeaderboards();
#endif
            CheckInitialized();

            return Settings.SupportsLeaderboards;
        }

        /// <summary>
        /// It sets the user score. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="score">score to set</param>
        /// <param name="data">data optional. If the platform supports it, additional data to set.</param>
        /// <returns>Task</returns>
        // public Task SetUserScore(double score, object data) 
        // {
        //     var aTcs = new TaskCompletionSource<object>();
        //     kiln.Call("setUserScore", score, data, new KilnCallback<object>() {
        //         Tcs = aTcs
        //     });
        //     return aTcs.Task;        
        // }

        /// <summary>
        /// It retrieves the user score. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="id">the user identifier</param>
        /// <returns>Task</returns>
        // public Task<KilnScore> GetUserScore(string id)
        // {
        //     var aTcs = new TaskCompletionSource<KilnScore>();
        //     kiln.Call("getUserScore", id, new KilnCallback<KilnScore>() {
        //         Tcs = aTcs,
        //         Wrapper = new KilnScore()
        //     });
        //     return aTcs.Task;        
        // }

        /// <summary>
        /// It gets leaderboard scores for all players. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="count">number of entries to retrieve. default to 10 if unspecified</param>
        /// <param name="offset">The offset from the top of the leaderboard that entries will be fetched from. default 0 if not specified</param>
        /// <param name="id">the leaderboard identifier</param>
        /// <returns></returns>
        // public Task<List<KilnScore>> GetScores(int count, int offset, string id)
        // {
        //     var aTcs = new TaskCompletionSource<List<KilnScore>>();
        //     kiln.Call("getScores",  count, offset, id, new KilnListCallback<KilnScore>() {
        //         Tcs = aTcs,
        //         Wrapper = new KilnScore()
        //     });
        //     return aTcs.Task;
        // }

        /// <summary>
        /// It checks if the current platform supports in app purchases
        /// </summary>
        /// <returns>true if supported, false otherwise</returns>
        public static bool SupportsIAP() {
#if ANDROID_DEVICE
            return Bridge.SupportsIAP();
#endif
            CheckInitialized();

            return Settings.SupportsIAP;
        }

        /// <summary>
        /// Retrieves the list of available products to be purchased. If the platform doesn't support
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <returns>Task</returns>
        // public Task<List<KilnProduct>> GetAvailableProducts() {
        //     var aTcs = new TaskCompletionSource<List<KilnProduct>>();
        //     kiln.Call("getAvailableProducts", new KilnListCallback<KilnProduct>() {
        //         Tcs = aTcs,
        //         Wrapper = new KilnProduct()
        //     });
        //     return aTcs.Task;
        // }

        /// <summary>
        /// It gets the list of products already purchased but still unconsumed. If the platform doesn't support 
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <returns>Task</returns>
        // public Task<List<KilnPurchase>> GetPurchasedProducts()
        // {
        //     var aTcs = new TaskCompletionSource<List<KilnPurchase>>();
        //     kiln.Call("getPurchasedProducts", new KilnListCallback<KilnPurchase>() {
        //         Tcs = aTcs,
        //         Wrapper = new KilnPurchase()
        //     });
        //     return aTcs.Task;
        // }

        /// <summary>
        /// Purchase of a product. If the platform doesn't support purchases 
        /// the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="productID">id to refer the product to be purchased</param>
        /// <param name="payload">additional data to send with the purchase</param>
        /// <returns></returns>
        // public Task<KilnPurchase> PurchaseProduct(string productID, string payload) 
        // {
        //     var aTcs = new TaskCompletionSource<KilnPurchase>();
        //     KilnPurchaseSettings settings = new KilnPurchaseSettings() { 
        //         ProductID = productID,
        //         DeveloperPayload = payload
        //     };
        //     kiln.Call("purchaseProduct", settings, new KilnCallback<KilnPurchase>() {
        //         Tcs = aTcs,
        //         Wrapper = new KilnPurchase()
        //     });
        //     return aTcs.Task;
        // }

        /// <summary>
        /// It consumes a product already purchased. If the platform doesn't support purchases 
        /// the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="purchaseToken">the product token</param>
        /// <returns>Task</returns>
        // public Task ConsumePurchasedProduct(string purchaseToken)
        // {
        //     var aTcs = new TaskCompletionSource<object>();
        //     kiln.Call("consumePurchasedProduct", purchaseToken, new KilnCallback<object>(){
        //         Tcs = aTcs
        //     });
        //     return aTcs.Task;
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        // public void SubmitAnalyticsEvent(KilnAnalyticEvent evt) 
        // {
        //     kiln.Call("submitAnalyticsEvent", evt);
        // }

    }

}