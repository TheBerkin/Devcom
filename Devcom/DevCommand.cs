using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Devcom
{
    public class DevCommand
    {
        private readonly string _name, _desc, _category;
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _paramList;
        private readonly bool hasParamsArgument;

        public string Name
        {
            get { return _name; }
        }

        public string Category
        {
            get { return _category; }
        }

        public string QualifiedName
        {
            get { return Category.Length > 0 ? String.Concat(Category, ".", Name) : Name; }
        }

        public DevCommand(MethodInfo method, string name, string desc, string category)
        {
            _method = method;

            var pl = _method.GetParameters();
            if (pl.Any())
            {
                hasParamsArgument = pl.Last().GetCustomAttribute<ParamArrayAttribute>() != null;
            }

            _paramList = pl;
            _name = name;
            _desc = desc;
            _category = category;
        }

        // TODO: Handle cases where parameters precede a params argument
        public bool Run(params string[] args)
        {
            int argc = args.Length;
            int paramc = _paramList.Length;
            try
            {
                object[] boxed;
                if (hasParamsArgument)
                {
                    boxed = new object[argc];
                }
                else
                {
                    if (args.Length != _paramList.Length)
                    {
                        Console.WriteLine("Parameter mismatch.");
                        return false;
                    }
                    boxed = new object[_paramList.Length];
                }

                for (int i = 0; i < argc; i++)
                {
                    float fl;
                    if (_paramList[i >= paramc ? paramc - 1 : i].ParameterType != typeof(string))
                    {
                        if (float.TryParse(args[i], out fl))
                        {
                            boxed[i] = fl;
                            continue;
                        }
                    }

                    boxed[i] = args[i];                    
                }
                                
                List<object> argsFormatted = boxed.Take(hasParamsArgument ? paramc - 1 : paramc).ToList();
                if (hasParamsArgument)
                {
                    argsFormatted.Add(args.Where((o, i) => i >= paramc - 1).ToArray());
                }

                _method.Invoke(null, argsFormatted.ToArray());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error:\n{0}", ex);
                return false;
            }
            return true;
        }
    }
}
