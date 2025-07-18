﻿using System.ComponentModel.DataAnnotations;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models.ViewModels
{
    public class AccountViewModel : IAccountData
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;


        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;


        [Required, Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;


        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
