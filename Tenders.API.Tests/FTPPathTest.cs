using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tenders.API.DAL.Interfaces;
using Tenders.Core.DI;

namespace Tenders.API.Tests
{
    [TestClass]
    public class FTPPathTest
    {
        private void registerServices()
        {
            Container.Init(
                new Startup(null).ConfigureServices
                );
        }

        [TestMethod]
        public void PathCanBeFiltered()
        {
            registerServices();
            var service = Container.GetService<IFTPPathRepo>();
            try
            {
                var result = service.Get(0, 10, "").ToArray()[0];
                var result1 = service.Get(0, 10, "asdf").ToArray();
            }
            catch (System.Exception e)
            {

                throw;
            }
        }
    }
}
