using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeveloperCommands
{
    internal static class Extensions
    {
        private static readonly Dictionary<char, char> _escapeChars = new Dictionary<char, char>()
        {
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
            {'b', '\b'},
            {'f', '\f'},
            {'v', '\v'},
            {'0', '\0'}
        };

        public static IEnumerable<string> ParseParams(this string str)
        {
            str = Regex.Replace(str, @"\\((?<u>u(?<hex>[a-fA-F0-9]{1,4}))|(?<c>\S))", m =>
            {
                var c = m.Groups["c"].Value;
                var u = m.Groups["u"].Value;
                if (u.Length > 0)
                {
                    return ((char)Convert.ToInt32(m.Groups["hex"].Value, 16)).ToString();
                }
                char ec;
                return _escapeChars.TryGetValue(c[0], out ec) ? ec.ToString() : m.Value;
            }, RegexOptions.ExplicitCapture);
            var sb = new StringBuilder();
            int spc = 0;
            bool quote = false;
            bool constant = false;
            bool escape = false;
            foreach(char c in str)
            {
                if (escape)
                {
                    sb.Append(c);
                    escape = false;
                    continue;
                }

                if(c == '\\')
                {
                    escape = true;
                    continue;
                }

                if (c == '>')
                {
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }
                    constant = true;
                    continue;
                }

                if (constant)
                {
                    if (c == ' ' && sb.Length == 0)
                    {
                        spc++;
                        continue;
                    }

                    if (spc > 0 && sb.Length > 0)
                    {
                        sb.Append(new String(' ', spc));
                        spc = 0;
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        quote = !quote;
                        if (!quote)
                        {
                            yield return sb.ToString();
                            sb.Clear();
                        }
                        continue;
                    }

                    if (c == ' ')
                    {
                        if (!quote)
                        {
                            if (sb.Length > 0)
                            {
                                yield return sb.ToString();
                                sb.Clear();
                            }
                            continue;
                        }
                    }
                }

                sb.Append(c);
                spc = 0;
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }
    }
}
