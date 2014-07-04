﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a convar bound to a static field.
    /// </summary>
    public sealed class FieldConvar : Convar
    {
        private readonly FieldInfo _field;
        internal FieldConvar(FieldInfo field, string name, string desc, string cat, object defaultValue, bool savable) : base(name, desc, cat, defaultValue, savable)
        {
            if (field != null)
            {
                if (!field.IsStatic)
                {
                    throw new ArgumentException("Convar creation failed: The field '" + field.Name + "' is not static.");
                }
                _field = field;
                if (defaultValue != null)
                {
                    _field.SetValue(null, defaultValue);
                }
            }
            else
            {
                throw new ArgumentNullException("field");
            }
        }

        /// <summary>
        /// The value of the convar.
        /// </summary>
        public override dynamic Value
        {
            get { return _field.GetValue(null); }
            set
            {
                try
                {
                    _field.SetValue(null, Util.ChangeType(value, _field.FieldType));
                }
                catch
                {
                    _field.SetValue(null, null);
                }
            }
        }
    }
}
