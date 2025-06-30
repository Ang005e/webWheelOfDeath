using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CAdminCredentials
    {
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

            // After Authenticate(), search for the admin to get their ID
            if (!loginAttemptFailed)
            {

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
