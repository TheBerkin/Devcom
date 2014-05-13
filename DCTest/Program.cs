using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperCommands;

namespace DCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Devcom";
            Devcom.Load();
            string cmdline;
            while(true)
            {
                Console.Write(Devcom.Prompt);
                cmdline = Console.ReadLine();
                Devcom.SendCommand(null, cmdline);
            }
        }
    }
}
