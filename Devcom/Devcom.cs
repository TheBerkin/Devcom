using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        internal static readonly Dictionary<string, Command> Commands = new Dictionary<string, Command>();
        internal static readonly Dictionary<string, Convar> Convars = new Dictionary<string, Convar>(); 

        private static bool _loaded;

        internal const string CopyrightString = "Powered by Devcom v1.3 - Copyright (c) 2014 Nicholas Fleck";

        /// <summary>
        /// Scans all assemblies in the current application domain, and their references, for command/convar definitions.
        /// </summary>
        /// <param name="loadConfig">Indicates if the engine should load a configuration file.</param>
        public static void Load(bool loadConfig = true)
        {
            if (_loaded) return;
            Scanner.FindAllDefs(Commands, Convars);
            if (loadConfig) ConvarConfig.LoadConvars();
            Log(CopyrightString);
            _loaded = true;
        }

        /// <summary>
        /// Scans the specified assemblies for command/convar definitions.
        /// </summary>
        /// <param name="loadConfig">Indicates if the engine should load a configuration file.</param>
        /// <param name="definitionAssemblies">The assemblies to search.</param>
        public static void Load(bool loadConfig, params Assembly[] definitionAssemblies)
        {
            if (_loaded) return;
            // Always scan this assembly
            Scanner.SearchAssembly(Assembly.GetAssembly(typeof(Devcom)), Commands, Convars);
            // Scan listed assemblies
            foreach (var ass in definitionAssemblies)
            {
                Scanner.SearchAssembly(ass, Commands, Convars);
            }

            if (loadConfig) ConvarConfig.LoadConvars();
            Log(CopyrightString);
            _loaded = true;
        }

        /// <summary>
        /// Prints an object's string value to the Devcom output.
        /// </summary>
        /// <param name="value">The value to print.</param>
        public static void Log(object value)
        {
            if (OnPrint != null)
            {
                OnPrint(value.ToString());
            }
            else
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Executes a command string under the default context.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public static void Submit(string command)
        {
            Submit(Context.Default, command);
        }

        /// <summary>
        /// Translates a command string into a reusable Call object.
        /// </summary>
        /// <param name="context">The context to attach to the call.</param>
        /// <param name="command">The command string to translate.</param>
        /// <returns></returns>
        public static Call TranslateCommand(Context context, string command)
        {
            if (!_loaded) throw new InvalidOperationException("Cannot translate command calls without loading Devcom.");

            context = context ?? Context.Default;

            if (context == null) throw new ArgumentNullException(nameof(context));
            if (String.IsNullOrEmpty(command))
            {
                return Call.None;
            }

            // Cut off spaces from both ends
            command = command.Trim();

            var calls = new List<CallArgs>();

            foreach (var cmdstr in command.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
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

                // Make sure the command exists
                Command cmd;
                if (!Commands.TryGetValue(first, out cmd))
                {
                    throw new ArgumentException(String.Concat("Command not found: '", first, "'"));
                }

                if (parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        parts[i] = Regex.Replace(parts[i], @":(?<name>\S+):",
                            m => Util.GetConvarValue(m.Groups["name"].Value, context.Category),
                            RegexOptions.ExplicitCapture);
                    }
                }

                calls.Add(cmd.TranslateArgs(context, parts));
            }

            return new Call(calls.ToArray());
        }

        /// <summary>
        /// Executes a command string under the specified context.
        /// </summary>
        /// <param name="context">The context under which to execute the command.</param>
        /// <param name="command">The command to execute.</param>
        public static void Submit(Context context, string command)
        {
            if (!_loaded) return;

            // Set the context to default if null was passed
            context = context ?? Context.Default;

            if (SystemConvars.EchoInput)
            {
                context.Notify("> " + command);
            }

            // Don't interpret empty commands
            if (String.IsNullOrEmpty(command)) return;

            // Cut off spaces from both ends
            command = command.Trim();

            foreach (var cmdstr in command.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
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
                Command cmd;
                if (!Commands.TryGetValue(qname, out cmd))
                {
                    context.PostCommandNotFound(qname);
                    continue;
                }

                if (parts.Length > 1)
                {
                    for (int i = 1; i < parts.Length; i++)
                    {
                        parts[i] = Regex.Replace(parts[i], @":(?<name>\S+):",
                            m => Util.GetConvarValue(m.Groups["name"].Value, context.Category),
                            RegexOptions.ExplicitCapture);
                    }
                }

                // Run the command
                cmd.Run(context, parts.Where((s, i) => i > 0).ToArray());
            }
        }



        /// <summary>
        /// Executes a command string asynchronously under the default context.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public static async void SendCommandAsync(string command)
        {
            await Task.Run(() => Submit(Context.Default, command));
        }

        /// <summary>
        /// Executes a command string asynchronously under the specified context.
        /// </summary>
        /// <param name="context">The context under which to execute the command.</param>
        /// <param name="command">The command to execute.</param>
        public static async void SendCommandAsync(Context context, string command)
        {
            await Task.Run(() => Submit(context, command));
        }

        /// <summary>
        /// Searches for commands containing the specified substring and returns a collection of matches.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        /// <param name="beginsWith">Indicates if the search should only return results that begin with the query.</param>
        /// <returns></returns>
        public static IEnumerable<Command> SearchCommands(string query, bool beginsWith = true)
        {
            query = query.ToLower();
            return
                Commands.Where(pair => beginsWith ? pair.Key.StartsWith(query) : pair.Key.Contains(query))
                    .Select(pair => pair.Value)
                    .OrderBy(fn => fn.QualifiedName.Length)
                    .ThenBy(fn => fn.ParamHelpString.Length);
        }

        /// <summary>
        /// Searches for convars containing the specified substring and returns a collection of matches.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        /// <param name="beginsWith">Indicates if the search should only return results that begin with the query.</param>
        /// <returns></returns>
        public static IEnumerable<Convar> SearchConvars(string query, bool beginsWith = true)
        {
            query = query.ToLower();
            return Convars.Where(pair => beginsWith ? pair.Key.StartsWith(query) : pair.Key.Contains(query))
                .Select(pair => pair.Value)
                .OrderBy(cv => cv.QualifiedName);
        }

        /// <summary>
        /// Creates a new command using the specified method, metadata, and an optional filter.
        /// </summary>
        /// <param name="method">The method to associate with the command.</param>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="category">The category under which to place the command.</param>
        /// <param name="filter">The filter rules to apply to the command.</param>
        /// <returns></returns>
        public static Command CreateCommand(MethodInfo method, string name, string description, string category, ContextFilter filter = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = name.Trim().ToLower();
            var qname = Util.Qualify(category ?? "", name);

            if (Commands.ContainsKey(qname)) return null;

            return Commands[qname] = new Command(method, name, description, category, filter);
        }

        /// <summary>
        /// Saves the current configuration of the engine.
        /// </summary>
        public static void SaveConfig()
        {
            ConvarConfig.SaveConvars();
        }
    }

    /// <summary>
    /// The callback type used by Devcom to route print messages.
    /// </summary>
    /// <param name="message">The message to pass.</param>
    public delegate void PrintCallback(string message);
}
