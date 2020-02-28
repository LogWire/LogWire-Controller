using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using LogWire.Controller.Services;

namespace LogWire.Controller.Client
{
    public class AuthenticationApiClient
    {

        public static async Task<string> Login(string endpoint, string token, string username, string password)
        {

            var headers = new Metadata();
            headers.Add("Authorization", token);

            var channel = GrpcChannel.ForAddress(endpoint);
            AuthenticationService.AuthenticationServiceClient client = new AuthenticationService.AuthenticationServiceClient(channel);

            try
            {

                var ret = await client.LoginAsync(new LoginRequestMessage { Username = username, Password = password}, headers: headers);
                return ret.UserId;

            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine(e);
            }

            return "";

        }

    }
}
