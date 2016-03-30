using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib
{
    public abstract class Disposable : IDisposable
    {
        ~Disposable()
        {
            DisposeInternal(false);
        }

        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        protected virtual void DisposeInternal(bool disposing)
        {
            lock(_disposingLock)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;

                Dispose(disposing);
            }
        }

        private bool _disposed;
        private object _disposingLock = new object();
    }
}
