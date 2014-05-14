using System;
using DeveloperCommands;

namespace DCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Devcom";
            Devcom.Load();
            while(true)
            {
                Console.Write(DevcomContext.Default.Prompt);
                Devcom.SendCommand(Console.ReadLine());
            }
        }
    }
}
