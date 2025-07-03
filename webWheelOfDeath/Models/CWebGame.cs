using System.Data;
using LibEntity.NetCore.Exceptions;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using webWheelOfDeath.Models.Infrastructure;

namespace webWheelOfDeath.Models
{
    public class CWebGame : CEntityModel<CGame>
    {
        #region Backing Properties
        private string? _game;
        private long? _fkDifficultyId;
        private short? _attempts;
        private short? _misses;
        private long? _durationMilliseconds;
        private short? _minBalloons;
        private short? _maxBalloons;
        private bool? _isActiveFlag;
        #endregion

        #region Public Properties
        public string Game
        {
            get => _game ?? string.Empty;
            set => _game = value;
        }

        public CWebDifficulty Difficulty
        {
            get
            {
                if (FkDifficultyId == 0)
                    return new CWebDifficulty(); // Return empty object for new games
                return new CWebDifficulty(FkDifficultyId);
            }
        }

        public long FkDifficultyId
        {
            get => _fkDifficultyId ?? 0L;
            set => _fkDifficultyId = value;
        }

        public short Attempts
        {
            get => _attempts ?? 0;
            set => _attempts = value;
        }

        public short Misses
        {
            get => _misses ?? 0;
            set => _misses = value;
        }

        public long DurationMilliseconds
        {
            get => _durationMilliseconds ?? 0L;
            set => _durationMilliseconds = value;
        }

        public short MinBalloons
        {
            get => _minBalloons ?? 0;
            set => _minBalloons = value;
        }

        public short MaxBalloons
        {
            get => _maxBalloons ?? 0;
            set => _maxBalloons = value;
        }

        public bool IsActiveFlag
        {
            get => _isActiveFlag ?? true;
            set => _isActiveFlag = value;
        }
        #endregion

        public CWebGame()
        {
            // Default constructor for creating a new game
            Id = 0L;
            Game = string.Empty;
            Attempts = 0;
            Misses = 0;
            DurationMilliseconds = 0;
            MinBalloons = 0;
            MaxBalloons = 0;
            FkDifficultyId = 0;
            IsActiveFlag = true;
        }

        public CWebGame(long id) : base(id) { }

        //public void Create()
        //{
        //    var game = new CGame();
        //    game.Game = Game;
        //    game.Attempts = Attempts;
        //    game.Misses = Misses;
        //    game.FkDifficultyId = FkDifficultyId;
        //    game.DurationMilliseconds = DurationMilliseconds;
        //    game.MinBalloons = MinBalloons;
        //    game.MaxBalloons = MaxBalloons;
        //    game.IsActiveFlag = IsActiveFlag;

        //    try
        //    {
        //        game.Create();
        //    }
        //    catch (CEntityException E)
        //    {
        //        throw new CWheelOfDeathException("Error creating game: " + E.Message);
        //    }
        //}

        //public void Update()
        //{
        //    var game = new CGame(Id);
        //    game.Game = Game;
        //    game.Attempts = Attempts;
        //    game.Misses = Misses;
        //    game.FkDifficultyId = FkDifficultyId;
        //    game.DurationMilliseconds = DurationMilliseconds;
        //    game.MinBalloons = MinBalloons;
        //    game.MaxBalloons = MaxBalloons;
        //    game.IsActiveFlag = IsActiveFlag;
        //    game.Update();
        //}

        public void SetActive(bool active)
        {
            IsActiveFlag = active;
            Update();
        }

        public IEnumerable<CWebGame> GetGames()
        {
            var games = new List<CWebGame>();
            CGame game = new CGame();
            foreach (CGame g in game.FetchAll())
            {
                CWebGame mapped = new();
                mapped.MapFromEntity(g);
                mapped.Id = g.Id;
                games.Add(mapped);
            }
            return games;
        }

        protected override void ValidateRequiredFields(bool isUpdate)
        {
            var errors = new List<string>();

            if (_game == null)
                errors.Add("Game name must be set");
            if (_fkDifficultyId == null)
                errors.Add("Difficulty must be set");
            if (_minBalloons == null)
                errors.Add("MinBalloons must be set");
            if (_maxBalloons == null)
                errors.Add("MaxBalloons must be set");
            if (_durationMilliseconds == null)
                errors.Add("DurationMilliseconds must be set");
            if (_attempts == null)
                errors.Add("Attempts must be set");
            if (_misses == null)
                errors.Add("Misses must be set");

            if (isUpdate)
            {
                if (_isActiveFlag == null) // true is an acceptable default for game creation
                    errors.Add("IsActiveFlag must be set when updating");
            }

            if (errors.Any())
                throw new InvalidOperationException($"Required fields not set: {string.Join(", ", errors)}");
        }


        protected override void MapFromEntity(CGame entity)
        {
            Game = entity.Game;
            Attempts = entity.Attempts;
            Misses = entity.Misses;
            DurationMilliseconds = entity.DurationMilliseconds;
            MinBalloons = entity.MinBalloons;
            MaxBalloons = entity.MaxBalloons;
            FkDifficultyId = entity.FkDifficultyId;
            IsActiveFlag = entity.IsActiveFlag;
        }

        protected override void MapToEntity(CGame entity)
        {
            entity.Game = Game;
            entity.Attempts = Attempts;
            entity.Misses = Misses;
            entity.DurationMilliseconds = DurationMilliseconds;
            entity.MinBalloons = MinBalloons;
            entity.MaxBalloons = MaxBalloons;
            entity.FkDifficultyId = FkDifficultyId;
            entity.IsActiveFlag = IsActiveFlag;
        }
    }
}
