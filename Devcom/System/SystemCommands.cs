using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DeveloperCommands
{
    [DevcomCategory]
    internal static class SystemCommands
    {
        [Command("cat", "Changes the active category. Pass $ or an empty string to return to root.")]
        public static void Cat(DevcomContext context, string category)
        {
            var cat = category.ToLower().Trim();
            context.Category = cat == "$" ? "" : cat;
        }

        [Command("root", "Returns the active category to the root.")]
        public static void Root(DevcomContext context)
        {
            context.Category = "";
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

        [Command("date", "Displays the current date.")]
        public static void Date(DevcomContext context)
        {
            context.Post(DateTime.Now.ToString("R"));
        }

        [Command("set", "Sets a convar.")]
        public static void SetConVar(DevcomContext context, string cvName, object value)
        {
            Convar convar;
            if (!Devcom.Convars.TryGetValue(Util.Qualify(context.Category, cvName), out convar))
            {
                context.Post("Convar '" + cvName + "' not found.");
                return;
            }
            convar.Value = value;
        }

        [Command("commands", "Displays a list of available commands.")]
        public static void ListCommands(DevcomContext context)
        {
            context.Post(Devcom.Commands.Select(cmd => cmd.Key).Aggregate((accum, name) => accum + "\n" + name));
        }

        [Command("help", "Displays the help text for a command.")]
        public static void Help(DevcomContext context, string command)
        {
            command = command.ToLower();
            CommandDef cmd;
            if (!Devcom.Commands.TryGetValue(Util.Qualify(context.Category, command), out cmd))
            {
                context.Post("Command not found: '" + command + "'");
                return;
            }
            var sb = new StringBuilder();
            sb.Append("Command: ").Append(command).AppendLine();
            sb.Append("Description: ").Append(cmd.Description).AppendLine();
            sb.Append("Usage: ").Append(cmd.QualifiedName).Append(" ").Append(cmd.ParamHelpString);
            context.Post(sb.ToString());
        }
    }
}
