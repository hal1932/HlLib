using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
{
    public class GitFileUpdateResult : FileUpdateResult
    {
        internal GitFileUpdateResult() { }

        internal GitFileUpdateResult(MergeResult result, Repository repo)
        {
            _commit = result.Commit;
            _repo = repo;

            Conflicts = (result.Status & MergeStatus.Conflicts) == MergeStatus.Conflicts;
        }

        internal GitFileUpdateResult(LibGit2Sharp.Commit commit, Repository repo)
        {
            _commit = commit;
            _repo = repo;
        }

        public override IEnumerable<FileStatus> QueryFileStatuses()
        {
            return (_commit != null) ?
                new GitCommit(_commit, _repo).QueryFileUpdates()
                : Enumerable.Empty<FileStatus>();
        }

        private LibGit2Sharp.Commit _commit;
        private Repository _repo;
    }
}
