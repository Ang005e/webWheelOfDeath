using System.ComponentModel;
using System.Reflection;
using LibEntity.NetCore.Exceptions;
using Microsoft.Data.SqlClient;
using LibEntity.NetCore.Annotations;
using LibEntity.NetCore.Infrastructure;

namespace LibEntity;

public abstract class CEntity : IEntity
{

    #region Constructors

    public CEntity(string tableName) : this(tableName, Global.ConnectionString) { }

    public CEntity(string tableName, string connectionString)
    {
        TableName = tableName;
        ConnectionString = connectionString;
    }

    #endregion


    #region Table Column Properties


    public long Id { get; set; }

    #endregion


    #region Other Properties
    public bool IsAddMode => Id == 0L;

    public string TableName { get; init; }

    public string ConnectionString { private get; init; }

    public string CommandText { get; set; } = string.Empty;

    /// <summary>
    /// ToDo; provide each seperate CRUDS class with an SqlParameter List--Parameters by 
    /// itself is far too prone to overlap issues between multiple layers of inheriting classes.
    /// </summary>
    public List<SqlParameter> Parameters = new List<SqlParameter>();

    #endregion


    #region CRUDS Methods

    /// <summary>
    /// Adds a new row to the table for the inherited class.
    /// 
    /// The auto-generated Id is then populated into the Id property 
    /// of this object.
    /// </summary>
    public virtual void Create() => Create(true);
    

    /// <summary>
    /// Adds a new row to the table for the inherited class.
    /// 
    /// <paramref name="isAutoIncrement"/> determines whether this
    /// insertion needs to interrogate the database for the generated
    /// identity value, and if so work this back into the Id property 
    /// of this object.
    /// </summary>
    public void Create(bool isAutoIncrement)
    {
        try
        {
            Validate(); // If any rule is violated, an exception is thrown

            // To place a call, we first need a phone line (SqlConnection)...
            using (SqlConnection conn = new SqlConnection()) 
            {
                // The ConnectionString property acts like a phone number to be dialled...
                conn.ConnectionString = this.ConnectionString;

                // Dial the number...
                conn.Open();

                // Create an object to have a conversation through this open phone line...
                using (SqlCommand cmd = new SqlCommand())
                {
                    // Ensure the conversation occurs through the connection 'conn' established above...
                    cmd.Connection = conn;

                    if (isAutoIncrement)
                    {
                        cmd.CommandText = $@"{CommandText};
                        select cast(scope_identity() as bigint);
                        ";
                    }
                    else
                    {
                        // Id won't need to be selected - it's being inserted, so we already know it.
                        cmd.CommandText = CommandText;
                    }


                    // Transfer the replacement parameters to the 
                    // SqlCommand object...
                    foreach (SqlParameter parameter in Parameters)
                    {
                        cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value ?? DBNull.Value);
                    }

                    // Execute the CommandText set above against the 
                    // connected database.
                    //
                    // *if autoIncrement == false*, the command text already supplies the database
                    // with the correct Id to insert.
                    //
                    // *However, if autoIncrement == true*, a new Id will be generated automatically by the Database.
                    // In this case, the final statement in the CommandText batch above is a 'Select'
                    // statement that returns the scope_identity() - i.e. the most
                    // recent system-generated id number allocated on this connection.
                    // As it is a 'Select' statement, the return type is a table
                    // with one column and one row. To retrieve the content of this
                    // first and only cell in this table, it is convenient to 
                    // use the ExecuteScalar() method, as it simply returns the 
                    // first cell value in the table of results.
                    if (isAutoIncrement)
                    {
                        try
                        {
                            // grab the newly-generated Id (retrived via scope_identity())
                            this.Id = (long)cmd.ExecuteScalar();
                        }
                        catch (SqlException E)
                        {
                            throw new CEntityException(E.Message, E);
                        }
                    }
                    else
                    {
                        // if the Id has been supplied - simply execute the query.
                        cmd.ExecuteNonQuery();
                    }
                }
                // Hang up the call...
                conn.Close();
            }

        }
        finally
        {
            Parameters.Clear();
        }    
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="isAutoIncrement"/> determines whether this
    /// insertion needs to interrogate the database for the generated
    /// identity value, and if so work this back into the Id property 
    /// of this object.
    /// <param name="runValidation"/> prevents Validate() from running, in 
    /// cases where inheriting classes call CRUDS methods (i.e. Search()) from 
    /// their Validate() method -- and as such, Parameters.Clear().
    //public void Create(bool isAutoIncrement, bool runValidation = true)
    //{
    //    if (runValidation) Validate();
    //    Create(isAutoIncrement);
    //}


