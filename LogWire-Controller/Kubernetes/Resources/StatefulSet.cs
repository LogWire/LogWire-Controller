using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class StatefulSet : KubernetesResource
    {
        private readonly IList<V1PersistentVolumeClaim> _volumeTemplate;
        private readonly string _serviceName;
        private readonly string _podManagementPolicy;
        private readonly int? _replicas;
        private readonly string _updateStratType;
        private readonly IDictionary<string, string> _selector;
        private readonly V1PodTemplateSpec _template;
        private IDictionary<string, string> _labels;

        public StatefulSet(string ns, string name, IDictionary<string, string> labels, string serviceName, string podManagementPolicy, int? replicas, string updateStratType, IDictionary<string, string> selector, V1PodTemplateSpec template, IList<V1PersistentVolumeClaim> volumeTemplate)
        {
            _namespace = ns.ToLower();
            _name = name.ToLower();
            _labels = labels;
            _serviceName = serviceName;
            _podManagementPolicy = podManagementPolicy;
            _replicas = replicas;
            _updateStratType = updateStratType;
            _selector = selector;
            _template = template;
            _volumeTemplate = volumeTemplate;
        }

        private V1StatefulSet CreateStatefulSetObject()
        {
            return new V1StatefulSet
            {
                Metadata = new V1ObjectMeta(namespaceProperty:_namespace, name:_name, labels: _labels),
                Spec = new V1StatefulSetSpec
                {
                    ServiceName = _serviceName,
                    PodManagementPolicy = _podManagementPolicy,
                    Replicas = _replicas,
                    UpdateStrategy =new V1StatefulSetUpdateStrategy(type: _updateStratType),
                    Selector = new V1LabelSelector(matchLabels: _selector),
                    Template = _template,
                    VolumeClaimTemplates = _volumeTemplate
                }
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedStatefulSetAsync(CreateStatefulSetObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedStatefulSetAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedStatefulSetAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }

        public override async Task<bool> ResourceReady(k8s.Kubernetes client)
        {

            if (!await ResourceExists(client))
                return false;

            var list = await client.ListNamespacedStatefulSetAsync(_namespace);
            var statefulSet = list.Items.FirstOrDefault(s => s.Metadata.Name.Equals(_name));

            return statefulSet?.Status.ReadyReplicas == _replicas;

        }
    }
}
