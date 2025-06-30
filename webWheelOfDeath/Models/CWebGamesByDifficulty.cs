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
            var result = LibWheelOfDeath.ReportClasses.CGamesByDifficulty.FetchGamesByDifficulty();

            // Ensure null never gets returned--create empty DataTable if needed
            if (result == null)
            {
                result = new DataTable();
                result.Columns.Add("Id", typeof(long));
                result.Columns.Add("Game", typeof(string));
                result.Columns.Add("FkDifficultyId", typeof(long));
                result.Columns.Add("Difficulty", typeof(string));
            }

            return result;
        }
    }
}
