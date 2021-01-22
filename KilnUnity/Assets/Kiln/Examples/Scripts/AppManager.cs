using UnityEngine;

namespace Kiln
{
    public class AppManager : MonoBehaviour
    {
        [SerializeField] private GameObject _initButton;
        
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
                Logger.Log("Platform is " + Kiln.API.PlatformAvailable());

                _initButton.SetActive(false);
                _sectionButtons.SetActive(true);
            }
            catch (KilnException ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex);
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
                // TODO: handle placement Ids
                await Kiln.API.LoadInterstitialAd("CHACHA");
                
                Logger.Log("Interstitial Ad Loaded");
            }
            catch (KilnException ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex);
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
                // TODO: handle placement Ids
                await Kiln.API.ShowInterstitialAd("CHACHA");
                
                Logger.Log("Interstitial Ad Displayed");
            }
            catch (KilnException ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex);
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
                // TODO: handle placement Ids
                await Kiln.API.LoadRewardedAd("CHACHA");
                
                Logger.Log("Rewarded Ad Loaded");
            }
            catch (KilnException ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex);
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
                // TODO: handle placement Ids
                await Kiln.API.ShowRewardedAd("CHACHA");
                
                Logger.Log("Interstitial Ad Displayed");
            }
            catch (KilnException ex) 
            {
                Logger.Log(ex);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex);
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

        #endregion

        #region Analytics

        /// <summary>
        /// 
        /// </summary>
        public void OnAnalyticsSectionButton()
        {
            EnableSection(_analyticsSection);
        }

        #endregion
    }
}