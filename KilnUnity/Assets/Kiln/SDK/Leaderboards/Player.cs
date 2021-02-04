namespace Kiln
{
    public interface IPlayer
    {
        string GetId();
        string GetName();
        string GetPhotoURL();
    }

    public class Player : IPlayer
    {
        protected string _id;
        public string ID { set { _id = value;  } }
        protected string _name;
        public string Name { set { _name = value; } }
        protected string _photoURL;
        public string PhotoURL { set { _photoURL = value; } }

        public string GetId()
        {
            return _id;
        }
        public string GetName()
        {
            return _name;
        }
        public string GetPhotoURL()
        {
            return _photoURL;
        }
    }
}