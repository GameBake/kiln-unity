#if UNITY_EDITOR
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class PlatformLeaderboardController : MonoBehaviour
    {
        [SerializeField] private Text _leaderboardData;
        [SerializeField] private IDSelector _idSelector;

        private TaskCompletionSource<object> _tcs;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        public void Show(TaskCompletionSource<object> tcs)
        {
            _tcs = tcs;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public async void OnSelectLeaderboardButton()
        {
            string leaderboardID = await _idSelector.SelectID(Kiln.API.Settings.GetLeaderboardIds());
            _idSelector.Close();

            List<ILeaderboardEntry> entries = await Kiln.API.GetScores(1000, 0, leaderboardID);

            _leaderboardData.text = "";

            foreach (LeaderboardEntry entry in entries)
            {
                _leaderboardData.text += $"{entry.ToString()}\n";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnCloseButton()
        {
            _tcs.SetResult(null);
            Destroy(gameObject);
        }
    }   
}
#endif