using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class ConfigMap : KubernetesResource
    {
        private readonly IDictionary<string, string> _data;

        public ConfigMap(string ns, string name, IDictionary<string, string> data)
        {
            _namespace = ns;
            _name = name.ToLower();
            _data = data;
        }

        private V1ConfigMap GetConfigMapObject()
        {
            return new V1ConfigMap
            {
                Metadata = new V1ObjectMeta(namespaceProperty:_namespace, name:_name),
                Data = _data
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedConfigMapAsync(GetConfigMapObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedConfigMapAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedConfigMapAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
