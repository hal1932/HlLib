using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
{
    public class GitCommit : Commit
    {
        public LibGit2Sharp.Commit Commit { get; private set; }

        internal GitCommit(LibGit2Sharp.Commit commit, Repository repo)
        {
            Commit = commit;
            _repo = repo;

            Created = commit.Author.When.DateTime;
            Author = commit.Author.Name;
            Message = commit.Message.Trim();
        }

        public IEnumerable<FileStatus> QueryFileUpdates()
        {
            foreach (var prevCommit in Commit.Parents)
            {
                var changes = _repo.Diff.Compare<TreeChanges>(prevCommit.Tree, Commit.Tree);
                foreach(var added in changes.Added)
                {
                    yield return new FileStatus(State.Added, added.Path);
                }
                foreach (var conflicted in changes.Conflicted)
                {
                    yield return new FileStatus(State.Conflicted, conflicted.Path);
                }
                foreach (var deleted in changes.Deleted)
                {
                    yield return new FileStatus(State.Deleted, deleted.Path);
                }
                foreach (var modified in changes.Modified)
                {
                    yield return new FileStatus(State.Modified, modified.Path);
                }
                foreach (var renamed in changes.Renamed)
                {
                    yield return new FileStatus(State.Deleted, renamed.OldPath);
                    yield return new FileStatus(State.Added, renamed.Path);
                }
                foreach (var typeChanged in changes.TypeChanged)
                {
                    yield return new FileStatus(State.Deleted, typeChanged.OldPath);
                    yield return new FileStatus(State.Added, typeChanged.Path);
                }
            }
        }

        private Repository _repo;
    }

    public class GitFileStatus : FileStatus
    {
        public StatusEntry StatusEntry { get; private set; }

        internal GitFileStatus(StatusEntry entry, State state)
            : base(state, entry.FilePath)
        {
            StatusEntry = entry;
        }

        internal static State GetState(StatusEntry entry)
        {
            var state = entry.State;
            Func<LibGit2Sharp.FileStatus, bool> match = (s) => (state & s) == s;

            if (match(LibGit2Sharp.FileStatus.Conflicted)) return State.Conflicted;
            if (match(LibGit2Sharp.FileStatus.DeletedFromIndex)) return State.Deleted;
            if (match(LibGit2Sharp.FileStatus.DeletedFromWorkdir)) return State.Deleted;
            if (match(LibGit2Sharp.FileStatus.NewInIndex)) return State.Added;
            if (match(LibGit2Sharp.FileStatus.NewInWorkdir)) return State.New;
            if (match(LibGit2Sharp.FileStatus.ModifiedInIndex)) return State.Modified;
            if (match(LibGit2Sharp.FileStatus.ModifiedInWorkdir)) return State.Modified;
            return State.Unknown;
        }
    }

    public class Git : IVersionControl
    {
        public Git(string path)
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

        ~Git()
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
