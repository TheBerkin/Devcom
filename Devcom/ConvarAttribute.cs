using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConvarAttribute : Attribute
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

        public ConvarAttribute(string name, string desc = "")
        {
            Name = name;
            Description = desc;
        }
    }
}
