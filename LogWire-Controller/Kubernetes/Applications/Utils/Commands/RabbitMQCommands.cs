using System.Collections.Generic;

namespace LogWire.Controller.Kubernetes.Applications.Utils.Commands
{
    public class RabbitMQCommands
    {

        private static readonly string CCommand = "-c";
        private static readonly string bash = "/bin/sh";

        private static readonly string Bootstrap1 = "set -e\n" +
                                                    "cp /configmap/* /etc/rabbitmq\n" +
                                                    "echo \"${RABBITMQ_ERLANG_COOKIE}\" > /var/lib/rabbitmq/.erlang.cookie";

        private static readonly string Probe = @"wget -O - -q --header ""Authorization: Basic `echo -n \""$RABBIT_MANAGEMENT_USER:$RABBIT_MANAGEMENT_PASSWORD\"" | base64`"" http://localhost:15672/api/healthchecks/node | grep -qF ""{\""status\"":\""ok\""}""";

        public static IList<string> ReadyCommands = new List<string> {bash, CCommand, Probe};
        public static IList<string> LiveCommands = new List<string> {bash, CCommand, Probe};
        public static IList<string> Commands = new List<string> {};
        public static IList<string> BootstrapArgs = new List<string> {CCommand, Bootstrap1};
    }
}
