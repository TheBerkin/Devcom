﻿using System;
using System.Collections.Generic;

namespace DeveloperCommands
{
    /// <summary>
    /// Represents a state object that is passed to all Devcom commands, and can be extended to customize command behavior and redirect output.
    /// </summary>
    public class Context
    {
        internal static Dictionary<string, Context> ActiveContextList = new Dictionary<string, Context>();

        /// <summary>
        /// The default admin context used by the Devcom engine.
        /// </summary>
        public static readonly AdminContext DefaultAdmin = CreateDefaultAdminContext();

        /// <summary>
        /// The default base context used by the Devcom engine.
        /// </summary>
        public static readonly Context Default = CreateDefaultContext();

        private readonly string _name;
        private bool _disposed, _locked;
        private string _cat;

        /// <summary>
        /// Creates a new DevcomContext with the specified name, and registers it.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        public Context(string name)
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
        public static Context GetContextByName(string name)
        {
            Context context;
            return !ActiveContextList.TryGetValue(name, out context) ? null : context;
        }

        /// <summary>
        /// Posts a string to the output of this context.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Notify(string message)
        {
            Devcom.Log(message);
        }

        /// <summary>
        /// Posts an object's string value to the output of this context.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Notify(object value)
        {
            Notify(value.ToString());
        }

        /// <summary>
        /// Post a formatted string to the output of this context.
        /// </summary>
        /// <param name="message">The format string to pass.</param>
        /// <param name="args">The arguments to insert into the format string.</param>
        public void NotifyFormat(string message, params object[] args)
        {
            Notify(String.Format(message, args));
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
            Notify("Convar '" + cvName + "' not found.");
            return false;
        }

        /// <summary>
        /// Searches for a numeric convar with the specified name and sends it to the 'convar' output parameter.
        /// If not found, a notification will be sent to the context.
        /// </summary>
        /// <param name="cvName">The name of the convar.</param>
        /// <param name="convar">The Convar object to send the result to.</param>
        /// <returns></returns>
        public bool RequestNumericConvar(string cvName, out Convar convar)
        {
            convar = null;
            Convar cv;
            if (!Devcom.Convars.TryGetValue(Util.Qualify(Category, cvName), out cv))
            {
                Notify("Convar '" + cvName + "' not found.");
                return false;
            }
            if (!Util.IsNumericType(cv.Value))
            {
                Notify("Convar '" + cvName + "' is not a numeric type.");
                return false;
            }
            convar = cv;
            return true;
        }

        internal void PostCommandNotFound(string commandName)
        {
            Notify("Command not found: '" + commandName + "'");
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
            get { return _name + (_cat.Length > 0 ? "." + _cat : "") + "> "; }
        }

        internal static AdminContext CreateDefaultAdminContext()
        {
            return new AdminContext("devcom")
            {
                _locked = true
            };
        }

        internal static Context CreateDefaultContext()
        {
            return new Context("devcom")
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
