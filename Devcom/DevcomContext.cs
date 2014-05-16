using System;
using System.Collections.Generic;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a state object that is passed to all Devcom commands, and can be extended to customize command behavior and redirect output.
    /// </summary>
    public class DevcomContext
    {
        internal static Dictionary<string, DevcomContext> ActiveContextList = new Dictionary<string, DevcomContext>();

        /// <summary>
        /// The default admin context used by the Devcom engine.
        /// </summary>
        public static readonly AdminContext DefaultAdmin = CreateDefaultAdminContext();

        /// <summary>
        /// The default base context used by the Devcom engine.
        /// </summary>
        public static readonly DevcomContext Default = CreateDefaultContext();

        private readonly string _name;
        private bool _disposed, _locked;
        private string _cat;

        /// <summary>
        /// Creates a new DevcomContext with the specified name, and registers it.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        public DevcomContext(string name)
        {
            if (!Util.IsValidName(name))
            {
                throw new ArgumentException("Context names can only contain letters, numbers, underscores, dashes and plus symbols.");
            }
            if (ActiveContextList.ContainsKey(name))
            {
                throw new InvalidOperationException("A context with this name already exists.");
            }
            _name = name;
            _cat = "";
            ActiveContextList[name] = this;
        }

        /// <summary>
        /// Searches for a context with the specified name.
        /// If the context is found, it will be the return value.
        /// If it is not found, the method will return null.
        /// </summary>
        /// <param name="name">The name of the context to search for (case-sensitive).</param>
        /// <returns></returns>
        public static DevcomContext GetContextByName(string name)
        {
            DevcomContext context;
            return !ActiveContextList.TryGetValue(name, out context) ? null : context;
        }

        /// <summary>
        /// Posts a string to the output of this context.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Post(string message)
        {
            Devcom.Print(message);
        }

        /// <summary>
        /// Posts an object's string value to the output of this context.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Post(object value)
        {
            Post(value.ToString());
        }

        /// <summary>
        /// Post a formatted string to the output of this context.
        /// </summary>
        /// <param name="message">The format string to pass.</param>
        /// <param name="args">The arguments to insert into the format string.</param>
        public void PostFormat(string message, params object[] args)
        {
            Post(String.Format(message, args));
        }

        /// <summary>
        /// Searches for a convar with the specified name and sends it to the 'convar' output parameter.
        /// If not found, a notification will be sent to the context.
        /// </summary>
        /// <param name="cvName">The name of the convar.</param>
        /// <param name="convar">The Convar object to send the result to.</param>
        /// <returns></returns>
        public bool RequestConvar(string cvName, out Convar convar)
        {
            if (Devcom.Convars.TryGetValue(Util.Qualify(Category, cvName), out convar)) return true;
            Post("Convar '" + cvName + "' not found.");
            return false;
        }

        internal void PostCommandNotFound(string commandName)
        {
            Post("Command not found: '" + commandName + "'");
        }

        /// <summary>
        /// The name of this context.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The current category.
        /// </summary>
        public string Category
        {
            get { return _cat; }
            set { _cat = value; }
        }

        /// <summary>
        /// The current prompt string.
        /// </summary>
        public string Prompt
        {
            get { return _name + (_cat.Length > 0 ? "." + _cat : "") + " > "; }
        }

        internal static AdminContext CreateDefaultAdminContext()
        {
            return new AdminContext("devcom")
            {
                _locked = true
            };
        }

        internal static DevcomContext CreateDefaultContext()
        {
            return new DevcomContext("devcom")
            {
                _locked = true
            };
        }

        /// <summary>
        /// Removes this context from the active context list.
        /// </summary>
        public void Unregister()
        {
            if (_disposed || _locked) return;
            _disposed = true;
            ActiveContextList.Remove(Name);
        }
    }
}
