using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Services;


namespace LogWire.Controller
{
    public class ConfigurationApiClient
    {

        public static async System.Threading.Tasks.Task<KeyValuePair<string, string>?> GetConfigurationValueAsync(string endpoint, string key, string token)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            ConfigurationService.ConfigurationServiceClient client = new ConfigurationService.ConfigurationServiceClient(channel);

            try
            {

                var value = await client.GetConfigurationValueAsync(new ConfigurationKeyMessage {Key = key}, headers:headers);

                if (!String.IsNullOrWhiteSpace(value?.Value))
                    return new KeyValuePair<string, string>(value.Key, value.Value);


            }
            catch (Exception)
            {
                // ignored
            }

            return null;

        }

        public static async Task<bool> AddConfigurationValueAsync(string endpoint, string key, string value, string token)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            ConfigurationService.ConfigurationServiceClient client = new ConfigurationService.ConfigurationServiceClient(channel);

            try
            {
                var ret = await client.AddConfigurationAsync(new ConfigurationMessage {Key = key, Value = value});
                return ret.Successful;
            }
            catch (Exception)
            {
                // ignored
            }

            return false;

        }

    }
}
