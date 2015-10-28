using System;
using DeveloperCommands;

namespace DCTest
{
    [Category]
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Devcom";
            Devcom.Load();
            var context = Context.DefaultAdmin;
            while(true)
            {
                Console.Write(context.Prompt);
                Devcom.Submit(context, Console.ReadLine());
            }
        }
    }
}
