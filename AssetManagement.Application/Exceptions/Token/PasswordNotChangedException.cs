namespace AssetManagement.Application.Exceptions.Token
{
    public class PasswordNotChangedException : Exception
    {
        public PasswordNotChangedException() : base() { }
        public PasswordNotChangedException(string message) : base(message) { }
        public PasswordNotChangedException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
