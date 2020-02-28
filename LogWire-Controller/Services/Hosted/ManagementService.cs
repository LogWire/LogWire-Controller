using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes;
using LogWire.Controller.Utils;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Core.Tokens;

namespace LogWire.Controller.Services.Hosted
{
    public class ManagementService : IHostedService
    {

        private IServiceScopeFactory _scopeFactory;

        private KubernetesManager _kubeManager;

        public ManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            SetupApiToken();
            await SetupKubernetes();

        }

        private void SetupApiToken()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IDataRepository<ConfigurationEntry>>();

                var token = repo.Get("system.api.token")?.Value;

                if (token == null)
                {
                    token = Guid.NewGuid().ToString();
                    repo.Add(new ConfigurationEntry("system.api.token", token));
                }

                ApiToken.Instance.Init(token);

            }
        }

        private async Task SetupKubernetes()
        {

            _kubeManager = KubernetesManager.Instance;

            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IDataRepository<ConfigurationEntry>>();

                _kubeManager.Init(repository);
            }

            await _kubeManager.Startup();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            return Task.CompletedTask;

        }

        public string GetSystemStatus()
        {
            return "[WARN] Starting Up";
        }
    }
}
