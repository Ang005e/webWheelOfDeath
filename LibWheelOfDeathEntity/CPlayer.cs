using LibEntity;
using LibEntity.NetCore.Exceptions;
using LibEntity.NetCore.Infrastructure;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace LibWheelOfDeath
{
    public class CPlayer : CEntity
    {
        #region Constructors

        public CPlayer() : base("tblPlayer") { }

        public CPlayer(long id) : this()
        {
            Read(id);
        }

        public CPlayer(string firstName, string lastName, string username, string password, bool? isActive = null) 
            : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Username = username;
            _isActiveFlag = isActive;
        }

        #endregion

        #region Table Column Properties

        public string Username { get; set; } = string.Empty;
        public bool OverrideValidate { get; set; } = false; //because usernames will be picked up as
                                                            //non-unique when Update() gets called if we do this.
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        internal bool? _isActiveFlag { get; private set; } // can be set null from inside the class, not from outside
                                                           // this essentially makes 'null' the default value--which cannot
                                                           // be re-set once a value is assigned.
        public bool IsActiveFlag
        {
            get { return _isActiveFlag ?? true; }
            set { _isActiveFlag = value; }
        }

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public new void Create()
        {
            CommandText = $@"
                insert into tblAccount
                (
                    [FirstName], 
                    [LastName], 
                    [Password], 
                    [IsActiveFlag]
                )
                values
                (
                    @pFirstName, 
                    @pLastName, 
                    @pPassword, 
                    @pIsActiveFlag
                );
                go
                insert into [tblPlayer]
                (
                    [Id], 
                    [Username]
                )
                values
                (   
                    @pId, 
                    @pUsername     
                );
            ";
            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);

            Create();
        }

        public new int Update()
        {

            CommandText = $@" 
            update 
                [tblAccount]

            set
                [FirstName] = @pFirstName,
				[LastName] = @pLastName,
				[Password] = @pPassword,
				[IsActiveFlag] = @pIsActiveFlag,
                
            where
                Id = @pId

            update
                [tblPlayer]
            set
                [Username] = @pUsername
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);

            return base.Update();
        }


        public override List<IEntity> Search()
        {
            string fromClause = "[tblPlayer] P inner join [tblAccount] AC on P.Id = AC.Id";
            string whereClause = "(1=1) ";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (Id != 0L)
            {
                whereClause += @$"and P.Id = @pId ";
                parameters.Add(new SqlParameter("@pId", this.Id));
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                whereClause += $"and P.Username = @pUsername ";
                parameters.Add(new SqlParameter("@pUsername", $"{this.Username}"));
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                whereClause += $"and AC.Password = @pPassword ";
                parameters.Add(new SqlParameter("@pPassword", $"{this.Password}"));
            }

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                whereClause += $"and AC.FirstName = @pFirstName ";
                parameters.Add(new SqlParameter("@pFirstName", $"{this.LastName}"));
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                whereClause += $"and AC.LastName = @pLastName ";
                parameters.Add(new SqlParameter("@pLastName", $"{this.LastName}"));
            }

            if (IsActiveFlag != null)
            {
                whereClause += $"and AC.IsActiveFlag = @pIsActiveFlag ";
                parameters.Add(new SqlParameter("@pIsActiveFlag", $"{this.IsActiveFlag}"));
            }



            CommandText = @$"
                select 
                    P.*, AC.*
                from
                    {fromClause}
                where
                    {whereClause}
            ";


            return base.Search(parameters);
        }


        #endregion




        #region Helpers

        /// <summary>
        /// Attempt to match this entities username property
        /// to a matching account in the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool UsernameExists()
        {
            // make a new CPlayer enitity -- there may be properties set in this instance
            // that affect a search result.
            CPlayer player = new CPlayer() { Username = this.Username };
            return AccountMatches(player, true);
        }

        public bool AccountMatches(CPlayer player, bool assertUnique = true)
        {
            // search for the player
            List<IEntity> matches = player.Search();

            switch (matches.Count)
            {
                case 0: // no matching credentials
                    return false;

                case 1: // matching credentials
                    return true;

                default: // impossible situation, but nonetheless...

                    // ToDo: LOG ERROR

                    if (assertUnique)
                    {
                        throw new CWheelOfDeathException($@"Internal data error: Player username matched multiple accounts");
                    }
                    return true;
            }
        }

        /// <summary>
        /// Attempt to match this entities username and password properties 
        /// to a matching account in the database.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool Authenticate(bool updateInstanceOnSuccess = true)
        {
            CPlayer player = new CPlayer() { Username = this.Username, Password = this.Password };
            if (AccountMatches(player, true))
            {
                if (updateInstanceOnSuccess)
                {
                    Read(player.Id);     // read the matching admin details from the database, since this instance 
                                         // will not have other account details set.
                }
                return true;
            }
            return false;
        }

        #endregion





        #region Other Methods


        public override void Reset()
        {
            Id = 0L;
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
            _isActiveFlag = null;
        }


        public override IEntity Populate(SqlDataReader reader, IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CPlayer player = (CPlayer?)entity ?? new CPlayer();

            player.Id = (long)reader["Id"];
            player.Username = (string)reader["Username"];
            player.FirstName = (string)reader["FirstName"];
            player.LastName = (string)reader["LastName"];
            player.Password = (string)reader["Password"];
            player.IsActiveFlag = (bool)reader["IsActiveFlag"];

            return player;
        }

        /// <summary>
        /// Validate the data entity is correctly formatted before sending to the database.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public override void Validate()
        {

            CValidator<CPlayer> validator = new(this);

            // validator.NoDefaults();


            if (UsernameExists() && !OverrideValidate)
            {
                validator.ManualAddFailure(
                    LibEntity.NetCore.Exceptions.EnumValidationFailure.FailsUnique,
                    $"The {nameof(Username)} \"{Username}\" is already taken\n");
            }

            if (Password.Length <= 12) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must be more than 12 characters long");

            if (!Password.Any(char.IsUpper)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one uppercase letter");

            if (!Password.Any(char.IsLower)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one lowercase letter");

            if (!Password.Any(char.IsDigit)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one number");

            if (!Password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>/?".Contains(ch))) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one special character");


            validator.Validate();
        }


        public override string ToString()
        {
            return $"{this.Id}: {this.Username}";
        }

        #endregion
    }
}
