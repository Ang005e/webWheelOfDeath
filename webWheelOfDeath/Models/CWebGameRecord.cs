using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CWebGameRecord
    {
        public int Id { get; set; }
        public long FkGameId { get; set; }
        public long FkPlayerId { get; set; }
        public long FkResultId { get; set; } 
        public bool IsWin { get;set; }
        public DateTime Date { get; set; }
        public long ElapsedTime { get; set; }
        public short BalloonsPopped { get; set; }
        public short Misses { get; set; }

        public void Create()
        {
            // convert to abstract class + interface + generics -- generic entity, can automate this function
            // by enforcing the addition of a Build() function.
            CGameRecord gameRec = Build();
            gameRec.Create();
        }

        private CGameRecord Build()
        {

            return new CGameRecord
            {
                FkGameId = FkGameId,
                FkPlayerId = FkPlayerId,
                FkResultId = FkResultId,
                Date = DateTime.UtcNow,
                ElapsedTime = ElapsedTime,
                BalloonsPopped = BalloonsPopped,
                Misses = Misses
            };
        }
    }
}
