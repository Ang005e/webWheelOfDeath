using LibEntity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWheelOfDeath
{

    // CAdminType will inherit from CEntity, and through inbuilt class library mechanisms, link together
    // the tblAccount field that this tblPlayer field is based on (in the database).

    // For the reader's information...
    // *This is not how I would have prefered to do it*. I would have simply omitted the CAccount Class
    // and had tblPlayer handle all the database inheritence logic.
    // However, after discussion with my lecturer, we agreed that reflecting the inheretance hirachy
    // in C# was required in order to meet the assessment crieteria.

    // The downside of this is that the CAdminType class will not have read/write access to any fields
    // NOT already in tblPlayer. A seperate CAccount object will be created to access them.
    //public class CAdminType : CAccount
    //{
    //    #region Constructors

    //    // TODO: Is this class really needed? why represent it? there's only 2 rows in that table for now!
    //    public CAdminType() : base("tblAdminType") { }

    //    public CAdminType(long id) : this()
    //    {
    //        Read(id);
    //    }

    //    #endregion

    //    #region Table Column Properties

    //    public string Username { get; set; } = string.Empty;

    //    #endregion

    //    #region Table Entity Properties


    //    #endregion

    //    #region Other Properties


    //    #endregion

    //    #region CRUDS

    //    public override void Create()
    //    {     
    //        // add the username...
    //        CommandText = $@"
    //            insert into [tblAdminType]
    //            (
    //                [Username]
    //            )
    //            values
    //            (          
    //                @pUsername     
    //            );
    //        ";
    //        Parameters.AddWithValue("@pUsername", Username);

    //        base.Create(); // Create the ID for the CAccount table

    //        Create(false); // Create the ID for THIS table, but based on the ID of the parent entity (CAccount)
    //    }

    //    public override int Update()
    //    {
            
    //        CommandText = $@" 
    //        update 
    //            [tblPlayer]
    //        set
    //            [Username] = @pUsername,
    //        where
    //            Id = @pId
    //        ";

    //        Parameters.AddWithValue("@pId", Id);
    //        Parameters.AddWithValue("@pUsername", Username); 

    //        return base.Update();
    //    }


    //    public override List<IEntity> Search()
    //    {

    //        string fromClause = "[tblPlayer] T ";
    //        string whereClause = "";


    //        if (Id != 0L)                                                               
    //        {
    //            whereClause += @$"T.Id = @pId ";
    //            Parameters.Add(new SqlParameter("@pId", this.Id));
    //        }

    //        if (!string.IsNullOrWhiteSpace(Username))
    //        {
    //            whereClause += $"and T.Username like @pUsername ";
    //            Parameters.Add(new SqlParameter("@pUsername", $"%{this.Username}%"));
    //        }                                                                           

    //            // TODO: Criteria for other fields.


    //            CommandText = @$"
    //            select 
    //                T.* 
    //            from
    //                {fromClause}
    //            where
    //                {whereClause}
    //        ";


    //        return base.Search();
    //    }


    //    #endregion

    //    #region Other Methods


    //    public override void Reset()
    //    {
    //        Id = 0L;
    //        Username = string.Empty;
    //    }

    //    public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
    //    {

    //        CAdminType row = (CAdminType?)entity ?? new CAdminType();

    //        row.Id = (long)reader["Id"];
    //        row.Username = (string)reader["Username"];

    //        return row;
    //    }

    //    public override void Validate()
    //    {
    //        string message = "";

    //        if (string.IsNullOrWhiteSpace(Username))
    //        {
    //            message += $"{nameof(Username)} must be provided\n";
    //        }

    //        if (Id < 0L)
    //        {
    //            message += $"{nameof(Id)} must be provided\n";
    //        }

    //        if (message.Length > 0)
    //        {
    //            throw new CWheelOfDeathException(message);
    //        }
            
    //    }

    //    public override string ToString()
    //    {
    //        return $"{this.Id}: {this.Username}";
    //    }

    //    #endregion
    //}
}

