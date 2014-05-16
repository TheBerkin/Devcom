using System;

namespace DeveloperCommands
{
    /// <summary>
    /// Used to mark a static property as a convar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConvarAttribute : Attribute
    {
        /// <summary>
        /// The name of the convar.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The description of the convar.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The default value of the convar.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Creates a new Convar attribute using the specified name and description.
        /// </summary>
        /// <param name="name">The name of the convar.</param>
        /// <param name="desc">The description of the convar.</param>
        public ConvarAttribute(string name, string desc = "", object defaultValue = null)
        {
            if (!Util.IsValidName(name, "-_"))
            {
                throw new ArgumentException("Convar names can only contain letters, numbers, underscores and dashes.");
            }
            Name = name.ToLower();
            Description = desc;
            DefaultValue = defaultValue;
        }
    }
}
