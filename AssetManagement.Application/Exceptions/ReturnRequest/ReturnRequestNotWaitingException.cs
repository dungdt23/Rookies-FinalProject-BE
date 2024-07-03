namespace AssetManagement.Application.Exceptions.ReturnRequest
{
    public class ReturnRequestNotWaitingException : Exception
    {
        public ReturnRequestNotWaitingException() : base() { }
        public ReturnRequestNotWaitingException(string message) : base(message) { }
        public ReturnRequestNotWaitingException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
