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
        public AssemblyName Name { get; }
        public string Location { get; private set; }
        public int ReferenceDepth { get; internal set; }
        public IReadOnlyCollection<AssemblyName> Sources { get; }

        internal AssemblyReference(AssemblyName assemblyName, string location)
        {
            Name = assemblyName;
            Location = location;
            Sources = _sources.AsReadOnly();
        }

        public bool Equals(AssemblyReference other)
        {
            return Name.FullName == other?.Name.FullName;
        }

        public void AddSource(AssemblyName assemblyName)
        {
            _sources.Add(assemblyName);
        }

        public void SetLocation(string location)
        {
            Location = location;
        }

        private List<AssemblyName> _sources = new List<AssemblyName>();
    }
}
