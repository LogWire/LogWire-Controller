using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace LogWire.Controller.Services.API
{
    public class ConfigurationService : Services.ConfigurationService.ConfigurationServiceBase
    {

        private IDataRepository<ConfigurationEntry> _repository;

        public ConfigurationService(IDataRepository<ConfigurationEntry> repository)
        {
            _repository = repository;
        }

        public override Task<ConfigurationStatusMessage> AddConfiguration(ConfigurationMessage request, ServerCallContext context)
        {
            _repository.Add(new ConfigurationEntry(request.Key, request.Value));
            return new Task<ConfigurationStatusMessage>(() => new ConfigurationStatusMessage{Successful = true});
        }

        public override Task<ConfigurationMessage> GetConfigurationValue(ConfigurationKeyMessage request, ServerCallContext context)
        {

            var data = _repository.Get(request.Key);
            var ret = new ConfigurationMessage();

            if (data != null)
            {
                ret.Value = data.Value;
                ret.Key = data.Key;
            }
            
            return new Task<ConfigurationMessage>(() => ret);

        }
    }
}
