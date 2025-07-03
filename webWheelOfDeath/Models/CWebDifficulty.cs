using LibWheelOfDeath;
using webWheelOfDeath.Models.Infrastructure;

public class CWebDifficulty : CEntityModel<CDifficulty>
{

    #region Backing Properties
    private string? _difficulty;
    #endregion

    #region Public Properties
    public string Difficulty
    {
        get => _difficulty ?? string.Empty;
        set => _difficulty = value;
    }
    #endregion


    #region Constructors
    public CWebDifficulty() : base() { }
    public CWebDifficulty(long id) : base(id) { }
    #endregion


    #region Entity Mapping
    protected override void MapFromEntity(CDifficulty entity)
    {
        Difficulty = entity.Difficulty;
    }

    protected override void MapToEntity(CDifficulty entity)
    {
        entity.Difficulty = Difficulty;
    }
    #endregion


    #region Validation
    protected override void ValidateRequiredFields(bool isUpdate)
    {
        if (string.IsNullOrWhiteSpace(Difficulty))
            throw new InvalidOperationException("Difficulty name is required");
    }
    #endregion


    #region Static Methods
    public static IEnumerable<CWebDifficulty> GetDifficulties()
    {
        CDifficulty diff = new();
        List<CDifficulty> difficulties = diff.GetDifficulties();
        foreach (var d in difficulties)
        {
            CWebDifficulty webDiff = new CWebDifficulty { Id = d.Id, Difficulty = d.Difficulty };
            yield return webDiff;
        }
    }
    #endregion
}