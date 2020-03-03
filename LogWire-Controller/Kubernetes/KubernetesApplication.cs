using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes.Resources;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace LogWire.Controller.Kubernetes
{

    public abstract class KubernetesApplication
    {

        protected List<KubernetesResource> ApplicationResources = new List<KubernetesResource>();

        protected KubernetesApplication(IDataRepository<ConfigurationEntry> repository)
        {

        }

        protected KubernetesApplication()
        {
            
        }

        protected abstract string Namespace { get; }

        public async Task Create(k8s.Kubernetes client)
        {

            var ns = new Namespace(this.Namespace);
            await ns.CreateResource(client);

            foreach (var applicationResource in ApplicationResources)
            {
                await applicationResource.CreateResource(client);
            }
        }

        public async Task<bool> IsReady(k8s.Kubernetes client)
        {

            bool resourceNotReady = false;

            foreach (var applicationResource in ApplicationResources)
            {
                resourceNotReady |= !await applicationResource.ResourceReady(client);
            }

            return !resourceNotReady;

        }

        public async Task Delete(k8s.Kubernetes client)
        {
            foreach (var applicationResource in ApplicationResources)
            {
                try
                {
                    await applicationResource.DeleteResource(client);
                }
                catch
                {
                    // ignored
                }
            }

            await client.DeleteNamespaceAsync(this.Namespace);
        }

    }
}
