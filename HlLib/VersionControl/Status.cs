﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlLib.VersionControl
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

    public class FileStatus
    {
        public State State { get; internal set; }
        public string FilePath { get; internal set; }

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