namespace AssetManagement.Application.Exceptions.Assignment
{
    public class ActiveReturnRequestAlreadyExistsException : Exception
    {
        public ActiveReturnRequestAlreadyExistsException(string message) : base(message) { }
    }
}