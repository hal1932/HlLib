using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HlLib.Reflection
{
    public class AssemblyReferenceResolver
    {
        public static IReadOnlyCollection<AssemblyReference> Resolve(string assemblyPath)
        {
            var domainSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetDirectoryName(assemblyPath),
            };

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(assemblyPath);
                domainSetup.ConfigurationFile = config.FilePath;
            }
            catch (ConfigurationErrorsException)
            {
                // 何もしない
            }

            using (var scope = new ScopedAppDomain(Guid.NewGuid().ToString(), null, domainSetup))
            {
                var proxy = (ResolverProxy)scope.Domain.CreateInstanceFromAndUnwrap(
                    Assembly.GetExecutingAssembly().Location,
                    typeof(ResolverProxy).FullName);
                return proxy.Resolve(assemblyPath);
            }
        }
    }

    class ResolverProxy : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public IReadOnlyCollection<AssemblyReference> Resolve(string assemblyPath)
        {
            var result = new List<AssemblyReference>();

            var queue = new Queue<Item>();
            queue.Enqueue(new Item() { Assembly = Assembly.LoadFile(assemblyPath), RefDepth = 0 });

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                foreach (var referenceName in item.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referenceName);
                    var reference = AssemblyReference.FromAssembly(referencedAssembly);

                    var found = result.FirstOrDefault(x => x.Equals(reference));
                    if (found == null)
                    {
                        reference.AddSource(item.Assembly);
                        reference.ReferenceDepth = item.RefDepth + 1;
                        result.Add(reference);

                        queue.Enqueue(new Item()
                        {
                            Assembly = referencedAssembly,
                            RefDepth = reference.ReferenceDepth,
                        });
                    }
                    else
                    {
                        found.AddSource(item.Assembly);
                    }
                }
            }

            return result.AsReadOnly();
        }

        private struct Item
        {
            public Assembly Assembly;
            public int RefDepth;
        }
    }
}
