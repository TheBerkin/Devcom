using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
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
            Category = category;
        }

        public DevcomCategoryAttribute()
        {
            Category = "";
        }
    }
}
