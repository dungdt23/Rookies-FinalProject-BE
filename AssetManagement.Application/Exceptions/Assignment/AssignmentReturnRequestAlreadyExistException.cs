namespace AssetManagement.Application.Exceptions.Assignment
{
    public class ActiveReturnRequestAlreadyExistsException : Exception
    {
        public ActiveReturnRequestAlreadyExistsException() : base() { }
        public ActiveReturnRequestAlreadyExistsException(string message) : base(message) { }
        public ActiveReturnRequestAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}