using System;
using System.Net.Http;

namespace Tenders.Sberbank.Abstractions.Models.PurchaseRequest
{
    public interface IAttachableFile
    {
        string FileName { get; }
        Uri Path { get; }
        string InputName { get; }
        bool IsUploaded { get; }
        string UploadedFileID { get; set; }
        string UploadedHash { get; set; }
        string UploadedSign { get; set; }
        string UploadedSignFingerprint { get; set; }
        string UploadedHash2012 { get; set; }

        void AttachFile(MultipartFormDataContent formData);
    }
}