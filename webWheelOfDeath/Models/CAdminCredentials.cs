using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CAdminCredentials
    {
        // ToDo: On login, initialise, and do Authenticate automatically -- set txtLoginSuccess property.
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool loginAttemptFailed { get; set; } = false;
        public long Id { get; set; } = 0L; 


        public void Authenticate()
        {
            // ToDo: put check into js/html form instead
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                loginAttemptFailed = true;
                return;
            }

            CAdmin admin = new CAdmin()
            {
                Username = Username,
                Password = Password,
            };
            loginAttemptFailed = !admin.Authenticate();
            Id = admin.Id;
        }
    }
}
