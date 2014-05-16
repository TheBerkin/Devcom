namespace DeveloperCommands
{
    /// <summary>
    /// Specifies the context filtering technique to use for a command.
    /// </summary>
    public enum ContextFilterType
    {
        /// <summary>
        /// Block all contexts specified in a list.
        /// </summary>
        Blacklist,
        /// <summary>
        /// Allow only contexts specified in a list.
        /// </summary>
        Whitelist
    }
}
