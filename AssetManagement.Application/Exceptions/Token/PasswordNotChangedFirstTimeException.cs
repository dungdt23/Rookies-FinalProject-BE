namespace AssetManagement.Application.Exceptions.Token
{
    public class PasswordNotChangedFirstTimeException : Exception
    {
        public PasswordNotChangedFirstTimeException() : base() { }
        public PasswordNotChangedFirstTimeException(string message) : base(message) { }
        public PasswordNotChangedFirstTimeException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
