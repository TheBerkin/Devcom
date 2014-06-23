using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperCommands
{
    /// <summary>
    /// Contains reusable translated arguments for executing one or more commands.
    /// </summary>
    public sealed class Call
    {
        private readonly CallArgs[] _calls;

        public static readonly Call None = new Call(null);

        internal Call(CallArgs[] calls)
        {
            _calls = calls;
        }

        /// <summary>
        /// Execute the call.
        /// </summary>
        public void Invoke()
        {
            if (_calls == null) return;

            foreach (var call in _calls)
            {
                call.Invoke();
            }
        }
    }

    internal class CallArgs
    {
        private readonly MethodInfo _method;
        private readonly object[] _args;

        public CallArgs(MethodInfo method, IEnumerable<object> args)
        {
            _method = method;
            _args = args.ToArray();
        }

        public void Invoke()
        {
            _method.Invoke(null, _args);
        }
    }
}
