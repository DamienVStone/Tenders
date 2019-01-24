using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tenders.Sberbank.Abstractions.Models.PurchaseRequest;

namespace Tenders.Core.Models
{
    public class AttachableFile : IAttachableFile
    {
        public AttachableFile(string filePath, string inputName)
        {
            Path = new Uri(filePath);
            if (!Path.IsFile)
                throw new ArgumentException($"{filePath} is not a file", "filePath");

            FileName = Path.Segments.Last();
            InputName = inputName;
        }

        public string FileName { get; }
        public Uri Path { get; }
        public string InputName { get; }
        public bool IsUploaded
        {
            get
            {
                return !string.IsNullOrEmpty(UploadedFileID) && !string.IsNullOrEmpty(UploadedHash) && !string.IsNullOrEmpty(UploadedHash2012);
            }
        }
        public string UploadedFileID { get; set; }
        public string UploadedHash { get; set; }
        public string UploadedSign { get; set; }
        public string UploadedSignFingerprint { get; set; }
        public string UploadedHash2012 { get; set; }

        public void AttachFile(MultipartFormDataContent formData)
        {
            if (IsUploaded)
                throw new Exception("File already uploaded! Please, create new instance.");

            formData.Add(new StreamContent(File.OpenRead(Path.AbsolutePath)), InputName, HttpUtility.UrlEncode(FileName));
        }
    }
}
