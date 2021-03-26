using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using System.Collections.Generic;
using Kiln.SimpleJSON;

namespace Kiln
{
    public class BuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            // Check if it's trying to build to an unsuportted platform
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                // Plugin might be on a project that's not making use of it. So we'll just show a warning.
                Debug.LogWarning($"<size=20>Kiln only supports Android for the moment.</size>");
                return;
            }
            else
            {
                Settings settings = Resources.Load<Settings>("KilnSettings");

                // If we do have a kiln configuration we'll copy that configuration to the Android Build
                if (settings != null)
                {

                    string assetsPath = $"{path}/src/main/assets";

                    string file;

                    // Copy the mocked leaderboard status
                    foreach (Settings.Leaderboard l in settings.Leaderboards)
                    {
                        if (Leaderboard.IsSaved(l.Id))
                        {
                            file = $"{assetsPath}/{Leaderboard.GetFileName(l.Id)}";
                            System.IO.File.Copy(Leaderboard.GetPath(l.Id), file, true);
                        }
                    }

                    // Copy the mocked In App Purchases status
                    file = $"{assetsPath}/{InAppPurchases.StorageFileName}";
                    if (InAppPurchases.IsSaved())
                    {
                        System.IO.File.Copy(InAppPurchases.StoragePath, file, true);
                    }
                    else
                    {
                        System.IO.File.WriteAllText(file, InAppPurchases.GetEmptyState());
                    }

                    // Create kiln definitions
                    var definitions = new KilnDefinition(settings);
                    file = $"{assetsPath}/{KilnDefinition.FileName}";
                    System.IO.File.WriteAllText(file, definitions.Serialize().ToString());
                }
            }
        }

        [System.Serializable]
        public class KilnDefinition
        {
            public static string FileName = "kiln-definitions-development.json";
            public struct LeaderboardSettings
            {
                public bool Ascending;

                public LeaderboardSettings(bool ascending)
                {
                    Ascending = ascending;
                }
            }

            public struct IAPSettings
            {
                public bool Consumable;
                public string Price;

                public IAPSettings(bool consumable, string price)
                {
                    Consumable = consumable;
                    Price = price;
                }
            }

            public List<string> Interstitials = new List<string>();
            public List<string> Banners = new List<string>();
            public List<string> RewardedVideos = new List<string>();
            public List<string> Events = new List<string>();
            public Dictionary<string, LeaderboardSettings> Leaderboards = new Dictionary<string, LeaderboardSettings>();
            public Dictionary<string, IAPSettings> IAP = new Dictionary<string, IAPSettings>();

            public KilnDefinition(Settings settings)
            {
                foreach (string id in settings.GetInterstitialsIds()) { Interstitials.Add(id); }
                foreach (string id in settings.GetRewardedIds()) { RewardedVideos.Add(id); }
                foreach (string id in settings.GetBannerIds()) { Banners.Add(id); }
                foreach (string id in settings.AnalyticsEvents) { Events.Add(id); }

                foreach (Kiln.Settings.Leaderboard l in settings.Leaderboards)
                {
                    Leaderboards.Add(l.Id, new LeaderboardSettings(l.Type == Settings.LeaderboardType.LOW_TO_HIGH));
                }

                foreach (Kiln.Settings.InAppPurchase i in settings.IAPs)
                {
                    IAP.Add(i.Id, new IAPSettings(i.Type == ProductType.CONSUMABLE, i.Price.ToString()));
                }
            }

            public JSONNode Serialize()
            {
                // {
                //   "ads":{
                //       "interstitial" : {
                //         "a1":"x1",
                //         "b1":"y1"
                //     }
                //       "banner" : {
                //         "c1":"z1",
                //         "d1":"w1"
                //     }
                //   },
                //   "events":{
                //     "a3":"x3",
                //     "b3":"y3"
                //   },
                //   "leaderboards":{
                //     "a": {
                //         "id":"x",
                //         "ascending":false
                //     },
                //   }
                //   "iap":{
                //     "a2": {
                //         "id":"z",
                //         "consumable":false,
                //         "price":"100.0"
                //     }
                //   },
                // }

                JSONObject data = new JSONObject();
                JSONObject ads = data["ads"].AsObject;
                JSONObject events = data["events"].AsObject;
                JSONObject leaderboards = data["leaderboards"].AsObject;
                JSONObject iap = data["iap"].AsObject;

                // Ads
                JSONObject interstitals = ads["interstitial"].AsObject;
                JSONObject banners = ads["banner"].AsObject;
                JSONObject rewardedVideos = ads["rewarded"].AsObject;

                foreach (string id in Interstitials) { interstitals[id] = id; }
                foreach (string id in Banners) { banners[id] = id; }
                foreach (string id in RewardedVideos) { rewardedVideos[id] = id; }

                // Events
                foreach (string id in Events) { events[id] = id; }

                // Leaderboards
                foreach (KeyValuePair<string, LeaderboardSettings> l in Leaderboards)
                {
                    JSONObject node = leaderboards[l.Key].AsObject;
                    node["id"] = l.Key;
                    node["ascending"] = l.Value.Ascending; ;
                }

                // In App Purchases
                foreach (KeyValuePair<string, IAPSettings> i in IAP)
                {
                    JSONObject node = iap[i.Key].AsObject;
                    node["id"] = i.Key;
                    node["consumable"] = i.Value.Consumable; ;
                    node["price"] = i.Value.Price; ;
                }

                return data;
            }
        }
    }
}