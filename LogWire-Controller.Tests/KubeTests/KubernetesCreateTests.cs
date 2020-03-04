using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using LogWire.Controller.Kubernetes.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogWire.Controller.Tests.KubeTests
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

        [TestMethod]
        public async Task ConfigMapTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);

            ConfigMap map = new ConfigMap("test", "test-map", new Dictionary<string, string>
            {
                {"rabbitmq.conf", "test=test1\r\ntest2=test2"}
            });

            await map.CreateResource(client);

            Assert.AreEqual(true, await map.ResourceExists(client));

        }

        [TestMethod]
        public async Task RoleTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);


            V1PolicyRule rolePolicy = new V1PolicyRule
            {
                Resources = new List<string> {"resources"},
                Verbs = new List<string> {"get"},
                ApiGroups = new List<string> {""}
            };

            Role r = new Role("test", "test-role", new List<V1PolicyRule> {rolePolicy});

            await r.CreateResource(client);

            Assert.AreEqual(true, await r.ResourceExists(client));

        }

        [TestMethod]
        public async Task ServiceAccountTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);

            ServiceAccount sa = new ServiceAccount("test", "test-account");

            await sa.CreateResource(client);

            Assert.AreEqual(true, await sa.ResourceExists(client));

        }

        [TestMethod]
        public async Task RoleBindingTest()
        {

            KubernetesClientConfiguration configuration = KubernetesClientConfiguration.BuildDefaultConfig();

            var client = new k8s.Kubernetes(configuration);

            Namespace n = new Namespace("test");

            await n.CreateResource(client);

            ServiceAccount sa = new ServiceAccount("test", "test-account");

            await sa.CreateResource(client);

            V1PolicyRule rolePolicy = new V1PolicyRule
            {
                Resources = new List<string> {"resources"},
                Verbs = new List<string> {"get"},
                ApiGroups = new List<string> {""}
            };

            Role r = new Role("test", "test-role", new List<V1PolicyRule> {rolePolicy});

            await r.CreateResource(client);

            RoleBinding rb = new RoleBinding("test", "testrb",
                new List<V1Subject> {new V1Subject("ServiceAccount", "test-account")},
                "rbac.authorization.k8s.io", "Role", "test-role");

            await rb.CreateResource(client);

            Assert.AreEqual(true, await rb.ResourceExists(client));
        }

    }

}