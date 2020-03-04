using System.Threading.Tasks;
using LogWire.Controller.Client.Clients.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire.Controller.Tests.ApiTests
{
    [TestClass]
    public class ApplicationsApiTest
    {
        [TestMethod]
        public async Task ApplicationAddTest()
        {
            var value = await ApplicationApiClient.AddApplication("https://localhost:5001", "9e8d17bd-dfc1-4d67-82a1-40f0507de27f", "TestApplication");

            Assert.AreNotEqual(null, value);
        }

    }
}
