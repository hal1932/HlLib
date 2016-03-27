using System;

namespace HlLib.VersionControl
{
    public abstract class Commit
    {
        public string Author { get; internal set; }
        public string Message { get; internal set; }
        public DateTime Created { get; internal set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Created, Author, Message);
        }
    }
}
