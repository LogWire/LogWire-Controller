using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using k8s.Models;
using LogWire.Controller.Data.Model;
using LogWire.Controller.Data.Repository;
using LogWire.Controller.Kubernetes.Applications.Utils.Commands;
using LogWire.Controller.Kubernetes.Resources;
using LogWire.Controller.Utils;
using Secret = LogWire.Controller.Kubernetes.Resources.Secret;

namespace LogWire.Controller.Kubernetes.Applications
{
    // ReSharper disable once InconsistentNaming
    public class RabbitMQApplication : KubernetesApplication
    {

        // User Editable Information
        private static string _rabbitManagerPass;
        private static string _rabbitGuestPass;
        private static string _cookie;

        protected override string Namespace => "rabbit-mq";
        
        private static Dictionary<string, string> _labels = new Dictionary<string, string> { { "app", "rabbitmq" } };
        private static List<V1ServicePort> _ports = new List<V1ServicePort> {
            new V1ServicePort(15672, "http", targetPort: "http"),
            new V1ServicePort(5672, "amqp", targetPort: "amqp"),
            new V1ServicePort(4369, "epmd", targetPort: "epmd")
        };

        private void ConstructService()
        {
            ApplicationResources.Add(new Service(this.Namespace, "rabbitmq", _labels, _ports));   
        }

