using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes.Applications;

namespace LogWire.Controller.Kubernetes
{
    public class KubernetesManager
    {

        private static KubernetesManager _instance;

        public static KubernetesManager Instance => _instance ??= new KubernetesManager();
        
        private k8s.Kubernetes _client;
        
        Dictionary<string, KubernetesApplication> Applications = new Dictionary<string, KubernetesApplication>
        {
            { "rabbitmq", new RabbitMQApplication() },
            { "elasticsearch", new ElasticSearchApplication() }
        };

        public KubernetesManager()
        {

            // If debugger is attached use local kube config
            KubernetesClientConfiguration configuration = !Debugger.IsAttached ? KubernetesClientConfiguration.InClusterConfig() : KubernetesClientConfiguration.BuildDefaultConfig();

            _client = new k8s.Kubernetes(configuration);

        }


        public async Task Startup(IDataRepository<ConfigurationEntry> repository)
        {

            foreach (var kubernetesApplication in Applications)
            {
                if(!await kubernetesApplication.Value.IsReady(_client))
                {

                    await kubernetesApplication.Value.Create(_client);

                    Console.WriteLine("Application " + kubernetesApplication.Key + " is not ready. Waiting for it to be ready");

                    while (!await kubernetesApplication.Value.IsReady(_client))
                    {
                        Thread.Sleep(1000);
                    }

                    Console.WriteLine("Application " + kubernetesApplication.Key + " started");

                }

                Console.WriteLine("Application " + kubernetesApplication.Key + " is running");

            }
            
        }
    }
}
