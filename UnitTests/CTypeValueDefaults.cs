using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    // ToDo: Singleton class should be replaced with static class -- no need for singleton here
    public class TypeValueDefaults
    {
        public const long Long = 0L;
        public const string String = ""; // can't use String.Empty -- it's a readonly field under the hood, which can't be used with const
        public const int Int = 0;
        public const short Short = 0;


        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private static TypeValueDefaults instance;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


        private TypeValueDefaults() { }
        public static TypeValueDefaults GetInstance()
        {
            if (instance == null)
            {
                instance = new();
            }
            return instance;
        }
    }
}
