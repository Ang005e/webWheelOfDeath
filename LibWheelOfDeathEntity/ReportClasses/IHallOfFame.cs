using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWheelOfDeath.ReportClasses
{
    /// <summary>
    /// Public interface. Allows access to Views (i.e. the *Hall of Fame* report view), through <see cref="LibWheelOfDeath"/>'s 
    /// static view classes (i.e.<see cref="Report_Classes.CHallOfFameReport{TModel}"/>).
    /// </summary>
    public interface IHallOfFame
    {
        public DateTime Date { get; set; }
        public long ElapsedTime { get; set; }
        public short BalloonsPopped { get; set; }
        public short Misses { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Game { get; set; }
        public string Difficulty { get; set; }
    }
}
