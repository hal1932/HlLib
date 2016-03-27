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
            var updates = Enumerable.Empty<TreeEntryChanges>();
            foreach (var prevCommit in Commit.Parents)
            {
                var changes = _repo.Diff.Compare<TreeChanges>(prevCommit.Tree, Commit.Tree);
                updates = updates.Union(changes.Added)
                    .Union(changes.Conflicted)
                    .Union(changes.Deleted)
                    .Union(changes.Modified)
                    .Union(changes.Renamed)
                    .Union(changes.TypeChanged);
            }

            foreach (var update in updates.Distinct(item => item.Path))
            {
                switch (update.Status)
                {
                    case ChangeKind.Added:
                        yield return new FileStatus(State.Added, update.Path);
                        break;

                    case ChangeKind.Conflicted:
                        yield return new FileStatus(State.Conflicted, update.Path);
                        break;

                    case ChangeKind.Deleted:
                        yield return new FileStatus(State.Deleted, update.Path);
                        break;

                    case ChangeKind.Modified:
                        yield return new FileStatus(State.Modified, update.Path);
                        break;

                    case ChangeKind.Renamed:
                        yield return new FileStatus(State.Deleted, update.OldPath);
                        yield return new FileStatus(State.Added, update.Path);
                        break;

                    case ChangeKind.TypeChanged:
                        yield return new FileStatus(State.Deleted, update.OldPath);
                        yield return new FileStatus(State.Added, update.Path);
                        break;

                    default:
                        yield return new FileStatus(State.Unknown, update.Path);
                        break;
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
