using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    
    public class Deployment : KubernetesResource
    {

        private readonly Dictionary<string, string> _labels;
        private readonly int _replicas;
        private readonly Dictionary<string, string> _selectors;
        private readonly string _image;
        private readonly List<V1ContainerPort> _ports;

        public Deployment (string ns, string name, Dictionary<string, string> labels, int replicas, Dictionary<string,string> selector, string image, List<V1ContainerPort> ports)
        {
            _namespace = ns;
            _name = name;
            _labels = labels;
            _replicas = replicas;
            _selectors = selector;
            _image = image;
            _ports = ports;
        }

        private V1Deployment GetDeploymentObject()
        {
            return new V1Deployment
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name:_name + "-deployment"),
                Spec = new V1DeploymentSpec
                {
                    Replicas = _replicas,
                    Selector = new V1LabelSelector(matchLabels: _selectors),
                    Template = new V1PodTemplateSpec
                    {
                        Metadata = new V1ObjectMeta(labels: _labels),
                        Spec = new V1PodSpec(new List<V1Container>
                        {
                            new V1Container
                            {
                                Name = _name,
                                Ports = _ports,
                                Image = _image
                            }
                        })
                    }
                }
            };
        }


        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if(!await ResourceExists(client))
                await client.CreateNamespacedDeploymentAsync(GetDeploymentObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedDeployment1Async(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedDeploymentAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name + "-deployment")) > 0;
        }

        public async Task<V1DeploymentStatus> GetStatus(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedDeploymentAsync(_namespace);
            var deployment = list.Items.FirstOrDefault(e => e.Metadata.Name.Equals(_name));

            return deployment?.Status;
        }
    }
}
