using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Specifies a category to assign to all commands within a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute : Attribute
    {
        /// <summary>
        /// The category name.
        /// </summary>
        public string Category
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new DevcomCategory attribute.
        /// </summary>
        /// <param name="category">The category name.</param>
        public CategoryAttribute(string category)
        {
            if (!Util.IsValidName(category, "_-"))
            {
                throw new ArgumentException("Command categories can only contain letters, numbers, underscores, and dashes.");
            }
            Category = category;
        }

        /// <summary>
        /// Creates a new, empty DevcomCategory attribute.
        /// </summary>
        public CategoryAttribute()
        {
            Category = "";
        }
    }
}
