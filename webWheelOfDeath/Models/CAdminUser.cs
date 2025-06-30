using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{

    public enum EnumAdminType
    {
        Admin = 1,
        SuperAdmin = 2
    }

    public class CAdminUser : CAdminCredentials
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public long AdminTypeId { get; set; }
        public EnumAdminType AdminType
        {
            get => (EnumAdminType)AdminTypeId;
            set => AdminTypeId = (long)value;
        }
        public bool isActive { get; set; } = true;

        public CAdminUser() { }
        public CAdminUser(long Id)
        {
            CAdmin admin = new();
            admin.Read(Id);
            FirstName = admin.FirstName;
            LastName = admin.LastName;
            Username = admin.Username;
            Password = admin.Password;
            isActive = admin.IsActiveFlag;
            AdminTypeId = admin.FkAdminTypeId;
        }

        public void Edit(long Id)
        {
            CAdmin newAdmin = new(Id);
            newAdmin.FirstName = FirstName;
            newAdmin.LastName = LastName;
            newAdmin.Username = Username;
            newAdmin.Password = Password;
            newAdmin.IsActiveFlag = isActive;
            newAdmin.FkAdminTypeId = AdminTypeId;

            newAdmin.Validate();

            newAdmin.Update();
        }

        public void Register()
        {
            CAdmin newAdmin = BuildAdmin();
            newAdmin.Validate();
            newAdmin.Create();
        }

        public bool UsernameExists()
        {
            CAdmin newAdmin = BuildAdmin();
            return newAdmin.UsernameExists();
        }

        public CAdmin BuildAdmin()
        {
            CAdmin admin = new CAdmin(
                FirstName,
                LastName,
                Username,
                Password
            );
            admin.IsActiveFlag = isActive;
            admin.FkAdminTypeId = AdminTypeId;

            return admin;
        }
    }
}
