using System.Data;

namespace webWheelOfDeath.Models
{
    public class CWebGamesByDifficulty
    {
        public DataTable GamesAndIds { get; set; } = GamesByDifficulty();
        /// <summary>
        /// Returns a DataTable of--for all game modes--name and id, grouped by difficulty.
        /// </summary>
        /// <returns></returns>
        public static DataTable GamesByDifficulty()
        {
            return LibWheelOfDeath.ReportClasses.CGamesByDifficulty.FetchGamesByDifficulty();
        }
    }
}
