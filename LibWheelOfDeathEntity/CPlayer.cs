using LibEntity;
using LibEntity.NetCore.Exceptions;
using LibEntity.NetCore.Infrastructure;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;

namespace LibWheelOfDeath
{
    public class CPlayer : CEntity
    {
        #region Constructors

        public CPlayer() : base("vwPlayerWithAccount") { }

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

        public string Username { get; set; }
        public bool OverrideValidate { get; set; } = false; //because usernames will be picked up as
                                                            //non-unique when Update() gets called if we do this.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }


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

        public override void Create()
        {
            // Insert into base table first, then get generated ID, then
            // use to insert into child table with the same ID, then return the ID.
            CommandText = $@"
            insert into 
                [tblAccount] 
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
        
            declare @pNewId bigint = SCOPE_IDENTITY();
        
            insert into 
                [tblPlayer] 
            (
                Id, 
                Username
            )
            values 
            (
                @pNewId, 
                @pUsername
            );
        
            select @pNewId;
        ";

            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);

            base.Create(false);
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblAccount]

            set
                [FirstName] = @pFirstName,
				[LastName] = @pLastName,
				[Password] = @pPassword,
				[IsActiveFlag] = @pIsActiveFlag
                
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
            string fromClause = "vwPlayerWithAccount"; 
            string whereClause = "(1=1) ";

            // Clear parameters first to avoid pollution
            Parameters.Clear();

            if (Id != 0L)
            {
                whereClause += "and Id = @pId ";
                Parameters.AddWithValue("@pId", this.Id);
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                whereClause += "and Username = @pUsername ";
                Parameters.AddWithValue("@pUsername", this.Username);
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                whereClause += "and Password = @pPassword ";
                Parameters.AddWithValue("@pPassword", this.Password);
            }

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                whereClause += "and FirstName = @pFirstName ";
                Parameters.AddWithValue("@pFirstName", this.FirstName);
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                whereClause += "and LastName = @pLastName ";
                Parameters.AddWithValue("@pLastName", this.LastName);
            }

            // Only add IsActiveFlag condition if explicitly set
            if (_isActiveFlag.HasValue)
            {
                whereClause += "and IsActiveFlag = @pIsActiveFlag ";
                Parameters.AddWithValue("@pIsActiveFlag", this.IsActiveFlag);
            }

            CommandText = $@"
            select 
                * 
            from 
                {fromClause}
            where 
                {whereClause}
            ";

            return base.Search();
        }


        #endregion




        #region Query Helpers

        /// <summary>
        /// Attempt to match this entity's username property
        /// to a matching account in the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool UsernameExists()
        {
            CPlayer player = new CPlayer() { Username = this.Username };
            return UniqueMatch(player) > 0;
        }

        /// <summary>
        /// Return an ID (opposed to 0L) only if the argued <see cref="CPlayer"/> 
        /// matches with a SINGLE database record.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public long UniqueMatch(CPlayer player)
        {
            // search for the admin
            List<IEntity> matches = player.Search();

            switch (matches.Count)
            {
                case 0: // no matches
                    return 0L;

                case 1:
                    return player.Id;

                default: // multiple matches
                    return 0L;
            }
        }

        /// <summary>
        /// Attempt to match this entities username and password properties 
        /// to a matching account in the database.
        /// Returns the ID of the matched instance, or 0L if no match is found.
        /// Multiple matches are impossible.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public long Authenticate()
        {
            CPlayer searchPlayer = new CPlayer
            {
                Username = this.Username,
                Password = this.Password
            };

            List<IEntity> matches = searchPlayer.Search();

            if (matches.Count == 1)
            {
                CPlayer foundAdmin = (CPlayer)matches[0];
                return foundAdmin.Id;
            }
            return 0L;
        }

        public (string username, long elapsedTime) GetFastestPlayer()
        {
            string sql = @"
                select top(1) R.ElapsedTime, P.Username
                from tblPlayer P
                inner join tblGameRecord R on P.Id = R.FkPlayerId
                order by R.ElapsedTime asc
            ";

            DataTable table = sql.Fetch<DataTable>();
            if (table.Rows.Count == 0)
                return (string.Empty, 0L);

            DataRow row = table.Rows[0];
            string username = row["Username"].ToString() ?? string.Empty;
            long elapsedTime = row["ElapsedTime"] is DBNull ? 0L : Convert.ToInt64(row["ElapsedTime"]);
            return (username, elapsedTime);
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
