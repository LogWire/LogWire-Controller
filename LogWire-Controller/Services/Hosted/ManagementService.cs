using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes;
using LogWire.Controller.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LogWire.Controller.Services.Hosted
{
    public class ManagementService : IHostedService
    {

        private readonly IServiceScopeFactory _scopeFactory;

        private KubernetesManager _kubeManager;

        public ManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            SetupApiToken();
            SetupDefaultUser();
            //await SetupKubernetes();

        }

        private void SetupDefaultUser()
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDataRepository<UserEntry>>();

            if (!repo.GetAll().Any())
            {
                Console.WriteLine("There is no users. Creating default administrator.");

                byte[] paswordSalt = PasswordUtils.CreateSalt();

                repo.Add(new UserEntry
                {
                    Username = "administrator",
                    Password = PasswordUtils.HashPassword("changeme", paswordSalt).ToBase64(),
                    Salt = paswordSalt.ToBase64()
                });

            }

        }

        private void SetupApiToken()
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<IDataRepository<ConfigurationEntry>>();

            var token = repo.Get("system.api.token")?.Value;

            if (token == null)
            {
                token = Guid.NewGuid().ToString();
                repo.Add(new ConfigurationEntry("system.api.token", token));
            }

            ApiToken.Instance.Init(token);
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
