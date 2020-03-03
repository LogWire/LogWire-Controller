using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class PersistentVolumeClaim : KubernetesResource
    {
        private readonly string[] _accessModes;
        private readonly string _size;

        public PersistentVolumeClaim(string ns, string name, string[] accessModes, string size)
        {
            _accessModes = accessModes;
            _size = size;
            _namespace = ns.ToLower();
            _name = name.ToLower();
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if (!await ResourceExists(client))
                await client.CreateNamespacedPersistentVolumeClaimAsync(GetPersistentVolumeClaimObject(), _namespace);
        }

        private V1PersistentVolumeClaim GetPersistentVolumeClaimObject()
        {
            return new V1PersistentVolumeClaim
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                Spec = new V1PersistentVolumeClaimSpec
                {
                    AccessModes = _accessModes,
                    Resources = new V1ResourceRequirements
                    {
                        Requests = new Dictionary<string, ResourceQuantity> { { "storage", new ResourceQuantity(_size) } }
                    }
                }
            };
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedPersistentVolumeClaimAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedPersistentVolumeClaimAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
