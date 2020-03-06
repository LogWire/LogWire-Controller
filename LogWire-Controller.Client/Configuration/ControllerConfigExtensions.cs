using Microsoft.Extensions.Configuration;

namespace LogWire.Controller.Client.Configuration
{
    public static class ControllerConfigExtensions
    {

        public static IConfigurationBuilder AddControllerConfiguration(this IConfigurationBuilder builder, string endpoint, string prefix, string token)
        {
            return builder.Add(new ControllerConfigurationSource(endpoint, token, prefix));
        }

    }
}
