using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace ChatApp2.Services.opc
{
    public static class utilities
    {

        static Session session;
        static List<Dictionary<string, object>> variables = new();

        static public IEnumerable<string> GetNodeId(String name)
        {
            var splits = name.Split('.');
            lock (variables)
            {
                var results = variables
                    .Where(dict =>
                    {
                        if (dict.ContainsKey("DisplayName"))
                        {
                            for(int i = 0; i < splits.Length; i++)
                            {
                                var split = splits[i];
                                if (dict["DisplayName"].ToString().IndexOf(split, StringComparison.OrdinalIgnoreCase) < 0)
                                {
                                    return false;
                                }
                            }

                            return true;
                        }

                        return false;
                    })
                    .Select(dict => dict.ContainsKey("NodeId") ? dict["NodeId"].ToString() : null)
                    .Where(nodeId => nodeId != null);
                var list = results.ToList();
                return results;
            }
        }

        static public IEnumerable<string> GetVariableName(String nodeid)
        {
            lock (variables)
            {
                var results = variables
                    .Where(dict => dict.ContainsKey("NodeId") && dict["NodeId"].ToString() == nodeid)
                    .Select(dict => dict.ContainsKey("DisplayName") ? dict["DisplayName"].ToString() : null)
                    .Where(nodeId => nodeId != null);
                var list = results.ToList();
                return results;
            }
        }

        static public String GetValue(String nodeid)
        {
            ReadValueId nodeToRead = new ReadValueId
            {
                NodeId = (NodeId)nodeid,
                AttributeId = Attributes.Value
            };

            DataValueCollection results;
            DiagnosticInfoCollection diagnosticInfos;
            session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                new ReadValueIdCollection { nodeToRead },
                out results,
                out diagnosticInfos);

            DataValue dataValue = results[0];
            return dataValue.ToString();
            //if (StatusCode.IsGood(dataValue.StatusCode))
            //{
            //    var value = dataValue.Value;
            //    // Use the value safely here
            //}
            //else
            //{
            //    // Handle non-good quality, e.g., skip or log
            //}
        }

        static public void SetValue(String nodeid, String value)
        {
        }

        static void BrowseNode(Session session, NodeId nodeId, String Folder)
        {
            ReferenceDescriptionCollection references;
            byte[] continuationPoint;

            do
            {
                // Browse all forward hierarchical references
                session.Browse(
                    null,
                    null,
                    nodeId,
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    0,
                    out continuationPoint,
                    out references);

                foreach (var rd in references)
                {
                    // Skip the Server object node
                    NodeId nodeIdServer = ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris);

                    if (nodeIdServer == ObjectIds.Server)
                    {
                        // Skip processing the Server object node
                        continue;
                    }

                    // If it's a variable node, read its important properties
                    if (rd.NodeClass == NodeClass.Variable)
                    {
                        var variableProperties = new Dictionary<string, object>();

                        // Read variable value and display name
                        try
                        {
                            //ReadValueId nodeToRead = new ReadValueId
                            //{
                            //    NodeId = (NodeId)rd.NodeId,
                            //    AttributeId = Attributes.Value
                            //};

                            //DataValueCollection results;
                            //DiagnosticInfoCollection diagnosticInfos;
                            //session.Read(
                            //    null,
                            //    0,
                            //    TimestampsToReturn.Neither,
                            //    new ReadValueIdCollection { nodeToRead },
                            //    out results,
                            //    out diagnosticInfos);

                            //DataValue dataValue = results[0];

                            //if (StatusCode.IsGood(dataValue.StatusCode))
                            //{
                            //    var value = dataValue.Value;
                            //    // Use the value safely here
                            //}
                            //else
                            //{
                            //    // Handle non-good quality, e.g., skip or log
                            //}

                            //variableProperties["Value"] = dataValue.Value;
                            variableProperties["DisplayName"] = Folder + rd.DisplayName.Text;
                            variableProperties["DataType"] = rd.TypeId;
                            variableProperties["NodeId"] = rd.NodeId;
                            // variableProperties["Browse"] = rd.BrowseName;

                            lock (variables)
                                variables.Add(variableProperties);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading node {rd.NodeId}: {ex.Message}");
                            continue;
                        }
                    }

                    // Recursively browse child nodes
                    if (rd.NodeClass == NodeClass.Object)
                    {
                        BrowseNode(session, (NodeId)rd.NodeId, String.IsNullOrEmpty(Folder) ? 
                                                            rd.DisplayName.Text + "." :
                                                            Folder + rd.DisplayName.Text + ".");
                    }
                }
            } while (continuationPoint != null && continuationPoint.Length > 0);
        }

        public static async Task ConnectToServer()
        {
            var serverUrl = "opc.tcp://villaedenserver:62841/VillaEden_IOServer"; // Server URL

            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationName = "UA Reference Client";
            application.ApplicationType = ApplicationType.Client;
            application.ConfigSectionName = "Quickstarts.ReferenceClient";

            // load the application configuration.
            var config = application.LoadApplicationConfigurationAsync(false).AsTask().Result;

            // check the application certificate.
            var certOK = application.CheckApplicationInstanceCertificatesAsync(false).AsTask().Result;
            if (!certOK)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            // Find the endpoint to connect to
            var endpointDescription = await CoreClientUtils.SelectEndpointAsync(config, serverUrl, useSecurity: false);
            var endpointConfig = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfig);

            // Create the session
            session = await Session.Create(config, endpoint, false, "MinimalClient", 60000, null, null);

            BrowseNode(session, ObjectIds.ObjectsFolder, String.Empty);

            var i = 0;
            // Close session
            // session.Close();
        }
    }
}

