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
        private static string _storagePath = $"{Application.persistentDataPath}/KilnIAPData.json";

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
            [System.Serializable]
            public struct PendingPurchase
            {
                public string ProductID;
                public string PurchaseToken;

                public PendingPurchase(string productID, string purchaseToken)
                {
                    ProductID = productID;
                    PurchaseToken = purchaseToken;
                }
            }

            public string[] Purchases;
            public string[] Owned;
            public PendingPurchase[] NonConsumed;
        }

        private List<Product> _products = new List<Product>();
        public List<Product> Products { get { return _products; } }
        private List<Product> _ownedProducts = new List<Product>();
        private List<Purchase> _purchases = new List<Purchase>();
        public List<Purchase> Purchases { get { return _purchases; } }
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
            if (File.Exists(_storagePath))
            {
                _state = JsonUtility.FromJson<IAPState>(File.ReadAllText(_storagePath));
                foreach (string purchaseID in _state.Purchases)
                {
                    Purchase p = new Purchase();
                    p.ProductID = purchaseID;

                    _purchases.Add(p);
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
                    p.ProductID = pending.ProductID;
                    p.PurchaseToken = pending.PurchaseToken;

                    _nonConsumedPurchases.Add(p);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            // Fill out the serializable IAPState class with the current state
            _state.Purchases = new string[_purchases.Count];
            for (int i = 0; i < _purchases.Count; i++)
            {
                _state.Purchases[i] = _purchases[i].GetProductID();
            }

            _state.Owned = new string[_ownedProducts.Count];
            for (int i = 0; i < _ownedProducts.Count; i++)
            {
                _state.Owned[i] = _ownedProducts[i].GetProductID();
            }

            _state.NonConsumed = new IAPState.PendingPurchase[_nonConsumedPurchases.Count];
            for (int i = 0; i < _nonConsumedPurchases.Count; i++)
            {
                _state.NonConsumed[i] = new IAPState.PendingPurchase(_nonConsumedPurchases[i].GetProductID(), _nonConsumedPurchases[i].GetPurchaseToken());
            }
            File.WriteAllText(_storagePath, JsonUtility.ToJson(_state));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Reset()
        {
            if (File.Exists(_storagePath))
            {
                File.Delete(_storagePath);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void ProcessCompletedPurchase(Purchase p)
        {
            _nonConsumedPurchases.Add(p);
            _purchases.Add(p);

            Save();
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
        /// <returns></returns>
        public List<string> GetNonConsumedIDs()
        {
            List<string> ids = new List<string>();

            foreach (Purchase p in _nonConsumedPurchases)
            {
                ids.Add(p.GetProductID());
            }

            return ids;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public Purchase GetNonConsumedPurchase(string productID)
        {
            foreach (Purchase p in _nonConsumedPurchases)
            {
                if (p.GetProductID() == productID) return p;
            }

            throw new Kiln.Exception($"No non consumed IAP with id {productID} found");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Task<Purchase> PurchaseProduct(string productID, string payload)
        {
            Product p = GetProduct(productID);

            var aTcs = new TaskCompletionSource<Purchase>();

            // If it's a non consumable, we'll check if it's already owned
            bool showPurchaseWindow = true;
            if (p.GetProductType() == Product.ProductType.NON_CONSUMABLE)
            {
                foreach (Purchase aux in _purchases)
                {
                    if (aux.GetProductID() == productID)
                    {
                        aTcs.SetException(new Kiln.Exception($"Product {p.GetProductID()} is a non consumable and already owned."));
                        showPurchaseWindow = false;
                        break;
                    }
                }
            }

            if (showPurchaseWindow)
            {
                IAPController controller = MonoBehaviour.Instantiate(IAPPrefab);
                controller.Show(aTcs, productID, p.GetPrice(), payload);

                System.Action<Task<Purchase>> purchaseTracker = async (Task<Purchase> t) =>
                {
                    Purchase purchase = await t;
                    ProcessCompletedPurchase(purchase);
                };

                purchaseTracker(aTcs.Task);
            }

            return aTcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseToken"></param>
        public void ConsumePurchasedProduct(string purchaseToken)
        {
            bool done = false;
            foreach (Purchase p in _nonConsumedPurchases)
            {
                if (p.GetPurchaseToken() == purchaseToken)
                {
                    _nonConsumedPurchases.Remove(p);
                    _ownedProducts.Add(GetProduct(p.GetProductID()));

                    done = true;
                    break;
                }
            }

            if (!done)
            {
                throw new Kiln.Exception($"No pending purchase with a {purchaseToken} token found.");
            }

            Save();
        }

        #endregion
    }
}

#endif