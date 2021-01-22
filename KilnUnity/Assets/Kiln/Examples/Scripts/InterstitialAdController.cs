using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class InterstitialAdController : MonoBehaviour
    {
        [SerializeField] private int _autoCloseTime = 5;
        [SerializeField] private Text _autoCloseLabel;

        private float _countdown;

        private void Awake()
        {
            _autoCloseLabel.text = _autoCloseTime.ToString();
            _countdown = _autoCloseTime;
        }
        
        void Update()
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
        /// 
        /// </summary>
        public void OnClose()
        {
            // TODO: Somehow notifiy Kiln API
            Destroy(gameObject);
        }
    }

}
