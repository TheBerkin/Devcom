using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DevcomCategoryAttribute : Attribute
    {
        public string Category
        {
            get;
            private set;
        }

        public DevcomCategoryAttribute(string category)
        {
            if (category.Any(c => !Char.IsLetterOrDigit(c) && !"_-+".Contains(c)))
            {
                throw new ArgumentException("Command categories can only contain letters, numbers, underscores, and dashes.");
            }
            Category = category;
        }

        public DevcomCategoryAttribute()
        {
            Category = "";
        }
    }
}
