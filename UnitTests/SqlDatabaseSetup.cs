using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System;


namespace UnitTests
{
    [TestClass()]
    public class SqlDatabaseSetup
    {
        private static string ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DbUnitTests;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        
        [AssemblyInitialize()]
        public static void InitializeAssembly(TestContext ctx)
        {
            // Direct database setup
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            // Run setup scripts here
            // (Create test database, deploy schema, etc)
            
        }
    }
}