using LibGit2Sharp;
using System;

namespace HlLib.VersionControl
{
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
}
