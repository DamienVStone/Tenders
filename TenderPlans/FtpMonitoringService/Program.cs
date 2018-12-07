using FtpMonitoringService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FtpMonitoringService
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient.Host = "http://localhost";

            var p = ApiClient.Get().GetNextPathToIndex();
            if (p.Equals(FtpPath.Empty)) return;

            List<FtpFile> allEntries = new List<FtpFile>();
#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
            p.Login = creds[0];
            p.Password = creds[1];
#endif
            var files = FtpClient.Get().ListDirectoryFiels(p.Path, p.Login, p.Password);
            ApiClient.Get().SendFilesAsync(files.Where(f => !f.Name.EndsWith(".zip")).ToList(), p).Wait();
            files
                .Where(f => f.Name.EndsWith(".zip"))
                .AsParallel()
                .ForAll(f =>
                {
                    var fs = ZipHelper.Get().ParseArchve(FtpClient.Get().GetArchiveEntries(p.Path + f.Name, p.Login, p.Password));
                    fs.ForEach(file => file.Parent = f.Id);
                    var res = ApiClient.Get().SendFileTreeAsync(fs, p, f).Result;
                    Console.WriteLine(res);
                });

            

//            allEntries.AddRange(files);
//            allEntries.AddRange(allArchivesEntries);

//            var rootFiles = allEntries.Where(f => f.Parent.Equals(ObjectId.Empty) && !f.Name.EndsWith(".zip")).ToList();

//#if DEBUG
//            ApiClient.Get().WriteRootFilesInfoToDisk(rootFiles, p);

//            allEntries
//                .Where(f => f.Name.EndsWith(".zip"))
//                .AsParallel()
//                .Select(f => new { Root = f, Children = allEntries.Where(ch => ch.Parent.Equals(f.Id)).ToList() })
//                .ForAll(f => ApiClient.Get().WriteFileTreeToDisk(f.Children, p, f.Root));
//#endif
        }
    }
}
