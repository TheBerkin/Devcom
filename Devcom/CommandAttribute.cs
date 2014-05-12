using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public CommandAttribute(string name, string desc = "")
        {
            if (name.Any(c => !Char.IsLetterOrDigit(c) && !"_-+".Contains(c)))
            {
                throw new ArgumentException("Command names can only contain letters, numbers, underscores, dashes and plus symbols.");
            }
            Name = name;
            Description = desc;
        }
    }
}
