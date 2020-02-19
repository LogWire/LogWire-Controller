using System;
using System.Threading;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogWire.Controller.Services.Hosted
{
    public class KubernetesManagementService : IHostedService
    {
        private KubernetesManager _manager;
        private readonly IServiceScopeFactory _scopeFactory;

        public KubernetesManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            _manager = KubernetesManager.Instance;

            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IDataRepository<ConfigurationEntry>>();

                _manager.Init(repository);
            }

            await _manager.Startup();

            Console.WriteLine("Loaded");

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
