using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Devcom
{
    internal static class CmdProvider
    {
        public static Dictionary<string, CommandDef> FindThemAll()
        {
            var list = new Dictionary<string, CommandDef>();
            SearchAssembly(Assembly.GetExecutingAssembly(), list);
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                SearchAssembly(asm, list);
                foreach(var asmr in asm.GetReferencedAssemblies().Select(Assembly.Load))
                {
                    SearchAssembly(asmr, list);
                }
            }
            return list;
        }

        private static void SearchAssembly(Assembly ass, Dictionary<string, CommandDef> cmdlist)
        {
            foreach(var cl in ass.GetTypes().Where(t => t.IsClass))           
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

                foreach (var command in cl.GetMethods()
                    .Where(m => m.IsStatic && m.IsPublic).SelectMany(method => method.GetCustomAttributes<CommandAttribute>().Where(attr => !cmdlist.ContainsKey(Util.Qualify(cat, attr.Name))).Select(attr => new CommandDef(method, attr.Name, attr.Description, cat))))
                {
                    cmdlist[command.QualifiedName.ToLower()] = command;
                }
            }
        }
    }
}
