using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CWebGameRecord
    {
        public int Id { get; set; }
        public long FkGameId { get; set; }
        public long FkPlayerId { get; set; }
        private long _fkResultId { get; set; } // frontened never needs to know
        public EnumResultType Result { get; set; }
        public bool IsWin { get;set; }
        public DateTime Date { get; set; }
        public long ElapsedTime { get; set; }
        public short BalloonsPopped { get; set; }
        public short Misses { get; set; }

        public void Create()
        {
            CGameUser game = new();
        }

        private void GetResult()
        {
            CResult result = new(_fkResultId);
            IsWin = result.IsWin??false;
            Result = result.ResultType;
        }
    }
}
