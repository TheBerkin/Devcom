using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperCommands
{
    internal static class Util
    {
        public static bool Increment(ref object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                    o = (byte)((int)o + 1);
                    return true;
                case TypeCode.SByte:
                    o = (sbyte)((int)o + 1);
                    return true;
                case TypeCode.UInt16:
                    o = (ushort)((int)o + 1);
                    return true;
                case TypeCode.UInt32:
                    o = (uint)o + 1;
                    return true;
                case TypeCode.UInt64:
                    o = (ulong)o + 1;
                    return true;
                case TypeCode.Int16:
                    o = (short)((int)o + 1);
                    return true;
                case TypeCode.Int32:
                    o = (int)o + 1;
                    return true;
                case TypeCode.Int64:
                    o = (long)o + 1;
                    return true;
                case TypeCode.Decimal:
                    o = (decimal)o + 1;
                    return true;
                case TypeCode.Double:
                    o = (double)o + 1;
                    return true;
                case TypeCode.Single:
                    o = (float)o + 1;
                    return true;
                default:
                    return false;
            }
        }

        public static bool Decrement(ref object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                    o = (byte)((int)o - 1);
                    return true;
                case TypeCode.SByte:
                    o = (sbyte)((int)o - 1);
                    return true;
                case TypeCode.UInt16:
                    o = (ushort)((int)o - 1);
                    return true;
                case TypeCode.UInt32:
                    o = (uint)o - 1;
                    return true;
                case TypeCode.UInt64:
                    o = (ulong)o - 1;
                    return true;
                case TypeCode.Int16:
                    o = (short)((int)o - 1);
                    return true;
                case TypeCode.Int32:
                    o = (int)o - 1;
                    return true;
                case TypeCode.Int64:
                    o = (long)o - 1;
                    return true;
                case TypeCode.Decimal:
                    o = (decimal)o - 1;
                    return true;
                case TypeCode.Double:
                    o = (double)o - 1;
                    return true;
                case TypeCode.Single:
                    o = (float)o - 1;
                    return true;
                default:
                    return false;
            }
        }

        public static string Qualify(string cat, string cmdName)
        {
            return ((cat.Length > 0 ? cat + "." : "") + cmdName).ToLower();
        }

        public static bool IsValidName(string name, string otherChars = "_-+")
        {
            return name.All(c => Char.IsLetterOrDigit(c) || otherChars.Contains(c));
        }

        public static string GetConvarValue(string name, string cat)
        {
            if (name.StartsWith("$"))
            {
                cat = "";
                name = name.Substring(1);
            }

            string qname = (cat.Length > 0 ? cat + "." : "") + name;
            Convar convar;
            if (!Devcom.Convars.TryGetValue(qname, out convar))
            {
                return "undefined";
            }
            object value = convar.Value;
            return value == null ? "null" : value.ToString();
        }
    }
}
