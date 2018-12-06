using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Net;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;

namespace TenderPlanAPI.Tests
{
    [TestClass]
    public class FTPPathTest
    {
        const string pathForFail = "arhiv/zakupki/file/zakupki/contract";

        ObjectId idBad = new ObjectId("5b7fc6b514a7a441888b47fc");

        FilterOptions options = new FilterOptions()
        {
            Page = 0,
            PageSize = 1,
            Path = "H://arhiv/zakupkiTEST"
        };

        FTPPathParam model = new FTPPathParam()
        {
            Id = ObjectId.GenerateNewId(),
            Path = "H://arhiv/zakupkiTEST/fileTEST/NeedDeleted/contract1234",
            Login = "Test@ingos.ru",
            Password = "GHs43ld#2"
    };
        FTPPathParam badModel = new FTPPathParam()
        {
            Path = "H://arhiv/zakupkiTEST/arhiv/zakupkiTEST/file",
            Login = "Edit",
            Password = "GHs43ld#2"
        };

        [TestMethod]
        public void AddPath_AddPathInDb_OkReturned()
        {
            var response = new FTPPathController().Post(model) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = new FTPPathController().Get(options) as JsonResult;

            var value = result.Value as ListResponse<FTPPath>;

            Assert.IsTrue(value.Data.Any(c => c.Path == model.Path));
        }

        [TestMethod]
        public void AddPath_AddPathInDb_BadReturned()
        {
            var response = new FTPPathController().Post(model) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var result = new FTPPathController().Get(options) as ObjectResult;
            var value = result.Value as ListResponse<FTPPath>;

            Assert.IsFalse(value.Data.ToList().FindAll(c => c.Path == model.Path).ToList().Count > 1);
        }

        [TestMethod]
        public void EditPath_EditPathInDb_OkReturned()
        {
            var result = new FTPPathController().Get(options) as ObjectResult;
            var value = result.Value as ListResponse<FTPPath>;

            var id = value.Data.ToList().FirstOrDefault().Id;
            badModel.Id = id;
            var response = new FTPPathController().Put(badModel) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            result = new FTPPathController().Get(options) as ObjectResult;
            value = result.Value as ListResponse<FTPPath>;
            Assert.IsTrue(value.Data.ToList().Any(c => c.Path == badModel.Path));
        }

        [TestMethod]
        public void EditPath_EditPathInDb_BadReturned()
        {
            var result = new FTPPathController().Get(options) as ObjectResult;
            var value = result.Value as ListResponse<FTPPath>;
            var id = value.Data.ToList().FirstOrDefault().Id;

            var response = new FTPPathController().Put(badModel) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            result = new FTPPathController().Get(options) as ObjectResult;
            value = result.Value as ListResponse<FTPPath>;
            Assert.IsFalse(value.Data.Any(c => c.Path == pathForFail));
        }

        [TestMethod]
        public void ChangeFlagActive_ChangeFlagInDb_OkReturned()
        {
            var db = new DBConnectContext();
            var filter = Builders<FTPPath>.Filter.Empty;
            var collection = db.FTPPath.Find(filter).ToList();
            var id = collection.FirstOrDefault().Id;

            var response = new FTPPathController().ChangeFlagActive(id) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var collection2 = db.FTPPath.Find(c => c.IsActive == false && c.Id == id).ToList();
            Assert.IsTrue(collection2.Count > 0);
        }

        [TestMethod]
        public void ChangeFlagActive_ChangeFlagInDb_BadReturned()
        {
            var response = new FTPPathController().ChangeFlagActive(idBad) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void DeletePath_DeletePathFromDb_OkReturned()
        {
            var db = new DBConnectContext();
            var filter = Builders<FTPPath>.Filter.Empty;
            var collection = db.FTPPath.Find(filter).ToList();
            var id = collection.FirstOrDefault().Id;

            var response = new FTPPathController().Delete(id) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var f = Builders<FTPPath>.Filter.Eq(c => c.Id, id);
            var collection2 = db.FTPPath.Find(f).ToList();

            Assert.IsFalse(collection2.Count > 0);
        }

        [TestMethod]
        public void DeletePath_DeletePathFromDb_BadReturned()
        {
            var response = new FTPPathController().Delete(idBad) as ObjectResult;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
