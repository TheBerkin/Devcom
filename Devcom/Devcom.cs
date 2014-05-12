using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    public static class Devcom
    {
        public static PrintCallback OnPrint;

        private static Dictionary<string, CommandDef> _commands;
        private static bool _loaded;
        private static string _cat;

        public static void Load()
        {
            if (_loaded) return;
            _commands = CmdProvider.FindThemAll();
            _loaded = true;
            _cat = "";
        }

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

        public static void Print(object value)
        {
            Print(value.ToString());
        }

        public static void Input(DevcomContext context, string command)
        {
            context = context ?? DevcomContext.Default;
            if (String.IsNullOrEmpty(command)) return;
            command = command.Trim();

            foreach (string cmdstr in command.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()))
            {
                var parts = cmdstr.ParseParams().ToArray();
                if (!parts.Any()) continue;
                var first = parts.First().ToLower();
                bool root = false;
                if (first == "$")
                {
                    CurrentCategory = "";
                    continue;
                }

                if (first.StartsWith("$"))
                {
                    root = true;
                    first = first.Substring(1);
                }

                string qname = root ? first : (_cat.Length > 0 ? _cat + "." : "") + first;
                CommandDef cmd;
                if (!_commands.TryGetValue(qname, out cmd))
                {
                    context.Post("Command not found: " + qname);
                    continue;
                }
                cmd.Run(context, parts.Where((s, i) => i > 0).ToArray());
            }
        }

        public static string Prompt
        {
            get { return "devcom" + (_cat.Length > 0 ? "." + _cat : "") + " > "; }
        }

        public static string CurrentCategory
        {
            get { return _cat; }
            set { _cat = value; }
        }
    }

    public delegate void PrintCallback(string message, object[] args);
}
