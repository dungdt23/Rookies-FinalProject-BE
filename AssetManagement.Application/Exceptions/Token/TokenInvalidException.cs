namespace AssetManagement.Application.Exceptions.Token
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException() : base() { }

        public TokenInvalidException(string message) : base(message) { }

        public TokenInvalidException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
