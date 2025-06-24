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
        public string txtPlayerUsername { get; set; } = string.Empty;
        public string txtPlayerPassword { get; set; } = string.Empty;
        public bool loginAttemptFailed { get; set; } = false;

        public void Authenticate()
        {
            if (string.IsNullOrWhiteSpace(txtPlayerUsername) || string.IsNullOrWhiteSpace(txtPlayerPassword))
            {
                loginAttemptFailed = true;
                return;
            }

            CPlayer player = new CPlayer()
            {
                Username = this.txtPlayerUsername,
                Password = this.txtPlayerPassword,
            };
            loginAttemptFailed = !player.Authenticate();
        }
    }
}
