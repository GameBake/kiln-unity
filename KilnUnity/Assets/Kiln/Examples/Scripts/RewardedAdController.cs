using UnityEngine;

namespace Kiln
{
    public class RewardedAdController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public void OnReward()
        {
            // TODO: Notify reward
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClose()
        {
            // TODO: Somehow notifiy Kiln API
            Destroy(gameObject);
        }
    }
}

