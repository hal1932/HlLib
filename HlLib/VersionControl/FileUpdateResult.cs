using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
{
    public abstract class FileUpdateResult
    {
        public bool Conflicts { get; protected set; }

        public abstract IEnumerable<FileStatus> QueryFileStatuses();
    }
}
