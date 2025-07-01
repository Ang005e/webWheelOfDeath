namespace LibEntity.NetCore.Exceptions
{
    public class CEntityException : ApplicationException
    {
        public CEntityException() { }

        public CEntityException(string? message) : base(message) { }

        public CEntityException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
