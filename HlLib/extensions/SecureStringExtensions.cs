using System.Security;

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
