using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devcom;

namespace DCTest
{
    [DevcomCategory]
    public class Commands
    {
        [Command("something", "Does something.", "do")]
        public static void DoSomething()
        {
            Console.WriteLine("Something.");
        }

        [Command("print", "Prints some text.")]
        public static void Print(params object[] message)
        {
            foreach(object msg in message)
            {
                Console.WriteLine(msg);
            }
        }

        [Command("printc", "Prints some colored text.")]
        public static void PrintColor(string colorName, params object[] message)
        {
            ConsoleColor color;
            if (!Enum.TryParse(colorName, true, out color)) return;
            Console.ForegroundColor = color;
            foreach (object msg in message)
            {
                Console.WriteLine(msg);
            }
            Console.ResetColor();
        }
    }
}
