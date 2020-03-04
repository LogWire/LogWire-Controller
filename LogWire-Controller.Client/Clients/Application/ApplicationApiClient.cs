using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Services;

namespace LogWire.Controller.Client.Clients.Application
{
    public class ApplicationApiClient
    {

        public static async Task<List<ApplicationListEntry>> ListApplications(string endpoint, string token)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            ApplicationService.ApplicationServiceClient client = new ApplicationService.ApplicationServiceClient(channel);

            try
            {

                var ret = await client.ListAsync(new ListRequestMessage(), headers: headers);
                return ret.Applications.ToList();

            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine(e);
            }

            return new List<ApplicationListEntry>();

        }

        public static async Task<Guid?> AddApplication(string endpoint, string token, string name)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            ApplicationService.ApplicationServiceClient client = new ApplicationService.ApplicationServiceClient(channel);

            try
            {

                var ret = await client.AddAsync(new AddRequestMessage{Name = name}, headers: headers);
                return Guid.Parse(ret.Id);

            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine(e);
            }

            return null;

        }

    }
}
