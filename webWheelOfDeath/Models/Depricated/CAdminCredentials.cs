using LibWheelOfDeath;
using webWheelOfDeath.Models.webWheelOfDeath.Models;

namespace webWheelOfDeath.Models.Depricated
{
    public class CAdminCredentials 
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public long AdminTypeId { get; set; } = 0L;
        public bool loginAttemptFailed { get; set; } = false;


    }
}
