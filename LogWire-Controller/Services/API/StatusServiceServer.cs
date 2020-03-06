using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Services.Hosted;

namespace LogWire.Controller.Services.API
{
    public class StatusServiceServer : Services.StatusService.StatusServiceBase
    {

        private readonly ManagementService _managementService;

        public StatusServiceServer(ManagementService managementService)
        {
            _managementService = managementService;
        }

        public override Task<Status> GetSystemStatus(SystemStatusParams request, ServerCallContext context)
        {

            string message = _managementService.GetSystemStatus();

            var ret = new Status
            {
                Message = message ?? "[OK] All Working",
                IsOK = message == null
            };

            return Task.FromResult(ret); ;
        }
    }
}
