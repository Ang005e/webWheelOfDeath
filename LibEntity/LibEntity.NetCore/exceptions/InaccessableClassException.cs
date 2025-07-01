
namespace LibEntity.NetCore.Exceptions
{
    public class InaccessableClassException : CEntityException
    {
        public InaccessableClassException() { }
        public InaccessableClassException(string? message) : base(message) { }
        public InaccessableClassException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
