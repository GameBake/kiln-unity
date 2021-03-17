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

        private static BannerAdController _bannerAdPrefab;
        public static BannerAdController BannerAdPrefab
        {
            get
            {
                if (_bannerAdPrefab == null)
                {
                    _bannerAdPrefab = Resources.Load<BannerAdController>("KilnBannerAd");

                    if (_bannerAdPrefab == null)
                    {
                        throw new System.Exception("Kiln Banner Ad Prefab Missing");
                    }
                }

                return _bannerAdPrefab;
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
        private static Dictionary<string, BannerAdController> _bannerAds = new Dictionary<string, BannerAdController>();
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
        /// Initializes the SDK
        /// </summary>
        /// <returns>Task that'll return upon initializing all modules in the SDK</returns>
        public static Task Init()
        {
#if ANDROID_DEVICE
            return Bridge.Init();
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

            if (SupportsBannerAds())
            {
                foreach (string id in Settings.GetBannerIds())
                {
                    _bannerAds.Add(id, null);
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

        #region Advertisement

        /// <summary>
        /// Checks for Interstitial Ads support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
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
        /// Loads an Interstitial Ad. If the current platform doesn't support Interstitial Ads (check 
        /// <see cref="SupportsInterstitialAds"/>), is provided an invalid identifier or there's a problem 
        /// loading the ad, the Task will get an <see cref="Kiln.Exception"/> 
        /// </summary>
        /// <param name="identifier">The Interstitial Ad identifier</param>
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
                throw new Kiln.Exception($"Interstitial Placement ID: {identifier} already loaded");
            }

            var aTcs = new TaskCompletionSource<object>();

            _interstitialAds[identifier] = MonoBehaviour.Instantiate(InterstitialAdPrefab);

            // aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Shows an Interstitial Ad. If the platform doesn't support Interstitial Ads (check <see cref="SupportsInterstitialAds"/>),
        /// an invalid identifier is provided or the Interstitial Ad is not loaded (check <see cref="LoadInterstitialAd"/>), the
        /// Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="identifier">The Interstitial Ad identifier</param>
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
        /// Checks for Rewarded Ads support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
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
        /// Loads a Rewarded Ad. If the current platform doesn't support Rewarded Ads (check 
        /// <see cref="SupportsRewardedAds"/>), is provided with an invalid identifier or there's a 
        /// problem loading the ad, the Task will get an <see cref="Kiln.Exception"/> 
        /// </summary>
        /// <param name="identifier">The Rewarded Ad identifier</param>
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
                throw new Kiln.Exception($"Rewarded Placement ID: {identifier} already loaded");
            }

            var aTcs = new TaskCompletionSource<object>();

            _rewardedAds[identifier] = MonoBehaviour.Instantiate(RewardedAdPrefab);

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Shows a Rewarded Ad. If the platform doesn't support Rewarded Ads (check <see cref="SupportsRewardedAds"/>),
        /// an invalid identifier is provided or the Rewarded Ad hasn't been loaded (check <see cref="LoadRewardedAd"/>) 
        /// the Task will get an <see cref="Kiln.Exception"/> 
        /// </summary>
        /// <param name="identifier">The Rewarded Ad identifier</param>
        /// <returns>Task that'll return a <see cref="Kiln.RewardedAdResponse"/> upon completion</returns>
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
        /// Checks for Banner Ads support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
        public static bool SupportsBannerAds()
        {
#if ANDROID_DEVICE
            return Bridge.SupportsBannerAds();
#else
            CheckInitialized();

            return Settings.SupportsBannerAds;
#endif
        }

        /// <summary>
        /// Loads a Banner Ad. If the current platform doesn't support Banner Ads (check <see cref="SupportsBannerAds"/>),
        /// the provided identifier is invalid or the Banner Ad has already been loaded
        /// the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="identifier">The Banner Ad identifier</param>
        /// <param name="position">Where in the screen to position the Banner Ad. See <see cref="Kiln.BannerPosition"/></param>
        /// <param name="maxSize">The maximum size of the Banner Ad to load. See <see cref="Kiln.BannerSize"/>.</param>
        /// <returns>Task</returns>
        public static Task LoadBannerAd(string identifier, BannerPosition position, BannerSize maxSize = BannerSize.Width320Height50)
        {
#if ANDROID_DEVICE
            return Bridge.LoadBannerAd(identifier, position, maxSize);
#else
            CheckInitialized();

            if (!SupportsBannerAds())
            {
                throw new Kiln.Exception($"Banner Ads not supported.");
            }
            
            if (!Settings.IsValidBannerId(identifier))
            {
                throw new Kiln.Exception($"Invalid Banner Placement ID: {identifier}");
            }

            if (_bannerAds[identifier] != null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Banner Placement ID: {identifier} already loaded");
            }

            var aTcs = new TaskCompletionSource<object>();

            _bannerAds[identifier] = MonoBehaviour.Instantiate(BannerAdPrefab);
            _bannerAds[identifier].Configure(identifier, position, maxSize);

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Shows a Banner Ad. If the platform doesn't support Banner Ads (check <see cref="SupportsBannerAds"/>),
        /// an invalid identifier is provided or the Banner Ad is not loaded (check <see cref="LoadBannerAd"/>), the
        /// Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="identifier">The Banner Ad identifier</param>
        /// <returns>Task</returns>
        public static Task ShowBannerAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.ShowBannerAd(identifier); 
#else
            CheckInitialized();

            if (!SupportsBannerAds())
            {
                throw new Kiln.Exception($"Banner Ads not supported.");
            }

            if (!Settings.IsValidBannerId(identifier))
            {
                throw new Kiln.Exception($"Invalid Banner Placement ID: {identifier}");
            }

            var aTcs = new TaskCompletionSource<object>();

            if (_bannerAds[identifier] == null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Banner Placement ID: {identifier} not loaded");
            }

            _bannerAds[identifier].ShowBanner();

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// Hides a Banner Ad. If the platform doesn't support Banner Ads (check <see cref="SupportsBannerAds"/>), 
        /// an invalid Banner id is provided, or the Banner Ad has not been previously loaded (check <see cref="LoadBannerAd"/>)
        /// the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="identifier">The Banner Ad identifier</param>
        /// <returns>Task</returns>
        public static Task HideBannerAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.HideBannerAd(identifier); 
#else
            CheckInitialized();

            if (!SupportsBannerAds())
            {
                throw new Kiln.Exception($"Banner Ads not supported.");
            }

            if (!Settings.IsValidBannerId(identifier))
            {
                throw new Kiln.Exception($"Invalid Banner Placement ID: {identifier}");
            }

            var aTcs = new TaskCompletionSource<object>();

            if (_bannerAds[identifier] == null)
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"Banner Placement ID: {identifier} not loaded");
            }

            _bannerAds[identifier].HideBanner();

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        /// <summary>
        /// It destroys the banner ad. If the platform doesn't support banner ads (check <see cref="SupportsBannerAds"/>),
        /// the identifier is invalid or the banner hasn't been previously loaded (check <see cref="LoadBannerAd"/>) 
        /// the Task will get an <see cref="Kiln.Exception"/>.
        /// </summary>
        /// <param name="identifier">The banner ad identifier</param>
        /// <returns>Task</returns>
        public static Task DestroyBannerAd(string identifier)
        {
#if ANDROID_DEVICE
            return Bridge.DestroyBannerAd(identifier); 
#else
            CheckInitialized();

            if (!SupportsBannerAds())
            {
                throw new Kiln.Exception($"Banner Ads not supported.");
            }

            if (!Settings.IsValidBannerId(identifier))
            {
                throw new Kiln.Exception($"Invalid Banner Placement ID: {identifier}");
            }

            var aTcs = new TaskCompletionSource<object>();

            if (_bannerAds[identifier] == null)
            {
                throw new Kiln.Exception($"Banner Placement ID: {identifier} not loaded");
            }
            
            _bannerAds[identifier].DestroyBanner();

            aTcs.SetResult(null);

            return aTcs.Task;
#endif
        }

        #endregion

        #region Leaderboards

        /// <summary>
        /// Checks for Leaderboards support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
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
        /// Sets the current <see cref="Kiln.Player"/>'s score. If the platform doesn't support Leaderboards
        /// (check <see cref="SupportsLeaderboards"/> the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="id">Leaderboard identifier</param>
        /// <param name="score">Score to set</param>
        /// <param name="data">(Optional) If the platform supports it, additional data to set.</param>
        /// <returns>Task</returns>
        public static Task SetUserScore(string id, double score, object data = null)
        {
#if ANDROID_DEVICE
            return Bridge.SetUserScore(id, score, data);
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
        /// Retrieves the current <see cref="Kiln.Player"/>'s <see cref="Kiln.LeaderboardEntry"/>. If the platform doesn't support 
        /// Leaderboards the Task (check <see cref="SupportsLeaderboards"/>) will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="id">The Leaderboard identifier</param>
        /// <returns>Task that'll return a <see cref="Kiln.LeaderboardEntry"/> upon completion</returns>
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
        /// Retrieves a list of <see cref="Kiln.LeaderboardEntry"/> for all players. If the platform doesn't support leaderboards 
        /// (check <see cref="SupportsLeaderboards"/>) the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="count">Amount of <see cref="Kiln.LeaderboardEntry"/> to retrieve</param>
        /// <param name="offset">Offset from the top of the Leaderboard that <see cref="Kiln.LeaderboardEntry"/> will be fetched from</param>
        /// <param name="id">Leaderboard identifier</param>
        /// <returns>Task that'll return a list of <see cref="Kiln.LeaderboardEntry"/> upon completion</returns>
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
        /// Checks for Native Leaderboard UI support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
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
        /// Shows native Leaderboard UI if supported (check <see cref="SupportsPlatformLeaderboardUI"/>). 
        /// Otherwise returns a <see cref="Kiln.Exception"/>
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

            PlatformLeaderboardController leaderboards = MonoBehaviour.Instantiate(PlatformLeaderboardPrefab);
            leaderboards.Show(aTcs);

            return aTcs.Task;
#endif
        }

        #endregion

        #region In App Purchases

        /// <summary>
        /// Checks for In App Purchases support
        /// </summary>
        /// <returns><c>true</c> if supported, <c>false</c> otherwise</returns>
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
        /// Retrieves a list of all available for purcharse <see cref="Kiln.Product"/>. If the platform 
        /// doesn't support In App Purchases (see <see cref="SupportsIAP"/>) the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <returns>Task that'll return a list of <see cref="Kiln.Product"/> upon completion</returns>
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
        /// Retrieves a filtered list of available for purcharse <see cref="Kiln.Product"/>. If the platform 
        /// doesn't support In App Purchases (see <see cref="SupportsIAP"/>) the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="ids">List of identifiers to get in the list of <see cref="Kiln.Product"/> response</param>
        /// <returns>Task that'll return a list of <see cref="Kiln.Product"/> upon completion</returns>
        public static Task<List<IProduct>> GetAvailableProducts(List<string> ids) 
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
        /// Retrieves a list of purchased <see cref="Kiln.Product"/>. If the platform doesn't support 
        /// In App Purchases (see <see cref="SupportsIAP"/>) the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <returns>Task that'll return a list of <see cref="Kiln.Product"/> upon completion</returns>
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
        /// Launches the purchase flow of a <see cref="Kiln.Product"/>. If the platform doesn't support 
        /// In App Purchases (check <see cref="SupportsIAP"/>) the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="productID">Identifier of the <see cref="Kiln.Product"/> to be purchased</param>
        /// <param name="payload">Additional data to send with the purchase</param>
        /// <returns>Task that'll return a <see cref="Kiln.Purchase"/> upon successful completion</returns>
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
        /// It consumes an already purchased consumable product. If the platform doesn't support purchases 
        /// the Task will get an <see cref="Kiln.Exception"/>
        /// </summary>
        /// <param name="purchaseToken">Token associated with the <see cref="Kiln.Purchase"/></param>
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

        #endregion

        #region Analytics

        /// <summary>
        /// Sends an Analytics Event.
        /// </summary>
        /// <param name="identifier">The Event identifier</param>
        public static void SubmitAnalyticsEvent(string identifier)
        {
#if ANDROID_DEVICE
            Bridge.SubmitAnalyticsEvent(identifier);
#else
            CheckInitialized();

            if (!Settings.IsValidAnalyticsEventId(identifier))
            {
                throw new Kiln.Exception($"Invalid Analytics Event ID: {identifier}");
            }
#endif
        }

        #endregion

    }

}