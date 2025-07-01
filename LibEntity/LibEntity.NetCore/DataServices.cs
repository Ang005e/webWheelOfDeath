using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace LibEntity;

/// <summary>
/// Last update: 18 March 2025
/// Updated by: Ramin Majidi
/// Comments: This static class is adapted from the CData instance class 
/// from Ramin Majidi's private LibORM class library.
/// It is made available here for use and unlimited modification by staff and
/// students of South Regional TAFE.
/// The author reserves the continued use and modification of this code
/// for private and commercial projects.
/// </summary>
public static class DataServices
{

    /// <summary>
    /// Retrieves the requested data and packages and returns in a DataTable object.
    /// </summary>
    /// <param name="commandText">
    ///     See <paramref name="commandType"/> below
    /// </param>
    /// <param name="sqlParameters">
    ///     List of substitution key-value SqlParameters.
    /// </param>
    /// <param name="commandType">
    ///     Specifies whether the incoming query contained in <paramref name="commandText"/> is:
    ///     CommandType.Text (default if omitted) : an SQL command
    ///     CommandType.StoredProcedure : an SQL stored procedure name
    /// </param>
    /// <returns>
    ///     A single DataTable object containing the query results
    ///     or a single DataSet object containing DataTables
    /// </returns>
    public static T Fetch<T>(
        this string commandText,
        List<SqlParameter>? sqlParameters = null,
        CommandType commandType = CommandType.Text
        )
        where T : MarshalByValueComponent, IListSource, ISupportInitializeNotification, ISupportInitialize, ISerializable, IXmlSerializable, new()
    {

        /// Declare an object variable that will hold the specific 
        /// type <typeparam name="T"/> instance...
        T data;

        // SqlConnection acts as the telephone line to the server and database...
        using (SqlConnection conn = new SqlConnection())
        {

            // The ConnectionString property acts like a telephone number to
            // establish a two-way connection...
            conn.ConnectionString = Global.ConnectionString;

            // Dial the number with Open()...
            conn.Open();

            // SqlCommand allows us to pass command (SQL) messages to the database
            // and receive responses and results back...
            using (SqlCommand comm = new SqlCommand())
            {
                // Link the SqlCommand object to the established connnection...
                comm.Connection = conn;

                // Set the Command Type of this query...
                comm.CommandType = commandType;

                // CommandText is the contents of the message to be sent...
                comm.CommandText = commandText;


                // Substitute parameters if any...
                if (sqlParameters != null)
                {
                    foreach (SqlParameter param in sqlParameters)
                    {
                        comm.Parameters.Add(param.NullFix());
                    }
                }

                // SqlDataReader receives the message response/results - usually a table...
                // DataSet is a structure that contains a List of DataTables.
                // DataTable is a convenient structure that allows the returned data to be
                // accessible through Rows and Columns properties. The data is cached locally
                // in this structure, so the connection can be closed quickly (long-distance
                // calls are expensive, so must be kept as short as possible)...

                //-------------------
                // Create an instance of the requested return object...
                if (typeof(T) == typeof(DataSet))
                {
                    DataSet ds = new();
                    new SqlDataAdapter(comm).Fill(ds);
                    data = (ds as T)!;
                }
                else if (typeof(T) == typeof(DataTable))
                {
                    DataTable dt = new();
                    new SqlDataAdapter(comm).Fill(dt);
                    data = (dt as T)!;
                }
                // Alternative approach is to create new objects and test type on the fly...
                //if (new T() is DataTable dt)
                //{
                //    new SqlDataAdapter(comm).Fill(dt);
                //    data = dt as T;
                //}
                //else if (new T() is DataSet ds)
                //{
                //    new SqlDataAdapter(comm).Fill(ds);
                //    data = ds as T;
                //}
                else
                {
                    throw new ApplicationException("Invalid Type <T> Request");
                }
            }

            // Hangup the connection with Close()...
            conn.Close();
        }

        return data!;
    }


