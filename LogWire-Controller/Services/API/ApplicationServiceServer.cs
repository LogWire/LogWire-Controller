using System;
using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Data.Model.Application;
using LogWire.Controller.Data.Repository;

namespace LogWire.Controller.Services.API.Application
{
    public class ApplicationServiceServer : ApplicationService.ApplicationServiceBase
    {

        private readonly IDataRepository<ApplicationEntry> _repository;

        public ApplicationServiceServer(IDataRepository<ApplicationEntry> repository)
        {
            _repository = repository;
        }

        public override Task<AddResponseMessage> Add(AddRequestMessage request, ServerCallContext context)
        {
            
            Guid id = Guid.NewGuid();

            _repository.Add(new ApplicationEntry
            {
                Name = request.Name,
                Id = id
            });

            return Task.FromResult(new AddResponseMessage
            {
                Id = id.ToString()
            });
        }

        public override Task<ListResponseMessage> List(ListRequestMessage request, ServerCallContext context)
        {

            ListResponseMessage message = new ListResponseMessage();

            foreach (var applicationEntry in _repository.GetAll())
            {
                message.Applications.Add(new ApplicationListEntry
                {
                    Name = applicationEntry.Name,
                    Id = applicationEntry.Id.ToString()
                });
            }

            return Task.FromResult(message);

        }
    }
}