    public virtual int Delete()
    {
        if (IsAddMode)
        {
            throw new CEntityException("Invalid Delete request in Add mode");
        }

        int rowsAffected = 0;


        // To place a call, we first need a phone line (SqlConnection)...
        using (SqlConnection conn = new SqlConnection())
        {
            // The ConnectionString property acts like a phone number to be dialled...
            conn.ConnectionString = this.ConnectionString;

            // Dial the number...
            conn.Open();

            // Create an object to have a conversation through this open phone line...
            using (SqlCommand cmd = new SqlCommand())
            {
                // Ensure the conversation occurs through the connection 'conn' established above...
                cmd.Connection = conn;

                cmd.CommandText = $@"delete [{TableName}] where Id = @pId";


                // Replace placeholder variables in above SQL statement
                // with actual values...
                cmd.Parameters.AddWithValue("@pId", this.Id);

                // Execute the CommandText set above against the 
                // connected database.
                //
                // For Insert/Update/Delete statements, a table of results
                // is not returned, only the number of rows that were 
                // affected by the command.
                // ExecuteNonQuery() executes code without retrieving data.
                rowsAffected = cmd.ExecuteNonQuery();
            }


            // Hang up the call...
            conn.Close();
        }

        return rowsAffected;
    }


    public void Read(long id)
    {
        Id = id;
        Read();
    }