    /// <summary>
    /// UPDATE/DELETE SQL commands may be passed to this
    /// method, optionally including a collection of 
    /// substitution parameters.
    /// The query is conducted against the connected database
    /// and a long value result is returned to indicate
    /// how many rows were affected by the UPDATE/DELETE query.
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns>Number of rows affected</returns>
    public static long Execute(this string commandText,
        List<SqlParameter>? sqlParameters = null,
        CommandType commandType = CommandType.Text)
    {
        long rowsAffected = 0;

        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = Global.ConnectionString;

            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = conn;
                comm.CommandType = commandType;
                comm.CommandText = commandText;

                // Substitute parameters if any...
                if (sqlParameters != null)
                {
                    foreach (SqlParameter param in sqlParameters)
                    {
                        comm.Parameters.Add(param.NullFix());
                    }
                }

                conn.Open();

                rowsAffected = comm.ExecuteNonQuery();
            }

            conn.Close();
        }

        return rowsAffected;

    }


    /// <summary>
    /// INSERT/UPDATE/DELETE stored procedures may be passed to this
    /// method, optionally including a collection of 
    /// substitution parameters.
    /// The query is conducted against the connected database
    /// and a long value result is returned to indicate
    /// how many rows were affected by the UPDATE/DELETE query 
    /// or to indicate the ID of the new row for INSERTions.
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns>Number of rows affected</returns>
    public static long ExecuteStoredProc(this string commandText,
        List<SqlParameter>? sqlParameters = null,
        string resultParamName = "@pBigResult",
        string errorParamName = "@pErrorMessage")
    {
        long result = 0;

        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = Global.ConnectionString;

            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = commandText;

                // Substitute parameters if any...
                if (sqlParameters != null)
                {
                    foreach (SqlParameter param in sqlParameters)
                    {
                        comm.Parameters.Add(param.NullFix());
                    }

                }

                // Add an Output parameter for any returned result values...
                SqlParameter paramResult = new SqlParameter
                {
                    // Set the parameter properties...
                    ParameterName = resultParamName,
                    Direction = ParameterDirection.Output,
                    DbType = DbType.Int64
                };

                comm.Parameters.Add(paramResult);

                // Add an Output parameter for any returned error strings...
                SqlParameter paramError = new SqlParameter
                {
                    // Set the parameter properties...
                    ParameterName = errorParamName,
                    Direction = ParameterDirection.Output,
                    DbType = DbType.String,
                    Size = -1
                };

                comm.Parameters.Add(paramError);

                conn.Open();

                comm.ExecuteNonQuery();

                string error = $"{paramError.Value}";

                if (!string.IsNullOrEmpty(error))
                {
                    throw new ApplicationException(error);
                }

                result = (long)paramResult.Value;

            }

            conn.Close();
        }

        return result;

    }


    /// <summary>
    /// INSERT SQL commands may be passed to this
    /// method, optionally including a collection of 
    /// substitution parameters.
    /// The query is conducted against the connected database
    /// and a long value result is returned signifying the
    /// system-generated uniqe ID of the newly inserted row.
    /// </summary>
    /// <param name="commandText"></param>
    /// <returns>System-generated unique ID</returns>
    public static long Insert(this string commandText,
                    List<SqlParameter>? sqlParameters = null)
    {
        long newID = 0;

        using (SqlConnection conn = new SqlConnection())
        {
            conn.ConnectionString = Global.ConnectionString;

            using (SqlCommand comm = new SqlCommand())
            {
                comm.Connection = conn;
                comm.CommandType = CommandType.Text;

                // Execute the query and retrieve the system-generated unique id in one step.
                comm.CommandText = $@"{commandText}; 
                        select cast(scope_identity() as bigint); ";


                // Substitute parameters if any...
                if (sqlParameters != null)
                {
                    foreach (SqlParameter param in sqlParameters)
                    {
                        comm.Parameters.Add(param.NullFix());
                    }
                }

                conn.Open();

                // Retrieve the scalar value from the first column of the first row of the return table...
                newID = (long)comm.ExecuteScalar();

            }
            conn.Close();

        }

        return newID;

    }


    private static SqlParameter NullFix(this SqlParameter param)
    {
        param.Value ??= DBNull.Value;

        return param;
    }
}
