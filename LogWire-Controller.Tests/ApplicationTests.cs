using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using k8s;
using LogWire.Controller.Kubernetes.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests
{

    [TestClass]
    public class ApplicationTests
    {

        [TestMethod]
        public async Task RabbitMQCreateTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            RabbitMQApplication rabbit = new RabbitMQApplication();

            await rabbit.Create(client);

        }

        [TestMethod]
        public async Task RabbitMQReadyTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            RabbitMQApplication rabbit = new RabbitMQApplication();

            Assert.AreEqual(true,await rabbit.IsReady(client));

        }

        [TestMethod]
        public async Task RabbitMQDeleteTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            RabbitMQApplication rabbit = new RabbitMQApplication();

            await rabbit.Delete(client);

        }

    }

}

