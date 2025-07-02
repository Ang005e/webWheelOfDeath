using System.ComponentModel.DataAnnotations;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models.ViewModels
{
    public class CredentialsViewModel : IWebCredentials
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;


        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;


        // Server side use only (not lnked to the models):
        public string? LoginAttemptFailed { get; set; }
    }
}
