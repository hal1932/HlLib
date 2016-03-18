using System.IO;

namespace HlLib.IO
{
    public class FileReadStream : FileStream
    {
        public FileReadStream(string path, FileShare share = FileShare.Read)
            : base(path, FileMode.Open, FileAccess.Read, share)
        { }
    }
}
