using System;
using System.Security.Policy;

namespace HlLib
{
    public class ScopedAppDomain : Disposable
    {
        public AppDomain Domain { get; }

        public ScopedAppDomain(string name = null, Evidence securityInfo = null, AppDomainSetup info = null)
        {
            name = name ?? Guid.NewGuid().ToString();
            Domain = AppDomain.CreateDomain(name, securityInfo, info);
        }

        protected override void Dispose(bool disposing)
        {
            AppDomain.Unload(Domain);
        }
    }
}
