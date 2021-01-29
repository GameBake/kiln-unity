#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using static Kiln.Settings;

namespace Kiln
{
    /// <summary>
    /// Incredibly inefficient implementation of a Leaderboard system for in editor development mocking purposes
    /// </summary>
    [System.Serializable]
    public class Leaderboard
    {
        [System.Serializable]
        public struct Entry
        {
            public string Name;
            public double Score;

            public Entry(string name, double score)
            {
                Name = name;
                Score = score;
            }
        }

        

        [SerializeField] private string _id;
        private Dictionary<string, double> _data = new Dictionary<string, double>();
        [SerializeField] private Entry[] _serializableData;
        [SerializeField] private LeaderboardType _type;
        private const string PLAYER_KEY = "PLAYER";

        public Leaderboard(string id = "leaderboard", int amount = 100, LeaderboardType type = LeaderboardType.HIGH_TO_LOW)
        {
            _id = id;
            _type = type;
            Initialize(amount);
            Save();
        }

        /// <summary>
        /// Returns the path to the file that'll hold the data for a leaderboard
        /// </summary>
        /// <param name="id">Leaderboard id</param>
        /// <returns></returns>
        public static string GetPath(string id)
        {
            return $"{Application.persistentDataPath}/KilnLeaderboard-{id}.json";
        }

        /// <summary>
        /// Erases the storage datafile for a leaderboard
        /// </summary>
        /// <param name="id">Leaderboard id</param>
        public static void Reset(string id)
        {
            string path = GetPath(id);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Creates an instance of a Leaderboard from a persisted json file
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Leaderboard Load(string id)
        {
            if (!IsSaved(id))
            {
                throw new System.Exception($"Leaderboard {id} not persisted on disk");
            }

            string path = GetPath(id);

            Leaderboard l = JsonUtility.FromJson<Leaderboard>(File.ReadAllText(path));

            // We'll recreate the non serializable data dictionary
            l._data = new Dictionary<string, double>();
            foreach (Entry e in l._serializableData)
            {
                l._data.Add(e.Name, e.Score);
            }

            return l;
        }

        /// <summary>
        /// Returns whether a saved file for a given Leaderboard id is present or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsSaved(string id)
        {
            string path = GetPath(id);

            return File.Exists(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        private void Initialize(int amount)
        {
            if (amount > 100)
            {
                amount = 100;
                Logger.Log("Dummy leaderboard capped at 100 entries. No reason to use more for development.", LogType.Warning);
            }

            int namesAmount = Kiln.Helper.Names.Count;
            double score = _type == LeaderboardType.HIGH_TO_LOW ? 10000d : 10d;

            for (int i = 0; i < amount; i++)
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        int randomIndex = Random.Range(0, namesAmount);
                        _data.Add(Kiln.Helper.Names[randomIndex], score);
                        score += (_type == LeaderboardType.HIGH_TO_LOW ? -1d : 1d) * 10d;

                        done = true;
                    }
                    catch (System.ArgumentException) { }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            // We'll first dump the dictionary into a serializable array of Entry structs
            _serializableData = new Entry[_data.Count];

            int index = 0;
            foreach (KeyValuePair<string, double> entry in _data)
            {
                Entry d = new Entry(entry.Key, entry.Value);
                _serializableData[index] = d;
                index++;
            }

            string path = GetPath(_id);

            File.WriteAllText(path, JsonUtility.ToJson(this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetRank(string id)
        {
            List<KeyValuePair<string, double>> dataList = _data.ToList();

            dataList.Sort((KeyValuePair<string, double> x, KeyValuePair<string, double> y) =>
            {
                if (_type == LeaderboardType.HIGH_TO_LOW)
                {
                    return y.Value.CompareTo(x.Value);
                }
                
                return x.Value.CompareTo(y.Value);
            });

            for (int i = 0; i < dataList.Count; i++)
            {
                KeyValuePair<string, double> x = dataList[i];
                
                if(dataList[i].Key == id) return i + 1;
            }

            throw new Kiln.Exception($"User {id} doesn't have a rank in leaderboard id: {_id}");
        }

        #region Public API

        /// <summary>
        /// Outputs Leaderboard to console
        /// </summary>
        public void Debug()
        {
            foreach (KeyValuePair<string, double> entry in _data)
            {
                Logger.Log($"{entry.Key} -- {entry.Value}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="score"></param>
        /// <param name="data"></param>
        public void SetUserScore(double score, object data)
        {
            if (_data.ContainsKey(PLAYER_KEY))
            {
                if ((_type == LeaderboardType.HIGH_TO_LOW && score > _data[PLAYER_KEY]) || (_type == LeaderboardType.LOW_TO_HIGH && score < _data[PLAYER_KEY]))
                {
                    _data[PLAYER_KEY] = score;
                    Save();
                }
            }
            else
            {
                _data.Add(PLAYER_KEY, score);
                Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LeaderboardEntry GetUserScore()
        {
            if (!_data.ContainsKey(PLAYER_KEY))
            {
                // TODO: How does this behave on the bridge ?
                throw new Kiln.Exception($"User {PLAYER_KEY} not present in leaderboard.");
            }

            LeaderboardEntry score = new LeaderboardEntry();

            var player = new Player();
            player.Name = PLAYER_KEY;

            score.Score = _data[PLAYER_KEY];
            score.Rank = GetRank(PLAYER_KEY);
            score.Player = player;

            return score;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<LeaderboardEntry> GetScores(int count, int offset)
        {
            List<LeaderboardEntry> scores = new List<LeaderboardEntry>();

            if (_data.Count > offset)
            {
                List<KeyValuePair<string, double>> dataList = _data.ToList();

                dataList.Sort((KeyValuePair<string, double> x, KeyValuePair<string, double> y) =>
                {
                    if (_type == LeaderboardType.HIGH_TO_LOW)
                    {
                        return y.Value.CompareTo(x.Value);
                    }
                    
                    return x.Value.CompareTo(y.Value);
                    
                });

                for (int i = offset; i < count + offset; i++)
                {
                    if (i >= _data.Count) break;

                    var player = new Player();
                    player.Name = dataList[i].Key;

                    LeaderboardEntry entry = new LeaderboardEntry();
                    entry.Score = dataList[i].Value;
                    entry.Rank = offset + i + 1;
                    entry.Player = player;

                    scores.Add(entry);
                }
            }

            return scores;
        }

        #endregion
    }
}
#endif