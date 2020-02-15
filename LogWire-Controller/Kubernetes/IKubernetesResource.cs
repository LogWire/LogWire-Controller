using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Kubernetes
{
    interface IKubernetesResource
    {

        Task CreateResource(k8s.Kubernetes client);

        Task DeleteResource(k8s.Kubernetes client);

        Task<bool> ResourceExists(k8s.Kubernetes client);

    }
}
