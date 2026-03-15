using System;
using System.Collections.Generic;
using System.Linq;

namespace OpcClientDriver
{
    public static class OpcClientDriverProtocol
    {
        public const string TEST_COMM_DYNAMIC_SETTINGS = "OpcClientDriver.Station={0}|LinkType=1|AN={1}|IP=\\Server\\ServerStatus|EU={2}|RP=/Server/ServerStatus|RN=i=2256|IE=False";

        public static string GetHostNameFromEndEpointUrl(string endpointUrl)
        {
            string hostName = string.Empty;
            if (!string.IsNullOrEmpty(endpointUrl))
            {
                //get host name for endpoint url
                var seu = endpointUrl;
                int start = seu.IndexOf("://");
                if (start != -1)
                {
                    start += 3;
                    int end = seu.IndexOf(":", start); //normal format
                    if (end == -1)
                        end = seu.IndexOf("/", start); //net.pipe
                    if (end == -1)
                        hostName = seu.Substring(start);
                    else
                        hostName = seu.Substring(start, end - start);
                }
            }

            return hostName;
        }
    }
}
