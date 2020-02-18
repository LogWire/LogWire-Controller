using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.Controller.Kubernetes.Applications.Utils.Commands
{
    public class ElasticsearchCommands
    {
        private static string command = "START_FILE=/tmp/.es_start_file\n\n" +
                                        "http () {\n" +
                                        "    local path=\"${1}\"\n" +
                                        "    if [ -n \"${ELASTIC_USERNAME}\" ] && [ -n \"${ELASTIC_PASSWORD}\" ]; then\n" +
                                        "      BASIC_AUTH=\"-u ${ELASTIC_USERNAME}:${ELASTIC_PASSWORD}\"\n" +
                                        "    else\n" +
                                        "      BASIC_AUTH=''\n" +
                                        "    fi\n" +
                                        "    curl -XGET -s -k --fail ${BASIC_AUTH} http://127.0.0.1:9200${path}\n" +
                                        "}\n\n" +
                                        "if [ -f \"${START_FILE}\" ]; then\n" +
                                        "    echo 'Elasticsearch is already running, lets check the node is healthy and there are master nodes available'\n" +
                                        "    http \"/_cluster/health?timeout=0s\"\n" +
                                        "else\n" +
                                        "    echo 'Waiting for elasticsearch cluster to become ready (request params: \"wait_for_status=green&timeout=1s\" )'\n" +
                                        "    if http \"/_cluster/health?wait_for_status=green&timeout=1s\" ; then\n" +
                                        "        touch ${START_FILE}\n" +
                                        "        exit 0\n" +
                                        "    else\n" +
                                        "        echo 'Cluster is not yet ready (request params: \"wait_for_status=green&timeout=1s\" )'\n" +
                                        "        exit 1\n" +
                                        "    fi\n" +
                                        "fi";

        public static IList<string> ReadCommands = new List<string> {"sh", "-c", command};
        
    }
}
