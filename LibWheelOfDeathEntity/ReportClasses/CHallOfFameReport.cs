using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibEntity;
using LibWheelOfDeath.ReportClasses;

namespace LibWheelOfDeath.Report_Classes
{
    /// <summary>
    /// Class reperesenting a pair of database Views with the same structure.
    /// Generic structure; allosws any model/entity class impementation of IHallOfFame to use. 
    /// Very useful for external code outside the library.
    /// </summary>
    public class CHallOfFameReport<TModel> where TModel : IHallOfFame, new()
    {
        public static List<TModel> GetTop()
        {
            string sql = "select * from vwHallOfFameTop";
            return FetchReport(sql);
        }
        public static List<TModel> GetBottom()
        {
            string sql = "select * from vwHallOfFameBottom";
            return FetchReport(sql);
        }

        private static List<TModel> FetchReport(string sql)
        {
            DataTable table = sql.Fetch<DataTable>();

            List<TModel> reportCollection = new List<TModel>();

            foreach (DataRow row in table.Rows)
            {
                TModel hof = new();
                hof.Date = (DateTime)row["Date"];
                hof.ElapsedTime = (long)row["ElapsedTime"];
                hof.BalloonsPopped = (short)row["BalloonsPopped"];
                hof.Misses = (short)row["Misses"];
                hof.FirstName = (string)row["FirstName"];
                hof.LastName = (string)row["LastName"];
                hof.Game = (string)row["Game"];
                hof.Difficulty = (string)row["Difficulty"];
                reportCollection.Add(hof);
            }

            return reportCollection;
        }
    }
}
