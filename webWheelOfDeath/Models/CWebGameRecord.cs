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
            // convert to abstract class + interface + generics -- generic entity, can automate this function
            // by enforcing the addition of a Build() function.
            CGameRecord gameRec = Build();
            gameRec.Create();
        }

        private void GetResultType()
        {
            CResult result = new(_fkResultId);
            IsWin = result.IsWin??false;
            Result = result.ResultType;
        }

        private void PopulateResultId(EnumResultType resultType)
        {
            CResult result = new();
            result.GetWithResultType(resultType);
            _fkResultId = result.Id;
        }

        private CGameRecord Build()
        {

            // get result type id
            PopulateResultId(Result);

            return new CGameRecord
            {
                FkGameId = FkGameId,
                FkPlayerId = FkPlayerId,
                FkResultId = _fkResultId,
                Date = DateTime.UtcNow,
                ElapsedTime = ElapsedTime,
                BalloonsPopped = BalloonsPopped,
                Misses = Misses
            };
        }
    }
}
