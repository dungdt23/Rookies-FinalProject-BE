namespace AssetManagement.Application.Exceptions.Assignment
{
    public class UnauthorizedAssignmentAccessException : Exception
    {
        public UnauthorizedAssignmentAccessException(string message) : base(message) { }
    }

}
