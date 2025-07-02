using LibEntity;
using LibEntity.NetCore.Exceptions;
using LibEntity.NetCore.Infrastructure;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibWheelOfDeath
{

    // CAdmin will inherit from CEntity, and through inbuilt class library mechanisms, link together
    // the tblAccount field that this tblPlayer field is based on (in the database).

    // For the reader's information...
    // *This is not how I would have prefered to do it*. I would have simply omitted the CAccount Class
    // and had tblPlayer handle all the database inheritence logic.
    // However, after discussion with my lecturer, we agreed that reflecting the inheretance hirachy
    // in C# was required in order to meet the assessment crieteria.

    // The downside of this is that the CAdmin class will not have read/write access to any fields
    // NOT already in tblPlayer. A seperate CAccount object will be created to access them.
    public class CAdmin : CEntity
    {
        #region Constructors

        public CAdmin() : base("vwAdminWithAccount") { }

        public CAdmin(long id) : this()
        {
            Read(id);
        }

        public CAdmin(string firstName, string lastName, string username, string password) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
        }
        public CAdmin(string firstName, string lastName, string username, string password, bool isActiveFlag, long fkAdminTypeId) : this(firstName, lastName, username, password)
        {
            IsActiveFlag = isActiveFlag;
            FkAdminTypeId = fkAdminTypeId;
        }

        #endregion

        #region Table Column Properties

        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        private bool? _isActiveFlag;
        public bool IsActiveFlag
        {
            get { return _isActiveFlag ?? true; }
            set { _isActiveFlag = value; }
        }

        private long? _fkAdminTypeId;
        public long FkAdminTypeId
        {
            get { return _fkAdminTypeId ?? 0L; }
            set { _fkAdminTypeId = value; }
        }

        #endregion

        #region Table Entity Properties

        #endregion

        #region Other Properties
        public bool OverrideValidate { get; set; } //because usernames will be picked up as
                                                            //non-unique when Update() gets called if we do this.

        #endregion

        #region CRUDS

        public override void Create()
        {
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
                )

                declare @pNewId bigint = SCOPE_IDENTITY();

                insert into [tblAdmin]
                (
                    [Id], 
                    [FkAdminTypeId], 
                    [Username]
                )
                values
                (   
                    @pNewId, 
                    @pFkAdminTypeId, 
                    @pUsername     
                );
                select @pNewId
            ";
            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);
            Parameters.AddWithValue("@pFkAdminTypeId", FkAdminTypeId);

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
            ;
            update
                [tblAdmin]
            set
                [Username] = @pUsername,
                [FkAdminTypeId] = @pFkAdminTypeId
            where
                Id = @pId
            ;";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);
            Parameters.AddWithValue("@pFkAdminTypeId", FkAdminTypeId);

            return base.Update();
        }


        public override List<IEntity> Search()
        {
            string fromClause = "vwAdminWithAccount";
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
            // Likewise with admin type id
            if (_fkAdminTypeId.HasValue)
            {
                whereClause += "and FkAdminTypeId = @pFkAdminTypeId ";
                Parameters.AddWithValue("@pFkAdminTypeId", FkAdminTypeId);
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

        public List<CAdmin> GetAllAdmins()
        {
            CommandText = $@" select  A.*, AC.*  from [tblAdmin] A inner join [tblAccount] AC on AC.Id = A.Id";
            List<IEntity> searchResults = Search();
            List<CAdmin> admins = new List<CAdmin>();
            foreach (IEntity entity in searchResults)
            {
                CAdmin admin = (CAdmin)entity;
                admins.Add(admin);
            }
            return admins;
        }

        /// <summary>
        /// Attempt to match this entity's username property
        /// to a matching account in the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool UsernameExists()
        {
            // make a new CAdmin enitity -- there may be properties set in this instance
            // that affect a search result.
            CAdmin admin = new CAdmin() { Username = this.Username };
            return UniqueMatch(admin) > 0;
        }

        /// <summary>
        /// Return an ID (opposed to 0L) only if the argued <see cref="CAdmin"/> 
        /// matches with a SINGLE database record.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public long UniqueMatch(CAdmin admin)
        {
            // search for the admin
            List<IEntity> matches = admin.Search();

            switch (matches.Count)
            {
                case 0: // no matches
                    return 0L;

                case 1: 
                    return admin.Id;

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
            CAdmin searchAdmin = new CAdmin
            {
                Username = this.Username,
                Password = this.Password
            };

            List<IEntity> matches = searchAdmin.Search();

            if (matches.Count == 1)
            {
                CAdmin foundAdmin = (CAdmin)matches[0];
                return foundAdmin.Id;
            }
            return 0L;
        }

        #endregion




        #region Other Methods

        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CAdmin admin = (CAdmin?)entity ?? new CAdmin();

            admin.Id = (long)reader["Id"];
            admin.FkAdminTypeId = (long)reader["FkAdminTypeId"];
            admin.Username = (string)reader["Username"];
            admin.FirstName = (string)reader["FirstName"];
            admin.LastName = (string)reader["LastName"];
            admin.Password = (string)reader["Password"];
            admin.IsActiveFlag = (bool)reader["IsActiveFlag"];

            return admin;
        }

        public override void Reset()
        {
            Id = 0L;
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
            IsActiveFlag = true;
            FkAdminTypeId = 0L; 
        }


        public override void Validate()
        {

            CValidator<CAdmin> validator = new(this);

            // validator.NoDefaults();


            if (UsernameExists() && !OverrideValidate)
            {
                validator.ManualAddFailure(
                    LibEntity.NetCore.Exceptions.EnumValidationFailure.FailsUnique, 
                    $"The {nameof(Username)} \"{Username}\" is already taken\n");
            }

            if (Password.Length < 12) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must be more than 12 characters long");

            if (!Password.Any(char.IsUpper)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one uppercase letter");

            if (!Password.Any(char.IsLower)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one lowercase letter");

            if (!Password.Any(char.IsDigit)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one number");

            if (!Password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>/?".Contains(ch))) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one special character");


            validator.Validate();
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.FirstName} {this.LastName} -- \"{this.Username}\" ({this.Password}). Account {(IsActiveFlag ? "active" : "inactive")}";
        }

        #endregion
    }
}

