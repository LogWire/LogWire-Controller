using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using k8s;

namespace LogWire.Controller.Resource
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
