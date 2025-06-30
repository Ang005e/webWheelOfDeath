using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CAdminCredentials
    {
        // ToDo: On login, initialise and do Authenticate automatically -- set txtLoginSuccess.
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool loginAttemptFailed { get; set; } = false;
        public long Id { get; set; } = 0L;

        public void Authenticate()
        {
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

            // after Authenticate(), search for the admin
            // to get their ID, because Authenticate() just validates credentials
            if (!loginAttemptFailed)
            {
                // Search for admin by username to get ID
                admin.Username = Username;
                admin.Search();

                // Only set Id if admin is found
                if (admin.Id > 0)
                {
                    Id = admin.Id;
                }
                else
                {
                    loginAttemptFailed = true;
                }
            }
        }
    }
}
