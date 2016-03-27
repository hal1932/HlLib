using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HlLib
{
    public static class SecureStringExtensions
    {
        public static SecureString SetString(this SecureString obj, string value)
        {
            obj.Clear();
            foreach (var c in value)
            {
                obj.AppendChar(c);
            }
            return obj;
        }
    }
}
