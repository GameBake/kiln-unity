using System.Collections.Generic;
using UnityEngine;

namespace Kiln
{
    public class Settings : ScriptableObject
    {
        public enum AdType
        {
            REWARDED_VIDEO, INTERSTITIAL, BANNER
        }

        [System.Serializable]
        public enum LeaderboardType
        {
            HIGH_TO_LOW,
            LOW_TO_HIGH
        }

        [System.Serializable]
        public struct InAppPurchase
        {
            public string Id;
            public float Price;
            public ProductType Type;
            public string MetaData;
        }

        [System.Serializable]
        public struct Ad
        {
            public string Id;
            public AdType Type;
        }

        [System.Serializable]
        public struct Leaderboard
        {
            public string Id;
            public LeaderboardType Type;
        }


        [SerializeField] private List<InAppPurchase> _iaps = new List<InAppPurchase>();
        public List<InAppPurchase> IAPs { get { return _iaps; } }
        [SerializeField] private List<Ad> _ads = new List<Ad>();
        public List<Ad> ADs { get { return _ads; } }
        [SerializeField] private List<Leaderboard> _leaderboards = new List<Leaderboard>();
        public List<Leaderboard> Leaderboards { get { return _leaderboards; } }
        [SerializeField] private List<string> _analyticsEvents = new List<string>();
        public List<string> AnalyticsEvents { get { return _analyticsEvents; } }
        [SerializeField] private bool _supportsInterstitialAds = true;
        public bool SupportsInterstitialAds
        {
            get { return _supportsInterstitialAds; }
            set { _supportsInterstitialAds = value; }
        }
        [SerializeField] private bool _supportsRewardedAds = true;
        public bool SupportsRewardedAds
        {
            get { return _supportsRewardedAds; }
            set { _supportsRewardedAds = value; }
        }
        [SerializeField] private bool _supportsBannerAds = true;
        public bool SupportsBannerAds
        {
            get { return _supportsBannerAds; }
            set { _supportsBannerAds = value; }
        }
        [SerializeField] private bool _supportsLeaderboards = true;
        public bool SupportsLeaderboards
        {
            get { return _supportsLeaderboards; }
            set { _supportsLeaderboards = value; }
        }
        [SerializeField] private bool _supportsIAP = true;
        public bool SupportsIAP
        {
            get { return _supportsIAP; }
            set { _supportsIAP = value; }
        }
        [SerializeField] private bool _supportsPlatformLeaderboardUI = true;
        public bool SupportsPlatformLeaderboardUI
        {
            get { return _supportsPlatformLeaderboardUI; }
            set { _supportsPlatformLeaderboardUI = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="adType"></param>
        /// <returns></returns>
        private List<string> GetPlacementIds(AdType adType)
        {
            List<string> data = new List<string>();

            foreach (Ad ad in _ads)
            {
                if (ad.Type == adType)
                {
                    data.Add(ad.Id);
                }
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="adType"></param>
        /// <returns></returns>
        private bool IsValidPlacementId(string id, AdType adType)
        {
            return GetPlacementIds(adType).Contains(id);
        }

        #region Public API

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInterstitialsIds()
        {
            return GetPlacementIds(AdType.INTERSTITIAL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetRewardedIds()
        {
            return GetPlacementIds(AdType.REWARDED_VIDEO);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetBannerIds()
        {
            return GetPlacementIds(AdType.BANNER);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInAppPurchasesIds()
        {
            List<string> data = new List<string>();

            foreach (InAppPurchase iap in _iaps)
            {
                data.Add(iap.Id);
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsValidInterstitialId(string id)
        {
            return IsValidPlacementId(id, AdType.INTERSTITIAL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsValidRewardedId(string id)
        {
            return IsValidPlacementId(id, AdType.REWARDED_VIDEO);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsValidBannerId(string id)
        {
            return IsValidPlacementId(id, AdType.BANNER);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsValidAnalyticsEventId(string id)
        {
            return AnalyticsEvents.Contains(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetLeaderboardIds()
        {
            List<string> data = new List<string>();

            foreach (Leaderboard l in _leaderboards)
            {
                data.Add(l.Id);
            }

            return data;
        }

        #endregion
    
    }
}
