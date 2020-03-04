using System.Threading.Tasks;
using LogWire.Controller.Client.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests.ApiTests
{
    [TestClass]
    public class StatusApiTests
    {

        [TestMethod]
        public async Task GetStatusTest()
        {

            var value = await StatusApiClient.GetSystemStatus("https://localhost:5001", "3d3e050f-b0a8-4cf8-846e-62710e7350d5");

            Assert.AreEqual(true, value.Key);

        }

    }
}
