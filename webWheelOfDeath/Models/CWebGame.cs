using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    public class CWebGame
    {
        public string Game { get; set; }
        public short Attempts { get; set; } 
        public short Misses { get; set; }
        public long DurationMilliseconds { get; set; }
        public short MinBalloons { get; set; }
        public short MaxBalloons { get; set; }

        public CWebGame(long Id)
        {
            CGame game = new CGame(Id);

            // Map properties from CGame to CWebGame
            Game = game.Game;
            Attempts = game.Attempts;
            Misses = game.Misses;
            DurationMilliseconds = game.DurationMilliseconds;
            MinBalloons = game.MinBalloons;
            MaxBalloons = game.MaxBalloons;
        }
    }
}
