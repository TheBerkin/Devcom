using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DeveloperCommands
{
    [Category]
    internal static class SystemCommands
    {
        [Command("cat", "Changes the active category. Pass $ to return to root.")]
        public static void Cat(Context context, string category)
        {
            var cat = category.ToLower().Trim();
            if (cat == "")
            {
                context.Notify("Current category: " + context.Category);
                return;
            }
            context.Category = cat == "$" ? "" : cat;
        }

        [Command("root", "Returns the active category to the root.")]
        public static void Root(Context context)
        {
            context.Category = "";
        }

        [Command("echo", "Prints some text.")]
        public static void Echo(Context context, params object[] message)
        {
            foreach (var msg in message)
            {
                context.Notify(msg);
            }
        }

        [DefaultAdminFilter]
        [Command("cfg_save", "Saves the engine configuration to " + ConvarConfig.DefaultConfigFile + ".")]
        public static void SaveConfig(Context context)
        {
            ConvarConfig.SaveConvars();
        }

        [DefaultAdminFilter]
        [Command("cfg_load", "Loads the engine configuration from " + ConvarConfig.DefaultConfigFile + ".")]
        public static void LoadConfig(Context context)
        {
            ConvarConfig.LoadConvars();
        }

        [DefaultAdminFilter]
        [Command("exec", "Executes commands from one or more files.")]
        public static void Exec(Context context, params string[] files)
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
                    context.NotifyFormat("Failed to load {0}: '{1}'", path, ex.Message);
                }
            }
        }

        [Command("date", "Displays the current date.")]
        public static void Date(Context context)
        {
            context.Notify(DateTime.Now.ToString("R"));
        }

        [DefaultAdminFilter]
        [Command("set", "Sets a convar.")]
        public static void SetConVar(Context context, string cvName, string value)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;
            convar.Value = value;
        }

        [DefaultAdminFilter]
        [Command("toggle", "Toggles a boolean-type convar.")]
        public static void Toggle(Context context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            if (convar.Value.GetType() != typeof(bool))
            {
                context.Notify("Convar '" + cvName + "' is not a boolean type.");
                return;
            }
            convar.Value = !((bool)convar.Value);
            context.NotifyFormat("{0} = {1}", convar.QualifiedName, convar.Value);
        }

        [DefaultAdminFilter]
        [Command("inc", "Adds 1 to the specified convar. Must be a number.")]
        public static void Increment(Context context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            var o = convar.Value;
            if (!Util.Increment(ref o))
            {
                context.Notify("Convar '" + cvName + "' is not a numeric type.");
                return;
            }
            convar.Value = o;
        }

        [DefaultAdminFilter]
        [Command("dec", "Subtracts 1 from the specified convar. Must be a number.")]
        public static void Decrement(Context context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;

            var o = convar.Value;
            if (!Util.Decrement(ref o))
            {
                context.Notify("Convar '" + cvName + "' is not a numeric type.");
                return;
            }
            convar.Value = o;
        }

        [DefaultAdminFilter]
        [Command("revert", "Sets a convar to its default value.")]
        public static void ResetConvar(Context context, string cvName)
        {
            Convar convar;
            if (!context.RequestConvar(cvName, out convar)) return;
            convar.Value = convar.DefaultValue;
        }

        [Command("commands", "Displays a list of available commands.")]
        public static void ListCommands(Context context)
        {
            var contextType = context.GetType();
            context.Notify(
                Devcom.Commands.Where(cmd => ContextFilter.Test(contextType, cmd.Value.Filter) && 
                    (cmd.Value.ContextType == contextType || contextType.IsSubclassOf(cmd.Value.ContextType)))
                .Select(cmd => cmd.Key)
                .Aggregate((accum, name) => accum + "\n" + name));
        }

        [Command("help", "Displays the help text for a command.")]
        public static void Help(Context context, string command)
        {
            command = command.ToLower();
            Command cmd;
            if (!Devcom.Commands.TryGetValue(Util.Qualify(context.Category, command), out cmd))
            {
                context.Notify("Command not found: '" + command + "'");
                return;
            }
            var sb = new StringBuilder();
            sb.Append(cmd.QualifiedName).Append(": ").Append(cmd.Description).AppendLine();
            sb.Append("Syntax: ").Append(cmd.QualifiedName).Append(" ").Append(cmd.ParamHelpString);
            context.Notify(sb.ToString());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class DefaultAdminFilterAttribute : Attribute
    {
    }
}
