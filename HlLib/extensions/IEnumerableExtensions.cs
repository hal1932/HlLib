using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib
{
    public static class IEnumerableExtensions
    {
        // http://neue.cc/2009/08/07_184.html
        internal class CompareSelector<T, TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> selector;

            public CompareSelector(Func<T, TKey> selector)
            {
                this.selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return selector(x).Equals(selector(y));
            }

            public int GetHashCode(T obj)
            {
                return selector(obj).GetHashCode();
            }
        }

        // http://neue.cc/2009/08/07_184.html
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }
    }
}
