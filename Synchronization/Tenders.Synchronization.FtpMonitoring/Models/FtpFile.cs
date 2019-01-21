using System;
using System.Collections.Generic;

namespace FtpMonitoringService.Models
{
    public class FtpFile
    {

        private IDictionary<string, FtpFile> _children = new Dictionary<string, FtpFile>();

        public readonly DateTimeOffset Modified;
        public readonly bool IsDirectory;
        public readonly string Name;
        public readonly long Size;
        public bool IsArchive;
        public IEnumerable<FtpFile> Children => _children.Values;

        public FtpFile(string name)
        {
            Name = name;
            IsDirectory = true;
            IsArchive = name.EndsWith(".zip");
        }

        public FtpFile(string name, long size, DateTimeOffset modified)
        {
            Modified = modified;
            Name = name;
            Size = size;
            IsDirectory = false;
            IsArchive = name.EndsWith(".zip");
        }

        public FtpFile AddChild(string name)
        {
            var key = $"{name}_dir";
            if (!_children.ContainsKey(key))
                _children[key] = new FtpFile(name);
            return _children[key];
        }

        public FtpFile AddChild(string name, long size, DateTimeOffset modified)
        {
            if (!_children.ContainsKey(name))
                _children[name] = new FtpFile(name, size, modified);

            return _children[name];
        }
    }
}
