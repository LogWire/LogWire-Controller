using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;

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
            return Task.FromResult(new ConfigurationStatusMessage{Successful = true});
        }

        public override Task<ConfigurationMessage> GetConfigurationValue(ConfigurationKeyMessage request, ServerCallContext context)
        {

            var data = _repository.Get(request.Key);
            var ret = new ConfigurationMessage
            {
                Value = "INVALID",
                Key =  "INVALID"
            };

            if (data != null)
            {
                ret.Value = data.Value;
                ret.Key = data.Key;
            }
            
            return Task.FromResult(ret);

        }
    }
}
