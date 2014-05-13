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

        private static Dictionary<string, CommandDef> _commands;
        private static bool _loaded;
        private static string _cat;

        /// <summary>
        /// Scans all loaded assemblies for commands and registers them.
        /// </summary>
        public static void Load()
        {
            if (_loaded) return;
            _commands = CmdProvider.FindThemAll();
            _loaded = true;
            _cat = "";
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
        /// Executes a command string under the specified context.
        /// </summary>
        /// <param name="context">The context under which to execute the command.</param>
        /// <param name="command">The command to execute.s</param>
        public static void SendCommand(DevcomContext context, string command)
        {
            if (!_loaded) return;

            // Set the context to default if null was passed
            context = context ?? DevcomContext.Default;

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
                    Category = "";
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
                string qname = root ? first : (_cat.Length > 0 ? _cat + "." : "") + first;

                // Make sure the command exists
                CommandDef cmd;
                if (!_commands.TryGetValue(qname, out cmd))
                {
                    context.Post("Command not found: " + qname);
                    continue;
                }

                // Run the command
                cmd.Run(context, parts.Where((s, i) => i > 0).ToArray());
            }
        }

        /// <summary>
        /// The current prompt string.
        /// </summary>
        public static string Prompt
        {
            get { return "devcom" + (_cat.Length > 0 ? "." + _cat : "") + " > "; }
        }

        /// <summary>
        /// The current category.
        /// </summary>
        public static string Category
        {
            get { return _cat; }
            set { _cat = value; }
        }
    }

    /// <summary>
    /// The callback type used by Devcom to route print messages.
    /// </summary>
    /// <param name="message">The format string to pass.</param>
    /// <param name="args">The arguments to insert in the format string.</param>
    public delegate void PrintCallback(string message, object[] args);
}
