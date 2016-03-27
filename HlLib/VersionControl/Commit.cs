using System;

namespace HlLib.VersionControl
{
    public abstract class Commit
    {
        public string Author { get; protected set; }
        public string Message { get; protected set; }
        public DateTime Created { get; protected set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Created, Author, Message);
        }
    }
}
