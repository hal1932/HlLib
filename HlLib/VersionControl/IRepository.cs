using System;
using System.Collections.Generic;

namespace HlLib.VersionControl
{
    public interface IRepository : IDisposable
    {
        IEnumerable<FileStatus> QueryFileStatuses();
        IEnumerable<Commit> QueryCommits();
        void SetCredentials(string username, string email, string password);
        FileUpdateResult UpdateLocalFiles();
        bool AddFiles(params string[] paths);
        bool CommitChanges(string message);
        bool UndoChanges(params string[] paths);
    }
}
