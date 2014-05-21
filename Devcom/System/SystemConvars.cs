namespace DeveloperCommands
{
    /// <summary>
    /// Contains the default convars used by the Devcom engine.
    /// </summary>
    [Category]
    public static class SystemConvars
    {
        /// <summary>
        /// Determines if input sent to the Devcom engine should be echoed in the output before execution.
        /// Alias: echo_input
        /// </summary>
        [Convar("echo_input",
            "Determines if input sent to the Devcom engine should be echoed in the output before execution.",
            false)]
        public static bool EchoInput = false;

        /// <summary>
        /// Determines if exceptions raised by commands are handled or not. If false, the exception and stack trace will be printed to the output.
        /// Alias: throws
        /// </summary>
        [Convar("throws",
            "Determines if exceptions raised by commands are handled or not. If false, the exception and stack trace will be printed to the output.")]
        public static bool Throws = false;
    }
}
