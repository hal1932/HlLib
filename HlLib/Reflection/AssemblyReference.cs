using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace HlLib.Reflection
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class AssemblyReference : IEquatable<AssemblyReference>
    {
        public AssemblyName Name { get; }
        public int ReferenceDepth { get; internal set; }
        public IReadOnlyCollection<AssemblyName> Sources { get; }

        internal AssemblyReference(Assembly assembly)
        {
            Name = assembly.GetName();
            Sources = _sources.AsReadOnly();
        }

        public bool Equals(AssemblyReference other)
        {
            return Name.FullName == other?.Name.FullName;
        }

        public void AddSource(Assembly assembly)
        {
            _sources.Add(assembly.GetName());
        }

        private List<AssemblyName> _sources = new List<AssemblyName>();
    }
}
