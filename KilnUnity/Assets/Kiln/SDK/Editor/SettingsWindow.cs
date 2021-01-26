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

        private bool _initialized = false;

        private float _listItemHeight = EditorGUIUtility.singleLineHeight * 1.2f; // Height of each item on the reorderable list

        
        [MenuItem("Window/Kiln")]
        public static void Init()
        {
            LoadOrCreateSettings();

            GetWindow<SettingsWindow>(false, "Kiln Settings Editor", true);
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
                string[] directories = Directory.GetDirectories(Application.dataPath, "Kiln", SearchOption.AllDirectories);
                string path = "";
                foreach (var item in directories)
                { 
                    path = $"{item.Substring(Application.dataPath.Length + 1)}/Editor/Resources";
                    break;
                }

                if (path == "")
                {
                    throw new System.Exception("Kiln folder not found. Can't locate Kiln settings nor create new ones.");
                }

                // We'll create the settings scriptable object
                _settings = ScriptableObject.CreateInstance<Settings>();
                
                AssetDatabase.CreateAsset(_settings, $"Assets/{path}/KilnSettings.asset");
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
            }
        }

        /// <summary>
        /// Ads Item Drawing for the Reorderable List
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        void DrawAdsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.Ad ad = _settings.ADs[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2);
            
            Rect idRect = new Rect(rect.x + 15, rect.y + itemInputYOffset, rect.width * 0.7f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorGUI.TextField(idRect, ad.Id);

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
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30, rect.y + itemInputYOffset, rect.width * 0.3f - 45, EditorGUIUtility.singleLineHeight);
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
        void DrawIAPsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.InAppPurchase iap = _settings.IAPs[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2);
            
            Rect idRect = new Rect(rect.x + 15, rect.y + itemInputYOffset, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorGUI.TextField(idRect, iap.Id);

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
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30, rect.y + itemInputYOffset, rect.width * 0.3f - 30, EditorGUIUtility.singleLineHeight);
            Product.ProductType type = iap.Type;
            Product.ProductType newType = (Product.ProductType)EditorGUI.EnumPopup(typeRect, iap.Type);

            if (newType != iap.Type)
            {
                iap.Type = newType;
                _settings.IAPs[index] = iap;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect priceRect = new Rect(rect.x + idRect.width + 45 + typeRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 45, EditorGUIUtility.singleLineHeight);
            float newPrice = EditorGUI.FloatField(priceRect, iap.Price);

            if (newPrice != iap.Price)
            {
                iap.Price = newPrice;
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
        void DrawLeaderboardsListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            Settings.Leaderboard leaderboard = _settings.Leaderboards[index];

            float itemInputYOffset = ((_listItemHeight - EditorGUIUtility.singleLineHeight) / 2);
            
            Rect idRect = new Rect(rect.x + 15, rect.y + itemInputYOffset, rect.width * 0.5f, EditorGUIUtility.singleLineHeight);
            string newId = (string) EditorGUI.TextField(idRect, leaderboard.Id);

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
            
            Rect typeRect = new Rect(rect.x + idRect.width + 30, rect.y + itemInputYOffset, rect.width * 0.3f - 30, EditorGUIUtility.singleLineHeight);
            Kiln.Leaderboard.LeaderboardType type = leaderboard.Type;
            Kiln.Leaderboard.LeaderboardType newType = (Kiln.Leaderboard.LeaderboardType)EditorGUI.EnumPopup(typeRect, leaderboard.Type);

            if (newType != leaderboard.Type)
            {
                leaderboard.Type = newType;
                _settings.Leaderboards[index] = leaderboard;
                
                EditorUtility.SetDirty(_settings);
            }

            Rect resetButtonRect = new Rect(rect.x + idRect.width + 45 + typeRect.width, rect.y + itemInputYOffset, rect.width * 0.2f - 45, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(resetButtonRect, "RESET"))
            {
                // Erase data file if it exists
                Leaderboard.Reset(leaderboard.Id);
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawFeaturesSupport()
        {
            EditorGUILayout.BeginVertical();

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

            EditorGUILayout.EndVertical();
        }

        private void OnGUI()
        {
            CheckInitialize();

            // TODO: Fix this. When entering play mode or exiting we're losing the _settings reference.
            if(Application.isPlaying ||  _settings == null) return;

            DrawFeaturesSupport();

            if (_settings.SupportsRewardedAds || _settings.SupportsInterstitialAds)
            {
                _ads.DoLayoutList();
            }

            if (_settings.SupportsIAP)
            {
                _iaps.DoLayoutList();
            }

            if (_settings.SupportsLeaderboards)
            {
                _leaderboards.DoLayoutList();
            }
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
