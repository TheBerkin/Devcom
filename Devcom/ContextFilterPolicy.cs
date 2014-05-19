namespace DeveloperCommands
{
    /// <summary>
    /// Describes how a context filter evaluates types.
    /// </summary>
    public enum ContextFilterPolicy
    {
        /// <summary>
        /// Affect only context types explicitly listed in the filter.
        /// </summary>
        Strict,
        /// <summary>
        /// Affect listed context types and any types that derive from them.
        /// </summary>
        IncludeDerived
    }
}
