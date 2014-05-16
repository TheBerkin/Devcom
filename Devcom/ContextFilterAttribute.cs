using System;
using System.Linq;

namespace DeveloperCommands
{
    /// <summary>
    /// Restricts availability of a command to certain context types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ContextFilterAttribute : Attribute
    {
        /// <summary>
        /// The type of filter to apply.
        /// </summary>
        internal ContextFilterType FilterType { get; private set; }
        /// <summary>
        /// The contexts that will be affected by the filter.
        /// </summary>
        internal Type[] ContextTypes { get; private set; }

        /// <summary>
        /// Creates a new ContextFilter attribute with the specified filter type and context types.
        /// </summary>
        /// <param name="filterType">The type of filter to apply</param>
        /// <param name="contextTypes">The contexts that will be affected by the filter.</param>
        public ContextFilterAttribute(ContextFilterType filterType, params Type[] contextTypes)
        {
            if (contextTypes != null)
            {
                var baseType = typeof (DevcomContext);
                if (contextTypes.Any(t => t != baseType && !t.IsSubclassOf(baseType)))
                {
                    throw new ArgumentException("Attempted to specify a non-context type in a context filter.");
                }
            }
            FilterType = filterType;
            ContextTypes = contextTypes;
        }

        internal ContextFilterInternal CreateFilterInternal()
        {
            return new ContextFilterInternal(FilterType, ContextTypes);
        }
    }
}
