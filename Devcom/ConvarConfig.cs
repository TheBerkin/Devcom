using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeveloperCommands
{
    /// <summary>
    /// Provides methods for saving and loading convars from configuration files.
    /// </summary>
    public static class ConvarConfig
    {
        /// <summary>
        /// The default file name for the convar configuration file.
        /// </summary>
        public const string DefaultConfigFile = "devcom.cfg";

        /// <summary>
        /// Saves all current convar values to the specified path.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        public static void SaveConvars(string path = DefaultConfigFile)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    string prev = "";
                    foreach (var convar in Devcom.Convars.Where(pair => pair.Value.Savable).OrderBy(kvp => kvp.Key))
                    {
                        if (prev != "" && convar.Value.Category != prev)
                        {
                            writer.Write("#   -- " + convar.Value.Category + " --");
                        }
                        prev = convar.Value.Category;
                        writer.Write("{0}={1}\r\n", convar.Key, convar.Value.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Devcom.Print("Failed to save "+path+": '" + ex.Message + "'");
            }
        }

        /// <summary>
        /// Loads convars from a file into memory.
        /// </summary>
        /// <param name="path">The path to the configuration file to load.</param>
        public static void LoadConvars(string path = DefaultConfigFile)
        {
            if (!File.Exists(DefaultConfigFile))
            {
                Devcom.Print("Couldn't find " + path + ".");
                return;
            }

            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine().Trim();
                        if (line.StartsWith("#")) continue;
                        var match = Regex.Match(line, @"(^|[^#])(?<name>[\w-]+)\s*=(?<value>.*)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                        if (!match.Success) continue;
                        var name = match.Groups["name"].Value.ToLower();
                        var value = match.Groups["value"].Value;
                        Convar convar;
                        if (!Devcom.Convars.TryGetValue(name, out convar)) continue;
                        convar.Value = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Devcom.Print("Failed to load " + path + ": '" + ex.Message + "'");
            }
        }
    }
}
