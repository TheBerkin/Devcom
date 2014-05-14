using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperCommands
{
    /// <summary>
    /// Contains methods for loading, prompting for, and executing commands using the Devcom engine.
    /// </summary>
    public static class Devcom
    {
        /// <summary>
        /// If assigned, this callback will be executed when Print() is called.
        /// </summary>
        public static PrintCallback OnPrint;

        internal static readonly Dictionary<string, CommandDef> Commands = new Dictionary<string, CommandDef>();
        internal static readonly Dictionary<string, Convar> Convars = new Dictionary<string, Convar>(); 
        private static bool _loaded;

        /// <summary>
        /// Scans all loaded assemblies for commands and registers them.
        /// </summary>
        public static void Load()
        {
            if (_loaded) return;
            Scanner.FindAllDefs(Commands, Convars);
            _loaded = true;
        }

        /// <summary>
        /// Prints a formatted message to the Devcom output.
        /// </summary>
        /// <param name="message">The format string to pass.</param>
        /// <param name="args">The arguments to insert into the format string.</param>
        public static void Print(string message, params object[] args)
        {
            if (OnPrint != null)
            {
                OnPrint(message, args);
            }
            else
            {
                Console.WriteLine(message, args);
            }
        }

        /// <summary>
        /// Prints an object's string value to the Devcom output.
        /// </summary>
        /// <param name="value">The value to print.</param>
        public static void Print(object value)
        {
            Print(value.ToString());
        }

        /// <summary>
        /// Executes a command string under the default context.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public static void SendCommand(string command)
        {
            SendCommand(DevcomContext.Default, command);
        }

        /// <summary>
        /// Executes a command string under the specified context.
        /// </summary>
        /// <param name="context">The context under which to execute the command.</param>
        /// <param name="command">The command to execute.</param>
        public static void SendCommand(DevcomContext context, string command)
        {
            if (!_loaded) return;

            // Set the context to default if null was passed
            context = context ?? DevcomContext.Default;

            if (SystemConvars.EchoInput)
            {
                context.Post(command);
            }

            // Don't interpret empty commands
            if (String.IsNullOrEmpty(command)) return;

            // Cut off spaces from both ends
            command = command.Trim();

            foreach (string cmdstr in command.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()))
            {
                // Split up the line into arguments
                var parts = cmdstr.ParseParams().ToArray();
                if (!parts.Any()) continue;

                // The first index will be the command name
                var first = parts.First().ToLower();
                
                // Check if it's a root marker
                if (first == "$")
                {
                    context.Category = "";
                    continue;
                }

                // Check for root marker at the start of command name
                bool root = false;
                if (first.StartsWith("$"))
                {
                    root = true;
                    first = first.Substring(1);
                }

                // Get the fully-qualified name, taking into account root markers and current category
                string qname = root ? first : (context.Category.Length > 0 ? context.Category + "." : "") + first;

                // Make sure the command exists
                CommandDef cmd;
                if (!Commands.TryGetValue(qname, out cmd))
                {
                    context.Post("Command not found: " + qname);
                    continue;
                }

                if (parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (parts[i].StartsWith("{") && parts[i].EndsWith("}"))
                        {
                            parts[i] = Util.GetConvarValue(parts[i].Trim(new[] {'{', '}'}), context.Category);
                        }
                    }
                }

                // Run the command
                cmd.Run(context, parts.Where((s, i) => i > 0).ToArray());
            }
        }
    }

    /// <summary>
    /// The callback type used by Devcom to route print messages.
    /// </summary>
    /// <param name="message">The format string to pass.</param>
    /// <param name="args">The arguments to insert in the format string.</param>
    public delegate void PrintCallback(string message, object[] args);
}
