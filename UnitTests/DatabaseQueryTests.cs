//using Microsoft.Data.SqlClient;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UnitTests
//{
    
//    internal class DatabaseQueryTests
//    {
//        [ClassInitialize]
//        private static void InsertTestData()
//        {
//            return;
//        }

//        private static string ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DbUnitTests;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

//        [TestMethod]
//        public void AccountQueryTest()
//        {
//            using SqlConnection conn = new SqlConnection(ConnectionString);
//            conn.Open();

//            string commandText = @$"
//                select * from tblAccount
//            ";

//            SqlCommand cmd = new(commandText);

//            SqlDataReader reader = cmd.ExecuteReader();

//            // Assert.AreEqual()

//        }
//    }
//}
