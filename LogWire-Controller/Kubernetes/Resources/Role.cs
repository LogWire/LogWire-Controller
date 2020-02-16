using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Role : KubernetesResource
    {
        private readonly IList<V1PolicyRule> _rules;

        public Role(string ns, string name, IList<V1PolicyRule> rules)
        {
            _name = name;
            _namespace = ns;
            _rules = rules;
        }


        private V1Role CreateRoleObject()
        {
            return new V1Role
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                Rules = _rules
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedRoleAsync(CreateRoleObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedRoleAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedRoleAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
