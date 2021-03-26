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
        private static string _storageFileName = "KilnIAPData.json";
        public static string StorageFileName { get { return _storageFileName; } }
        private static string _storagePath = $"{Application.persistentDataPath}/{_storageFileName}";
        public static string StoragePath { get { return _storagePath;  } }

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
            public PendingPurchase[] NonConsumed;
        }

        private List<IProduct> _products = new List<IProduct>();
        public List<IProduct> Products { get { return _products; } }
        private List<IPurchase> _purchases = new List<IPurchase>();
        public List<IPurchase> Purchases { get { return _purchases; } }
        private List<IPurchase> _nonConsumedPurchases = new List<IPurchase>();
        public List<IPurchase> NonConsumedPurchases { get { return _nonConsumedPurchases; } }
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
        /// Returns whether a saved file for with IAP Data id is present or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsSaved()
        {
            return File.Exists(InAppPurchases.StoragePath);
        }

        /// <summary>
        /// Returns a JSON string with an valid empty IAP State
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyState()
        {
            IAPState state = new IAPState();
            return JsonUtility.ToJson(state);
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
        private void ProcessCompletedPurchase(IPurchase p)
        {
            // Even if it's a non consumable, we'll add it to the non consumed purchases list
            // which is the list of active IAPs to return upon calling GetPurchasedProducts
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
        public Task<IPurchase> PurchaseProduct(string productID, string payload)
        {
            Product p = GetProduct(productID);

            var aTcs = new TaskCompletionSource<IPurchase>();

            bool showPurchaseWindow = true;
            foreach (Purchase aux in _nonConsumedPurchases)
            {
                if (aux.GetProductID() == productID)
                {
                    if (p.GetProductType() == ProductType.NON_CONSUMABLE) 
                    {
                        aTcs.SetException(new Kiln.Exception($"Product {p.GetProductID()} is a non consumable and already owned."));
                    }
                    else
                    {
                        aTcs.SetException(new Kiln.Exception($"Product {p.GetProductID()} is already owned and waiting for consumption."));
                    }
                    
                    showPurchaseWindow = false;
                    break;
                }
            }

            if (showPurchaseWindow)
            {
                IAPController controller = MonoBehaviour.Instantiate(IAPPrefab);
                controller.Show(aTcs, productID, p.GetPrice(), payload);

                System.Action<Task<IPurchase>> purchaseTracker = async (Task<IPurchase> t) =>
                {
                    IPurchase purchase = await t;
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
        public Task ConsumePurchasedProduct(string purchaseToken)
        {
            var aTcs = new TaskCompletionSource<object>();

            bool done = false;
            foreach (Purchase p in _nonConsumedPurchases)
            {
                if (p.GetPurchaseToken() == purchaseToken)
                {
                    if (GetProduct(p.GetProductID()).GetProductType() == ProductType.NON_CONSUMABLE) 
                    {
                        aTcs.SetException(new Kiln.Exception($"Product {p.GetProductID()} is a non consumable and can't be consumed."));
                    }
                    else
                    {
                        _nonConsumedPurchases.Remove(p);
                        aTcs.SetResult(null);

                        Save();
                    }
                    
                    done = true;
                    break;
                }
            }

            if (!done)
            {
                throw new Kiln.Exception($"No pending purchase with a {purchaseToken} token found.");
            }

            return aTcs.Task;
        }

        #endregion
    }
}
#endif