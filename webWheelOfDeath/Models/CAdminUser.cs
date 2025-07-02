using LibWheelOfDeath;
using webWheelOfDeath.Exceptions;
using webWheelOfDeath.Models.Infrastructure;
using webWheelOfDeath.Models;

namespace webWheelOfDeath.Models
{
    public enum EnumAdminType
    {
        Admin = 1,
        SuperAdmin = 2
    }

    public class CAdminUser : CEntityModel<CAdmin, CAdminUser>, IAdminAccountData
    {
        #region Backing Properties
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
            get => _firstName ?? string.Empty;
            set => _firstName = value;
        }

        public string LastName
        {
            get => _lastName ?? string.Empty; 
            set => _lastName = value;
        }

        public string Username
        {
            get => _username ?? string.Empty; 
            set => _username = value;
        }

        public string Password
        {
            get => _password ?? string.Empty;
            set => _password = value;
        }

        public bool IsActive
        {
            get => _isActive ?? true;
            set => _isActive = value;
        }
        public long AdminTypeId
        {
            get => _adminTypeId ?? 0L;
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

            long matchedId = admin.Authenticate();

            if (matchedId > 0L) 
            {
                // Load the authenticated admin's account data
                this.Id = matchedId; // SYNC THE ID
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



        #region Validation
        /// <summary>
        /// Call before any database operations to ensure required fields are set
        /// </summary>
        protected override void ValidateRequiredFields(bool isUpdate)
        {
            var errors = new List<string>();

            if (_firstName == null)
                errors.Add("FirstName must be set");
            if (_lastName == null)
                errors.Add("LastName must be set");
            if (_username == null)   
                errors.Add("Username must be set");
            if (_password == null)   
                errors.Add("Password must be set");
            if (isUpdate)
            {
                if (_isActive == null)
                    errors.Add("IsActive must be set");
                if (_adminTypeId == null)
                    errors.Add("AdminType must be set");
            }

            if (errors.Any())
                throw new InvalidOperationException($"Required fields not set: {string.Join(", ", errors)}");
        }
        #endregion

    }
}