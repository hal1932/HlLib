using System.IO;

namespace HlLib.IO
{
    public class FileWriteStream : FileStream
    {
        public FileWriteStream(string path, FileShare share = FileShare.None)
            : base(path, FileMode.OpenOrCreate, FileAccess.Write, share)
        { }

        public StreamWriter CreateStringWriter()
        {
            return new StreamWriter(this);
        }

        public BinaryWriter CreateBinaryWriter()
        {
            return new BinaryWriter(this);
        }
    }
}
