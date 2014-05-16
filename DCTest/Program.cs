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
            var context = DevcomContext.DefaultAdmin;
            while(true)
            {
                Console.Write(context.Prompt);
                Devcom.SendCommand(context, Console.ReadLine());
            }
        }
    }
}
