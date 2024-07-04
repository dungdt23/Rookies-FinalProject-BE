namespace AssetManagement.Application.Exceptions.Token
{
    public class WrongTokenFormatException : Exception
    {
        public WrongTokenFormatException() : base()
        {
        }

        public WrongTokenFormatException(string? message) : base(message)
        {
        }

        public WrongTokenFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
