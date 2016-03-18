using System;
using System.Linq;

namespace HlLib.CommandLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommandLineArgAttribute : Attribute
    {
        public string name;
        public object defaultValue;
    }

    public class CommandLineOptions<TOptions>
        where TOptions : class, new()
    {
        public TOptions Parse(string[] args)
        {
            var result = new TOptions();

            var argDic = args.Where(arg => arg.StartsWith("/"))
                .Select(arg => arg.TrimStart('/').Split(':'))
                .ToDictionary(
                    arg => arg[0],
                    arg => (arg.Length == 2) ? arg[1] : null);

            var props = typeof(TOptions).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            foreach (var prop in props)
            {
                var attributes = prop.GetCustomAttributes(typeof(CommandLineArgAttribute), true);
                foreach (CommandLineArgAttribute attribute in attributes)
                {
                    var name = (!string.IsNullOrEmpty(attribute.name)) ? attribute.name : prop.Name;
                    var argKey = argDic.Keys.FirstOrDefault(key => key == name);
                    var argValue = (argKey != null) ? argDic[argKey] : null;

                    if (prop.PropertyType.IsArray)
                    {
                        object[] values;
                        if (argKey != null)
                        {
                            values = argValue.Split(';')
                                    .Select(v => ConvertArgType(attribute, prop.PropertyType.GetElementType(), argKey, v))
                                    .ToArray();
                        }
                        else
                        {
                            values = new object[] { };
                        }

                        var array = Array.CreateInstance(prop.PropertyType.GetElementType(), values.Length);
                        for (var i = 0; i < values.Length; ++i)
                        {
                            array.SetValue(values[i], i);
                        }
                        prop.SetValue(result, array);
                    }
                    else
                    {
                        var value = ConvertArgType(attribute, prop.PropertyType, argKey, argValue);
                        prop.SetValue(result, value);
                    }
                }
            }

            return result;
        }

        private object ConvertArgType(CommandLineArgAttribute attribute, Type type, string name, string value)
        {
            if (type == typeof(string)) return (value != null) ? value : GetDefaultValue<string>(attribute);
            else if (type == typeof(bool)) return (name != null) ? true : GetDefaultValue<bool>(attribute);
            else if (type == typeof(int)) return (value != null) ? int.Parse(value) : GetDefaultValue<int>(attribute);
            else if (type == typeof(double)) return (value != null) ? double.Parse(value) : GetDefaultValue<double>(attribute);
            else if (type == typeof(char)) return (value != null) ? value.First() : GetDefaultValue<char>(attribute);
            else if (type == typeof(decimal)) return (value != null) ? decimal.Parse(value) : GetDefaultValue<decimal>(attribute);
            else if (type == typeof(byte)) return (value != null) ? byte.Parse(value) : GetDefaultValue<byte>(attribute);
            else if (type == typeof(sbyte)) return (value != null) ? sbyte.Parse(value) : GetDefaultValue<sbyte>(attribute);
            else if (type == typeof(uint)) return (value != null) ? uint.Parse(value) : GetDefaultValue<uint>(attribute);
            else if (type == typeof(short)) return (value != null) ? short.Parse(value) : GetDefaultValue<short>(attribute);
            else if (type == typeof(ushort)) return (value != null) ? ushort.Parse(value) : GetDefaultValue<ushort>(attribute);
            else if (type == typeof(long)) return (value != null) ? long.Parse(value) : GetDefaultValue<long>(attribute);
            else if (type == typeof(ulong)) return (value != null) ? ulong.Parse(value) : GetDefaultValue<ulong>(attribute);
            else if (type == typeof(float)) return (value != null) ? short.Parse(value) : GetDefaultValue<float>(attribute);
            else throw new ArgumentException(string.Format("the supported types are ValueType, string, and decimal: {0}, {1}", name, type.Name));
        }

        private T GetDefaultValue<T>(CommandLineArgAttribute attribute)
        {
            return (attribute.defaultValue != null) ? (T)attribute.defaultValue : default(T);
        }
    }
}
