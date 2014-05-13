using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    [DevcomCategory]
    internal static class BaseCommands
    {
        [Command("cat", "Changes the active category. Pass $ or an empty string to return to root.")]
        public static void Cat(DevcomContext context, string category)
        {
            var cat = category.ToLower().Trim();
            Devcom.Category = cat == "$" ? "" : cat;
        }

        [Command("root", "Returns the active category to the root.")]
        public static void Root(DevcomContext context)
        {
            Devcom.Category = "";
        }

        [Command("echo", "Prints some text.")]
        public static void Echo(DevcomContext context, params object[] message)
        {
            foreach (object msg in message)
            {
                context.Post(msg);
            }
        }

        [Command("exec", "Executes commands from one or more files.")]
        public static void Exec(DevcomContext context, params string[] files)
        {
            foreach (string path in files)
            {
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            Devcom.SendCommand(context, reader.ReadLine().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.PostFormat("Failed to load {0}: '{1}'", path, ex.Message);
                }
            }
        }
    }
}
