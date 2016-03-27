using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
{
    public class GitRepository : IRepository
    {
        public GitRepository(string path)
        {
            _repo = new Repository(path);
        }

        public IEnumerable<FileStatus> QueryFileStatuses()
        {
            foreach (var entry in _repo.RetrieveStatus())
            {
                var state = GitFileStatus.GetState(entry);
                if (state != State.Unknown)
                {
                    yield return new GitFileStatus(entry, state);
                }
            }
        }

        public IEnumerable<Commit> QueryCommits()
        {
            return _repo.Commits
                .QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Time,
                })
                .Select(commit => new GitCommit(commit, _repo));
        }

        #region IDisposable
        private bool _disposed;
        private object _disposingLock = new object();

        ~GitRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock(_disposingLock)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;

                if (disposing)
                {
                    if (_repo != null)
                    {
                        _repo.Dispose();
                    }
                }
            }
        }
        #endregion

        private Repository _repo;
    }
}
