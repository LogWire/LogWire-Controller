using System;
using System.Threading;
using System.Threading.Tasks;
using LogWire.Controller.Kubernetes;
using Microsoft.Extensions.Hosting;

namespace LogWire.Controller.Services.Hosted
{
    public class KubernetesManagementService : IHostedService
    {
        private KubernetesManager _manager;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            _manager = KubernetesManager.Instance;

            _manager.Startup();

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
