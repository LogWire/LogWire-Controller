using System.Threading.Tasks;
using LogWire.Controller.Client.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests.ApiTests
{
    [TestClass]
    public class ConfigurationApiTests
    {

        [TestMethod]
        public async Task GetValueTest()
        {

            var value = await ConfigurationApiClient.GetConfigurationValueAsync("https://localhost:5001", "rabbitmq.cookie", "3d3e050f-b0a8-4cf8-846e-62710e7350d5");

            Assert.AreEqual("rabbitmq.cookie", value.Value.Key);
            Assert.AreEqual("W25UGEV740UMU79Z20QU0H1U442LU4QY", value.Value.Value);

        }

        [TestMethod]
        public async Task InvalidTokenTest()
        {

            var value = await ConfigurationApiClient.GetConfigurationValueAsync("https://localhost:5001", "rabbitmq.cookie", "-b0a8-4cf8-846e-62710e7350d5");

            Assert.AreEqual(null, value);
        }

    }
}
