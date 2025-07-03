using LibWheelOfDeath;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using webWheelOfDeath.Exceptions;
using webWheelOfDeath.Models.Infrastructure;
using webWheelOfDeath.Models;



// CAdminCredentials and CPlayerCredentials were originally classes seperated because i
// didnt need full account details for a simple login.
// But instead, I'll make a CGamePlayer class -- a builder, with components for login and account details.
// class with seperate components (ILoginCredentials and IAccountDetails)
// use the interfaces in the controller!!! The page model
// litterally only needs to know the strings of username and password, so don't
// give it any more, just cast to a new instance of the correct interface!

namespace webWheelOfDeath.Models
{
    // relies on the principle that we store the player's ID server-side after authentication, otherwise we can't subsequently find the player.
    public class CGameUser : CEntityModel<CPlayer>, IAccountData
    {
        #region Backing Properties
        private string? _firstName;
        private string? _lastName;
        private string? _username;
        private string? _password;
        private bool? _isActive;
        #endregion

        #region Public Properties
        //public string FirstName
        //{
        //    get => _firstName ?? string.Empty;
        //    set => _firstName = value;
        //}
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

        //public bool IsActive
        //{
        //    get => _isActive ?? true;
        //    set => _isActive = value;
        //}
        public bool IsActive
        {
            get
            {
                DebugTrace.Enter($"CGameUser.IsActive.get");
                try
                {
                    return _isActive ?? true;
                }
                finally
                {
                    DebugTrace.Exit($"CGameUser.IsActive.get");
                }
            }
            set => _isActive = value;
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
            player.Validate(); // validate password, etc.
            player.Create();
            // Sync the generated Id back
            this.Id = player.Id;
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

            long matchedId = player.Authenticate();

            if (matchedId > 0L) // Update so we get the ID 
            {
                Id = matchedId; // SYNC THE ID
                this.Refresh(); // Refresh for other account details than just credentials

                // check their account is active
                if (player.IsActiveFlag == false) throw new AuthenticationFailureException("This account has been deactivated");

                return true;
            }
            else throw new AuthenticationFailureException("Invalid username or password");
        }

        public bool UsernameExists()
        {
            var player = new CPlayer { Username = this.Username };
            return player.UsernameExists();
        }
        public List<CGameUser> GetAllPlayers()
        {
            var players = new List<CGameUser>();
            var player = new CPlayer();

            foreach (CPlayer p in player.FetchAll())
            {
                CGameUser mapped = new CGameUser();
                mapped.MapFromEntity(p);
                mapped.Id = p.Id;
                players.Add(mapped);
            }
            return players;
        }
        public (string, long) GetFastestPlayerForGame(long gameId)
        {
            return new CPlayer().GetFastestPlayerForGame(gameId);
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
                    errors.Add("IsActive must be set when updating");
            }

            if (errors.Any())
                throw new InvalidOperationException($"Required fields not set: {string.Join(", ", errors)}");
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
