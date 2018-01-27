using System.Collections.Generic;

namespace HlLib.VersionControl
{
    public abstract class FileUpdateResult
    {
        public bool Conflicts { get; protected set; }

        public abstract IEnumerable<FileStatus> QueryFileStatuses();
    }
}
