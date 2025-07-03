using LibWheelOfDeath.Report_Classes;
using LibWheelOfDeath.ReportClasses;

namespace webWheelOfDeath.Models
{
    public sealed class CNewWheelGame : CWebGame
    {
        public CNewWheelGame(long id) : base(id)
        {
            var user = new CGameUser();
            (FastestPlayer, FastestTime) = user.GetFastestPlayerForGame(id); // pass game id
        }
        public long FastestTime { get; set; }
        public string FastestPlayer { get; set; }
    }
}







