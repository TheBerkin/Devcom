using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
{
    public static class Devcom
    {
        public static PrintCallback OnPrint;

        private static Dictionary<string, DevCommand> _commands;
        private static bool _loaded;
        private static string _cat;

        public static void Load()
        {
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

        public static void Input(string command)
        {
            if (String.IsNullOrEmpty(command)) return;
            command = command.Trim();

            foreach (string cmdstr in command.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()))
            {
                var parts = cmdstr.ParseParams();
                if (!parts.Any()) continue;
                string qname = (_cat.Length > 0 ? _cat + "." : "") + parts.First().ToLower();
                var cmd = list.FirstOrDefault(c => c.QualifiedName == qname);
                if (cmd == null)
                {
                    Print("Command not found: " + qname);
                    continue;
                }
                cmd.Run(parts.Where((s, i) => i > 0).ToArray());
            }
        }

        public static string Prompt
        {
            get { return "devcom" + (_cat.Length > 0 ? "." + _cat : "") + " >"; }
        }

        public static string CurrentCategory
        {
            get { return _cat; }
            set { _cat = value; }
        }
    }

    public delegate void PrintCallback(string message, object[] args);
}
