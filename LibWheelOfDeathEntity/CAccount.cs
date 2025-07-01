using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using LibEntity;
using LibEntity.NetCore.Exceptions;
using LibEntity.NetCore.Infrastructure;
using Microsoft.Data.SqlClient;
namespace LibWheelOfDeath;

public class CAccount : CEntity
{

    #region Constructors


    public CAccount() : this("tblAccount") { }
    public CAccount(string tableName) : base(tableName) { }
    public CAccount(string tableName, string firstName, string lastName, string password) : base(tableName)
    {
        FirstName = firstName;
        LastName = lastName;
        Password = password;
    }
    public CAccount(long id) : this() 
    {
        Read(id);
    }

    #endregion
    
    #region Table Column Properties

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;


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

    new public virtual void Create()
    {
        CommandText = $@"

            insert into [tblAccount]
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
        ";

        Parameters.AddWithValue("@pFirstName", FirstName);
        Parameters.AddWithValue("@pLastName", LastName);
        Parameters.AddWithValue("@pPassword", Password);
        Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);

        base.Create();
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
            ";

        Parameters.AddWithValue("@pId", Id);
        Parameters.AddWithValue("@pFirstName", FirstName);
        Parameters.AddWithValue("@pLastName", LastName);
        Parameters.AddWithValue("@pPassword", Password);
        Parameters.AddWithValue("@pIsActiveFlag", IsActiveFlag);

        return base.Update();
    }




    #endregion

    #region Other Methods

    public override void Reset()
    {
        Id = 0L;
        FirstName = string.Empty;
        Password = string.Empty;
        LastName = string.Empty;
        IsActiveFlag = true;

    }


    public override LibEntity.IEntity Populate(SqlDataReader reader, LibEntity.IEntity? entity = null) //IEntity Populate(SqlDataReader reader, IEntity? entity = null)
    {
        //CAccount individual;
        //if (entity == null)
        //{
        //    individual = new CAccount();
        //}
        //else
        //{
        //    individual = entity;
        //}

        CAccount individual = (CAccount?)entity ?? new CAccount();

        individual.Id = (long)reader["Id"];
        individual.FirstName = (string)reader["FirstName"];
        individual.LastName = (string)reader["LastName"];
        individual.Password = (string)reader["Password"];
        individual.IsActiveFlag = (bool)reader["IsActiveFlag"];

        return individual;
    }

    public override void Validate()
    {
        CValidator<CAccount> validator = new CValidator<CAccount>(this);
        // validator.NoDefaults();


        if (Password.Length <= 12) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must be more than 12 characters long");

        if (!Password.Any(char.IsUpper)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one uppercase letter");

        if (!Password.Any(char.IsLower)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one lowercase letter");

        if (!Password.Any(char.IsDigit)) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one number");

        if (!Password.Any(ch => "!@#$%^&*()_+-=[]{}|;:',.<>/?".Contains(ch))) validator.ManualAddFailure(EnumValidationFailure.OutOfRange, $"{nameof(Password)} must contain at least one special character");

        validator.Validate();
    }

    public override string ToString()
    {
        return $"{this.Id}: {this.FirstName} {this.LastName} ({this.Password}). Account {(IsActiveFlag ? "active" : "inactive")}";
    }

    #endregion

}
