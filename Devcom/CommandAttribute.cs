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

        public string Category
        {
            get;
            private set;
        }

        public CommandAttribute(string name, string desc = "", string category = "")
        {
            Name = name;
            Description = desc;
            Category = category;
        }
    }
}
