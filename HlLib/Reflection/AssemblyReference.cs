using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace HlLib.Reflection
{
    [Serializable]
    [DebuggerDisplay("{Name.FullName}")]
    public class AssemblyReference : IEquatable<AssemblyReference>
    {
        public AssemblyName Name { get; private set; }
        public string Location { get; private set; }
        public int ReferenceDepth { get; internal set; }
        public IReadOnlyCollection<AssemblyName> Sources { get; }

        internal static AssemblyReference FromAssembly(Assembly assembly)
        {
            return new AssemblyReference()
            {
                Name = assembly.GetName(),
                Location = assembly.Location,
            };
        }

        private AssemblyReference()
        {
            Sources = _sources.AsReadOnly();
        }

        public bool Equals(AssemblyReference other)
        {
            return Name.FullName == other?.Name.FullName;
        }

        internal void AddSource(Assembly assembly)
        {
            _sources.Add(assembly.GetName());
        }

        private List<AssemblyName> _sources = new List<AssemblyName>();
    }
}
