using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kiln
{
    public class AppManager : MonoBehaviour
    {
        [SerializeField] private GameObject _initButton;
        [SerializeField] private IDSelector _idSelector;

        [Header("Sections")]
        [SerializeField] private GameObject _sectionButtons;
        [SerializeField] private GameObject _adsSection;
        [SerializeField] private GameObject _leaderboardsSection;
        [SerializeField] private GameObject _iapSection;
        [SerializeField] private GameObject _analyticsSection;

        private GameObject _currentSection;

        private double _scoreToSubmit;
        public string ScoreToSubmit
        {
            set
            {
                _scoreToSubmit = double.Parse(value);
            }
        }

        private int _getScoresAmount;
        public string GetScoresAmount
        {
            set
            {
                _getScoresAmount = int.Parse(value);
            }
        }

        private int _getScoresOffset;
        public string GetScoresOffset
        {
            set
            {
                _getScoresOffset = int.Parse(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        private void EnableSection(GameObject section)
        {
            if (_currentSection != null)
            {
                _currentSection.SetActive(false);
            }

            section.SetActive(true);

            _currentSection = section;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnInitButton()
        {
            try
            {
                await Kiln.API.Init();   
                
                Logger.Log("Initialized");

                _initButton.SetActive(false);
                _sectionButtons.SetActive(true);
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        #region Advertisement

        /// <summary>
        /// 
        /// </summary>
        public void OnAdvertisementSectionButton()
        {
            if (!Kiln.API.SupportsInterstitialAds() && !Kiln.API.SupportsRewardedAds())
            {
                Logger.Log("Both Interstitials and Rewarded ads are not supported.", LogType.Warning);
                return;
            }

            EnableSection(_adsSection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnLoadInterstitialAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetInterstitialsIds());
                _idSelector.Close();

                await Kiln.API.LoadInterstitialAd(placementId);
                
                Logger.Log("Interstitial Ad Loaded");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnShowInterstitialAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetInterstitialsIds());
                _idSelector.Close();

                await Kiln.API.ShowInterstitialAd(placementId);
                
                Logger.Log("Interstitial Ad Displayed");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnLoadRewardedAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetRewardedIds());
                _idSelector.Close();

                await Kiln.API.LoadRewardedAd(placementId);
                
                Logger.Log("Rewarded Ad Loaded");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnShowRewardedAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetRewardedIds());
                _idSelector.Close();

                IRewardedAdResponse response = await Kiln.API.ShowRewardedAd(placementId);
                
                Logger.Log($"Rewarded Ad Displayed. With reward: {response.GetWithReward()}");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnLoadBannerAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetBannerIds());
                _idSelector.Close();

                // Now we need to display a placement list
                string alignment = await _idSelector.SelectID(Enum.GetNames(typeof(BannerPosition)).ToList());
                _idSelector.Close();
                BannerPosition bannerPosition = (BannerPosition) Enum.Parse(typeof(BannerPosition), alignment, false);

                // Finally we need a banner size
                string size = await _idSelector.SelectID(Enum.GetNames(typeof(BannerSize)).ToList());
                _idSelector.Close();
                BannerSize bannerSize = (BannerSize) Enum.Parse(typeof(BannerSize), size, false);

                await Kiln.API.LoadBannerAd(placementId, bannerPosition, bannerSize);
                
                Logger.Log("Banner Ad Loaded");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnShowBannerAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetBannerIds());
                _idSelector.Close();

                await Kiln.API.ShowBannerAd(placementId);
                
                Logger.Log("Banner Ad Displayed");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnHideBannerAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetBannerIds());
                _idSelector.Close();

                await Kiln.API.HideBannerAd(placementId);
                
                Logger.Log("Banner Ad Hidden");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnDestroyBannerAdButton()
        {
            try
            {
                string placementId = await _idSelector.SelectID(Kiln.API.Settings.GetBannerIds());
                _idSelector.Close();

                await Kiln.API.DestroyBannerAd(placementId);
                
                Logger.Log("Banner Ad Destroyed");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        #endregion

        #region Leaderboards

        /// <summary>
        /// 
        /// </summary>
        public void OnLeaderboardsSectionButton()
        {
            if (!Kiln.API.SupportsLeaderboards())
            {
                Logger.Log("Leaderboards are not supported.", LogType.Warning);
                return;
            }

            EnableSection(_leaderboardsSection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnGetUserScoreButton()
        {
             try
            {
                string leaderboardID = await _idSelector.SelectID(Kiln.API.Settings.GetLeaderboardIds());
                _idSelector.Close();

                ILeaderboardEntry entry = await Kiln.API.GetUserScore(leaderboardID);

                Logger.Log(entry.ToString());
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnSetUserScoreButton()
        {
             try
            {
                string leaderboardID = await _idSelector.SelectID(Kiln.API.Settings.GetLeaderboardIds());
                _idSelector.Close();

                await Kiln.API.SetUserScore(leaderboardID, _scoreToSubmit);

                Logger.Log("User score submitted successfully");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnGetScoresButton()
        {
            try
            {
                string leaderboardID = await _idSelector.SelectID(Kiln.API.Settings.GetLeaderboardIds());
                _idSelector.Close();

                List<ILeaderboardEntry> entries = await Kiln.API.GetScores(leaderboardID, _getScoresAmount, _getScoresOffset);

                foreach (ILeaderboardEntry entry in entries)
                {
                    Logger.Log(entry.ToString());
                }
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public async void OnPlatformLeaderboardsButton()
        {
            try
            {
                await Kiln.API.ShowPlatformLeaderboardUI();
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        #endregion

        #region In App Purchases

        /// <summary>
        /// 
        /// </summary>
        public void OnIAPSectionButton()
        {
            if (!Kiln.API.SupportsIAP())
            {
                Logger.Log("In App Purchases are not supported.", LogType.Warning);
                return;
            }

            EnableSection(_iapSection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnGetAvailableProductsButton()
        {
            try
            {
                List<IProduct> products = await Kiln.API.GetAvailableProducts();

                foreach (IProduct p in products)
                {
                    Logger.Log(p.ToString());
                }
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnGetPurchasesButton()
        {
            try
            {
                List<IPurchase> purchases = await Kiln.API.GetPurchases();

                foreach (IPurchase p in purchases)
                {
                    Logger.Log(p.ToString());
                }
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnPurchaseProductButton()
        {
            try
            {
                string productID = await _idSelector.SelectID(Kiln.API.Settings.GetInAppPurchasesIds());
                _idSelector.Close();

                IPurchase purchase = await Kiln.API.PurchaseProduct(productID, "DEVELOPER PAYLOAD TEST");

                Logger.Log($"Product {purchase.GetProductID()} ready for consumption");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async void OnConsumeProductButton()
        {
            try
            {
                // First we'll compose a list of purchase Tokens available
                List<IPurchase> activePurchases = await Kiln.API.GetPurchases();
                List<string> tokenList = new List<string>();

                foreach (IPurchase p in activePurchases)
                {
                    tokenList.Add(p.GetPurchaseToken());
                }

                string purchaseToken = await _idSelector.SelectID(tokenList);
                _idSelector.Close();

                await Kiln.API.ConsumePurchasedProduct(purchaseToken);

                Logger.Log($"Product with token {purchaseToken} consumed.");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }


#endregion

#region Analytics

        /// <summary>
        /// 
        /// </summary>
        public void OnAnalyticsSectionButton()
        {
            EnableSection(_analyticsSection);
        }

        /// <summary>
        /// 
        /// </summary>
        public async void OnAnalyticsEventButton()
        {
            try
            {
                string eventID = await _idSelector.SelectID(Kiln.API.Settings.AnalyticsEvents);
                _idSelector.Close();

                Kiln.API.SubmitAnalyticsEvent(eventID);

                Logger.Log($"Analytics Event Fired.");
            }
            catch (Kiln.Exception ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            return;
        }

#endregion
    }
}