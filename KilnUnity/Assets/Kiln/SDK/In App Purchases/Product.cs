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
        string ToString();
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
        private string _description;
        public string Description { set { _description = value; } }
        private string _imageURI;
        public string ImageURI { set { _imageURI = value; } }
        private CurrencyCode _currencyCode;
        public CurrencyCode CurrencyCode { set { _currencyCode = value; } }

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
            return $@"Product:
                ID: {GetProductID()}
                Type: {GetProductType()}
                Price: {GetPrice()}
                Currency: {GetPriceCurrencyCode()}
                Image URI: {GetImageURI()}
                Description: {GetDescription()}";
        }

        public string GetDescription()
        {
            return _description;
        }

        public string GetImageURI()
        {
            return _imageURI;
        }

        public string GetPriceCurrencyCode()
        {
            return _currencyCode.ToString("G");
        }
    }

}