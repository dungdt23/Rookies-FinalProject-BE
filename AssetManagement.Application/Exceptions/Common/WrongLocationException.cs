namespace AssetManagement.Application.Exceptions.Common
{
    public class WrongLocationException : Exception
    {
        public WrongLocationException() : base() { }
        public WrongLocationException(string message) : base(message) { }
        public WrongLocationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
