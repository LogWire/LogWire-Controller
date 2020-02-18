using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using k8s.Models;
using LogWire.Controller.Kubernetes.Applications.Utils.Commands;
using LogWire.Controller.Kubernetes.Resources;

namespace LogWire.Controller.Kubernetes.Applications
{
    public class ElasticSearchApplication : KubernetesApplication
    {

        Dictionary<string,string> _labels = new Dictionary<string, string>{{"app", "elasticsearch"}}; 
        
        private List<V1ServicePort> _ports = new List<V1ServicePort>{
            new V1ServicePort(9200, "http"),
            new V1ServicePort(9300, "transport")
        };

        private IDictionary<string, ResourceQuantity> _resourceLimits = new Dictionary<string, ResourceQuantity>
        {
            { "cpu", new ResourceQuantity("1000m") },
            { "memory", new ResourceQuantity("2Gi") } 
        };

        private int _replicas = 3;
        private int _ramLimit = 1;

        protected override string Namespace => "elasticsearch";

        protected override void ConstructResources()
        {
            ConstructPodDisruptionBudget();
            ConstructService();
            ConstructHeadlessService();
            ConstructStatefulSet();
        }

        private void ConstructStatefulSet()
        {
            V1PodTemplateSpec podTemplate = new V1PodTemplateSpec {
                Metadata = new V1ObjectMeta(namespaceProperty: Namespace, labels:_labels),
                Spec = new V1PodSpec
                {
                    SecurityContext = new V1PodSecurityContext(fsGroup: 1000, runAsUser: 1000),
                    Affinity = new V1Affinity
                    {
                        PodAffinity = new V1PodAffinity
                        {
                            RequiredDuringSchedulingIgnoredDuringExecution = new List<V1PodAffinityTerm>
                            {
                                new V1PodAffinityTerm("kubernetes.io/hostname", new V1LabelSelector(matchLabels: _labels))
                            }
                        }
                    },
                    TerminationGracePeriodSeconds = 120,
                    InitContainers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "configure-sysctl",
                            SecurityContext = new V1SecurityContext(runAsUser: 0, privileged: true),
                            Image = "docker.elastic.co/elasticsearch/elasticsearch:7.6.0",
                            ImagePullPolicy = "IfNotPresent",
                            Command = new List<string>
                            {
                                "sysctl", "-w", "vm.max_map_count=262144"
                            }
                        }
                    },
                    Containers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "elasticsearch",
                            SecurityContext = new V1SecurityContext
                            {
                                Capabilities = new V1Capabilities(drop: new List<string>{ "ALL" }),
                                RunAsNonRoot = true,
                                RunAsUser = 1000
                            },
                            Image = "docker.elastic.co/elasticsearch/elasticsearch:7.6.0",
                            ImagePullPolicy = "IfNotPresent",
                            ReadinessProbe = new V1Probe
                            {
                                FailureThreshold = 3,
                                InitialDelaySeconds = 10,
                                PeriodSeconds = 10,
                                SuccessThreshold = 3,
                                TimeoutSeconds = 5,
                                Exec = new V1ExecAction(ElasticsearchCommands.ReadCommands)
                            },
                            Ports = new List<V1ContainerPort>
                            {
                                new V1ContainerPort(9200, name: "http"),
                                new V1ContainerPort(9300, name: "transport")
                            },
                            Resources = new V1ResourceRequirements
                            {
                                Limits = _resourceLimits,
                                Requests = _resourceLimits
                            },
                            Env = new List<V1EnvVar>
                            {
                                new V1EnvVar("node.name", valueFrom: new V1EnvVarSource(fieldRef: new V1ObjectFieldSelector(fieldPath: "metadata.name"))),
                                new V1EnvVar("cluster.initial_master_nodes", value: GetMasterNodes()),
                                new V1EnvVar("discovery.seed_hosts", "elasticsearch-headless"),
                                new V1EnvVar("cluster.name", "elasticsearch"),
                                new V1EnvVar("network.host", "0.0.0.0"),
                                new V1EnvVar("ES_JAVA_OPTS", "-Xmx" + _ramLimit + "g -Xms" + _ramLimit + "g"),
                                new V1EnvVar("node.data", "true"),
                                new V1EnvVar("node.ingest", "true"),
                                new V1EnvVar("node.master", "true")
                            },
                            VolumeMounts = new List<V1VolumeMount>
                            {
                                new V1VolumeMount("/usr/share/elasticsearch/data", "elasticsearch")
                            }
                        }
                    }
                }
            };


            IList<V1PersistentVolumeClaim> volumeTemplate = new List<V1PersistentVolumeClaim>{
                new V1PersistentVolumeClaim
                {
                    Metadata = new V1ObjectMeta(name:"elasticsearch"),
                    Spec = new V1PersistentVolumeClaimSpec
                    {
                        AccessModes = new List<string> { "ReadWriteOnce" },
                        Resources = new V1ResourceRequirements
                        {
                            Requests = new Dictionary<string, ResourceQuantity> {{ "storage", new ResourceQuantity("30Gi") }}
                        }
                    }
                }
            };

            ApplicationResources.Add(new StatefulSet(Namespace, "elasticsearch", _labels, "elasticsearch-headless",
                "Parallel", _replicas, "RollingUpdate", _labels, podTemplate, volumeTemplate));
        }

        private string GetMasterNodes()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _replicas; i++)
            {
                sb.Append("elasticsearch-" + i + ",");
            }

            return sb.ToString();
        }

        private void ConstructHeadlessService()
        {
            ApplicationResources.Add(new Service(Namespace, "elasticsearch-headless", _labels, _ports, publishNotReady: true, clusterIP: "None"));
        }

        private void ConstructService()
        {
            ApplicationResources.Add(new Service(Namespace, "elasticsearch", _labels, _ports));
        }

        private void ConstructPodDisruptionBudget()
        {
            ApplicationResources.Add(new PodDisruptionBudget(Namespace, "elasticsearch-pdb", 1, new V1LabelSelector(matchLabels: _labels)));
        }

    }
}
