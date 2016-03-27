using System;
using System.Collections.Generic;

namespace HlLib.VersionControl
{
    public interface IRepository : IDisposable
    {
        IEnumerable<FileStatus> QueryFileStatuses();
        IEnumerable<Commit> QueryCommits();
    }
}
