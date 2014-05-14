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

        public static bool IsValidName(string name, string otherChars = "_-+")
        {
            return name.All(c => Char.IsLetterOrDigit(c) || otherChars.Contains(c));
        }

        public static string GetConvarValue(string name, string cat)
        {
            if (name.StartsWith("$"))
            {
                cat = "";
                name = name.Substring(1);
            }

            string qname = (cat.Length > 0 ? cat + "." : "") + name;
            Convar convar;
            if (!Devcom.Convars.TryGetValue(qname, out convar))
            {
                return "undefined";
            }
            object value = convar.Value;
            return value == null ? "null" : value.ToString();
        }
    }
}
