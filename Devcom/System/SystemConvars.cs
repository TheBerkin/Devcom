using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Contains the default convars used by the Devcom engine.
    /// </summary>
    [DevcomCategory]
    public static class SystemConvars
    {
        private static bool _echoInput = false;

        /// <summary>
        /// Determines if input sent to the Devcom engine should be echoed in the output before execution.
        /// Alias: echo_input
        /// </summary>
        [Convar("echo_input", "Determines if input sent to the Devcom engine should be echoed in the output before execution.")]
        public static bool EchoInput
        {
            get { return _echoInput; }
            set { _echoInput = value; }
        }
    }
}
