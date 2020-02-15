using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Kubernetes
{
    public abstract class KubernetesResource
    {

        public abstract Task CreateResource(k8s.Kubernetes client);

        public abstract Task DeleteResource(k8s.Kubernetes client);

        public abstract Task<bool> ResourceExists(k8s.Kubernetes client);

    }
}
