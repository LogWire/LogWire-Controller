using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class CustomResourceDefinition : KubernetesResource
    {
        private readonly V1beta1CustomResourceDefinitionNames _names;
        private readonly string _scope;
        private readonly string _group;
        private readonly string _version;

        public CustomResourceDefinition(string name, string version, string group, string scope, V1beta1CustomResourceDefinitionNames names)
        {
            _version = version;
            _group = group;
            _scope = scope;
            _names = names;
            _name = name;
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateCustomResourceDefinition1Async(GetDefinitionObject());
        }

        private V1beta1CustomResourceDefinition GetDefinitionObject()
        {
            return new V1beta1CustomResourceDefinition
            {
                Metadata = new V1ObjectMeta(name: _name),
                Spec = new V1beta1CustomResourceDefinitionSpec
                {
                    Group = _group,
                    Version = _version,
                    Scope = _scope,
                    Names= _names
                }
            };
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteCustomResourceDefinition1Async(_name);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListCustomResourceDefinition1Async();
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
