using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using LogWire.Controller.Kubernetes.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire_Controller.Tests
{
    [TestClass]
    public class KubernetesTests
    {
        [TestMethod]
        public async Task NamespaceTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);

            Assert.AreEqual(true, await n.ResourceExists(client));

            await n.CreateResource(client);

            Assert.AreEqual(true, await n.ResourceExists(client));

            await n.DeleteResource(client);

            Thread.Sleep(10000);

            Assert.AreEqual(false, await n.ResourceExists(client));

        }

        [TestMethod]
        public async Task DeploymentTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);

            var labels = new Dictionary<string, string>();
            labels.Add("app", "nginx");

            var ports = new List<V1ContainerPort>();
            ports.Add(new V1ContainerPort(80));

            Deployment deployment = new Deployment("test", "nginx", labels, 1, labels, "nginx:1.7.9", ports);
            await deployment.CreateResource(client);

        }

    }

}