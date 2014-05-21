using System;
using System.Linq;

namespace DeveloperCommands
{
    internal static class Util
    {
        public static object ChangeType(object obj, Type convertType)
        {
            if (obj == null) return null;
            if (convertType == typeof(bool))
            {
                var objString = obj.ToString().ToLower().Trim();
                switch (objString)
                {
                    case "true":
                    case "yes":
                    case "on":
                    case "y":
                    case "t":
                    case "1":
                    case "1.0":
                        return true;
                    case "false":
                    case "no":
                    case "off":
                    case "n":
                    case "f":
                    case "0":
                    case "0.0":
                        return false;
                    default:
                        return false;
                }
            }
            try
            {
                return Convert.ChangeType(obj, convertType);
            }
            catch
            {
                return null;
            }
        }

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

        public static bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
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
