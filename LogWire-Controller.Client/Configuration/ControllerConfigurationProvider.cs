using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Services;
using Microsoft.Extensions.Configuration;

namespace LogWire.Controller.Client.Configuration
{
    public class ControllerConfigurationProvider : ConfigurationProvider
    {
        private readonly string _endpoint;
        private readonly string _token;
        private readonly string _prefix;

        public ControllerConfigurationProvider(string endpoint, string token, string prefix)
        {
            _endpoint = endpoint;
            _token = token;
            _prefix = prefix;
        }

        public override void Load()
        {

            Console.WriteLine("Loading Config from Controller.");

            var headers = new Metadata();
            headers.Add("Authorization", _token);

            var channel = GrpcChannel.ForAddress(_endpoint);
            ConfigurationService.ConfigurationServiceClient client = new ConfigurationService.ConfigurationServiceClient(channel);

            var ret = client.GetAllConfigurationValuesForPrefix(new ConfigurationPrefixMessage { Prefix = _prefix }, headers: headers);
            if (ret != null)
            {
                foreach (var config in ret.ConfigList)
                {
                    Data.Add(config);
                }
            }

        }

        public override void Set(string key, string value)
        {

            this.Data[key] = value;

            var headers = new Metadata();
            headers.Add("Authorization", _token);

            var channel = GrpcChannel.ForAddress(_endpoint);
            ConfigurationService.ConfigurationServiceClient client = new ConfigurationService.ConfigurationServiceClient(channel);

            client.AddConfiguration(new ConfigurationMessage { Key = key, Value = value }, headers: headers);

        }

    }
}
