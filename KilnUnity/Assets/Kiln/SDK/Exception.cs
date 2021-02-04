namespace Kiln
{
    public class Exception : System.Exception
    {
        private static readonly string DefaultMessage = "Exception.";

        public Exception() : base(DefaultMessage) { }
        public Exception(string message) : base(message) { }
        public Exception(string message, System.Exception innerException): base(message, innerException) { }    
    }
}