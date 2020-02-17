using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class RoleBinding : KubernetesResource
    {
        private readonly string _roleName;
        private readonly string _kind;
        private readonly string _apiGroup;
        private readonly IList<V1Subject> _subjects;

        public RoleBinding(string ns, string name, IList<V1Subject> subjects, string apiGroup, string kind, string roleName)
        {
            _name = name;
            _namespace = ns;
            _subjects = subjects;
            _apiGroup = apiGroup;
            _kind = kind;
            _roleName = roleName;
        }

        private V1RoleBinding CreateRoleBindingOject()
        {
            return new V1RoleBinding {
                Metadata = new V1ObjectMeta(namespaceProperty:_namespace, name:_name),
                RoleRef = new V1RoleRef(_apiGroup, _kind, _roleName),
                Subjects = _subjects
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedRoleBindingAsync(CreateRoleBindingOject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedRoleBindingAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedRoleBindingAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
