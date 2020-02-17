using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Namespace : KubernetesResource
    {

        public Namespace(string name)
        {
            _name = name;
        }

        private V1Namespace GetNamespaceObject()
        {
            return new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = _name
                }
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
            {
                client.CreateNamespace(GetNamespaceObject());
            }
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespaceAsync(_name);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            await Task.Delay(1);

            return client.ListNamespace().Items.Count(n => n.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
