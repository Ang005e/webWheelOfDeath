using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CAdminCredentials
    {
        // ToDo: On login, initialise, and do Authenticate automatically -- set txtLoginSuccess property.
        public string txtAdminUsername { get; set; } = string.Empty;
        public string txtAdminPassword { get; set; } = string.Empty;
        public bool loginAttemptFailed { get; set; } = false;
        // ToDo: public string errorMessage = string.Empty; // could just throw an error too...

        public void Authenticate()
        {
            // ToDo: put check into js/html form instead
            if (string.IsNullOrWhiteSpace(txtAdminUsername) || string.IsNullOrWhiteSpace(txtAdminPassword))
            {
                loginAttemptFailed = true;
                return;
            }

            CAdmin admin = new CAdmin()
            {
                Username = txtAdminUsername,
                Password = txtAdminPassword,
            };
            loginAttemptFailed = !admin.Authenticate();
        }
    }
}
