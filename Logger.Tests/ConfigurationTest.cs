using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void LoggerCreationTest() {
            var logger = AppLogger.Logger.GetLogger("Profiler1");
            Assert.IsNotNull(logger);
        }
    }
}
