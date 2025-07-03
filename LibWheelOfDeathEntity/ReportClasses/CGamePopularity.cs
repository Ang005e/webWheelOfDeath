using System;
using System.ComponentModel.DataAnnotations;

namespace LibWheelOfDeath.ReportClasses
{
    public class CGamePopularity
    {
        public string Game { get; set; }
        public long GameId { get; set; }
        public string Difficulty { get; set; }
        public int TimesPlayed { get; set; }
        public bool IsArchived { get; set; }
    }

    public class GamePopularityFilter
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-6); // 6 months

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1); // today

        public bool SortAscending { get; set; } = false;
    }
}