using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class InterstitialAdController : MonoBehaviour
    {
        [SerializeField] private int _autoCloseTime = 5;
        [SerializeField] private Text _autoCloseLabel;
        [SerializeField] private Text _placementIDLabel;

        private TaskCompletionSource<object> _tcs;

        private float _countdown;

        private void Awake()
        {
            _autoCloseLabel.text = _autoCloseTime.ToString();
            _countdown = _autoCloseTime;
        }
        
        private void Update()
        {
            if (_countdown > 0)
            {
                _autoCloseLabel.text = Mathf.Floor(_countdown).ToString();
                _countdown -= Time.deltaTime;
            }
            else
            {
                OnClose();
            }
        }

        /// <summary>
        /// The prefab is disabled by default, display it
        /// </summary>
        /// <param name="task">Task Completion Source to communicate once interstitial's done showing</param>
        public void Show(TaskCompletionSource<object> tcs, string placementId)
        {
            _tcs = tcs;
            _placementIDLabel.text = placementId;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnClose()
        {
            _tcs.SetResult(null);
            Destroy(gameObject);
        }
    }

}
