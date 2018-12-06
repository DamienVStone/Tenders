using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenderPlanAPI.Models;

namespace TenderPlanAPI.Parameters
{
    public class FTPPathParam 
    {
        public ObjectId Id { get; set; }
        public string Path { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
