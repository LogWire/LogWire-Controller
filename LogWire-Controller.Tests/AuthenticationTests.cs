using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LogWire.Controller.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests
{

    [TestClass]
    public class AuthenticationTests
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
