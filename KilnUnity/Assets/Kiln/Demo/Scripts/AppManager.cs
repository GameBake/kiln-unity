using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class AppManager : MonoBehaviour
    {
        [SerializeField] private GameObject _initButton;
        [SerializeField] private IDSelector _idSelector;
        
        [Header("Ads Buttons")]
        [SerializeField] private GameObject[] _interstitialsAdsButtons;
        [SerializeField] private GameObject[] _rewardedAdsButtons;

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
        /// Once Initialized we can check if we support different features, and thus disable those features
        /// </summary>
        private void ConfigureButtons()
        {
            // If we don't support interstitials ads, we'll disable the buttons
            if (!Kiln.API.SupportsInterstitialAds())
            {
                Logger.Log("Interstitials Ads not Supported", LogType.Warning);
                foreach (GameObject go in _interstitialsAdsButtons)
                {
                    go.SetActive(false);
                }
            }

            // If we don't support rewarded ads, we'll disable the buttons
            if (!Kiln.API.SupportsRewardedAds())
            {
                Logger.Log("Rewarded Ads not Supported", LogType.Warning);
                foreach (GameObject go in _rewardedAdsButtons)
                {
                    go.SetActive(false);
                }
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

                RewardedAdResponse response = await Kiln.API.ShowRewardedAd(placementId);
                
                Logger.Log($"Rewarded Ad Displayed. With reward: {response.getWithReward()}");
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

                LeaderboardEntry entry = await Kiln.API.GetUserScore(leaderboardID);

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
                
                List<LeaderboardEntry> entries = await Kiln.API.GetScores(_getScoresAmount, _getScoresOffset, leaderboardID);

                foreach (LeaderboardEntry entry in entries)
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
                List<Product> products = await Kiln.API.GetAvailableProducts();

                foreach (Product p in products)
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
                List<Purchase> purchases = await Kiln.API.GetPurchasedProducts();

                foreach (Purchase p in purchases)
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
                string productID = await _idSelector.SelectID(Kiln.API.IAP.GetProductIDs());
                _idSelector.Close();

                Purchase purchase = await Kiln.API.PurchaseProduct(productID, "DEVELOPER PAYLOAD TEST");
                
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
                string productID = await _idSelector.SelectID(Kiln.API.IAP.GetNonConsumedIDs());
                _idSelector.Close();

                Purchase pendingPurchase = Kiln.API.IAP.GetNonConsumedPurchase(productID);

                await Kiln.API.ConsumePurchasedProduct(pendingPurchase.GetPurchaseToken());
                
                Logger.Log($"Product {productID} consumed.");
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
        public void OnAnalyticsEventButton()
        {
            try
            {
                // TODO: Finish up implementation when we know what we'll be using

                // AnalyticEvent event = new AnalyticEvent();
                // Kiln.API.SubmitAnalyticsEvent();

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