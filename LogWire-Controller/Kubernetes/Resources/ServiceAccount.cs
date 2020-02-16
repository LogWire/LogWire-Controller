using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class ServiceAccount : KubernetesResource
    {

        public ServiceAccount(string ns, string name)
        {
            _namespace = ns;
            _name = name;
        }

        private V1ServiceAccount GetServiceAccountObject()
        {
            return new V1ServiceAccount {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedServiceAccountAsync(GetServiceAccountObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedServiceAccountAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedServiceAccountAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
