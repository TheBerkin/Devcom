using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperCommands
{
    internal class CommandDef
    {
        private readonly string _name, _desc, _category, _paramHelpString;
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _paramList;
        private readonly bool _hasParamsArgument;
        private readonly Type _contextType;
        private readonly ContextFilterInternal _filter;

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _desc; }
        }

        public string ParamHelpString
        {
            get { return _paramHelpString; }
        }

        public string Category
        {
            get { return _category; }
        }

        public string QualifiedName
        {
            get { return Util.Qualify(_category, _name); }
        }

        public ContextFilterInternal ContextFilter
        {
            get { return _filter; }
        }

        public Type ContextType
        {
            get { return _contextType; }
        }

        public CommandDef(MethodInfo method, string name, string desc, string category, ContextFilterInternal filter = null)
        {
            _method = method;
            _filter = filter;

            var pl = _method.GetParameters();
            if (pl.Any())
            {
                _contextType = pl[0].ParameterType;
                var type = typeof (Context);
                if (!_contextType.IsSubclassOf(type) && type != _contextType)
                {
                    throw new ArgumentException("Command creation failed: Method '" + method.Name + "' requires a DevcomContext as the first parameter.");
                }
                _hasParamsArgument = pl.Last().GetCustomAttribute<ParamArrayAttribute>() != null;
            }
            else
            {
                throw new ArgumentException("Command creation failed: Method '" + method.Name + "' requires a DevcomContext as the first parameter.");
            }

            _paramList = pl;
            _name = name;
            _desc = desc;
            _category = category;

            _paramHelpString = _paramList.Length > 1
                ? _paramList.Where((p, i) => i > 0)
                .Select(p => "<" + p.Name + (p.IsDefined(typeof(ParamArrayAttribute)) || p.IsOptional ? "(optional)>" : ">"))
                .Aggregate((accum, pname) => accum + " " + pname)
                : "";
        }

        public bool Run(Context context, params string[] args)
        {
            var currentContextType = context.GetType();

            // Check context type compatability
            if ((!currentContextType.IsSubclassOf(_contextType) && currentContextType != _contextType) || !ContextFilterInternal.Test(currentContextType, _filter))
            {
                context.PostCommandNotFound(QualifiedName);
                return false;
            }

            int argc = args.Length;
            int paramc = _paramList.Length - 1;
            try
            {
                object[] boxed;
                if (_hasParamsArgument)
                {
                    boxed = new object[argc];
                }
                else
                {
                    if (args.Length != paramc)
                    {
                        context.Post("Parameter count mismatch.");
                        return false;
                    }
                    boxed = new object[paramc];
                }

                // Convert parameters to the proper types
                for (int i = 0; i < argc; i++)
                {
                    var paramType = _paramList[(i >= paramc ? paramc - 1 : i) + 1].ParameterType;
                    try
                    {
                        boxed[i] = Convert.ChangeType(args[i], paramType);
                    }
                    catch
                    {
                        boxed[i] = null;
                    }               
                }
                
                var argsFormatted = new List<object> { context };

                // Add all arguments except for any marked as 'params'
                argsFormatted.AddRange(boxed.Take(_hasParamsArgument ? paramc - 1 : paramc));

                // Insert params argument as an array (it needs to be represented as a single object)
                if (_hasParamsArgument)
                {
                    argsFormatted.Add(args.Where((o, i) => i >= paramc - 1).ToArray());
                }

                // Call the method with our parameters
                _method.Invoke(null, argsFormatted.ToArray());
            }
            catch(Exception ex)
            {
                context.Post("Error: " + ex);
                return false;
            }
            return true;
        }
    }
}
