using System;
using System.Linq;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a set of rules used to limit access to a command to certain contexts.
    /// </summary>
    public class ContextFilter
    {
        /// <summary>
        /// The type of the filter.
        /// </summary>
        public ContextFilterType FilterType { get; set; }
        /// <summary>
        /// The filter policy that determines which types are affected by the filter.
        /// </summary>
        public ContextFilterPolicy FilterPolicy { get; set; }
        /// <summary>
        /// The context types affected by the filter.
        /// </summary>
        public Type[] ContextTypes { get; set; }

        /// <summary>
        /// Creates a new ContextFilter with the specified rules.
        /// </summary>
        /// <param name="filterType">The type of the filter.</param>
        /// <param name="filterPolicy">The filter policy that determines which types are affected by the filter.</param>
        /// <param name="contextTypes">The context types affected by the filter.</param>
        public ContextFilter(ContextFilterType filterType, ContextFilterPolicy filterPolicy, Type[] contextTypes)
        {
            FilterType = filterType;
            FilterPolicy = filterPolicy;
            ContextTypes = contextTypes;
        }

        internal static bool Test(Type contextType, ContextFilter contextFilter)
        {
            if (contextType == null) return false; // Fail for null context
            if (contextFilter == null) return true; // Pass for no filter

            bool listed = contextFilter.ContextTypes.Any(type =>
            {
                switch (contextFilter.FilterPolicy)
                {
                    case ContextFilterPolicy.IncludeDerived:
                        return type == contextType || type.IsSubclassOf(contextType);
                    default: // ContextFilterPolicy.Strict
                        return type == contextType;
                }
            });

            return contextFilter.FilterType == ContextFilterType.Whitelist ? listed : !listed;
        }
    }
}
