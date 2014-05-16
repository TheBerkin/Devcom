using System;
using System.Linq;

namespace DeveloperCommands
{
    internal class ContextFilterInternal
    {
        public ContextFilterType FilterType { get; set; }
        public Type[] ContextTypes { get; set; }

        public ContextFilterInternal(ContextFilterType filterType, Type[] contextTypes)
        {
            FilterType = filterType;
            ContextTypes = contextTypes;
        }

        public static bool Test(Type contextType, ContextFilterInternal contextFilter)
        {
            if (contextType == null) return false; // Fail for null context
            if (contextFilter == null) return true; // Pass for no filter

            switch (contextFilter.FilterType)
            {
                case ContextFilterType.Blacklist:
                    return !contextFilter.ContextTypes.Contains(contextType);
                case ContextFilterType.Whitelist:
                    return contextFilter.ContextTypes.Contains(contextType);
                default:
                    return true;
            }
        }
    }
}
