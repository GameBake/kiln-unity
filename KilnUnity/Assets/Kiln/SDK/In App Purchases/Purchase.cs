namespace Kiln
{
    public interface IPurchase
    {
        string GetDeveloperPayload();
        string GetProductID();
        string GetPurchaseToken();
        string GetPurchaseTime();
        string GetSignedRequest();
        string ToString();
    }

    public class Purchase : IPurchase
    {
        protected string _productID;
        public string ProductID { set { _productID = value; } }
        protected string _purchaseToken;
        public string PurchaseToken { set { _purchaseToken = value; } }
        protected string _developerPayload;
        public string DeveloperPayload { set { _developerPayload= value; } }

        public string GetDeveloperPayload()
        {
            return _developerPayload;
        }    

        public string GetProductID()
        {
            return _productID;
        }    

        public string GetPurchaseToken()
        {
            return _purchaseToken;
        }    

        public string GetPurchaseTime()
        {
            throw new System.NotImplementedException();
        }

        public string GetSignedRequest()
        {
            throw new System.NotImplementedException();
        }

        new public string ToString()
        {
            return $"-----\nProduct ID: {_productID}\nPurchase Token: {_purchaseToken}\nDeveloper Payload: {_developerPayload}\n";
        }
    }
}