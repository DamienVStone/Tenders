using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FtpMonitoringService.Models
{
    public class FtpFile
    {

        public FtpFile()
        {
            Id = ObjectId.GenerateNewId();
            IsDirectory = false;
        }

        public ObjectId Id {get; private set;}
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime? DateModified { get; set; }
        public ObjectId Parent { get; set; }
    }
}
