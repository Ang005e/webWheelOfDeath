using Azure.Identity;
using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    /// <summary>
    /// Exposes appropriate API functions required by the web controller
    /// 
    /// </summary>
    public class CPlayerCredentials
    {
        // ToDo: On login, initialise, and do Authenticate automatically -- set txtLoginSuccess property.
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

            CPlayer player = new CPlayer()
            {
                Username = this.Username,
                Password = this.Password,
            };
            loginAttemptFailed = !player.Authenticate();
            Id = player.Id;
        }
    }
}
