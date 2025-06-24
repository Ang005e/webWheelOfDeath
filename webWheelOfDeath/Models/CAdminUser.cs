using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CAdminUser : CAdminCredentials
    {
        public string txtAdminFirstName { get; set; } = string.Empty;
        public string txtAdminLastName { get; set; } = string.Empty;
        public long adminTypeId { get; set; }
        public bool isActive { get; set; } = true;

        public CAdminUser (long Id)
        {
            CAdmin admin = new CAdmin(Id);
            txtAdminFirstName = admin.FirstName;
            txtAdminLastName = admin.LastName;  
            txtAdminUsername = admin.Username;
            txtAdminPassword = admin.Password;
            isActive = admin.IsActiveFlag;
            admin.Create();
        }

        public void Register()
        {
            CAdmin admin = new(
                txtAdminFirstName,
                txtAdminLastName,
                txtAdminUsername,
                txtAdminPassword,
                isActive,
                adminTypeId
            );
            admin.Create();
        }
        public void Edit()
        {

        }
    }
}
