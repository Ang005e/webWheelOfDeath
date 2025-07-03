using System;
using System.Collections.Generic;
using System.Data;
using LibEntity;
using Microsoft.Data.SqlClient;

namespace LibWheelOfDeath.ReportClasses
{
    public static class CGamePopularityReport
    {
        public static List<CGamePopularity> GetPopularity(DateTime startDate, DateTime endDate, bool ascending = false)
        {
            string sql = @"exec uspGamePopularity @pStartDate, @pEndDate";

            var parameters = new List<SqlParameter>();
            parameters.AddWithValue("@pStartDate", startDate);
            parameters.AddWithValue("@pEndDate", endDate);

            DataTable table = DataServices.Fetch<DataTable>(sql, parameters);
            var results = new List<CGamePopularity>();

            foreach (DataRow row in table.Rows)
            {
                var item = new CGamePopularity
                {
                    Game = row["Game"].ToString() ?? string.Empty,
                    GameId = (long)row["Id"],
                    Difficulty = row["Difficulty"].ToString() ?? string.Empty,
                    TimesPlayed = Convert.ToInt32(row["TotalGamesPlayed"])
                };
                results.Add(item);
            }

            if (ascending)
                return results.OrderBy(x => x.TimesPlayed).ToList();
            else
                return results.OrderByDescending(x => x.TimesPlayed).ToList();
        }
    }
}