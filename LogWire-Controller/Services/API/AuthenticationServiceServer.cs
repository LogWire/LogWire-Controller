using System;
using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Utils;

namespace LogWire.Controller.Services.API
{
    public class AuthenticationServiceServer : Services.AuthenticationService.AuthenticationServiceBase
    {

        private readonly IDataRepository<UserEntry> _repository;

        public AuthenticationServiceServer(IDataRepository<UserEntry> repository)
        {
            _repository = repository;
        }

        public override Task<LoginResponseMessage> Login(LoginRequestMessage request, ServerCallContext context)
        {

            var ret = new LoginResponseMessage
            {
                UserId = ""
            };

            var user = _repository.Get(request.Username);
            if (user != null && PasswordUtils.VerifyHash(request.Password, Convert.FromBase64String(user.Salt), Convert.FromBase64String(user.Password)))
            {
                ret.UserId = user.UserId.ToString();
            }

            return Task.FromResult(ret);

        }
    }
}
