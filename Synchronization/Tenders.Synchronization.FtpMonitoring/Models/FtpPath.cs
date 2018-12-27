using MongoDB.Bson;

namespace FtpMonitoringService.Models
{
    public class FtpPath
    {
        public static FtpPath Empty = new FtpPath
        {
            Id = ObjectId.Empty.ToString(),
            Path = string.Empty,
            Login = string.Empty,
            Password = string.Empty
        };

        public string Id;
        public string Path { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
