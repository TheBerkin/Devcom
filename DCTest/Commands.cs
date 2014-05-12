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
        [Command("printc", "Prints some colored text.")]
        public static void PrintColor(DevcomContext context, string colorName, params object[] message)
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

        [Command("add", "Adds two numbers. Because why not.")]
        public static void Add(DevcomContext context, float a, float b)
        {
            DevcomEngine.Print(a + b);
        }
    }
}
