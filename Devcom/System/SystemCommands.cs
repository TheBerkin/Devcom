using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DeveloperCommands
{
    [DevcomCategory]
    internal static class SystemCommands
    {
        [Command("cat", "Changes the active category. Pass $ to return to root.")]
        public static void Cat(DevcomContext context, string category)
        {
            var cat = category.ToLower().Trim();
            if (cat == "")
            {
                context.Post("Current category: " + context.Category);
                return;
            }
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
            foreach (var msg in message)
            {
                context.Post(msg);
            }
        }

        [Command("exec", "Executes commands from one or more files.")]
        public static void Exec(AdminContext context, params string[] files)
        {
            foreach (var path in files)
            {
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            // ReSharper disable once PossibleNullReferenceException
                            // ReadLine() should never be null since the loop breaks at EOF
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
        public static void SetConVar(AdminContext context, string cvName, object value)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;
            convar.Value = value;
        }

        [Command("toggle", "Toggles a boolean-type convar.")]
        public static void Toggle(AdminContext context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            if (convar.Value.GetType() != typeof(bool))
            {
                context.Post("Convar '" + cvName + "' is not a boolean type.");
                return;
            }
            convar.Value = !((bool)convar.Value);
        }

        [Command("inc", "Adds 1 to the specified convar. Must be a number.")]
        public static void Increment(AdminContext context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            var o = convar.Value;
            if (!Util.Increment(ref o))
            {
                context.Post("Convar '" + cvName + "' is not a numeric type.");
                return;
            }
            convar.Value = o;
        }

        [Command("dec", "Subtracts 1 from the specified convar. Must be a number.")]
        public static void Decrement(AdminContext context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            var o = convar.Value;
            if (!Util.Decrement(ref o))
            {
                context.Post("Convar '" + cvName + "' is not a numeric type.");
                return;
            }
            convar.Value = o;
        }

        [Command("revert", "Sets a convar to its default value.")]
        public static void ResetConvar(AdminContext context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;
            convar.Value = convar.DefaultValue;
        }

        [Command("commands", "Displays a list of available commands.")]
        public static void ListCommands(DevcomContext context)
        {
            var contextType = context.GetType();
            context.Post(
                Devcom.Commands.Where(cmd => ContextFilterInternal.Test(contextType, cmd.Value.ContextFilter) && 
                    (cmd.Value.ContextType == contextType || contextType.IsSubclassOf(cmd.Value.ContextType)))
                .Select(cmd => cmd.Key)
                .Aggregate((accum, name) => accum + "\n" + name));
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
