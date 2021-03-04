using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kiln
{
    /// <summary>
    /// In App Purchase Helper to save some info that for the moment is unsupported in the API (like non consumed products)
    /// </summary>
    public class IAPHelper
    {
        private static IAPHelper _instance;
        public static IAPHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IAPHelper();
                }

                return _instance;
            }
        }

        private static string _storageFileName = "KilnIAPData.json";
        public static string StorageFileName { get { return _storageFileName; } }
        private static string _storagePath = $"{Application.persistentDataPath}/{_storageFileName}";
        public static string StoragePath { get { return _storagePath;  } }

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

            public PendingPurchase[] NonConsumed;
        }

        private List<IProduct> _products = new List<IProduct>();
        public List<IProduct> Products
        {
            get { return _products; }
            set { _products = value; }
        }
        private List<IPurchase> _nonConsumedPurchases = new List<IPurchase>();
        public List<IPurchase> NonConsumedPurchases { get { return _nonConsumedPurchases; } }
        [SerializeField] private IAPState _state;

        public IAPHelper()
        {
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

        #region Public API

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetProductIDs()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            List<string> ids = new List<string>();

            foreach (Product p in _products)
            {
                ids.Add(p.GetProductID());
            }

            return ids;
#else
            return Kiln.API.IAP.GetProductIDs();
#endif

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetNonConsumedIDs()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            List<string> ids = new List<string>();

            foreach (Purchase p in _nonConsumedPurchases)
            {
                ids.Add(p.GetProductID());
            }

            return ids;
#else
            return Kiln.API.IAP.GetNonConsumedIDs();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public Purchase GetNonConsumedPurchase(string productID)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            foreach (Purchase p in _nonConsumedPurchases)
            {
                if (p.GetProductID() == productID) return p;
            }

            throw new Kiln.Exception($"No non consumed IAP with id {productID} found");
#else
            return Kiln.API.IAP.GetNonConsumedPurchase(productID);
#endif
        }

#endregion
    }
}
