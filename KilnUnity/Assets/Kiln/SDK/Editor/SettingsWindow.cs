using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

namespace Kiln
{
    public class SettingsWindow : EditorWindow
    {
        private static Settings _settings;
        private ReorderableList _ads;
        private ReorderableList _iaps;
        private ReorderableList _leaderboards;
        private ReorderableList _analyticsEvents;

        private bool _initialized = false;

        private float _listItemHeight = EditorGUIUtility.singleLineHeight * 1.2f; // Height of each item on the reorderable list

        
        [MenuItem("Window/Kiln")]
        public static void Init()
        {
            LoadOrCreateSettings();

            var windows = GetWindow<SettingsWindow>(false, "Kiln Settings Editor", true);
            windows.minSize = new Vector2(900, 700);
        }

        /// <summary>
        /// Loads Settings resources or create a new asset if it doesn't exist
        /// </summary>
        private static void LoadOrCreateSettings()
        {
            // TODO: Research if there's a better way to implement this. This depends on Kiln folder and structure to be kept as is by the user
            _settings = Resources.Load<Settings>("KilnSettings");

            if (_settings == null)
            {
                string kilnPath = Utils.GetFolderPath("Kiln");
                if (kilnPath == null)
                {
                    throw new System.Exception("Kiln plugin folder not found. Can't locate Kiln settings nor create new ones.");
                }

                string path = $"{kilnPath}/{Constants.Folders.Settings}";

                // If the folder doesn't exist, we'll create it
                Directory.CreateDirectory(path);

                // We'll create the settings scriptable object
                _settings = ScriptableObject.CreateInstance<Settings>();
                
                AssetDatabase.CreateAsset(_settings, $"{path}/KilnSettings.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CheckInitialize()
        {
            if (!_initialized)
            {
                _initialized = true;
    
                // Ads list
                _ads = new ReorderableList(_settings.ADs, typeof(Settings.Ad), false, true, true, true);
                _ads.drawElementCallback = DrawAdsListItems;
                _ads.drawHeaderCallback = (Rect rect) => {
                    string name = "Ad Placements Setup";
                    EditorGUI.LabelField(rect, name);
                };
                _ads.onRemoveCallback = (ReorderableList list) => {
                    _settings.ADs.RemoveAt(list.index);
                    EditorUtility.SetDirty(_settings);
                };
                _ads.onAddCallback = (ReorderableList list) => {
                    _settings.ADs.Add(new Settings.Ad());
                    EditorUtility.SetDirty(_settings);
                };
                _ads.elementHeight = _listItemHeight;

                // IAPs list
                _iaps = new ReorderableList(_settings.IAPs, typeof(Settings.InAppPurchase), false, true, true, true);
                _iaps.drawElementCallback = DrawIAPsListItems;
                _iaps.drawHeaderCallback = (Rect rect) => {
                    string name = "In App Purchases Setup";
                    EditorGUI.LabelField(rect, name);
                    Rect resetButtonRect = new Rect(rect.x + rect.width - 60, rect.y, 100 - 35, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(resetButtonRect, "RESET"))
                    {
                        // Erase data file if it exists
                        InAppPurchases.Reset();
                        UnityEditor.AssetDatabase.Refresh();
                    }

                    // We'll setup a project wide selector for mocking the currency IAPs are going to use
                    Rect currencyCodeRect = new Rect(rect.x + 200, rect.y, 100 - 35, EditorGUIUtility.singleLineHeight);
                    string label = "Currency";
                    EditorGUI.LabelField(currencyCodeRect, label);
                    currencyCodeRect.x += 50;
                    CurrencyCode currencyCode = _settings.CurrencyCode;
                    CurrencyCode newCurrencyCode = (CurrencyCode)EditorGUI.EnumPopup(currencyCodeRect, currencyCode);

                    if (newCurrencyCode != currencyCode)
                    {
                        _settings.CurrencyCode = newCurrencyCode;
                        EditorUtility.SetDirty(_settings);
                    }

                };
                _iaps.onRemoveCallback = (ReorderableList list) => {
                    _settings.IAPs.RemoveAt(list.index);
                    EditorUtility.SetDirty(_settings);
                };
                _iaps.onAddCallback = (ReorderableList list) => {
                    _settings.IAPs.Add(new Settings.InAppPurchase());
                    EditorUtility.SetDirty(_settings);
                };
                _iaps.elementHeight = _listItemHeight;

                // Leaderboard list
                _leaderboards = new ReorderableList(_settings.Leaderboards, typeof(Settings.Leaderboard), false, true, true, true);
                _leaderboards.drawElementCallback = DrawLeaderboardsListItems;
                _leaderboards.drawHeaderCallback = (Rect rect) => {
                    string name = "Leaderboards Setup";
                    EditorGUI.LabelField(rect, name);
                };
                _leaderboards.onRemoveCallback = (ReorderableList list) =>
                {
                    // Erase data file if it exists
                    Leaderboard.Reset(_settings.Leaderboards[list.index].Id);
                    UnityEditor.AssetDatabase.Refresh();
                    
                    _settings.Leaderboards.RemoveAt(list.index);
                    EditorUtility.SetDirty(_settings);
                };
                _leaderboards.onAddCallback = (ReorderableList list) => {
                    _settings.Leaderboards.Add(new Settings.Leaderboard());
                    EditorUtility.SetDirty(_settings);
                };
                _leaderboards.elementHeight = _listItemHeight;

                // Analytic Events list
                _analyticsEvents = new ReorderableList(_settings.AnalyticsEvents, typeof(string), false, true, true, true);
                _analyticsEvents.drawElementCallback = DrawAnalyticEventsListItems;
                _analyticsEvents.drawHeaderCallback = (Rect rect) => {
                    string name = "Analytic Events";
                    EditorGUI.LabelField(rect, name);
                };
                _analyticsEvents.onRemoveCallback = (ReorderableList list) =>
                {
                    _settings.AnalyticsEvents.RemoveAt(list.index);
                    EditorUtility.SetDirty(_settings);
                };
                _analyticsEvents.onAddCallback = (ReorderableList list) => {
                    _settings.AnalyticsEvents.Add("");
                    EditorUtility.SetDirty(_settings);
                };
                _analyticsEvents.elementHeight = _listItemHeight;
            }
        }

        /// <summary>
        /// Ads Item Drawing for the Reorderable List
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawAdsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.Ad ad = _settings.ADs[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2);
            
            Rect idRect = new Rect(rect.x + 15f, rect.y + itemInputYOffset, rect.width * 0.7f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorUtils.TextFieldWithPlaceholder(idRect, ad.Id, "Placement ID");

            if (ad.Id != newId)
            {
                // We'll check first if this is not a duplicate ID
                bool duplicate = false;
                for (int i = 0; i < _settings.ADs.Count; i++)
                {
                    if (index == i) continue;

                    if (newId == _settings.ADs[i].Id)
                    {
                        // Duplicate !
                        Debug.LogError("IDs must be unique.");
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                {
                    ad.Id = newId;
                    _settings.ADs[index] = ad;
                    
                    EditorUtility.SetDirty(_settings);
                }
            }
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30f, rect.y + itemInputYOffset, rect.width * 0.3f - 45f, EditorGUIUtility.singleLineHeight);
            Settings.AdType type = ad.Type;
            Settings.AdType newType = (Settings.AdType)EditorGUI.EnumPopup(typeRect, ad.Type);

            if (newType != ad.Type)
            {
                ad.Type = newType;
                _settings.ADs[index] = ad;
                
                EditorUtility.SetDirty(_settings);
            }
        }

        /// <summary>
        /// In App Purchase Item Drawing for the Reorderable List
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawIAPsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.InAppPurchase iap = _settings.IAPs[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2f);
            
            Rect idRect = new Rect(rect.x + 15f, rect.y + itemInputYOffset, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorUtils.TextFieldWithPlaceholder(idRect, iap.Id, "ID");

            if (iap.Id != newId)
            {
                // We'll check first if this is not a duplicate ID
                bool duplicate = false;
                for (int i = 0; i < _settings.IAPs.Count; i++)
                {
                    if (index == i) continue;

                    if (newId == _settings.IAPs[i].Id)
                    {
                        // Duplicate !
                        Debug.LogError("IDs must be unique.");
                        duplicate = true;
                        break;
                    }
                }
                
                if (!duplicate)
                {
                    iap.Id = newId;
                    _settings.IAPs[index] = iap;
                    
                    EditorUtility.SetDirty(_settings);
                }
            }
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30f, rect.y + itemInputYOffset, rect.width * 0.2f - 15f, EditorGUIUtility.singleLineHeight);
            ProductType type = iap.Type;
            ProductType newType = (ProductType)EditorGUI.EnumPopup(typeRect, iap.Type);

            if (newType != iap.Type)
            {
                iap.Type = newType;
                _settings.IAPs[index] = iap;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect priceRect = new Rect(rect.x + idRect.width + 45f + typeRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 15f, EditorGUIUtility.singleLineHeight);
            float newPrice = EditorUtils.FloatFieldWithPlaceholder(priceRect, iap.Price, "Price");

            if (newPrice != iap.Price)
            {
                iap.Price = newPrice;
                _settings.IAPs[index] = iap;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect descriptionRect = new Rect(rect.x + idRect.width + 60f + typeRect.width + priceRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 15f, EditorGUIUtility.singleLineHeight);
            string newDescription = EditorUtils.TextFieldWithPlaceholder(descriptionRect, iap.Description, "Description");

            if (newDescription != iap.Description)
            {
                iap.Description = newDescription;
                _settings.IAPs[index] = iap;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect imageURIRect = new Rect(rect.x + idRect.width + 75f + typeRect.width + priceRect.width + descriptionRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 30f, EditorGUIUtility.singleLineHeight);
            string newImageURI = EditorUtils.TextFieldWithPlaceholder(imageURIRect, iap.ImageURI, "Image URI");

            if (newImageURI != iap.ImageURI)
            {
                iap.ImageURI = newImageURI;
                _settings.IAPs[index] = iap;
                
                EditorUtility.SetDirty(_settings);
            }
        }

         /// <summary>
        /// Leaderboard Item Drawing for the Reorderable List
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawLeaderboardsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.Leaderboard leaderboard = _settings.Leaderboards[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2f);
            
            Rect idRect = new Rect(rect.x + 15f, rect.y + itemInputYOffset, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorUtils.TextFieldWithPlaceholder(idRect, leaderboard.Id, "Leaderboard ID");

            if (leaderboard.Id != newId)
            {
                // We'll check first if this is not a duplicate ID
                bool duplicate = false;
                for (int i = 0; i < _settings.Leaderboards.Count; i++)
                {
                    if (index == i) continue;

                    if (newId == _settings.Leaderboards[i].Id)
                    {
                        // Duplicate !
                        Debug.LogError("IDs must be unique.");
                        duplicate = true;
                        break;
                    }
                }
                
                if (!duplicate)
                {
                    leaderboard.Id = newId;
                    _settings.Leaderboards[index] = leaderboard;
                    
                    EditorUtility.SetDirty(_settings);
                }
            }
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30f, rect.y + itemInputYOffset, rect.width * 0.3f - 30f, EditorGUIUtility.singleLineHeight);
            Kiln.Settings.LeaderboardType type = leaderboard.Type;
            Kiln.Settings.LeaderboardType newType = (Kiln.Settings.LeaderboardType)EditorGUI.EnumPopup(typeRect, leaderboard.Type);

            if (newType != leaderboard.Type)
            {
                leaderboard.Type = newType;
                _settings.Leaderboards[index] = leaderboard;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect resetButtonRect = new Rect(rect.x + idRect.width + 45f + typeRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 45f, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(resetButtonRect, "RESET"))
            {
                // Erase data file if it exists
                Leaderboard.Reset(leaderboard.Id);
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        private void DrawAnalyticEventsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            string analyticsEvent = _settings.AnalyticsEvents[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2);
            
            Rect idRect = new Rect(rect.x + 15f, rect.y + itemInputYOffset, rect.width - 15f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorUtils.TextFieldWithPlaceholder(idRect, analyticsEvent, "Event ID");

            if (analyticsEvent != newId)
            {
                // We'll check first if this is not a duplicate ID
                bool duplicate = false;
                for (int i = 0; i < _settings.AnalyticsEvents.Count; i++)
                {
                    if (index == i) continue;

                    if (newId == _settings.AnalyticsEvents[i])
                    {
                        // Duplicate !
                        Debug.LogError("IDs must be unique.");
                        duplicate = true;
                        break;
                    }
                }
                
                if (!duplicate)
                {
                    _settings.AnalyticsEvents[index] = newId;
                    
                    EditorUtility.SetDirty(_settings);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawFeaturesSupport()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            
            // Supported Mock Features
            EditorGUILayout.LabelField("Supported Mock Features:", EditorStyles.boldLabel);

            bool supportsIAP = EditorGUILayout.Toggle("Supports In App Purchases", _settings.SupportsIAP);
            if (supportsIAP != _settings.SupportsIAP)
            {
                _settings.SupportsIAP = supportsIAP;
                EditorUtility.SetDirty(_settings);
            }

            bool supportsLeaderboards = EditorGUILayout.Toggle("Supports Leaderboards", _settings.SupportsLeaderboards);
            if (supportsLeaderboards != _settings.SupportsLeaderboards)
            {
                _settings.SupportsLeaderboards = supportsLeaderboards;
                EditorUtility.SetDirty(_settings);
            }

            bool supportsRewardedAds = EditorGUILayout.Toggle("Supports Rewarded Ads", _settings.SupportsRewardedAds);
            if (supportsRewardedAds != _settings.SupportsRewardedAds)
            {
                _settings.SupportsRewardedAds = supportsRewardedAds;
                EditorUtility.SetDirty(_settings);
            }

            bool supportsInterstitialAds = EditorGUILayout.Toggle("Supports Interstitial Ads", _settings.SupportsInterstitialAds);
            if (supportsInterstitialAds != _settings.SupportsInterstitialAds)
            {
                _settings.SupportsInterstitialAds = supportsInterstitialAds;
                EditorUtility.SetDirty(_settings);
            }

            bool supportsBannerAds = EditorGUILayout.Toggle("Supports Banner Ads", _settings.SupportsBannerAds);
            if (supportsBannerAds != _settings.SupportsBannerAds)
            {
                _settings.SupportsBannerAds = supportsBannerAds;
                EditorUtility.SetDirty(_settings);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            // Build Export Options
            EditorGUILayout.LabelField("Build Export Options:", EditorStyles.boldLabel);

            bool exportIAP = EditorGUILayout.Toggle("Export IAP State", _settings.ExportIAPState);
            if (exportIAP != _settings.ExportIAPState)
            {
                _settings.ExportIAPState = exportIAP;
                EditorUtility.SetDirty(_settings);
            }

            bool exportLeaderboards = EditorGUILayout.Toggle("Export Leaderboards State", _settings.ExportLeaderboardState);
            if (exportLeaderboards != _settings.ExportLeaderboardState)
            {
                _settings.ExportLeaderboardState = exportLeaderboards;
                EditorUtility.SetDirty(_settings);
            }

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            CheckInitialize();

            // TODO: Fix this. When entering play mode or exiting we're losing the _settings reference.
            if(Application.isPlaying || _settings == null) return;

            DrawFeaturesSupport();

            GUILayout.Space(20);

            if ((_settings.SupportsRewardedAds || _settings.SupportsInterstitialAds) && _ads != null)
            {
                _ads.DoLayoutList();
                GUILayout.Space(20);
            }

            if (_settings.SupportsIAP && _iaps != null)
            {
                _iaps.DoLayoutList();
                GUILayout.Space(20);
            }

            if (_settings.SupportsLeaderboards && _leaderboards != null)
            {
                _leaderboards.DoLayoutList();
                GUILayout.Space(20);
            }

            _analyticsEvents.DoLayoutList();
            GUILayout.Space(20);
        }

        void OnDestroy()
        {
            // We'll remove the invalid entries in settings. Can't have entries without IDs
            for (int i = 0; i < _settings.ADs.Count; i++)
            {
                Settings.Ad ad = _settings.ADs[i];

                if (string.IsNullOrEmpty(ad.Id))
                {
                    _settings.ADs.RemoveAt(i);
                    i--;
                    EditorUtility.SetDirty(_settings);

                    Debug.Log("Removing empty entry from Ads. Can't have empty IDs");
                }
            }

            for (int i = 0; i < _settings.IAPs.Count; i++)
            {
                Settings.InAppPurchase iap = _settings.IAPs[i];

                if (string.IsNullOrEmpty(iap.Id))
                {
                    _settings.IAPs.RemoveAt(i);
                    i--;
                    EditorUtility.SetDirty(_settings);

                    Debug.Log("Removing empty entry from IAPs. Can't have empty IDs");
                }
            }
        }
    }
}
