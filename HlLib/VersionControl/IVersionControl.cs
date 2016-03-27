using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
{
    public interface IVersionControl : IDisposable
    {
        IEnumerable<FileStatus> QueryFileStatuses();
        IEnumerable<Commit> QueryCommits();
    }
}
