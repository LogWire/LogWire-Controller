using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Service : KubernetesResource
    {

        private readonly Dictionary<string, string> _selector;
        private readonly List<V1ServicePort> _ports;
        private readonly bool _loadbalancer;
        private readonly string _clusterIP;
        private bool? _publishNotReady;

        // Create a ClusterIP Service
        
        public Service(string ns, string name, Dictionary<string, string> selector, List<V1ServicePort> ports, bool? publishNotReady = null, bool loadbalancer = false, string clusterIP = null)
        {
            _name = name;
            _namespace = ns;
            _selector = selector;
            _ports = ports;
            _publishNotReady = publishNotReady;
            _loadbalancer = loadbalancer;
            _clusterIP = clusterIP;
        }

        private V1Service GetServiceObject()
        {
            return new V1Service
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                Spec = new V1ServiceSpec
                {
                    Type = _loadbalancer ? "LoadBalancer" : "",
                    Selector = _selector,
                    Ports = _ports,
                    ClusterIP = _clusterIP,
                    PublishNotReadyAddresses = _publishNotReady
                }
            };
        }
        
        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if(!await ResourceExists(client))
                client.CreateNamespacedService(GetServiceObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedServiceAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedServiceAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
