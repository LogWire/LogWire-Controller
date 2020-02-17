using System.Threading.Tasks;

namespace LogWire.Controller.Kubernetes
{

    public abstract class KubernetesApplication
    {
        protected KubernetesApplication()
        {

            ConstructResources();

        }

        protected abstract void ConstructResources();

        public abstract Task CreateResources(k8s.Kubernetes client);

    }
}
