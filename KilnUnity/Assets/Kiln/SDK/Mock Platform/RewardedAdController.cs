#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class RewardedAdController : MonoBehaviour
    {
        [SerializeField] private Text _placementID;
        private TaskCompletionSource<IRewardedAdResponse> _tcs;

        /// <summary>
        /// The prefab is disabled by default, display it
        /// </summary>
        /// <param name="task">Task Completion Source to communicate once interstitial's done showing</param>
        public void Show(TaskCompletionSource<IRewardedAdResponse> tcs, string placementId)
        {
            _tcs = tcs;
            _placementID.text = placementId;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClose()
        {
            RewardedAdResponse response = new RewardedAdResponse();
            _tcs.SetResult(response);

            Destroy(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnReward()
        {
            RewardedAdResponse response = new RewardedAdResponse();
            response.RewardUser = true;
            _tcs.SetResult(response);
            
            Destroy(gameObject);
        }
    }
}
#endif