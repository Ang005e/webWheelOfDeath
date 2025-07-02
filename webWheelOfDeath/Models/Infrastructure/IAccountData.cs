using webWheelOfDeath.Exceptions;

namespace webWheelOfDeath.Models.Infrastructure
{
    public interface IAccountData : IWebCredentials
    {
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
    }
}