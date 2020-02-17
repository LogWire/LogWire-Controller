using System.Threading.Tasks;

namespace LogWire.Controller.Kubernetes
{
    public abstract class KubernetesResource
    {
        protected string _namespace;
        protected string _name;

        public abstract Task CreateResource(k8s.Kubernetes client);

        public abstract Task DeleteResource(k8s.Kubernetes client);

        public abstract Task<bool> ResourceExists(k8s.Kubernetes client);

        public virtual async Task<bool> ResourceReady(k8s.Kubernetes client)
        {
            return await ResourceExists(client);
        }

    }
}
