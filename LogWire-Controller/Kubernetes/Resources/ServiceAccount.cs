using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class ServiceAccount : KubernetesResource
    {
        private readonly bool? _automatedToken;

        public ServiceAccount(string ns, string name, bool? automatedToken = null)
        {
            _namespace = ns.ToLower();
            _name = name.ToLower();
            _automatedToken = automatedToken;
        }

        private V1ServiceAccount GetServiceAccountObject()
        {
            return new V1ServiceAccount {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                AutomountServiceAccountToken = _automatedToken
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedServiceAccountAsync(GetServiceAccountObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            try
            {
                await client.DeleteNamespacedServiceAccountAsync(_name, _namespace);
            }
            catch
            {
                // ignored
            }
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedServiceAccountAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
