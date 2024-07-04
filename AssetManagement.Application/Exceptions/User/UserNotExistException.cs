namespace AssetManagement.Application.Exceptions.User
{
    public class UserNotExistException : Exception
    {
        public UserNotExistException() : base() { }
        public UserNotExistException(string? message) : base(message) { }
        public UserNotExistException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
