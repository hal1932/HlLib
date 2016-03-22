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
        public char OptionStartChar { get; set; } = '/';
        public char OptionSplitChar { get; set; } = '=';
        public char OptionArraySeparatorChar { get; set; } = ';';

        /// <summary>
        /// コマンドライン引数を解析してTOptionsにいれて返す
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns></returns>
        public TOptions Parse(string[] args)
        {
            string[] dummy;
            return Parse(args, out dummy);
        }

        /// <summary>
        /// コマンドライン引数を解析してTOptionsにいれて返す
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <param name="rest">TOptionsのメンバに合致しなかった引数</param>
        /// <returns></returns>
        public TOptions Parse(string[] args, out string[] rest)
        {
            var result = new TOptions();

            // StartCharとSplitCharにしたがって引数を分解
            var argDic = args.Where(arg => arg.StartsWith(OptionStartChar.ToString()))
                .Select(arg => arg.TrimStart(OptionStartChar).Split(OptionSplitChar))
                .ToDictionary(
                    arg => arg[0],
                    arg => (arg.Length == 2) ? arg[1] : null);

            // TOptionsのプロパティに合致する引数を拾ってresultに設定
            // 拾われなかったのはrestArgListにいれる
            var props = typeof(TOptions).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            var restArgList = ((string[])args.Clone()).ToList();
            foreach (var prop in props)
            {
                var attributes = prop.GetCustomAttributes(typeof(CommandLineArgAttribute), true);
                foreach (CommandLineArgAttribute attribute in attributes)
                {
                    var name = (!string.IsNullOrEmpty(attribute.name)) ? attribute.name : prop.Name;
                    var argKey = argDic.Keys.FirstOrDefault(key => key == name);
                    var argValue = (argKey != null) ? argDic[argKey] : null;

                    // プロパティが配列型だったら、ArraySeparatorCharにしたがって分割
                    // そうでなければそのまま拾う
                    if (prop.PropertyType.IsArray)
                    {
                        object[] values;
                        if (argKey != null)
                        {
                            values = argValue.Split(OptionArraySeparatorChar)
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

                    var handledArg = restArgList.FirstOrDefault(arg => arg.StartsWith(OptionStartChar + name));
                    if (handledArg != null)
                    {
                        restArgList.Remove(handledArg);
                    }
                }
            }

            rest = restArgList.ToArray();
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
