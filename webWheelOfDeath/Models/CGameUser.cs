using LibWheelOfDeath;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using webWheelOfDeath.Exceptions;
using webWheelOfDeath.Models.Infrastructure;
using webWheelOfDeath.Models.webWheelOfDeath.Models;



// CAdminCredentials and CPlayerCredentials were originally classes seperated because i
// didnt need full account details for a simple login.
// But instead, I'll make a CGamePlayer class -- a builder, with components for login and account details.
// class with seperate components (ILoginCredentials and IAccountDetails)
// use the interfaces in the controller!!! The page model
// litterally only needs to know the strings of username and password, so don't
// give it any more, just cast to a new instance of the correct interface!

namespace webWheelOfDeath.Models
{
    public class CGameUser : CEntityModel<CPlayer>, IAccountData
    {

        #region Backing Properties
        // for the sake of allowing default values initially BUT erroring out if access to an unset prop is attempted

        private string? _firstName;
        private string? _lastName;
        private string? _username;
        private string? _password;
        private bool? _isActive;
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
        #endregion


        #region Constructors

        public CGameUser() : base() { }
        public CGameUser(long id) : base(id) { }

        #endregion


        #region Frontend Functions
        public void Register()
        {
            var player = BuildEntity();
            player.Validate();
            player.Create();

            // SYNC THE ID BACK AFTER CREATION
            this.Id = player.Id;
            // no need to run refresh since it was just created
        }
        public bool Authenticate()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                // this can't succeed
                return true;
            }

            var player = new CPlayer
            {
                Username = this.Username,
                Password = this.Password
            };

            if (player.Authenticate(false)) // Don't update, its  a temp instance
            {
                this.Id = player.Id;
                this.Refresh(); // Refresh for other account details than just credentials
                return false;
            }
            else return true;
        }

        public bool UsernameExists()
        {
            var player = new CPlayer { Username = this.Username };
            return player.UsernameExists();
        }
        #endregion


        #region Entity Mapping
        protected override void MapFromEntity(CPlayer entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Username = entity.Username;
            Password = entity.Password;
            IsActive = entity.IsActiveFlag;
        }

        protected override void MapToEntity(CPlayer entity)
        {
            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.Username = Username;
            entity.Password = Password;
            entity.IsActiveFlag = IsActive;
        }
        #endregion 
    }
}


//public class CGameUser : CPlayerCredentials
//{
//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;

//    public void Register()
//    {
//        CPlayer newPlayer = BuildPlayer();

//        newPlayer.Validate();

//        newPlayer.Create();
//    }

//    public bool UsernameExists()
//    {
//        CPlayer newPlayer = BuildPlayer();
//        return newPlayer.UsernameExists();
//    }

//    public CPlayer BuildPlayer()
//    {
//        return new CPlayer(
//            FirstName,
//            LastName,
//            Username,
//            Password
//        );
//    }
//}
