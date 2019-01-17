using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenderPlanAPI.Models;

namespace Tenders.API.Tests
{
    [TestClass]
    public class ModelBaseTest
    {
        [TestMethod]
        public void QuickSearchGenerated() {
            var tp = new TenderPlanPosition();
            tp.GenerateQuickSearchString();
        }
    }
}
