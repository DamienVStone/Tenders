using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLogger;

namespace AppLogger.Tests
{
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void ClearOutdatedTest()
        {
            Common.ClearOutdated();
        }
    }
}
