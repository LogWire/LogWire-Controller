using System.Diagnostics;
using k8s;

namespace LogWire.Controller.Kubernetes
{
    public class KubernetesManager
    {

        private static KubernetesManager _instance;

        public static KubernetesManager Instance => _instance ??= new KubernetesManager();
        
        private k8s.Kubernetes _client;
        
        public KubernetesManager()
        {

            // If debugger is attached use local kube config
            KubernetesClientConfiguration configuration = Debugger.IsAttached ? KubernetesClientConfiguration.InClusterConfig() : KubernetesClientConfiguration.BuildDefaultConfig();

            _client = new k8s.Kubernetes(configuration);

        }


        public void Startup()
        {
            
        }
    }
}
