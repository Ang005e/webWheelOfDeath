using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibWheelOfDeath;

namespace webWheelOfDeath.Models
{
    class CWebGameDifficulty
    {

        public long Id { get; set; } = 0L;
        public string Difficulty { get; set; } = string.Empty;

        public static IEnumerable<CWebGameDifficulty> GetDifficulties()
        {
            CDifficulty diff = new();
            List<CDifficulty> difficulties = diff.GetDifficulties();
            foreach(var d in difficulties)
            {
                CWebGameDifficulty webDiff = new CWebGameDifficulty
                {
                    Id = d.Id,
                    Difficulty = d.Difficulty
                };
                yield return webDiff; // yield is kool
                // you can use yield return to create an iterator
                // this allows you to return a sequence of items one at a time
                // without loading the entire collection into memory at once
            }
        }

    }
}
