using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
{
    [DevcomCategory]
    internal class BaseCommands
    {
        [Command("cat", "Changes the active category. Pass $ or an empty string to return to root.")]
        public static void Cat(DevcomContext context, string category)
        {
            var cat = category.ToLower().Trim();
            DevcomEngine.CurrentCategory = cat == "$" ? "" : cat;
        }

        [Command("root", "Returns the active category to the root.")]
        public static void Root(DevcomContext context)
        {
            DevcomEngine.CurrentCategory = "";
        }

        [Command("echo", "Prints some text.")]
        public static void Echo(DevcomContext context, params object[] message)
        {
            foreach (object msg in message)
            {
                DevcomEngine.Print(msg);
            }
        }
    }
}
