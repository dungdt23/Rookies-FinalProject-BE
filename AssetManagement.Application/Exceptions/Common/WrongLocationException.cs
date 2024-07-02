namespace AssetManagement.Application.Exceptions.Common
{
    public class WrongLocationException : Exception
    {
        public WrongLocationException(string message) : base(message) { }
    }
}
