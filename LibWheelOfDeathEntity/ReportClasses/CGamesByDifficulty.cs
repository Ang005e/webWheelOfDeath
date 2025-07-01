using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibEntity;

namespace LibWheelOfDeath.ReportClasses
{
    public static class CGamesByDifficulty
    {
        public static DataTable FetchGamesByDifficulty()
        {
            string sql = "exec [uspGamesByDifficulty]";
            return sql.Fetch<DataTable>();
        }
    }
}
