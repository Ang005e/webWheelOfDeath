using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace webWheelOfDeath.Models
{
    public class CGameAndRecord
    {
        public CWebGame Game { get; set; } = new();
        public CWebGameRecord GameRecord { get; set; } = new();
        public CGameAndRecord()
        {
            Game = new CWebGame();
            GameRecord = new CWebGameRecord();
        }
    }
}
