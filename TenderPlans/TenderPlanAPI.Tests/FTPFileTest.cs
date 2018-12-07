using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Parameters;

namespace TenderPlanAPI.Tests
{
    [TestClass]
    public class FTPFileTest
    {
        const string pathId = "5b8560f9cb051d0ea0699dd4";

        [TestMethod]
        public void AddFile_AddFileInDb_OkReturned()
        {
            var list = new List<FTPEntryParam>{new FTPEntryParam()
            {
                Name = "TestFirstFile.xml",
                Size = 10000000,
                Modified = DateTime.Now
            }, new FTPEntryParam()
            {
                Name = "TestSecondFile.xml",
                Size = 4387,
                Modified = DateTime.Now
            },new FTPEntryParam()
            {
                Name = "TestThirdFile.xml",
                Size = 116232,
                Modified = DateTime.Now
            },new FTPEntryParam()
            {
                Name = "NEWFILEEEE.xml",
                Size = 11392,
                Modified = DateTime.Now
            },new FTPEntryParam()
            {
                Name = "TenderPlan.xml",
                Size = 1384,
                Modified = DateTime.Now
            }
            };
            var response = new FTPFileController().Post(pathId, list.ToArray()) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var checkInDb = new FTPFileController().Get();
            //TODO дописать
        }
    }
}
