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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            
            _manager = KubernetesManager.Instance;

            await _manager.Startup();


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
