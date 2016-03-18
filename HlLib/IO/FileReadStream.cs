using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.IO
{
    public class FileReadStream : FileStream
    {
        public bool EndOfStream
        {
            get { return (Position < Length); }
        }

        public FileReadStream(string path, FileShare share = FileShare.Read)
            : base(path, FileMode.Open, FileAccess.Read, share)
        { }

        public StreamReader CreateStringReader()
        {
            return new StreamReader(this);
        }

        public BinaryReader CreateBinaryReader()
        {
            return new BinaryReader(this);
        }
     }
}
