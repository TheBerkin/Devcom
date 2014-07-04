using System;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a convar bound to an object instance.
    /// </summary>
    public sealed class ObjectConvar : Convar
    {
        private dynamic _value;
        private readonly Type _valueType;

        internal ObjectConvar(object value, string name, string desc, string cat, bool savable) : base(name, desc, cat, value, savable)
        {
            if (_value == null)
            {
                throw new ArgumentNullException("value", "The initial value cannot be null.");
            }
            _value = value;
            _valueType = value.GetType();
        }

        public override dynamic Value
        {
            get { return _value; }
            set
            {
                try
                {
                    _value = Util.ChangeType(value, _valueType);
                }
                catch
                {
                    _value = null;
                }
            }
        }
    }
}
