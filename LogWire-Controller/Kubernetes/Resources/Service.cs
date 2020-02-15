using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Service : IKubernetesResource
    {
        private string _namespace;
        private string _name;
        private Dictionary<string, string> _selector;
        private List<V1ServicePort> _ports;

        public Service(string ns, string name, Dictionary<string, string> selector, List<V1ServicePort> ports)
        {
            _name = name;
            _namespace = ns;
            _selector = selector;
            _ports = ports;
        }

        private V1Service GetServiceObject()
        {
            return new V1Service
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                Spec = new V1ServiceSpec
                {
                    Selector = _selector,
                    Ports = _ports
                }
            };
        }


        public async Task CreateResource(k8s.Kubernetes client)
        {
            if(!await ResourceExists(client))
                client.CreateNamespacedService(GetServiceObject(), _namespace);
        }

        public async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedServiceAsync(_name, _namespace);
        }

        public async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedServiceAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 1;
        }
    }
}
