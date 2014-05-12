using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devcom;

namespace DCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Devcom";
            DevcomEngine.Load();
            string cmdline;
            while(true)
            {
                Console.Write(DevcomEngine.Prompt);
                cmdline = Console.ReadLine();
                DevcomEngine.Input(null, cmdline);
            }
        }
    }
}
