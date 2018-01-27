using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HlLib
{
    // http://neue.cc/2013/03/09_400.html
    public class DisposableThreadLocal<T> : ThreadLocal<T>
        where T : IDisposable, new()
    {
        public DisposableThreadLocal()
            : base(() => new T(), true)
        { }

        public DisposableThreadLocal(Func<T> valueFactory)
            : base(valueFactory, true)
        { }

        protected override void Dispose(bool disposing)
        {
            var innerExceptions = new List<Exception>();

            foreach (var item in Values.OfType<T>())
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception e)
                {
                    innerExceptions.Add(e);
                }
            }

            base.Dispose(disposing);

            if (innerExceptions.Any())
            {
                throw new AggregateException(innerExceptions);
            }
        }
    }
}
