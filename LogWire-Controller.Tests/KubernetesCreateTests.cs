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
    public class KubernetesCreateTests
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

            Assert.AreEqual(true, await deployment.ResourceExists(client));

        }

        [TestMethod]
        public async Task ServiceTest()
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

            Assert.AreEqual(true, await deployment.ResourceExists(client));

            var servicePorts = new List<V1ServicePort>();
            servicePorts.Add(new V1ServicePort(80));

            Service s = new Service("test", "nginx-clusterip-service", labels, servicePorts, false);
            await s.CreateResource(client);

            Assert.AreEqual(true, await s.ResourceExists(client));

            var servicePortsLB = new List<V1ServicePort>();
            servicePortsLB.Add(new V1ServicePort(port: 80));

            Service slb = new Service("test", "nginx-lb-service", labels, servicePortsLB, true);
            await slb.CreateResource(client);

            Assert.AreEqual(true, await slb.ResourceExists(client));

        }

        [TestMethod]
        public async Task IngressTest()
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

            Assert.AreEqual(true, await deployment.ResourceExists(client));

            var servicePorts = new List<V1ServicePort>();
            servicePorts.Add(new V1ServicePort(80));

            Service s = new Service("test", "nginx-clusterip-service", labels, servicePorts, false);
            await s.CreateResource(client);

            Assert.AreEqual(true, await s.ResourceExists(client));

            IList<Extensionsv1beta1IngressRule> rules = new List<Extensionsv1beta1IngressRule>();

            rules.Add(new Extensionsv1beta1IngressRule
            {
                Host = "test.local",
                Http = new Extensionsv1beta1HTTPIngressRuleValue
                {
                    Paths = new List<Extensionsv1beta1HTTPIngressPath>
                    {
                        new Extensionsv1beta1HTTPIngressPath
                        {
                            Path = "/",
                            Backend = new Extensionsv1beta1IngressBackend("nginx-clusterip-service", 80)
                        }
                    }
                }
            });

            Ingress ingress = new Ingress("test", "test-ingress", rules);
            await ingress.CreateResource(client);

            Assert.AreEqual(true, await ingress.ResourceExists(client));

        }

    }

}