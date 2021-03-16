#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kiln
{
    public class IAPController : MonoBehaviour
    {
        [SerializeField] private Text _idLabel;
        [SerializeField] private Text _priceLabel;

        private TaskCompletionSource<IPurchase> _tcs;

        private string _productID;
        private string _developerPayload;


        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        /// <summary>
        /// The prefab is disabled by default, display it
        /// </summary>
        /// <param name="task">Task Completion Source to communicate once interstitial's done showing</param>
        public void Show(TaskCompletionSource<IPurchase> tcs, string productID, string price, string payload)
        {
            _tcs = tcs;

            _idLabel.text = productID;
            _priceLabel.text = price;

            _productID = productID;
            _developerPayload = payload;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnPurchaseButton()
        {
            var purchase = new Purchase();
            purchase.ProductID = _productID;
            purchase.PurchaseToken = System.Guid.NewGuid().ToString();
            purchase.DeveloperPayload = _developerPayload;

            _tcs.SetResult(purchase);

            Destroy(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnCancelButton()
        {
            _tcs.SetCanceled();
            Destroy(gameObject);
        }
    }

}
#endif