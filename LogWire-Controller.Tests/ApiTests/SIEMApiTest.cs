using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LogWire.Controller.Client.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire.Controller.Tests.ApiTests
{
    [TestClass]
    public class SIEMApiTest
    {

        [TestMethod]
        public async Task UserListTest()
        {
            var value = await SIEMApiClient.ListUsers("https://localhost:5001", "9e8d17bd-dfc1-4d67-82a1-40f0507de27f", 10, 0);

            Assert.AreNotEqual(null, value);
        }

        [TestMethod]
        public async Task UserAddTest()
        {
            var value = await SIEMApiClient.AddUsers("https://localhost:5001", "9e8d17bd-dfc1-4d67-82a1-40f0507de27f", "Test.User");

            Assert.AreNotEqual(null, value);
        }

    }
}
