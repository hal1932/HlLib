using System.Collections.Generic;
using System.Linq;

namespace HlLib
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object obj, bool canWrite = true, bool canRead = false)
        {
            var props = obj.GetType().GetProperties()
                .Where(prop => canWrite && prop.CanWrite)
                .Where(prop => canRead && prop.CanRead);
            return props.ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(obj));
        }
    }
}
