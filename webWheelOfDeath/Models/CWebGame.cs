using LibEntity.NetCore.Exceptions;
using LibWheelOfDeath;
using LibWheelOfDeath.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace webWheelOfDeath.Models
{
    public class CWebGame
    {
        private long _id; // just in case
        public string Game { get; set; }
        public string Difficulty { get; set; }
        public long FkDifficultyId { get; set; }
        public short Attempts { get; set; } 
        public short Misses { get; set; }
        public long DurationMilliseconds { get; set; }
        public short MinBalloons { get; set; }
        public short MaxBalloons { get; set; }

        public CWebGame()
        {
            // Default constructor for creating a new game
            _id = 0L;
            Game = string.Empty;
            Attempts = 0;
            Misses = 0;
            DurationMilliseconds = 0;
            MinBalloons = 0;
            MaxBalloons = 0;
            Difficulty = string.Empty;
            FkDifficultyId = 0;
        }

        public CWebGame(long Id)
        {
            CGame game = new CGame(Id);
            _id = Id;

            // Map properties from CGame to CWebGame
            Game = game.Game;
            Attempts = game.Attempts;
            Misses = game.Misses;
            DurationMilliseconds = game.DurationMilliseconds;
            MinBalloons = game.MinBalloons;
            MaxBalloons = game.MaxBalloons;

            // Find the difficulty name
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

            try
            {
                game.Create();
            }
            catch (CEntityException E)
            { 
                throw new CWheelOfDeathException("Error creating game: " + E.Message);
            }
        }

        public List<CGame> GetGamesByDifficulty()
        {
            return CGame.GetGamesByDifficulty();
        }
    }
}
