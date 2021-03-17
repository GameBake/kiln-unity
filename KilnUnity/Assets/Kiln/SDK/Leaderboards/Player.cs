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
        private string _id;
        public string ID { set { _id = value;  } }
        private string _name;
        public string Name { set { _name = value; } }
        private string _photoURL;
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