using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

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
                        yield return new GitFileStatus(State.Added, update.Path);
                        break;

                    case ChangeKind.Conflicted:
                        yield return new GitFileStatus(State.Conflicted, update.Path);
                        break;

                    case ChangeKind.Deleted:
                        yield return new GitFileStatus(State.Deleted, update.Path);
                        break;

                    case ChangeKind.Modified:
                        yield return new GitFileStatus(State.Modified, update.Path);
                        break;

                    case ChangeKind.Renamed:
                        yield return new GitFileStatus(State.Deleted, update.OldPath);
                        yield return new GitFileStatus(State.Added, update.Path);
                        break;

                    case ChangeKind.TypeChanged:
                        yield return new GitFileStatus(State.Deleted, update.OldPath);
                        yield return new GitFileStatus(State.Added, update.Path);
                        break;

                    default:
                        yield return new GitFileStatus(State.Unknown, update.Path);
                        break;
                }
            }
        }

        private Repository _repo;
    }
}
