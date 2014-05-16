using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperCommands
{
    internal static class Scanner
    {
        public static void FindAllDefs(Dictionary<string, CommandDef> commands, Dictionary<string, Convar> convars)
        {
            SearchAssembly(Assembly.GetExecutingAssembly(), commands, convars);
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                SearchAssembly(asm, commands, convars);
                foreach(var asmr in asm.GetReferencedAssemblies().Select(Assembly.Load))
                {
                    SearchAssembly(asmr, commands, convars);
                }
            }
        }

        private static void SearchAssembly(Assembly ass, Dictionary<string, CommandDef> cmdlist, Dictionary<string, Convar> convars)
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
                    .Where(m => m.IsStatic && m.IsPublic)
                    .SelectMany(method => method.GetCustomAttributes<CommandAttribute>()
                        .Where(attr => !cmdlist.ContainsKey(Util.Qualify(cat, attr.Name)))
                        .Select(attr => new CommandDef(method, attr.Name, attr.Description, cat))))
                {
                    cmdlist[command.QualifiedName.ToLower()] = command;
                }

                foreach (var convar in cl.GetProperties()
                            .Where(
                                p => p.GetGetMethod().IsStatic && p.GetGetMethod().IsPublic && p.GetSetMethod().IsPublic)
                            .SelectMany(p => p.GetCustomAttributes<ConvarAttribute>()
                                .Where(attr => !convars.ContainsKey(Util.Qualify(cat, attr.Name)))
                                .Select(attr => new Convar(p, attr.Name, attr.Description, cat, attr.DefaultValue))))
                {
                    convars[convar.QualifiedName.ToLower()] = convar;
                }
            }
        }
    }
}
