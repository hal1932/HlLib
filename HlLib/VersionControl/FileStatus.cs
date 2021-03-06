﻿namespace HlLib.VersionControl
{
    public enum State
    {
        Unknown,
        Added,
        Conflicted,
        Deleted,
        Modified,
        New,
    }

    public abstract class FileStatus
    {
        public State State { get; protected set; }
        public string FilePath { get; protected set; }

        internal FileStatus(State state, string filePath)
        {
            State = state;
            FilePath = filePath;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", State, FilePath);
        }
    }
}
