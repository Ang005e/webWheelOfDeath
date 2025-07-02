using LibWheelOfDeath;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models
{
    public class CWebGameRecord : CEntityModel<CGameRecord>
    {
        #region Backing Properties
        private long? _fkGameId;
        private long? _fkPlayerId;
        private long? _fkResultId;
        private DateTime? _date;
        private long? _elapsedTime;
        private short? _balloonsPopped;
        private short? _misses;
        #endregion

        #region Public Properties
        public long FkGameId
        {
            get => _fkGameId ?? 0L;
            set => _fkGameId = value;
        }
        public long FkPlayerId
        {
            get => _fkPlayerId ?? 0L;
            set => _fkPlayerId = value;
        }
        public long FkResultId
        {
            get => _fkResultId ?? 0L;
            set => _fkResultId = value;
        }
        public DateTime Date
        {
            get => _date ?? DateTime.UtcNow;
            set => _date = value;
        }
        public long ElapsedTime
        {
            get => _elapsedTime ?? 0L;
            set => _elapsedTime = value;
        }
        public short BalloonsPopped
        {
            get => _balloonsPopped ?? 0;
            set => _balloonsPopped = value;
        }
        public short Misses
        {
            get => _misses ?? 0;
            set => _misses = value;
        }
        #endregion


        public IEnumerable<CWebGameRecord> GetAll()
        {
            CGameRecord gr = new();
            List<CWebGameRecord> list = new List<CWebGameRecord>();
            foreach(CGameRecord record in gr.FetchAll())
            {
                CWebGameRecord webRecord = new CWebGameRecord();
                webRecord.MapFromEntity(record);
                webRecord.Id = record.Id;
                list.Add(webRecord);
            }
            return list;
        }


        #region Entity Mapping
        protected override void MapFromEntity(CGameRecord entity)
        {
            FkGameId = entity.FkGameId;
            FkPlayerId = entity.FkPlayerId;
            FkResultId = entity.FkResultId;
            Date = entity.Date ?? DateTime.UtcNow;
            ElapsedTime = entity.ElapsedTime;
            BalloonsPopped = entity.BalloonsPopped;
            Misses = entity.Misses;
        }

        protected override void MapToEntity(CGameRecord entity)
        {
            entity.FkGameId = FkGameId;
            entity.FkPlayerId = FkPlayerId;
            entity.FkResultId = FkResultId;
            entity.Date = DateTime.UtcNow;
            entity.ElapsedTime = ElapsedTime;
            entity.BalloonsPopped = BalloonsPopped;
            entity.Misses = Misses;
        }
        #endregion

        /// <summary>
        /// Call before any database operations to ensure required fields are set
        /// </summary>
        protected override void ValidateRequiredFields(bool isUpdate = false) // model class is not updatable
        {
            var errors = new List<string>();

            if (_fkGameId == null)
                errors.Add("FkGameId must be set");
            if (_fkPlayerId == null)
                errors.Add("FkPlayerId must be set");
            if (_fkResultId == null)
                errors.Add("FkResultId must be set");
            //if (_date == null)
            //    errors.Add("Date must be set"); // has an acceptable default population value
            if (_elapsedTime == null)
                errors.Add("ElapsedTime must be set");
            if (_balloonsPopped == null)
                errors.Add("BalloonsPopped must be set");
            if (_misses == null)
                errors.Add("Misses must be set");

            if (errors.Any())
                throw new InvalidOperationException($"Required fields not set: {string.Join(", ", errors)}");
        }
    }
}
