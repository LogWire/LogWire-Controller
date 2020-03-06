using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Protos.SIEM;
using LogWire.Controller.Services;

namespace LogWire.Controller.Client.Clients
{
    public class SIEMApiClient
    {

        public static async Task<UserListResponse> ListUsers(string endpoint, string token, int pageSize, int page)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            SIEMService.SIEMServiceClient client = new SIEMService.SIEMServiceClient(channel);

            try
            {

                var ret = await client.GetUserListAsync(new UserListMessage{PageNumber = page, ResultsPerPage = pageSize}, headers: headers);
                return ret;

            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine(e);
            }

            return new UserListResponse();

        }

        public static async Task<string> AddUsers(string endpoint, string token, string username)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            SIEMService.SIEMServiceClient client = new SIEMService.SIEMServiceClient(channel);

            try
            {

                var ret = await client.AddUserAsync(new AddUserMessage{Username = username}, headers: headers);
                return ret.Id;

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
