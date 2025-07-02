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

        public string Username { get; set; }
        public long FkAdminTypeId { get; set; }
        public bool OverrideValidate { get; set; } //because usernames will be picked up as
                                                            //non-unique when Update() gets called if we do this.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }


        private bool? _isActiveFlag;
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
                insert into [tblAdmin]
                (
                    [Id], 
                    [FkAdminTypeId], 
                    [Username]
                )
                values
                (   
                    @pId, 
                    @pFkAdminTypeId, 
                    @pUsername     
                );
            ";
            Parameters.AddWithValue("@pFirstName", FirstName);
            Parameters.AddWithValue("@pLastName", LastName);
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pPassword", Password);
            Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);
            Parameters.AddWithValue("@pFkAdminTypeId", FkAdminTypeId);

            Create();
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
				[IsActiveFlag] = @pIsActiveFlag,
                
            where
                Id = @pId

            update
                [tblAdmin]
            set
                [Username] = @pUsername,
                [FkAdminTypeId] = @pFkAdminTypeId
            where
                Id = @pId
            ";

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

            string fromClause = "[tblAdmin] A inner join [tblAccount] AC on A.Id = AC.Id";
            string whereClause = "(1=1) ";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (Id != 0L)
            {
                whereClause += @$"and A.Id = @pId ";
                parameters.Add(new SqlParameter("@pId", this.Id));
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                whereClause += $"and A.Username = @pUsername ";
                parameters.Add(new SqlParameter("@pUsername", $"{this.Username}"));
            }

            if (FkAdminTypeId>0L)
            {
                whereClause += $"and A.FkAdminTypeId = @pFkAdminTypeId ";
                parameters.Add(new SqlParameter("@pFkAdminTypeId", $"{this.FkAdminTypeId}"));
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

            if (_isActiveFlag != null)
            {
                whereClause += $"and AC.IsActiveFlag = @pIsActiveFlag ";
                parameters.Add(new SqlParameter("@pIsActiveFlag", $"{this.IsActiveFlag}"));
            }



            CommandText = @$"
                select 
                    A.*, AC.*
                from
                    {fromClause}
                where
                    {whereClause}
            ";


            return base.Search(parameters);
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
        /// Attempt to match this entities username property
        /// to a matching account in the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool UsernameExists()
        {
            // make a new CAdmin enitity -- there may be properties set in this instance
            // that affect a search result.
            CAdmin player = new CAdmin() { Username = this.Username };
            return AccountMatches(player, true);
        }

        /// <summary>
        /// ToDo: assertUnique paramater causes inconsistant and untrackable behaviour; 
        /// need some kind of warning that many accounts were picked up.
        /// (it's also kind of redundant).
        /// </summary>
        /// <param name="player"></param>
        /// <param name="assertUnique"></param>
        /// <returns></returns>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool AccountMatches(CAdmin admin, bool assertUnique = true)
        {
            // search for the admin
            List<IEntity> matches = admin.Search();

            switch (matches.Count)
            {
                case 0: // no matching credentials
                    return false;

                case 1: // matching credentials
                        // Copy the found Id back to the admin object
                    //var foundAdmin = (CAdmin)matches[0];
                    //admin.Id = foundAdmin.Id;
                    //admin.FirstName = foundAdmin.FirstName;
                    //admin.LastName = foundAdmin.LastName;
                    //admin.IsActiveFlag = foundAdmin.IsActiveFlag;
                    //admin.FkAdminTypeId = foundAdmin.FkAdminTypeId;
                    return true;

                default: // multiple matches
                    if (assertUnique)
                    {
                        throw new CWheelOfDeathException($@"Internal data error: Admin username matched multiple accounts");
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
            CAdmin admin = new CAdmin() { Username = this.Username, Password = this.Password };
            if (AccountMatches(admin, true))
            {
                if (updateInstanceOnSuccess)
                {
                    Read(admin.Id);     // read the matching admin details from the database, since this instance 
                                        // will not have other account details set.
                }
                return true;
            }
            return false;
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

            if (Password.Length <= 12) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must be more than 12 characters long");

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

