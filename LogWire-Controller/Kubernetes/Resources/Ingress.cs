using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace LogWire.Controller.Kubernetes.Resources
{
    public class Ingress : KubernetesResource
    {
       
        private readonly IList<Extensionsv1beta1IngressRule> _rules;

        public Ingress(string ns, string name, IList<Extensionsv1beta1IngressRule> rules)
        {
            _namespace = ns.ToLower();
            _name = name.ToLower();
            _rules = rules;
        }

        private Extensionsv1beta1Ingress CreateIngressObject()
        {
            return new Extensionsv1beta1Ingress
            {
                Metadata = new V1ObjectMeta(namespaceProperty: _namespace, name: _name),
                Spec = new Extensionsv1beta1IngressSpec
                {
                    Rules = _rules
                }
            };
        }

        public override async Task CreateResource(k8s.Kubernetes client)
        {
            if(!await ResourceExists(client))
                await client.CreateNamespacedIngressAsync(CreateIngressObject(), _namespace);
        }

        public override async Task DeleteResource(k8s.Kubernetes client)
        {
            await client.DeleteNamespacedIngressAsync(_name, _namespace);
        }

        public override async Task<bool> ResourceExists(k8s.Kubernetes client)
        {
            var list = await client.ListNamespacedIngressAsync(_namespace);
            return list.Items.Count(s => s.Metadata.Name.Equals(_name)) > 0;
        }
    }
}
