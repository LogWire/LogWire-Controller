using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Secret : KubernetesResource
    {
        private readonly Dictionary<string, string> _data;

        public Secret(string ns, string name, Dictionary<string, string> data)
        {
            _namespace = ns.ToLower();
            _name = name.ToLower();
            _data = data;
        }

        private V1Secret GetSecretObject()
        {
            return new V1Secret
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name:_name),
                StringData = _data
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedSecretAsync(GetSecretObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedSecretAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedSecretAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
