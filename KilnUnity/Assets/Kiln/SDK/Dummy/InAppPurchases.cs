#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Kiln
{
    /// <summary>
    /// In App Purchase Manager to mock IAP behavior for in Editor development purposes.
    /// </summary>
    public class InAppPurchases
    {
        public string storagePath = $"{Application.dataPath}/KilnIAPData.json";

        private static IAPController _iapPrefab;
        public static IAPController IAPPrefab
        {
            get
            {
                if (_iapPrefab == null)
                {
                    _iapPrefab = Resources.Load<IAPController>("KilnIAP");

                    if (_iapPrefab == null)
                    {
                        throw new System.Exception("Kiln IAP Prefab Missing");
                    }
                }

                return _iapPrefab;
            }
        }

        [System.Serializable]
        private class IAPState
        {
            public struct PendingPurchase
            {
                public string productID;
                public string purchaseToken;
            }

            public string[] Purchases;
            public string[] Owned;
            public PendingPurchase[] NonConsumed;
        }

        private List<Product> _products = new List<Product>();
        public List<Product> Products { get { return _products; } }
        private List<Product> _ownedProducts = new List<Product>();
        private List<Purchase> _purchasedProducts = new List<Purchase>();
        public List<Purchase> PurchasedProducts { get { return _purchasedProducts; } }
        private List<Purchase> _nonConsumedPurchases = new List<Purchase>();
        [SerializeField] private IAPState _state;

        public InAppPurchases()
        {
            foreach (Settings.InAppPurchase iap in Kiln.API.Settings.IAPs)
            {
                var product = new Product();
                product.ID = iap.Id;
                product.Price = iap.Price.ToString();
                product.Type = iap.Type;

                _products.Add(product);
            }

            _state = new IAPState();

            Load();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Load()
        {
            if (File.Exists(storagePath))
            {
                _state = JsonUtility.FromJson<IAPState>(File.ReadAllText(storagePath));

                foreach (string purchaseID in _state.Purchases)
                {
                    Purchase p = new Purchase();
                    p.ProductID = purchaseID;

                    _purchasedProducts.Add(p);
                }

                foreach (string productID in _state.Owned)
                {
                    Product p = new Product();
                    p.ID = productID;

                    _ownedProducts.Add(p);
                }

                foreach (IAPState.PendingPurchase pending in _state.NonConsumed)
                {
                    Purchase p = new Purchase();
                    p.ProductID = pending.productID;
                    p.PurchaseToken = pending.purchaseToken;

                    _nonConsumedPurchases.Add(p);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Product GetProduct(string id)
        {
            foreach (Product p in _products)
            {
                if (p.GetProductID() == id) return p;
            }

            throw new Kiln.Exception($"Product {id} not in the catalogue.");
        }

        #region Public API

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetProductIDs()
        {
            List<string> ids = new List<string>();

            foreach (Product p in _products)
            {
                ids.Add(p.GetProductID());
            }

            return ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Task<Purchase> PurchaseProduct(string productID, string payload)
        {
            IAPController controller = MonoBehaviour.Instantiate(IAPPrefab);

            var aTcs = new TaskCompletionSource<Purchase>();

            // TODO: Create a callback on the mockup IAPController so we can resume the processing once that's done.

            controller.Show(aTcs, productID, GetProduct(productID).GetPrice(), payload);

            return aTcs.Task;
        }

        // public Task ConsumePurchasedProduct(string purchaseToken)
        // {

        // }

        #endregion
    }
}

#endif