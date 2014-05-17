using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeveloperCommands
{
    internal static class ConvarConfig
    {
        public const string ConfigFile = "devcom.cfg";

        public static void Save()
        {
            try
            {
                using (var writer = new StreamWriter(ConfigFile))
                {
                    bool diff = true;
                    Convar prev = null;
                    foreach (var convar in Devcom.Convars.OrderBy(kvp => kvp.Key))
                    {
                        if (prev == null || diff)
                        {
                            diff = false;
                            if (convar.Value.Category.Length > 0) writer.Write("# " + convar.Value.Category);
                        }
                        prev = convar.Value;
                        writer.Write("{0}={1}\r\n", convar.Key, convar.Value.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Devcom.Print("Failed to save configuration: '" + ex.Message + "'");
            }
        }

        public static void Load()
        {
            if (!File.Exists(ConfigFile))
            {
                Devcom.Print("Couldn't find " + ConfigFile + ".");
                return;
            }

            try
            {
                using (var reader = new StreamReader(ConfigFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine().Trim();
                        if (line.StartsWith("#")) continue;
                        var match = Regex.Match(line, @"(^|[^#])(?<name>[\w-]+)\s*=(?<value>.*)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                        if (!match.Success) continue;
                        string name = match.Groups["name"].Value.ToLower();
                        string value = match.Groups["value"].Value.ToLower();
                        Convar convar;
                        if (!Devcom.Convars.TryGetValue(name, out convar)) continue;
                        convar.Value = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Devcom.Print("Failed to load " + ConfigFile + ": '" + ex.Message + "'");
            }
        }
    }
}
