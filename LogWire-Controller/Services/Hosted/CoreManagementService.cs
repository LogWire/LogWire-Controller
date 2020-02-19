using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Utils;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Core.Tokens;

namespace LogWire.Controller.Services.Hosted
{
    public class CoreManagementService : IHostedService
    {

        private IServiceScopeFactory _scopeFactory;

        public CoreManagementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IDataRepository<ConfigurationEntry>>();

                var token = repo.Get("system.api.token")?.Value;

                if(token == null)
                {
                    token = Guid.NewGuid().ToString();
                    repo.Add(new ConfigurationEntry("system.api.token", token));
                }

                ApiToken.Instance.Init(token);

            }

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            return Task.CompletedTask;

        }
    }
}
