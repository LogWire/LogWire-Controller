using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class ClusterRole : KubernetesResource
    {
        private IList<V1beta1PolicyRule> _rules;

        public ClusterRole(string name, string ns, IList<V1beta1PolicyRule> rules)
        {
            _rules = rules;
            _name = name;
            _namespace = ns;
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateClusterRole2Async(GetClusterRoleObject());
        }

        private V1beta1ClusterRole GetClusterRoleObject()
        {
            return new V1beta1ClusterRole{
                Metadata =  new V1ObjectMeta(name: _name),
                Rules = _rules
            };
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteClusterRole2Async(_name);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            try
            {
                var list = await client.ListClusterRole2Async();
                return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
            }
            catch
            {
                // ignored
            }

            return false;
        }
    }
}
