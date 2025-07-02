using LibWheelOfDeath;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models
{
    public class CWebGameDifficulty : CEntityModel<CDifficulty>
    {
        public string Difficulty { get; set; } = string.Empty;

        public CWebGameDifficulty() : base() { }
        public CWebGameDifficulty(long id) : base(id) { }
        public static IEnumerable<CWebGameDifficulty> GetDifficulties()
        {
            CDifficulty diff = new();
            List<CDifficulty> difficulties = diff.GetDifficulties();
            foreach(var d in difficulties)
            {
                CWebGameDifficulty webDiff = new CWebGameDifficulty { Id = d.Id, Difficulty = d.Difficulty };
                yield return webDiff; // yield is kool
                // you can use yield return to create an iterator
                // that allows you to return a sequence of items one at a time
                // without loading the entire collection into memory at once
            }
        }

        protected override void MapFromEntity(CDifficulty entity) => Difficulty = entity.Difficulty;

        protected override void MapToEntity(CDifficulty entity) => entity.Difficulty = Difficulty;

        protected override void ValidateRequiredFields(bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(Difficulty))
                throw new InvalidOperationException($"Required field is not set: {Difficulty}");
        }
    }
}
