
using LibEntity.NetCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace LibEntity.NetCore.Exceptions
{
    public class CEntityValidationException : CEntityException
    {
        public CEntityValidationException() : base() { }
        public CEntityValidationException(CValidatorFailureBuilder bd) : base(bd.Build())
        {
            
        }
        public CEntityValidationException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public enum EnumValidationFailure
    {
        None = 0,
        Required = 1,
        NotSet = 2,
        OutOfRange = 3,
        InvalidValue = 5,
        FailsUnique = 6,
        Other = 99
    }
}
