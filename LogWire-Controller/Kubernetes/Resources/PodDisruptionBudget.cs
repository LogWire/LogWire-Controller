using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class PodDisruptionBudget : KubernetesResource
    {
        private readonly IntstrIntOrString _maxUnavailable;
        private readonly V1LabelSelector _selector;

        public PodDisruptionBudget(string ns, string name, IntstrIntOrString maxUnavailable, V1LabelSelector selector)
        {
            _namespace = ns.ToLower();
            _name = name.ToLower();
            _maxUnavailable = maxUnavailable;
            _selector = selector;
        }

        private V1beta1PodDisruptionBudget GetResourceObject()
        {
            return new V1beta1PodDisruptionBudget {
                Metadata = new V1ObjectMeta(name: _name, namespaceProperty: _namespace),
                Spec = new V1beta1PodDisruptionBudgetSpec
                {
                    MaxUnavailable = _maxUnavailable,
                    Selector = _selector
                }
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedPodDisruptionBudgetAsync(GetResourceObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedPodDisruptionBudgetAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedPodDisruptionBudgetAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
