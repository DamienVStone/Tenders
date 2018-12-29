using MongoDB.Bson;
using System;

namespace Tenders.Synchronization.FtpMonitoring.Abstractions
{
    public interface IFtpFile
    {
        ObjectId Id { get; }
        bool IsDirectory { get; set; }
        string Name { get; set; }
        long Size { get; set; }
        DateTime? DateModified { get; set; }
        ObjectId Parent { get; set; }
    }
}
