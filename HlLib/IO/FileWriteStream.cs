using System.IO;
using System.Text;

namespace HlLib.IO
{
    public class FileWriteStream : FileStream
    {
        public FileWriteStream(string path, FileShare share = FileShare.None)
            : base(path, FileMode.OpenOrCreate, FileAccess.Write, share)
        { }
    }
}
