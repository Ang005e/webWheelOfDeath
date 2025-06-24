//using Microsoft.Data.SqlClient;

//namespace UnitTests;

//[TestClass]
//public class DatabaseInsertTests
//{
//    private static string ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DbUnitTests;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";



//    [TestMethod]
//    public void TestAccounts()
//    {
//        using SqlConnection conn = new SqlConnection(ConnectionString);
//        conn.Open();

//        string commandText = @$"
//            select * from tblAccount
//        ";

//        SqlCommand cmd = new(commandText);

//        SqlDataReader reader = cmd.ExecuteReader();

//        // Assert.AreEqual()

//    }


//}