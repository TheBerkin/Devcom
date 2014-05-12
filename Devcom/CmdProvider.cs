using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Devcom
{
    public static class CmdProvider
    {
        public static List<DevCommand> FindThemAll()
        {
            var list = new List<DevCommand>();
            var ass = Assembly.GetExecutingAssembly();
            list.AddRange(SearchAssembly(ass));
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                list.AddRange(SearchAssembly(asm));
                foreach(var asmr in asm.GetReferencedAssemblies().Select(Assembly.Load))
                {
                    list.AddRange(SearchAssembly(asmr));
                }
            }
            return list;
        }

        private static Dictionary<string, DevCommand> SearchAssembly(Assembly ass)
        {
            var cmdlist = new Dictionary<string, DevCommand>();
            foreach(var cl in ass.GetTypes().Where(t => t.IsClass && t.IsVisible))           
            {
                string cat = "";
                bool found = false;
                foreach(var attr in cl.GetCustomAttributes<DevcomCategoryAttribute>())
                {
                    cat = attr.Category;
                    found = true;
                    break;
                }

                if (!found) continue;

                foreach(var method in cl.GetMethods()
                .Where(m => m.IsStatic && m.IsPublic))
                {
                    foreach(var attr in method.GetCustomAttributes<CommandAttribute>())
                    {
                        Console.WriteLine("Found function {0} ({1})", attr.Name, method.Name);
                        var command = new DevCommand(method, attr.Name, attr.Description, cat);
                        cmdlist[command.QualifiedName.ToLower()] = command;
                    }
                }
            }
            return cmdlist;
        }
    }
}
