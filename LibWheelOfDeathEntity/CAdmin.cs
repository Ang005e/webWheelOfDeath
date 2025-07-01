using LibEntity;
using LibEntity.NetCore.Infrastructure;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CAdmin : CAccount
    {
        #region Constructors

        public CAdmin() : base("tblAdmin") { }

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
        public bool OverrideValidate { get; set; } = false; //because usernames will be picked up as
                                                            //non-unique when Update() gets called if we do this.

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public override void Create()
        {

            base.Create(); // Create the ID for the CAccount table

            // add the username...
            CommandText = $@"
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
            Parameters.AddWithValue("@pUsername", Username);
            Parameters.AddWithValue("@pFkAdminTypeId", FkAdminTypeId); 
            Parameters.AddWithValue("@pId", Id);

            Create(false); // Create the ID for THIS table, but based on the ID of the parent entity (CAccount)
        }

        public override int Update()
        {

            CommandText = $@" 
            update 
                [tblPlayer]
            set
                [Username] = @pUsername,
            where
                Id = @pId
            ";

            // Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pUsername", Username);

            return base.Update();
        }


        public override List<IEntity> Search()
        {

            string fromClause = "[tblAdmin] A ";
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


            CommandText = @$"
                select 
                    A.* 
                from
                    {fromClause}
                where
                    {whereClause}
            ";


            return base.Search(parameters);
        }


        #endregion

        #region Other Methods


        public List<CAdmin> GetAllAdmins()
        {
            CommandText = $@" select  A.*  from [tblAdmin] A";
            List<IEntity> searchResults = Search();
            List<CAdmin> admins = new List<CAdmin>();
            foreach (IEntity entity in searchResults)
            {
                CAdmin admin = (CAdmin)entity;
                admins.Add(admin);
            }
            return admins;
        }

        public override void Reset()
        {
            Id = 0L;
            Username = string.Empty;
        }

        public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CAdmin row = (CAdmin?)entity ?? new CAdmin();

            row.Id = (long)reader["Id"];
            row.FkAdminTypeId = (long)reader["FkAdminTypeId"];
            row.Username = (string)reader["Username"];

            return row;
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
        /// Attempt to match this entities username and password properties 
        /// to a matching account in the database.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool Authenticate()
        {
            CAdmin player = new CAdmin() { Username = this.Username, Password = this.Password };
            if (AccountMatches(player, true))
            {
                CAdmin ad = (CAdmin)this.Search()[0];
                Id = ad.Id;
                FkAdminTypeId = ad.FkAdminTypeId;
                return true;
            }
            return false;
        }

        public bool AccountMatches(CAdmin player, bool assertUnique)
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

                    // ToDo: LOG REPORT

                    if (assertUnique)
                    {
                        throw new CWheelOfDeathException($@"Internal data error: Player username matched multiple accounts");
                    }
                    return true;
            }
        }


        public override void Validate()
        {

            CValidator<CAdmin> validator = new(this);

            // validator.NoDefaults();
            //validator.Password;

            if (UsernameExists() && !OverrideValidate)
            {
                validator.ManualAddFailure(
                    LibEntity.NetCore.Exceptions.EnumValidationFailure.FailsUnique, 
                    $"The {nameof(Username)} \"{Username}\" is already taken\n");
            }
            validator.Validate();
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.Username}";
        }

        #endregion
    }
}

