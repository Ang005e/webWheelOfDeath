using System.ComponentModel.DataAnnotations;

namespace webWheelOfDeath.Models.ViewModels
{
    public class CredentialsViewModel
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;


        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
