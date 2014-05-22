using System.Reflection;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a variable that can be edited from within Devcom.
    /// </summary>
    public abstract class Convar
    {
        private readonly string _name, _desc, _cat;
        protected internal readonly dynamic _defaultValue;

        internal Convar(string name, string desc, string cat, object defaultValue)
        {
            _defaultValue = defaultValue;
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
        /// The default value of the convar.
        /// </summary>
        public dynamic DefaultValue
        {
            get { return _defaultValue; }
        }

        /// <summary>
        /// The value of the convar.
        /// </summary>
        public abstract dynamic Value { get; set; }

        /// <summary>
        /// Registers a new object-based convar.
        /// </summary>
        /// <param name="value">The initial and default value of the convar.</param>
        /// <param name="name">The name of the convar.</param>
        /// <param name="description">The descriptiono of the convar.</param>
        /// <param name="category">The category the convar belongs to.</param>
        /// <returns>Returns the created convar, or null if a convar with the specified name already exists.</returns>
        public static ObjectConvar CreateConvar(object value, string name, string description, string category)
        {
            if (Devcom.Convars.ContainsKey(Util.Qualify(category, name))) return null;
            var convar = new ObjectConvar(value, name, description, category);
            Devcom.Convars[convar.QualifiedName] = convar;
            return convar;
        }

        /// <summary>
        /// Registers a new property-based convar.
        /// </summary>
        /// <param name="property">The static property to associate with the convar.</param>
        /// <param name="name">The name of the convar.</param>
        /// <param name="description">The descriptiono of the convar.</param>
        /// <param name="category">The category the convar belongs to.</param>
        /// <param name="defaultValue">The default value to assign to the convar.</param>
        /// <returns>Returns the created convar, or null if a convar with the specified name already exists.</returns>
        public static PropertyConvar CreateConvar(PropertyInfo property, string name, string description, string category, object defaultValue = null)
        {
            if (Devcom.Convars.ContainsKey(Util.Qualify(category, name))) return null;
            var convar = new PropertyConvar(property, name, description, category, defaultValue);
            Devcom.Convars[convar.QualifiedName] = convar;
            return convar;
        }

        /// <summary>
        /// Registers a new field-based convar.
        /// </summary>
        /// <param name="field">The static field to associate with the convar.</param>
        /// <param name="name">The name of the convar.</param>
        /// <param name="description">The descriptiono of the convar.</param>
        /// <param name="category">The category the convar belongs to.</param>
        /// <param name="defaultValue">The default value to assign to the convar.</param>
        /// <returns>Returns the created convar, or null if a convar with the specified name already exists.</returns>
        public static FieldConvar CreateConvar(FieldInfo field, string name, string description, string category, object defaultValue = null)
        {
            if (Devcom.Convars.ContainsKey(Util.Qualify(category, name))) return null;
            var convar = new FieldConvar(field, name, description, category, defaultValue);
            Devcom.Convars[convar.QualifiedName] = convar;
            return convar;
        }
    }
}
