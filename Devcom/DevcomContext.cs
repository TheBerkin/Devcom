using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    public class DevcomContext
    {
        internal static List<string> ActiveContextList = new List<string>();

        public static readonly DevcomContext Default = new DevcomContext("Default");

        private readonly string _name;

        protected DevcomContext(string name)
        {
            if (ActiveContextList.Contains(name))
            {
                throw new InvalidOperationException("A context with this name already exists.");
            }
            _name = name;
            ActiveContextList.Add(name);
        }

        public virtual void Post(string message)
        {
            Devcom.Print(message);
        }

        public virtual void Post(object value)
        {
            Post(value.ToString());
        }

        public void PostFormat(string message, params object[] args)
        {
            Post(String.Format(message, args));
        }

        public string Name
        {
            get { return _name; }
        }

        public static DevcomContext CreateContext(string name)
        {
            return new DevcomContext(name);
        }
    }
}
