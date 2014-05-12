using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    internal static class Util
    {
        public static string Qualify(string cat, string cmdName)
        {
            return (cat.Length > 0 ? cat + "." : "") + cmdName;
        }
    }
}
