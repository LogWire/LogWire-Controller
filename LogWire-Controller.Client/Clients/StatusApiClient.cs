using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Services;

namespace LogWire.Controller.Client.Clients
{
    public class StatusApiClient
    {

        public static async Task<KeyValuePair<bool, string>> GetSystemStatus(string endpoint, string token)
        {
            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            StatusService.StatusServiceClient client = new StatusService.StatusServiceClient(channel);

            try
            {

                var value = await client.GetSystemStatusAsync(new SystemStatusParams(), headers: headers);

                if (value != null)
                    return new KeyValuePair<bool, string>(value.IsOK, value.Message);

            }
            catch (Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }

            return new KeyValuePair<bool, string>(false, "System can not communicate with controller");
        }

    }
}
