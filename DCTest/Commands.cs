using System;
using DeveloperCommands;

namespace DCTest
{
    [DevcomCategory]
    public class Commands
    {
        [Command("printc", "Prints some colored text.")]
        public static void PrintColor(DevcomContext context, string colorName, params object[] message)
        {
            ConsoleColor color;
            if (!Enum.TryParse(colorName, true, out color))
            {
                color = ConsoleColor.Gray;
            }
            
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
            Devcom.Print(a + b);
        }

        [Command("clear", "Clears the console.")]
        public static void Clear(DevcomContext context)
        {
            Console.Clear();
        }

        [Command("quit", "Closes the application.")]
        public static void Quit(DevcomContext context)
        {
            Environment.Exit(0);
        }
    }
}
