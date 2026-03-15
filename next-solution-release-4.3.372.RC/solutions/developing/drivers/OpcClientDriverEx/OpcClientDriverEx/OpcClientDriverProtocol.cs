using System;
using System.Collections.Generic;
using System.Linq;

namespace OpcClientDriver
{
    public static class OpcClientDriverProtocol
    {
        public const string TEST_COMM_DYNAMIC_SETTINGS = "OpcClientDriver.Station={0}|LinkType=1|AN={1}|IP=\\Server\\ServerStatus|EU={2}|RP=/Server/ServerStatus|RN=i=2256|IE=False";

        public const char BACKUP_HOST_LIST_FIELD_SEPARATOR = '|';

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

        /// <summary>
        /// Return the list (is some elements were sets) of backup opc server list (used to simulate redundancy for OPC who doesn't support it), otherwise null
        /// </summary>
        /// <param name="hostList"></param>
        /// <returns></returns>
        public static String[] GetValidBackupHostList(string hostList)
        {       
            if (string.IsNullOrWhiteSpace(hostList))
                return null;

            var list = hostList.Split(OpcClientDriverProtocol.BACKUP_HOST_LIST_FIELD_SEPARATOR);

            for (int i=0; i < list.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(list[i]))
                    return null;
            }
            
            return list;
        }
    }
}
