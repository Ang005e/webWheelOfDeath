using LibEntity;
using LibWheelOfDeath.Exceptions;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Numerics;

namespace LibWheelOfDeath
{
    // CPlayer will inherit from CEntity, and through inbuilt class library mechanisms, link together
    // the tblAccount field that this tblPlayer field is based on (in the database).

    // For the reader's information...
    // *This is not how I would have prefered to do it*. I would have simply omitted the CAccount Class
    // and had tblPlayer handle all the database inheritence logic.
    // However, after discussion with my lecturer, we agreed that reflecting the inheretance hirachy
    // in C# was required in order to meet the assessment crieteria.

    // The downside of this is that the CPlayer class will not have read/write access to any fields
    // NOT already in tblPlayer. A seperate CAccount object will be created to access them.
    public class CPlayer : CAccount
    {
        #region Constructors

        public CPlayer() : base("tblPlayer") { }

        public CPlayer(long id) : this()
        {
            Read(id);
        }

        public CPlayer(string firstName, string lastName, string username, string password) 
            : base("tblPlayer", firstName, lastName, password)
        {
            Username = username;
        }

        #endregion

        #region Table Column Properties

        public string Username { get; set; } = string.Empty;

        #endregion

        #region Table Entity Properties


        #endregion

        #region Other Properties


        #endregion

        #region CRUDS

        public new void Create()
        {
            base.Create(); // Create the CAccount table entry first. BEFORE updating this entities parameters.

            Validate();

            // Change the command text & parameters, adding the username...
            CommandText = $@"
                insert into [tblPlayer]
                (
                    [Username],
                    [Id]
                )
                values
                (          
                    @pUsername,
                    @pId
                );
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pUsername", Username);

            base.Create(false); // Create the ID for THIS table, but based on the ID of the parent entity (CAccount)
        }

        public new int Update()
        {
            
            CommandText = $@" 
            update 
                [tblPlayer]
            set
                [Username] = @pUsername,
            where
                Id = @pId
            ";

            Parameters.AddWithValue("@pId", Id);
            Parameters.AddWithValue("@pUsername", Username); 

            return base.Update();
        }


        public override List<IEntity> Search()
        {
            string fromClause = "[tblAccount] A, [tblPlayer] P";
            string whereClause = "(1 = 1) ";

            // Parameters.Clear();

            List <SqlParameter> parameters = new();

            if (Id != 0L)                                                               
            {
                whereClause += @$"and P.Id = @pId ";
                parameters.AddWithValue("@pId", this.Id);
            }

            if (!string.IsNullOrWhiteSpace(Username))
            {
                whereClause += $"and P.Username = @pUsername ";
                parameters.AddWithValue("@pUsername", $"{this.Username}");
            }

            if (FirstName.Length > 0)
            {
                whereClause += $"and A.FirstName = @pFirstName ";
                parameters.AddWithValue("@pFirstName", $"{this.FirstName}");
            }

            if (LastName.Length > 0)
            {
                whereClause += $"and A.LastName = @pLastName ";
                parameters.AddWithValue("@pLastName", $"{this.LastName}");
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                whereClause += $"and A.Password = @pPassword ";
                parameters.AddWithValue("@pPassword", $"{this.Password}");
            }

            CommandText = @$"
                select 
                    A.*, P.Username
                from 
                    {fromClause}
                where 
                    {whereClause}
            ";


            return base.Search(parameters);
        }


        #endregion

        #region Other Methods


        public new void Reset()
        {
            Id = 0L;
            Username = string.Empty;
        }

        public new LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
        {

            CPlayer row = (CPlayer?)entity ?? new CPlayer();

            row.Id = (long)reader["Id"];
            row.Username = (string)reader["Username"];

            return row;
        }

        /// <summary>
        /// Validate the data entity is correctly formatted before sending to the database.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public new void Validate()
        {
            string message = "";

            if (UsernameExists())
            {
                message += $"The {nameof(Username)} \"{Username}\" is already taken\n";
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                message += $"{nameof(Username)} must be provided\n";
            }

            if (Id < 0L)
            {
                message += $"{nameof(Id)} must be provided\n";
            }

            if (message.Length > 0)
            {
                throw new CWheelOfDeathException(message);
            }

            // ToDo: Add password validation
        }


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
            CPlayer player = new CPlayer() {Username = this.Username};
            return AccountMatches(player, true);
        }

        /// <summary>
        /// Attempt to match this entities username and password properties 
        /// to a matching account in the database.
        /// </summary>
        /// <exception cref="CWheelOfDeathException"></exception>
        public bool Authenticate()
        {
            CPlayer player = new CPlayer() { Username = this.Username, Password = this.Password };
            if (AccountMatches(player, true))
            {
                Id = this.Search()[0].Id;
                return true;
            }
            return false;
        }

        public bool AccountMatches(CPlayer player, bool assertUnique)
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

        public new string ToString()
        {
            return $"{this.Id}: {this.Username}";
        }

        #endregion
    }
}
