using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibEntity;

public static class Global
{
    public static string ConnectionString { internal get; set; } = string.Empty;

    public static void Initialise(string connectionString) => ConnectionString = connectionString;
    
}
