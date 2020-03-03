using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class ClusterRoleBinding : KubernetesResource
    {
        private readonly string _roleName;
        private readonly string _kind;
        private readonly string _apiGroup;
        private readonly IList<V1beta1Subject> _subjects;

        public ClusterRoleBinding(string ns, string name, IList<V1beta1Subject> subjects, string apiGroup, string kind, string roleName)
        {
            _name = name.ToLower();
            _namespace = ns.ToLower();
            _subjects = subjects;
            _apiGroup = apiGroup;
            _kind = kind;
            _roleName = roleName;
        }

        private V1beta1ClusterRoleBinding CreateRoleBindingOject()
        {
            return new V1beta1ClusterRoleBinding
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                RoleRef = new V1beta1RoleRef(_apiGroup, _kind, _roleName),
                Subjects = _subjects
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateClusterRoleBinding2Async(CreateRoleBindingOject());
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteClusterRoleBinding2Async(_name);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListClusterRoleBinding2Async();
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}