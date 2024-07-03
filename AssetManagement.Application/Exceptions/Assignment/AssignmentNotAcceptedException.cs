namespace AssetManagement.Application.Exceptions.Assignment
{
    public class AssignmentNotAcceptedException : Exception
    {
        public AssignmentNotAcceptedException() : base() { }
        public AssignmentNotAcceptedException(string message) : base(message) { }
        public AssignmentNotAcceptedException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
