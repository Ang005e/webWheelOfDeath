using LibWheelOfDeath;
using webWheelOfDeath.Exceptions;
using webWheelOfDeath.Models.Infrastructure;
using webWheelOfDeath.Models.webWheelOfDeath.Models;

namespace webWheelOfDeath.Models
{
    public enum EnumAdminType
    {
        Admin = 1,
        SuperAdmin = 2
    }

    public class CAdminUser : CEntityModel<CAdmin>, IAdminAccountData
    {
        #region Backing Properties
        // for the sake of allowing default values initially BUT erroring out if access to an unset prop is attempted
        private string? _firstName;
        private string? _lastName;
        private string? _username;
        private string? _password;
        private bool? _isActive;
        private long? _adminTypeId;
        #endregion

        #region Public Properties
        public string FirstName
        {
            get => _firstName ?? throw new UnsetPropertyException<string>(_firstName);
            set => _firstName = value;
        }

        public string LastName
        {
            get => _lastName ?? throw new UnsetPropertyException<string>(_lastName);
            set => _lastName = value;
        }

        public string Username
        {
            get => _username ?? throw new UnsetPropertyException<string>(_username);
            set => _username = value;
        }

        public string Password
        {
            get => _password ?? throw new UnsetPropertyException<string>(_password);
            set => _password = value;
        }
        public bool IsActive
        {
            get
            {
                if (_isActive.HasValue)
                    return _isActive.Value;
                throw new UnsetPropertyException<bool?>(_isActive);
            }
            set { _isActive = value; }
        }
        public long AdminTypeId
        {
            get
            {
                if (_adminTypeId.HasValue)
                    return _adminTypeId.Value;

                throw new UnsetPropertyException<long?>(_adminTypeId);
            }
            set => _adminTypeId = value;
        }
        #endregion




        #region Constructors
        // Default constructor
        public CAdminUser() : base() { }

        // Load constructor - properly loads existing admin
        public CAdminUser(long id) : base(id) { }
        #endregion


        #region Frontend Functions
        public void Register()
        {
            var admin = BuildEntity();
            admin.Validate();
            admin.Create();

            // SYNC THE ID BACK AFTER CREATION
            this.Id = admin.Id;
            // no need to run refresh since it was just created
        }

        public bool Authenticate()
        {
            var admin = new CAdmin
            {
                Username = this.Username,
                Password = this.Password
            };

            if (admin.Authenticate(true)) 
            {
                // Load the authenticated admin's data
                this.Id = admin.Id; // SYNC THE ID
                this.Refresh(); // This will properly load all data
                return true;
            }
            return false;
        }

        public bool UsernameExists()
        {
            var admin = new CAdmin { Username = this.Username };
            return admin.UsernameExists();
        }

        public List<CAdminUser> GetAllAdmins()
        {
            var adminUsers = new List<CAdminUser>();
            var admin = new CAdmin();

            foreach (CAdmin a in admin.GetAllAdmins())
            {
                var adminUser = new CAdminUser
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Username = a.Username,
                    Password = a.Password,
                    IsActive = a.IsActiveFlag,
                    AdminTypeId = a.FkAdminTypeId
                };
                adminUsers.Add(adminUser);
            }
            return adminUsers;
        }
        #endregion



        #region Entity Mapping
        protected override void MapFromEntity(CAdmin entity)
        {
            // Id is handled by base class
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Username = entity.Username;
            Password = entity.Password;
            IsActive = entity.IsActiveFlag;
            AdminTypeId = entity.FkAdminTypeId;
        }

        protected override void MapToEntity(CAdmin entity)
        {
            // Id is handled by base class
            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.Username = Username;
            entity.Password = Password;
            entity.IsActiveFlag = IsActive;
            entity.FkAdminTypeId = AdminTypeId;
        }
        #endregion

    }
}