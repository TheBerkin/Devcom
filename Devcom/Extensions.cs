using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcom
{
    public static class Extensions
    {
        public static IEnumerable<string> ParseParams(this string str)
        {
            var list = new List<string>();
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
                        list.Add(sb.ToString());
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
                    else if (spc > 0 && sb.Length > 0)
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
                            list.Add(sb.ToString());
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
                                list.Add(sb.ToString());
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
                list.Add(sb.ToString());
            }

            return list;
        }
    }
}
