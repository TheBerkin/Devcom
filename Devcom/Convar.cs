using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a variable that can be edited from within Devcom.
    /// </summary>
    public class Convar
    {
        private readonly string _name, _desc, _cat;
        private readonly PropertyInfo _property;
        private object _valueObject;

        internal Convar(PropertyInfo property, string name, string desc, string cat)
        {
            if (property != null)
            {
                if (!property.GetGetMethod().IsStatic)
                {
                    throw new ArgumentException("Convar creation failed: The property '" + property.Name + "' is not static.");
                }
                _property = property;
            }
            else
            {
                _property = null;
                _valueObject = null;
            }
            _name = name;
            _desc = desc;
            _cat = cat;
        }

        /// <summary>
        /// The name of the convar.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The category of the convar.
        /// </summary>
        public string Category
        {
            get { return _cat; }
        }

        /// <summary>
        /// The qualified name of the convar.
        /// </summary>
        public string QualifiedName
        {
            get { return Util.Qualify(_cat, _name); }
        }

        /// <summary>
        /// The description of the convar.
        /// </summary>
        public string Description
        {
            get { return _desc; }
        }

        /// <summary>
        /// The value of the convar.
        /// </summary>
        public object Value
        {
            get
            {
                return _property == null ? _valueObject : _property.GetValue(null);
            }
            set
            {
                if (_property == null)
                {
                    _valueObject = value;
                }
                else
                {
                    try
                    {
                        _property.SetValue(null, Convert.ChangeType(value, _property.PropertyType));
                    }
                    catch
                    {
                        _property.SetValue(null, null);
                    }
                }
            }
        }

        /// <summary>
        /// Registers a new convar and returns the resulting Convar object. If a convar with the specified name already exists, the old one will be returned.
        /// </summary>
        /// <param name="convarName">The name of the convar.</param>
        /// <param name="value">The initial value of the convar.</param>
        /// <param name="cat">The category under which to put the convar.</param>
        /// <param name="desc">The description of the convar.</param>
        /// <returns></returns>
        public static Convar Register(string convarName, object value, string cat = "", string desc = "")
        {
            Convar convar;
            if (Devcom.Convars.TryGetValue(Util.Qualify(cat, convarName), out convar))
            {
                return convar;
            }
            convar = new Convar(null, convarName, desc, cat);
            convar.Value = value;
            return convar;
        }
    }
}
