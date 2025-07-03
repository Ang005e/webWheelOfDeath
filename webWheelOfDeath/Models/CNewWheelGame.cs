using LibWheelOfDeath.Report_Classes;
using LibWheelOfDeath.ReportClasses;

namespace webWheelOfDeath.Models
{
    /// <summary>
    /// singleton pattern, for funzies, was feeling cute, may delete later (when it breaks my app)
    /// (also because i needed rthe fastest player information)
    /// </summary>
    public sealed class CNewWheelGame : CWebGame
    {
        public CNewWheelGame(long id) : base(id)
        {
            var user = new CGameUser();
            (FastestPlayer, FastestTime) = user.FastestPlayerAndTheirTimeBecauseIDontCareAboutSeperationOfConcernsIts3AmInTheMorning();
        }
        public long FastestTime { get; set; }
        public string FastestPlayer { get; set; }
    }
}







