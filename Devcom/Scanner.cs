using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        public static void SearchAssembly(Assembly ass, Dictionary<string, CommandDef> cmdlist, Dictionary<string, Convar> convars)
        {
            foreach(var cl in ass.GetTypes().Where(t => t.IsClass))           
            {
                string cat = "";
                ContextFilterInternal categoryFilter = null;
                bool found = false;
                foreach(var attr in cl.GetCustomAttributes())
                {
                    var attrCat = attr as CategoryAttribute;
                    var attrFilter = attr as ContextFilterAttribute;
                    if (attrCat != null)
                    {
                        cat = attrCat.Category;
                        found = true;
                    }
                    else if (attrFilter != null)
                    {
                        categoryFilter = attrFilter.CreateFilterInternal();
                    }
                }

                if (!found) continue;

                // Load commands
                foreach (var method in cl.GetMethods().Where(m => m.IsStatic && m.IsPublic))
                {
                    var attrs = method.GetCustomAttributes();
                    var attributes = attrs as Attribute[] ?? attrs.ToArray();
                    var cmdAttr = attributes.FirstOrDefault(attr => attr is CommandAttribute) as CommandAttribute;

                    if (cmdAttr == null) continue;
                    if (cmdlist.ContainsKey(Util.Qualify(cat, cmdAttr.Name))) continue;

                    var filterAttr = attributes.FirstOrDefault(attr => attr is ContextFilterAttribute) as ContextFilterAttribute;

                    var command = new CommandDef(method, cmdAttr.Name, cmdAttr.Description, cat,
                        categoryFilter ?? (filterAttr == null ? null : filterAttr.CreateFilterInternal()));

                    cmdlist[command.QualifiedName] = command;
                }

                // Load convars
                foreach (var convar in cl.GetProperties()
                            .Where(
                                p => p.GetGetMethod().IsStatic && p.GetGetMethod().IsPublic && p.GetSetMethod().IsPublic)
                            .SelectMany(p => p.GetCustomAttributes<ConvarAttribute>()
                                .Where(attr => !convars.ContainsKey(Util.Qualify(cat, attr.Name)))
                                .Select(attr => new PropertyConvar(p, attr.Name, attr.Description, cat, attr.DefaultValue))))
                {
                    convars[convar.QualifiedName] = convar;
                }

                foreach (var convar in cl.GetFields()
                    .Where(
                        f => f.IsStatic && f.IsPublic)
                    .SelectMany(f => f.GetCustomAttributes<ConvarAttribute>()
                        .Where(attr => !convars.ContainsKey(Util.Qualify(cat, attr.Name)))
                        .Select(attr => new FieldConvar(f, attr.Name, attr.Description, cat, attr.DefaultValue))))
                {
                    convars[convar.QualifiedName] = convar;
                }
            }
        }
    }
}
