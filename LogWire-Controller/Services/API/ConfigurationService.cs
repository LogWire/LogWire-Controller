using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
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
            var entry = _repository.Get(request.Key);
            if (entry == null)
            {
                _repository.Add(new ConfigurationEntry(request.Key, request.Value));
            }
            else
            {
                _repository.Update(entry, new ConfigurationEntry(entry.Key, request.Value));
            }
            
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

        public override Task<ConfigrationListMessage> GetAllConfigurationValuesForPrefix(ConfigurationPrefixMessage request, ServerCallContext context)
        {

            var repo = _repository as ConfigurationRepository;

            var data = repo.GetByPrefix(request.Prefix);

            var ret = new ConfigrationListMessage();

            foreach (var configurationEntry in data)
            {
                ret.ConfigList.Add(configurationEntry.Key, configurationEntry.Value);
            }

            return Task.FromResult(ret);

        }
    }
}
