#if !UNITY_EDITOR && UNITY_ANDROID
#define ANDROID_DEVICE
#endif

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kiln
{
    public class API
    {
#if ANDROID_DEVICE
        private static Bridge _bridge;
        public static Bridge Bridge
        {
            get
            {
                if (_bridge == null)
                {
                    _bridge = new Bridge();
                }

                return _bridge;
            }
        }
#endif

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

#if UNITY_EDITOR
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

        private static PlatformLeaderboardController _platformLeaderboardPrefab;
        public static PlatformLeaderboardController PlatformLeaderboardPrefab
        {
            get
            {
                if (_platformLeaderboardPrefab == null)
                {
                    _platformLeaderboardPrefab = Resources.Load<PlatformLeaderboardController>("KilnPlatformLeaderboard");

                    if (_platformLeaderboardPrefab == null)
                    {
                        throw new System.Exception("Kiln Platform Leaderboard Prefab Missing");
                    }
                }

                return _platformLeaderboardPrefab;
            }
        }

        private static bool _initialized = false;
        private static Dictionary<string, RewardedAdController> _rewardedAds = new Dictionary<string, RewardedAdController>();
        private static Dictionary<string, InterstitialAdController> _interstitialAds = new Dictionary<string, InterstitialAdController>();
        private static Dictionary<string, Leaderboard> _leaderboards = new Dictionary<string, Leaderboard>();
        private static InAppPurchases _iap;
        public static InAppPurchases IAP { get { return _iap; } }

        /// <summary>
        /// Throw an exception if SDK is not initialized
        /// </summary>
        private static void CheckInitialized()
        {
            if (!_initialized)
            {
                throw new Kiln.Exception("Kiln is not initialized.");
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task Init()
        {
#if ANDROID_DEVICE
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
            return Bridge.Init(config);
#else
            var aTcs = new TaskCompletionSource<object>();

            _initialized = true;

            if (SupportsInterstitialAds())
            {
                foreach (string id in Settings.GetInterstitialsIds())
                {
                    _interstitialAds.Add(id, null);
                }
            }

            if (SupportsRewardedAds())
            {
                foreach (string id in Settings.GetRewardedIds())
                {
                    _rewardedAds.Add(id, null);
                }
            }

            if (SupportsLeaderboards())
            {
                foreach (Settings.Leaderboard l in _settings.Leaderboards)
                {
                    Leaderboard leaderboard = Leaderboard.IsSaved(l.Id) ? Leaderboard.Load(l.Id) : new Leaderboard(l.Id, type: l.Type);

                    _leaderboards.Add(l.Id, leaderboard);
                }
            }

            if (SupportsIAP())
            {
                _iap = new InAppPurchases();
            }

            aTcs.SetResult(0);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Use this to check if the underlying platform supports interstitial ads
        /// </summary>
        /// <returns>boolean true if it's supported, false otherwise</returns>
        public static bool SupportsInterstitialAds()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsInterstitialAds();
#else
            CheckInitialized();

            return Settings.SupportsInterstitialAds;
#endif
        }

        /// <summary>
        /// It loads an interstitial ad. If the current platform doesn't support interstitial ads or there's a 
        /// problem loading the Task will fail with an exception <c>Kiln.Exception</c> 
        /// (you can check supportsInterstitialAds previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task LoadInterstitialAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.LoadInterstitialAd(identifier);
#else
            CheckInitialized();

            if (!SupportsInterstitialAds())
            {
                throw new Kiln.Exception($"Interstitial Ads not supported.");
            }

            if (!Settings.IsValidInterstitialId(identifier))
            {
                throw new Kiln.Exception($"Invalid Interstitial Placement ID: {identifier}");
            }

            if (_interstitialAds[identifier] != null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Interstitial Placement ID: {identifier} already loaded");
            }

            var aTcs = new TaskCompletionSource<object>();

            _interstitialAds[identifier] = MonoBehaviour.Instantiate(InterstitialAdPrefab);

            // aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It shows the interstitial ad. If the platform doesn't support interstitial ads or there's any problem the 
        /// Task will get an exception <c>Kiln.Exception</c> 
        /// (check supportsInterstitialAds). Remember to call loadInterstitialAd previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task ShowInterstitialAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.ShowInterstitialAd(identifier);
#else
            CheckInitialized();

            if (!SupportsInterstitialAds())
            {
                throw new Kiln.Exception($"Interstitial Ads not supported.");
            }

            if (!Settings.IsValidInterstitialId(identifier))
            {
                throw new Kiln.Exception($"Invalid Interstitial Placement ID: {identifier}");
            }

            var aTcs = new TaskCompletionSource<object>();

            if (_interstitialAds[identifier] == null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Interstitial Placement ID: {identifier} not loaded");
            }
            else
            {
                _interstitialAds[identifier].Show(aTcs, identifier);
                _interstitialAds[identifier] = null;
            }

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Use this to check if the underlying platform supports rewarded ads
        /// </summary>
        /// <returns>boolean true if it's supported, false otherwise</returns>
        public static bool SupportsRewardedAds()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsRewardedAds();
#else
            CheckInitialized();

            return Settings.SupportsRewardedAds;
#endif
        }

        /// <summary>
        /// It loads a rewarded ad. If the current platform doesn't support rewarded ads or there's a 
        /// problem loading the Task will fail with an exception <c>Kiln.Exception</c> 
        /// (you can check supportsRewardedAds previously).
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task LoadRewardedAd(string identifier)  
        {
#if ANDROID_DEVICE
            return Bridge.LoadRewardedAd(identifier);
#else
            CheckInitialized();

            if (!SupportsRewardedAds())
            {
                throw new Kiln.Exception($"Rewarded Ads not supported.");
            }
            
            if (!Settings.IsValidRewardedId(identifier))
            {
                throw new Kiln.Exception($"Invalid Rewarded Placement ID: {identifier}");
            }

            if (_rewardedAds[identifier] != null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Rewarded Placement ID: {identifier} already loaded");
            }

            var aTcs = new TaskCompletionSource<object>();

            _rewardedAds[identifier] = MonoBehaviour.Instantiate(RewardedAdPrefab);

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It shows the rewarded ad. If the platform doesn't support rewarded ads or there's any problem the 
        /// Task will get an exception <c>Kiln.Exception</c> 
        /// (check supportsRewardedAds). Remember to call loadRewardedAd previously.
        /// </summary>
        /// <param name="identifier">the ad identifier</param>
        /// <returns>Task</returns>
        public static Task<IRewardedAdResponse> ShowRewardedAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.ShowRewardedAd(identifier); 
#else
            CheckInitialized();

            if (!SupportsRewardedAds())
            {
                throw new Kiln.Exception($"Rewarded Ads not supported.");
            }

            if (!Settings.IsValidRewardedId(identifier))
            {
                throw new Kiln.Exception($"Invalid Rewarded Placement ID: {identifier}");
            }

            var aTcs = new TaskCompletionSource<IRewardedAdResponse>();

            if (_rewardedAds[identifier] == null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Rewarded Placement ID: {identifier} not loaded");
            }
            else
            {
                _rewardedAds[identifier].Show(aTcs, identifier);
                _rewardedAds[identifier] = null;
            }

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Check If the current platform supports leaderboards
        /// </summary>
        /// <returns><c>true</c> if platform supports leaderboards, false otherwise</returns>
        public static bool SupportsLeaderboards() 
        {
#if ANDROID_DEVICE
            return Bridge.SupportsLeaderboards();
#else
            CheckInitialized();

            return Settings.SupportsLeaderboards;
#endif
        }

        /// <summary>
        /// It sets the user score. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>Kiln.Exception</c>
        /// </summary>
        /// <param name="score">score to set</param>
        /// <param name="data">data optional. If the platform supports it, additional data to set.</param>
        /// <returns>Task</returns>
        public static Task SetUserScore(string id, double score, object data = null)
        {
#if ANDROID_DEVICE
            // TODO: Need to pass the id as well ?
            return Bridge.SetUserScore(score, data);
#else          
            CheckInitialized();

            if (!SupportsLeaderboards())
            {
                throw new Kiln.Exception($"Leaderboards not supported.");
            }

            if (!_leaderboards.ContainsKey(id))
            {
                throw new Kiln.Exception($"There's no Leaderboards with id {id}.");
            }

            var aTcs = new TaskCompletionSource<object>();

            _leaderboards[id].SetUserScore(score, data);

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It retrieves the user score. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>Kiln.Exception</c>
        /// </summary>
        /// <param name="id">the user identifier</param>
        /// <returns>Task</returns>
        public static Task<ILeaderboardEntry> GetUserScore(string id)
        {
#if ANDROID_DEVICE
            return Bridge.GetUserScore(id);
#else
            CheckInitialized();

            if (!SupportsLeaderboards())
            {
                throw new Kiln.Exception($"Leaderboards not supported.");
            }

            if (!_leaderboards.ContainsKey(id))
            {
                throw new Kiln.Exception($"There's no Leaderboards with id {id}.");
            }

            var aTcs = new TaskCompletionSource<ILeaderboardEntry>();

            aTcs.SetResult(_leaderboards[id].GetUserScore());

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It gets leaderboard scores for all players. If the platform doesn't support leaderboards the Task  
        /// will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="count">number of entries to retrieve. default to 10 if unspecified</param>
        /// <param name="offset">The offset from the top of the leaderboard that entries will be fetched from. default 0 if not specified</param>
        /// <param name="id">the leaderboard identifier</param>
        /// <returns></returns>
        public static Task<List<ILeaderboardEntry>> GetScores(int count, int offset, string id)
        {
#if ANDROID_DEVICE
            return Bridge.GetScores(count, offset, id);
#else
            CheckInitialized();

            if (!SupportsLeaderboards())
            {
                throw new Kiln.Exception($"Leaderboards not supported.");
            }

            if (!_leaderboards.ContainsKey(id))
            {
                throw new Kiln.Exception($"There's no Leaderboards with id {id}.");
            }

            var aTcs = new TaskCompletionSource<List<ILeaderboardEntry>>();
            
            aTcs.SetResult(_leaderboards[id].GetScores(count, offset));

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Check If the current platform supports a native leaderboards ui
        /// </summary>
        /// <returns><c>true</c> if platform supports native leaderboards ui, false otherwise</returns>
        public static bool SupportsPlatformLeaderboardUI()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsPlatformLeaderboardUI();
#else
            CheckInitialized();

            return Settings.SupportsIAP;
#endif
        }

        /// <summary>
        /// Shows native leaderboard ui if supported. Otherwise return Kiln.Exception
        /// </summary>
        /// <returns>Task</returns>
        public static Task ShowPlatformLeaderboardUI()
        {
#if ANDROID_DEVICE
            return Bridge.ShowPlatformLeaderboardUI();
#else
            CheckInitialized();

            if (!SupportsPlatformLeaderboardUI())
            {
                throw new Kiln.Exception("Platform Leaderboard UI not supported.");
            }

            var aTcs = new TaskCompletionSource<object>();

            PlatformLeaderboardController leaderboards = GameObject.Instantiate(PlatformLeaderboardPrefab);
            leaderboards.Show(aTcs);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It checks if the current platform supports in app purchases
        /// </summary>
        /// <returns>true if supported, false otherwise</returns>
        public static bool SupportsIAP()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsIAP();
#else
            CheckInitialized();

            return Settings.SupportsIAP;
#endif
        }

        /// <summary>
        /// Retrieves the list of available products to be purchased. If the platform doesn't support
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <returns>Task</returns>
        public static Task<List<IProduct>> GetAvailableProducts()
        {
#if ANDROID_DEVICE
            return Bridge.GetAvailableProducts();
#else
            CheckInitialized();

            if (!SupportsIAP())
            {
                throw new Kiln.Exception("In App Purchases not supported.");
            }

            var aTcs = new TaskCompletionSource<List<IProduct>>();

            aTcs.SetResult(_iap.Products);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Retrieves the list of available products to be purchased. If the platform doesn't support
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="ids">List of identifiers to retrieve desired products</param>
        /// <returns></returns>
        public Task<List<IProduct>> GetAvailableProducts(List<string> ids) 
        {
#if ANDROID_DEVICE
            return Bridge.GetAvailableProducts(ids);
#else
            CheckInitialized();

            if (!SupportsIAP())
            {
                throw new Kiln.Exception("In App Purchases not supported.");
            }

            var aTcs = new TaskCompletionSource<List<IProduct>>();

            aTcs.SetResult(_iap.Products);

            return aTcs.Task;
#endif            
        }

        /// <summary>
        /// It gets the list of products already purchased but still unconsumed. If the platform doesn't support 
        /// purchases the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <returns>Task</returns>
        public static Task<List<IPurchase>> GetPurchasedProducts()
        {
#if ANDROID_DEVICE
            return Bridge.GetPurchasedProducts();
#else
            CheckInitialized();

            if (!SupportsIAP())
            {
                throw new Kiln.Exception("In App Purchases not supported.");
            }

            var aTcs = new TaskCompletionSource<List<IPurchase>>();

            aTcs.SetResult(_iap.Purchases);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Purchase of a product. If the platform doesn't support purchases 
        /// the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="productID">id to refer the product to be purchased</param>
        /// <param name="payload">additional data to send with the purchase</param>
        /// <returns></returns>
        public static Task<IPurchase> PurchaseProduct(string productID, string payload)
        {
#if ANDROID_DEVICE
            return Bridge.PurchaseProduct(productID, payload);
#else
            CheckInitialized();

            if (!SupportsIAP())
            {
                throw new Kiln.Exception("In App Purchases not supported.");
            }

            var task = _iap.PurchaseProduct(productID, payload);

            return task;
#endif
        }

        /// <summary>
        /// It consumes a product already purchased. If the platform doesn't support purchases 
        /// the Task will get an exception <c>KilnException</c>
        /// </summary>
        /// <param name="purchaseToken">the product token</param>
        /// <returns>Task</returns>
        public static Task ConsumePurchasedProduct(string purchaseToken)
        {
#if ANDROID_DEVICE
            return Bridge.ConsumePurchasedProduct(purchaseToken);
#else
            CheckInitialized();

            if (!SupportsIAP())
            {
                throw new Kiln.Exception("In App Purchases not supported.");
            }

            var aTcs = new TaskCompletionSource<object>();

            _iap.ConsumePurchasedProduct(purchaseToken);

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public static void SubmitAnalyticsEvent(IAnalyticEvent evt)
        {
#if ANDROID_DEVICE
            Bridge.SubmitAnalyticsEvent(evt);
#else
            CheckInitialized();
#endif
        }

    }

}