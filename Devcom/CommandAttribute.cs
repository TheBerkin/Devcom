using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Used to instruct the Devcom engine to register methods marked with this attribute as commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The description given for the command.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new Command attribute with the specified name and description.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="desc">The description for the command.</param>
        public CommandAttribute(string name, string desc = "")
        {
            if (name.Any(c => !Char.IsLetterOrDigit(c) && !"_-+".Contains(c)))
            {
                throw new ArgumentException("Command names can only contain letters, numbers, underscores, dashes and plus symbols.");
            }
            Name = name.ToLower();
            Description = desc;
        }
    }
}
