using System;

namespace DeveloperCommands
{
    public sealed class ObjectConvar : Convar
    {
        private object _value;
        private readonly Type _valueType;

        public ObjectConvar(object value, string name, string desc, string cat) : base(name, desc, cat, value)
        {
            if (_value == null)
            {
                throw new ArgumentNullException("value", "The initial value cannot be null.");
            }
            _value = value;
            _valueType = value.GetType();
        }

        public override object Value
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
