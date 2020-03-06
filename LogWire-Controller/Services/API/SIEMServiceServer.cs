using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using LogWire.Controller.Data.Model.SIEM;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Data.Repository.SIEM;
using LogWire.Controller.Protos.SIEM;

namespace LogWire.Controller.Services.API
{
    public class SIEMServiceServer : SIEMService.SIEMServiceBase
    {

        private readonly IDataRepository<SIEMUserEntry> _repository;

        public SIEMServiceServer(IDataRepository<SIEMUserEntry> repository)
        {
            _repository = repository;
        }

        public override Task<UserListResponse> GetUserList(UserListMessage request, ServerCallContext context)
        {
            var repo = _repository as SIEMUserRepository;

            var result = repo.GetPagedList(request.ResultsPerPage, request.PageNumber);

            var ret = new UserListResponse();
            ret.TotalPages = result.PageCount;

            foreach (var siemUserEntry in result.Results)
            {
                ret.Users.Add(new SIEMUser
                {
                    Username = siemUserEntry.Username,
                    Id = siemUserEntry.Id.ToString()
                });
            }

            return Task.FromResult(ret);

        }

        public override Task<AddUserResposne> AddUser(AddUserMessage request, ServerCallContext context)
        {
            Guid id = Guid.NewGuid();

            _repository.Add(new SIEMUserEntry
            {
                Id = id,
                Username = request.Username
            });

            return Task.FromResult(new AddUserResposne {Id = id.ToString()});

        }
    }
}
