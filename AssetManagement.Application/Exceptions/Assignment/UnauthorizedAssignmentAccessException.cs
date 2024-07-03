namespace AssetManagement.Application.Exceptions.Assignment
{
    public class UnauthorizedAssignmentAccessException : Exception
    {
        public UnauthorizedAssignmentAccessException() : base() { }
        public UnauthorizedAssignmentAccessException(string message) : base(message) { }
        public UnauthorizedAssignmentAccessException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
