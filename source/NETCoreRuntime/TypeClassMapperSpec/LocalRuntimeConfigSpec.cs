using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MsTestOnNetRuntime
{
    /// <summary>
    /// Check the runtime effect of x64 option of menu Test/ProcessorArchitecture on System.Configuration.ConfigurationManager.
    /// </summary>
    [TestClass]
    public class LocalRuntimeConfigSpec
    {
        [TestMethod, TestCategory("Runtime")]
        public void CheckMenuOptionSelected_TEST_ProcessorArchitecture_x64()
        {
            Assert.IsNull(System.Configuration.ConfigurationManager.ConnectionStrings["NonExtant"]);
            Assert.AreEqual(null, System.Configuration.ConfigurationManager.AppSettings["NonExtant"]);
        }
    }
}