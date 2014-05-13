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
                Console.Write(Devcom.Prompt);
                Devcom.SendCommand(null, Console.ReadLine());
            }
        }
    }
}
