namespace DeveloperCommands
{
    /// <summary>
    /// The base context type for accessing system commands.
    /// </summary>
    public class AdminContext : Context
    {
        /// <summary>
        /// Creates a new AdminContext with the specified name.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        public AdminContext(string name) : base(name + "-admin")
        {
        }
    }
}
