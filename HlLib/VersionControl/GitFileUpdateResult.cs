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
        internal GitFileUpdateResult(MergeResult result, Repository repo)
        {
            _commit = result.Commit;
            _repo = repo;

            Conflicts = (result.Status & MergeStatus.Conflicts) == MergeStatus.Conflicts;
        }

        public override IEnumerable<FileStatus> QueryFileStatuses()
        {
            return new GitCommit(_commit, _repo).QueryFileUpdates();
        }

        private LibGit2Sharp.Commit _commit;
        private Repository _repo;
    }
}
