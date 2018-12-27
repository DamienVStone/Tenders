using FtpMonitoringService.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace FtpMonitoringService
{
    public class ZipHelper
    {
        private Dictionary<string, FtpFile> _folders;

        private Dictionary<string, FtpFile> Folders
        {
            get
            {
                if (_folders == null) _folders = new Dictionary<string, FtpFile>();
                return _folders;
            }
        }

        public static ZipHelper Get()
        {
            return new ZipHelper();
        }

        private ZipHelper() { }

        public List<FtpFile> ParseArchve(ZipArchiveEntry[] entries)
        {
            var ftpEntries = entries.Where(e => !e.FullName.EndsWith("/")).Select(_parseEntry).ToList();
            if (_folders != null && Folders.Values.Count > 0) ftpEntries.AddRange(Folders.Values);
            return ftpEntries;
        }

        private FtpFile _parseEntry(ZipArchiveEntry entry)
        {
            var nameParts = entry.FullName.Split("/");
            ObjectId parentId = ObjectId.Empty;
            for(var i =0; i<nameParts.Length-1; i++)
            {
                var folderName = nameParts[i];
                if ((parentId.Equals(ObjectId.Empty)&&!Folders.ContainsKey(folderName))||(!parentId.Equals(ObjectId.Empty)&&!Folders.ContainsKey(folderName+"_"+parentId)))
                {
                    var newFolder = new FtpFile()
                    {
                        Name = folderName,
                        IsDirectory = true
                    };

                    if (parentId != null) newFolder.Parent = parentId;
                    Folders[!parentId.Equals(ObjectId.Empty) ? folderName + "_" + parentId : folderName] = newFolder;
                }

                parentId = Folders[!parentId.Equals(ObjectId.Empty) ? folderName + "_" + parentId : folderName].Id;
            }

            var file =  new FtpFile()
            {
                Name = entry.Name,
                Size = entry.Length,
                DateModified = entry.LastWriteTime.DateTime,
            };
            if (parentId != null)
            {
                file.Parent = parentId;
            }

            return file;
        }

    }
}
