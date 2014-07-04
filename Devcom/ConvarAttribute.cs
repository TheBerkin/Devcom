using System;

namespace DeveloperCommands
{
    /// <summary>
    /// Used to mark a static property as a convar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
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
        /// <param name="Name">The name of the convar.</param>
        /// <param name="Description">The description of the convar.</param>
        /// <param name="DefaultValue">The default value of the convar.</param>
        /// <param name="Savable">Determines if the convar should be allowed to have its value saved in configuration files.</param>
        public ConvarAttribute(string Name, string Description = "", object DefaultValue = null, bool Savable = true)
        {
            if (!Util.IsValidName(Name, "-_"))
            {
                throw new ArgumentException("Convar names can only contain letters, numbers, underscores and dashes.");
            }
            this.Name = Name.ToLower();
            this.Description = Description;
            this.DefaultValue = DefaultValue;
            this.Savable = Savable;
        }

        /// <summary>
        /// Determines if the convar should be allowed to have its value saved in configuration files.
        /// </summary>
        public bool Savable { get; private set; }
    }
}
