using System.Data;
using LibEntity.NetCore.Exceptions;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace webWheelOfDeath.Models
{
    public class CWebGame
    {
        public long Id;
        public string Game { get; set; }
        public string Difficulty { get; set; }
        public long FkDifficultyId { get; set; }
        public short Attempts { get; set; }
        public short Misses { get; set; }
        public long DurationMilliseconds { get; set; }
        public short MinBalloons { get; set; }
        public short MaxBalloons { get; set; }
        public bool IsActiveFlag { get; set; } = true;

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
            Difficulty = string.Empty;
            FkDifficultyId = 0;
            IsActiveFlag = true;
        }

        public CWebGame(long id)
        {
            CGame game = new CGame(id);

            this.Id = game.Id;
            Game = game.Game;
            Attempts = game.Attempts;
            Misses = game.Misses;
            DurationMilliseconds = game.DurationMilliseconds;
            MinBalloons = game.MinBalloons;
            MaxBalloons = game.MaxBalloons;
            FkDifficultyId = game.FkDifficultyId;
            IsActiveFlag = game.IsActiveFlag;

            CDifficulty difficulty = new CDifficulty(game.FkDifficultyId);
            Difficulty = difficulty.Difficulty;
        }

        public void Create()
        {
            var game = new CGame();
            game.Game = Game;
            game.Attempts = Attempts;
            game.Misses = Misses;
            game.FkDifficultyId = FkDifficultyId;
            game.DurationMilliseconds = DurationMilliseconds;
            game.MinBalloons = MinBalloons;
            game.MaxBalloons = MaxBalloons;
            game.IsActiveFlag = IsActiveFlag;

            try
            {
                game.Create();
            }
            catch (CEntityException E)
            {
                throw new CWheelOfDeathException("Error creating game: " + E.Message);
            }
        }

        public void Update()
        {
            var game = new CGame(Id);
            game.Game = Game;
            game.Attempts = Attempts;
            game.Misses = Misses;
            game.FkDifficultyId = FkDifficultyId;
            game.DurationMilliseconds = DurationMilliseconds;
            game.MinBalloons = MinBalloons;
            game.MaxBalloons = MaxBalloons;
            game.IsActiveFlag = IsActiveFlag;
            game.Update();
        }

        public void SetActive(bool active)
        {
            var game = new CGame(Id);
            game.IsActiveFlag = active;
            game.Update();
        }

        public IEnumerable<CWebGame> GetGames()
        {
            var games = new List<CWebGame>();
            var gameList = new CGame().GetAllGames();
            return games;
        }
    }
}
