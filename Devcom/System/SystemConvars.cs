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
    }
}
