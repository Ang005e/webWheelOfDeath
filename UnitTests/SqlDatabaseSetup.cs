using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using LibEntity;
using System;


namespace UnitTests
{
    [TestClass]
    public class SqlDatabaseSetup
    {
        private static string ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DbUnitTests;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        
        [AssemblyInitialize]
        public static void InitializeAssembly(TestContext _)
        {
            // Direct database setup
            Global.ConnectionString = ConnectionString;

            // Run setup scripts here
            //using var connection = new SqlConnection(ConnectionString);
            //connection.Open();

            // (Create test database, deploy schema, etc)

        }
    }
}