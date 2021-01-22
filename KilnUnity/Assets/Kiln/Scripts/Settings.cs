#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Kiln
{
    // [CreateAssetMenu(menuName = "Kiln/Configuration")]
    public class Settings : ScriptableObject
    {
        public enum AdType
        {
            REWARDED_VIDEO, INTERSTITIAL
        }

        public enum InAppPurchaseType
        {
            CONSUMABLE, NON_CONSUMABLE
        }

        [System.Serializable]
        public struct InAppPurchase
        {
            public string Id;
            public float Price;
            public InAppPurchaseType Type;
            public string MetaData;
        }

        [System.Serializable]
        public struct Ad
        {
            public string Id;
            public AdType Type;
        }


        [SerializeField] private List<InAppPurchase> _iaps = new List<InAppPurchase>();
        public List<InAppPurchase> IAPs { get { return _iaps; } }
        [SerializeField] private List<Ad> _ads = new List<Ad>();
        public List<Ad> ADs { get { return _ads; } }
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
    }

}
#endif