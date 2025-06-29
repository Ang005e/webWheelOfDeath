using LibWheelOfDeath.Report_Classes;
using LibWheelOfDeath.ReportClasses;

namespace webWheelOfDeath.Models;

public class CHallOfFame : IHallOfFame
{
    public CHallOfFame() { }
    public DateTime Date { get; set; } = DateTime.Now;
    public long ElapsedTime { get; set; } = 0L;
    public short BalloonsPopped { get; set; } = 0;
    public short Misses { get; set; } = 0;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Game { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;

    public CHallOfFame(DateTime date, long elapsedTime, short balloonsPopped, short misses, string firstName, string lastName, string game, string difficulty)
    {
        Date = date;
        ElapsedTime = elapsedTime;
        BalloonsPopped = balloonsPopped;
        Misses = misses;
        FirstName = firstName;
        LastName = lastName;
        Game = game;
        Difficulty = difficulty;
    }
    public List<CHallOfFame> TopReport() => CHallOfFameReport<CHallOfFame>.GetTop();
    public List<CHallOfFame> BottomReport() => CHallOfFameReport<CHallOfFame>.GetBottom();
}