        private void ConstructStatefulSet()
        {
            V1PodTemplateSpec podSpec = new V1PodTemplateSpec{
                Metadata = new V1ObjectMeta(namespaceProperty:this.Namespace, labels:_labels),
                Spec = new V1PodSpec
                {
                    TerminationGracePeriodSeconds = 10,
                    SecurityContext = new V1PodSecurityContext(101,101, true, 100),
                    ServiceAccountName = "rabbitmq",
                    InitContainers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "bootstrap",
                            Image = "busybox:1.30.1",
                            ImagePullPolicy = "IfNotPresent",
                            Command = new List<string> {"sh"},
                            Args = RabbitMQCommands.BootstrapArgs,
                            Env = new List<V1EnvVar>
                            {
                                new V1EnvVar("POD_NAME", valueFrom: new V1EnvVarSource(fieldRef: new V1ObjectFieldSelector("metadata.name"))),
                                new V1EnvVar("RABBITMQ_ERLANG_COOKIE", valueFrom:new V1EnvVarSource(secretKeyRef: new V1SecretKeySelector("rabbitmq-erlang-cookie", "rabbitmq"))),
                                new V1EnvVar("RABBITMQ_MNESIA_DIR", "/var/lib/rabbitmq/mnesia/rabbit@$(POD_NAME).rabbitmq-discovery." + this.Namespace + ".svc.cluster.local")
                            },
                            VolumeMounts = new List<V1VolumeMount>
                            {
                                new V1VolumeMount("/configmap", "configmap"),
                                new V1VolumeMount("/etc/rabbitmq", "config"),
                                new V1VolumeMount("/var/lib/rabbitmq", "data")
                            }
                        }
                    },
                    Containers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "rabbitmq",
                            Image = "rabbitmq:3.8.0-alpine",
                            ImagePullPolicy = "IfNotPresent",
                            Ports = new List<V1ContainerPort>
                            {
                                new V1ContainerPort(4369, name: "epmd"),
                                new V1ContainerPort(5672, name: "amqp"),
                                new V1ContainerPort(15672, name: "http")
                            }, 
                            LivenessProbe = new V1Probe
                            {
                                Exec = new V1ExecAction(RabbitMQCommands.LiveCommands),
                                FailureThreshold = 6,
                                InitialDelaySeconds = 120,
                                PeriodSeconds = 10,
                                TimeoutSeconds = 5
                            },
                            ReadinessProbe = new V1Probe
                            {
                                Exec = new V1ExecAction(RabbitMQCommands.ReadyCommands),
                                FailureThreshold = 6,
                                InitialDelaySeconds = 20,
                                PeriodSeconds = 5,
                                TimeoutSeconds = 3
                            },
                            Env = new List<V1EnvVar>
                            {
                                new V1EnvVar("K8S_SERVICE_NAME", "rabbitmq-discovery"),
                                new V1EnvVar("RABBITMQ_USE_LONGNAME","true"),
                                new V1EnvVar("MY_POD_NAME", valueFrom: new V1EnvVarSource(fieldRef: new V1ObjectFieldSelector("metadata.name"))),
                                new V1EnvVar("MY_PODthis.Namespace", valueFrom: new V1EnvVarSource(fieldRef: new V1ObjectFieldSelector("metadata.namespace"))),
                                new V1EnvVar("RABBITMQ_NODENAME", "rabbit@$(MY_POD_NAME).$(K8S_SERVICE_NAME).$(MY_PODthis.Namespace).svc.cluster.local"),
                                new V1EnvVar("K8S_HOSTNAME_SUFFIX", ".$(K8S_SERVICE_NAME).$(MY_PODthis.Namespace).svc.cluster.local"),
                                new V1EnvVar("RABBITMQ_ERLANG_COOKIE", valueFrom:new V1EnvVarSource(secretKeyRef:new V1SecretKeySelector("rabbitmq-erlang-cookie", "rabbitmq"))),
                                new V1EnvVar("RABBIT_MANAGEMENT_USER", valueFrom:new V1EnvVarSource(secretKeyRef:new V1SecretKeySelector("rabbitmq-management-username", "rabbitmq"))),
                                new V1EnvVar("RABBIT_MANAGEMENT_PASSWORD", valueFrom:new V1EnvVarSource(secretKeyRef:new V1SecretKeySelector("rabbitmq-management-password", "rabbitmq")))
                            },
                            VolumeMounts = new List<V1VolumeMount>
                            {
                                new V1VolumeMount("/var/lib/rabbitmq", "data"),
                                new V1VolumeMount("/etc/rabbitmq", "config"),
                                new V1VolumeMount("/etc/definitions", "definitions", readOnlyProperty: true)
                            }
                        }
                    },
                    Affinity = new V1Affinity
                    {
                        PodAntiAffinity = new V1PodAntiAffinity(preferredDuringSchedulingIgnoredDuringExecution: new List<V1WeightedPodAffinityTerm> {
                            new V1WeightedPodAffinityTerm(new V1PodAffinityTerm("kubernetes.io/hostname", new V1LabelSelector(matchLabels: _labels)), 1)
                        })
                    },
                    Volumes = new List<V1Volume>
                    {
                        new V1Volume("config", emptyDir: new V1EmptyDirVolumeSource()),
                        new V1Volume("configmap", configMap:new V1ConfigMapVolumeSource(name: "rabbitmq")),
                        new V1Volume("definitions", secret: new V1SecretVolumeSource(secretName: "rabbitmq", items: new List<V1KeyToPath>{new V1KeyToPath("definitions.json", "definitions.json")})),
                        new V1Volume("data", emptyDir: new V1EmptyDirVolumeSource())
                    }
                }
            };

            IList<V1PersistentVolumeClaim> volumeTemplate = new List<V1PersistentVolumeClaim> {

            };

            ApplicationResources.Add(new StatefulSet(this.Namespace, "rabbitmq", _labels, "rabbitmq-discovery",
                "OrderedReady", 3, "OnDelete", _labels, podSpec, volumeTemplate));
        }

        private void ConstructDiscoveryService()
        {
            ApplicationResources.Add(new Service(this.Namespace, "rabbitmq-discovery", _labels, _ports, true, clusterIP: "None"));
        }

        private void ConstructRoleBinding()
        {
            IList<V1Subject> subjects = new List<V1Subject>
            {
                new V1Subject("ServiceAccount", "rabbitmq", namespaceProperty: this.Namespace)
            };

            ApplicationResources.Add(new RoleBinding(this.Namespace, "rabbitmq", subjects, "rbac.authorization.k8s.io", "Role", "rabbitmq"));
        }

        private void ConstructRole()
        {
            V1PolicyRule policy = new V1PolicyRule{
                ApiGroups = new List<string> { "" },
                Verbs = new List<string> { "get" },
                Resources = new List<string> { "endpoints" }
            };

            ApplicationResources.Add(new Role(this.Namespace, "rabbitmq", new List<V1PolicyRule>{policy}));
        }

        private void ConstructServiceAccount()
        {
            ApplicationResources.Add(new ServiceAccount(this.Namespace, "rabbitmq", true));
        }

        private void ConstructSecret()
        {
            Dictionary<string, string> secret = new Dictionary<string, string>();

            secret.Add("rabbitmq-username", "guest"); // guest
            secret.Add("rabbitmq-password", _rabbitGuestPass); // 0naJ1k2PG3TL1IUBUzBxVah8
            secret.Add("rabbitmq-management-username", "management"); // management
            secret.Add("rabbitmq-management-password", _rabbitManagerPass); // y8Qa10wqJ6iDB6XcTh0DQI8F
            secret.Add("rabbitmq-erlang-cookie", _cookie);
            secret.Add("definitions.json", @"{""global_parameters"":[],""users"":[{""name"":""management"",""password"":""" + _rabbitManagerPass + @""",""tags"":""management""},{""name"":""guest"",""password"":""" + _rabbitGuestPass + @""",""tags"":""administrator""}],""vhosts"":[{""name"":""/""}],""permissions"":[{""user"":""guest"",""vhost"":""/"",""configure"":"".*"",""read"":"".*"",""write"":"".*""}],""parameters"":[],""policies"":[],""queues"":[],""exchanges"":[],""bindings"":[]}");
            
            ApplicationResources.Add(new Secret(this.Namespace, "rabbitmq", secret));
        }

        private void ConstructConfigMap()
        {
            IDictionary<string, string> data = new Dictionary<string, string>();

            data.Add("enabled_plugins", "[rabbitmq_shovel, rabbitmq_shovel_management, rabbitmq_federation,rabbitmq_federation_management, rabbitmq_consistent_hash_exchange, rabbitmq_management, rabbitmq_peer_discovery_k8s].");
            data.Add("rabbitmq.conf", @"cluster_formation.peer_discovery_backend  = rabbit_peer_discovery_k8s
                                        cluster_formation.k8s.host = kubernetes.default.svc.cluster.local
                                        cluster_formation.k8s.address_type = hostname
                                        cluster_formation.node_cleanup.interval = 10
                                        cluster_formation.node_cleanup.only_log_warning = true
                                        cluster_partition_handling = autoheal
                                        loopback_users.guest = false
                                        management.load_definitions = /etc/definitions/definitions.json
                                        vm_memory_high_watermark.absolute = 256MB");

            ApplicationResources.Add(new ConfigMap(this.Namespace, "rabbitmq", data));
        }

        public RabbitMQApplication(IDataRepository<ConfigurationEntry> repository) : base(repository)
        {

            _rabbitManagerPass = repository.Get("rabbitmq.management.pass")?.Value;
            _rabbitGuestPass = repository.Get("rabbitmq.guest.pass")?.Value;
            _cookie = repository.Get("rabbitmq.cookie")?.Value;

            if (_rabbitGuestPass == null)
            {
                _rabbitGuestPass = StringUtils.RandomString(20);
                repository.Add(new ConfigurationEntry("rabbitmq.guest.pass", _rabbitGuestPass));
            }

            if (_rabbitManagerPass == null)
            {
                _rabbitManagerPass = StringUtils.RandomString(20);
                repository.Add(new ConfigurationEntry("rabbitmq.management.pass", _rabbitManagerPass));
            }
            
            if(_cookie == null)
            {
                _cookie = StringUtils.RandomString(32);
                repository.Add(new ConfigurationEntry("rabbitmq.cookie", _cookie));
            }

            ConstructResources();

        }

        // Test Only
        public RabbitMQApplication()
        {

            _rabbitManagerPass = "test";
            _rabbitGuestPass = "test";
            _cookie = StringUtils.RandomString(32);

            ConstructResources();

        }

        private void ConstructResources()
        {
            ConstructConfigMap();
            ConstructSecret();
            ConstructServiceAccount();
            ConstructRole();
            ConstructRoleBinding();
            ConstructDiscoveryService();
            ConstructService();
            ConstructStatefulSet();
        }
    }
}
