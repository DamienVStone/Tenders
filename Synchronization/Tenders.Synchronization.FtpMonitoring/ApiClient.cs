//using FtpMonitoringService.Models;
//using MongoDB.Bson;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.Mime;
//using System.Threading.Tasks;

//namespace FtpMonitoringService
//{
//    public class ApiClient
//    {

//        private static string _host;
        
//        public static string Host
//        {
//            set
//            {
//                if (_host == null) _host = value;
//            }

//            private get
//            {
//                if (_host == null) throw new ArgumentException("Не указан хост");
//                return _host;
//            }
//        }

//        public static ApiClient Get()
//        {
//            return new ApiClient();
//        }

//        private ApiClient() { }

//        public FtpPath GetNextPathToIndex()
//        {
//            FtpPath path;

//            using (var client = new HttpClient())
//            using (var response = client.GetAsync(Host + "/api/FtpPath/Next").Result)
//            {
//                if(response.StatusCode == HttpStatusCode.NoContent)
//                {
//                    return FtpPath.Empty;
//                }

//                path = JsonConvert.DeserializeObject<FtpPath>(response.Content.ReadAsStringAsync().Result);
//            }

//            return path ?? FtpPath.Empty;
//        }
        
//        public async Task<string> SendFilesAsync(List<FtpFile> files, FtpPath path)
//        {
//            var content = new StringContent(JsonConvert.SerializeObject(files));
//            content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
//            using (var client = new HttpClient())
//            {
//                try
//                {
//                    using (var httpResult = await client.PostAsync(Host + $"/api/FtpFile?pathId={path.Id}", content))
//                    {
//                        return await httpResult.Content.ReadAsStringAsync();
//                    }
//                }
//                catch (WebException ex)
//                {
//                    Debug.WriteLine(ex.Message);
//                    Debug.Write(ex.Response);
//                    return $"Error " + ex.Message;
//                }
//            }
//        }

//        public async Task<string> SendFileTreeAsync(List<FtpFile> files, FtpPath path, FtpFile rootFile)
//        {
//            var content = new StringContent(JsonConvert.SerializeObject(new { PathId = path.Id, TreeRoot = rootFile, Files = files }));
//            content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
//            using (var client = new HttpClient())
//            {
//                try
//                {
//                    using (var response = await client.PostAsync(Host + "/api/FtpFile/AddFileTree", content))
//                    {
//                        return await response.Content.ReadAsStringAsync();
//                    }
//                }
//                catch (WebException ex)
//                {
//                    Debug.WriteLine(ex.Message);
//                    Debug.Write(ex.Response);
//                    return $"Error " + ex.Message;
//                }
//            }
//        }

////#if DEBUG
////        public void WriteRootFilesInfoToDisk(List<FtpFile> files, FtpPath path)
////        {
////            File.WriteAllText($"{path.Id}.dump", JsonConvert.SerializeObject(files, Formatting.Indented));
////        }

////        public void WriteFileTreeToDisk(List<FtpFile> files, FtpPath path, FtpFile rootFile)
////        {
////            File.WriteAllText($"{path.Id}_{rootFile.Id}.dump", JsonConvert.SerializeObject(new { PathId = path.Id, TreeRoot = rootFile, Files = files }, Formatting.Indented));
////        }

////#endif


//    }
//}
