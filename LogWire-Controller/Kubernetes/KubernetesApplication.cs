﻿using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using LogWire.Controller.Kubernetes.Resources;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace LogWire.Controller.Kubernetes
{

    public abstract class KubernetesApplication
    {

        protected List<KubernetesResource> ApplicationResources = new List<KubernetesResource>();
        
        protected KubernetesApplication()
        {

            ConstructResources();

        }

        protected abstract string Namespace { get; }


        protected abstract void ConstructResources();

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
            await client.DeleteNamespaceAsync(this.Namespace);
        }

    }
}
