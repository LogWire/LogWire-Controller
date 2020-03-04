using System.Threading.Tasks;
using LogWire.Controller.Client.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests.ApiTests
{

    [TestClass]
    public class AuthenticationApiTests
    {

        [TestMethod]
        public async Task CorrectUserTest()
        {

            var value = await AuthenticationApiClient.Login("https://localhost:5001", "154191d1-1dbd-434d-ae0d-627ae87c2548", "administrator", "changeme");

            Assert.AreNotEqual("", value);

        }

        [TestMethod]
        public async Task IncorrectUserTest()
        {

            var value = await AuthenticationApiClient.Login("https://localhost:5001", "154191d1-1dbd-434d-ae0d-627ae87c2548", "administrator", "nope");

            Assert.AreEqual("", value);

        }

    }
}
