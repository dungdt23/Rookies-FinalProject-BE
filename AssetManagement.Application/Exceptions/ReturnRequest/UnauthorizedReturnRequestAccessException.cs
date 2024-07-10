namespace AssetManagement.Application.Exceptions.ReturnRequest
{
    public class UnauthorizedReturnRequestAccessException : Exception
    {
        public UnauthorizedReturnRequestAccessException() : base()
        {
        }

        public UnauthorizedReturnRequestAccessException(string? message) : base(message)
        {
        }

        public UnauthorizedReturnRequestAccessException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
