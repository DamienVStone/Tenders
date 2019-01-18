using System;
using System.Collections.Generic;

namespace Tenders.API.Parameters
{
    public class FTPEntriesTreeParam
    {
        public readonly DateTimeOffset Modified;
        public readonly bool IsDirectory;
        public readonly string Name;
        public readonly long Size;
        public readonly IEnumerable<FTPEntriesTreeParam> Children;
        public readonly bool IsArchive;

        public FTPEntriesTreeParam(DateTimeOffset modified, bool isDirectory, string name, long size, IEnumerable<FTPEntriesTreeParam> children)
        {
            Modified = modified;
            IsDirectory = isDirectory;
            Name = name;
            Size = size;
            Children = children;
        }
    }
}
