namespace Kiln
{
    public interface IProduct
    {
        string GetProductID();
        string GetPrice();
        ProductType GetProductType();
        string GetDescription();
        string GetImageURI();
        string GetPriceCurrencyCode();
    }

    public enum ProductType
    {
        CONSUMABLE, NON_CONSUMABLE
    }
    
    public class Product : IProduct
    {
        private string _id;
        public string ID { set { _id = value; } }
        private string _price;
        public string Price { set { _price = value; } }
        private ProductType _type;
        public ProductType Type { set { _type = value; } }

        public string GetProductID()
        {
            return _id;
        }

        public string GetPrice()
        {
            return _price;
        }

        public ProductType GetProductType()
        {
            return _type;
        }

        new public string ToString()
        {
            return $"-----\nID: {GetProductID()}\nPrice: {GetPrice()}\nType: {GetProductType()}\n";
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public string GetImageURI()
        {
            throw new System.NotImplementedException();
        }

        public string GetPriceCurrencyCode()
        {
            throw new System.NotImplementedException();
        }
    }

}