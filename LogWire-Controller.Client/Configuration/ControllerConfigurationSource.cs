using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LogWire.Controller.Client.Configuration
{
    public class ControllerConfigurationSource : IConfigurationSource
    {
        private string _token, _endpoint, _prefix;

        public ControllerConfigurationSource(string endpoint, string token, string prefix)
        {
            _token = token;
            _endpoint = endpoint;
            _prefix = prefix;
        }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ControllerConfigurationProvider(_endpoint, _token, _prefix);
        }
    }
}
