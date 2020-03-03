using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using LogWire.Controller.Kubernetes;
using LogWire.Controller.Services.Hosted;
using Microsoft.Extensions.Hosting;

namespace LogWire.Controller.Services.API
{
    public class StatusService : Services.StatusService.StatusServiceBase
    {

        private readonly ManagementService _managementService;

        public StatusService(ManagementService managementService)
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