    /// <summary>
    /// Retrieves the row matching the current Id.
    /// 
    /// If a matching row is located, the corresponding properties 
    /// of 'this' object are Populated.
    /// 
    /// Note: As the search is based on the primary key, there will at
    /// most be one matching row retrieved.
    /// </summary>
    /// <exception cref="CEntityException"></exception>
    public void Read()
    {
        if (IsAddMode)
        {
            throw new CEntityException("Invalid Read request in Add mode");
        }

        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = this.ConnectionString;

            conn.Open();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = $@"
                    select
	                    *
                    from
	                    [{TableName}]
                    where
	                    [Id] = @pId
                    ;
                ";

                // Substitute placeholder parameters in above query with 
                // actual values...
                cmd.Parameters.AddWithValue("@pId", this.Id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Populate(reader, this);
                    }
                }
                else
                {
                    // No matching rows found for this Id.
                    // Reset the property to 0L to reflect this...
                    this.Id = 0L;
                }

            }

            conn.Close();
        }

    }

    public void Save()
    {
        if (IsAddMode)
        {
            Create();
        }
        else
        {
            Update();
        }
    }


    /// <summary>
    /// Retrieves rows matching the criteria supplied in the incoming SQL 
    /// via the CommandText property.
    /// 
    /// For each row retrieved, a new object of this class is created
    /// and its properties Populated by the inheriting class and then 
    /// added to a list that is returned.
    /// </summary>
    /// <returns></returns>
    public virtual List<IEntity> Search()
    {
        // This method overload is for when we need the parameters to be cleared
        try
        {
            return Search(Parameters);
        }
        finally
        {
            Parameters.Clear();
        }
    }


    /// <summary>
    /// Retrieves rows matching the criteria supplied in the incoming SQL 
    /// via the CommandText property.
    /// 
    /// For each row retrieved, a new object of this class is created
    /// and its properties Populated by the inheriting class and then 
    /// added to a list that is returned.
    /// <paramref name="parameters"/> The list of parameters that 
    /// will be used to search the database.
    /// </summary>
    /// <returns></returns>
    public virtual List<IEntity> Search(List<SqlParameter> parameters)
    {

        List<IEntity> list = new List<IEntity>();

        try
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = this.ConnectionString;

                conn.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = this.CommandText;

                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    //foreach (SqlParameter parameter in this.Parameters)
                    //{
                    //    cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value ?? DBNull.Value);
                    //}

                    // Execute the above SQL statement and package the resulting
                    // table into a Reader object.
                    // The Reader object intially is positioned before the
                    // first row. Every time we issue the Read() method
                    // on the Reader, it advances to the next row (if any).
                    // The Read() method returns true if there is a next
                    // row, otherwise returns false...
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows == false)
                    {
                        return list; // return the empty list
                    }
                    while (reader.Read())
                    {
                        // Create a new object of the same type...
                        IEntity entity = Populate(reader);

                        // Add the newly created and populated object 
                        // to the result list...
                        list.Add(entity);
                    }
                }
                conn.Close();
            }
        }
        catch (System.InvalidOperationException e)
        {
            if (string.IsNullOrEmpty(this.CommandText))
            {
                throw new InaccessableClassException("CommandText is empty. Was Search() called from a searchable object?\n System.InvalidOperationException:", e);
            }
        }
        return list;
    }



    /// <summary>
    /// Method that updates an existing table row corresponding to the 
    /// populated Id.
    /// 
    /// The inheriting class must override this method to specify the 
    /// CommandText SQL and any Parameters that need replacement.
    /// 
    /// </summary>
    /// <returns>
    ///     The number of rows affected by this operation are returned.
    ///     As the update is based on the primary key, this return value
    ///     cannot exceed 1.
    /// </returns>
    /// <exception cref="CEntityException"></exception>
    public virtual int Update()
    {
        if (IsAddMode)
        {
            throw new CEntityException("Invalid Update request in Add mode");
        }

        Validate();

        int rowsAffected = 0;

        try
        {
            // To place a call, we first need a phone line (SqlConnection)...
            using (SqlConnection conn = new SqlConnection())
            {
                // The ConnectionString property acts like a phone number to be dialled...
                conn.ConnectionString = this.ConnectionString;

                // Dial the number...
                conn.Open();

                // Create an object to have a conversation through this open phone line...
                using (SqlCommand cmd = new SqlCommand())
                {
                    // Ensure the conversation occurs through the connection 'conn' established above...
                    cmd.Connection = conn;

                    cmd.CommandText = CommandText;


                    // Transfer the replacement parameters to the 
                    // SqlCommand object...
                    foreach (SqlParameter parameter in Parameters)
                    {
                        //cmd.Parameters.Add(parameter);
                        cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value ?? DBNull.Value);
                    }


                    // Execute the CommandText set above against the 
                    // connected database.
                    //
                    // For Insert/Update/Delete statements, a table of results
                    // is not returned, only the number of rows that were 
                    // affected by the command.
                    // ExecuteNonQuery() executes code without retrieving data.
                    rowsAffected = cmd.ExecuteNonQuery();

                    Parameters.Clear();
                }

                // Hang up the call...
                conn.Close();
            }
        }
        finally
        {
            Parameters.Clear();
        }

        return rowsAffected;

    }

    public abstract void Validate();


    /// <summary>
    /// Abstract method that takes in an SqlDataReader object as well as an IEntity
    /// object. The data property members of the IEntity object are populated
    /// from the current row of the <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public abstract IEntity Populate(SqlDataReader reader, IEntity? entity = null);

    
    /// <summary>
    /// Resets this object to its initial state.
    /// </summary>
    public virtual void Reset()
    {
        Id = 0L;
    }

    #endregion
}
