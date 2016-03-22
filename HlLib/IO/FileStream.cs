using System.IO;

namespace HlLib.IO
{
    public class FileReadStream : FileStream
    {
        public FileReadStream(string path, FileShare share = FileShare.ReadWrite)
            : base(path, FileMode.Open, FileAccess.Read, share)
        { }
    }

    public class FileWriteStream : FileStream
    {
        public FileWriteStream(string path, FileShare share = FileShare.Read)
            : base(path, FileMode.OpenOrCreate, FileAccess.Write, share)
        { }
    }
}
