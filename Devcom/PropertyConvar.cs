using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a convar bound to a static property.
    /// </summary>
    public sealed class PropertyConvar : Convar
    {
        private readonly PropertyInfo _property;

        internal PropertyConvar(PropertyInfo property, string name, string desc, string cat, object defaultValue, bool savable) : base(name, desc, cat, defaultValue, savable)
        {
            if (property != null)
            {
                if (!property.GetGetMethod().IsStatic)
                {
                    throw new ArgumentException("Convar creation failed: The property '" + property.Name + "' is not static.");
                }
                _property = property;
                if (defaultValue != null)
                {
                    _property.SetValue(null, defaultValue);
                }
            }
            else
            {
                throw new ArgumentNullException("property");
            }
        }

        public override dynamic Value
        {
            get { return _property.GetValue(null); }
            set
            {
                try
                {
                    _property.SetValue(null, Util.ChangeType(value, _property.PropertyType));
                }
                catch
                {
                    _property.SetValue(null, null);
                }
            }
        }
    }
}
