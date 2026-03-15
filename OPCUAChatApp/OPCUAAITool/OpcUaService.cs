using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OPCUAAITool
{
    public class OpcUaService : IOpcUaService
    {
        readonly OpcUaSettings _settings;
        Session? _session;
        List<Dictionary<string, object>> _variables = new();

        public OpcUaService(OpcUaSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IEnumerable<OpcVariableDto> GetDiscoveredVariables()
        {
            lock (_variables)
            {
                return _variables.Select(d => new OpcVariableDto
                {
                    DisplayName = d.ContainsKey("DisplayName") ? d["DisplayName"].ToString() ?? string.Empty : string.Empty,
                    NodeId = d.ContainsKey("NodeId") ? d["NodeId"].ToString() ?? string.Empty : string.Empty,
                    DataType = d.ContainsKey("DataType") ? d["DataType"] : null
                }).ToList();
            }
        }

        public Task<IEnumerable<string>> GetNodeIdAsync(string name, string device)
        {
            var splits = name.Split('.');
            lock (_variables)
            {
                var results = _variables
                    .Where(dict =>
                    {
                        if (!string.IsNullOrEmpty(device))
                        {
                            if (dict["DisplayName"].ToString().IndexOf(device, StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                return false;
                            }
                        }

                        if (dict.ContainsKey("DisplayName"))
                        {
                            for (int i = 0; i < splits.Length; i++)
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
                return Task.FromResult(results);
            }
        }

        public Task<IEnumerable<string>> GetVariableNameAsync(string nodeid)
        {
            lock (_variables)
            {
                var results = _variables
                    .Where(dict => dict.ContainsKey("NodeId") && dict["NodeId"].ToString() == nodeid)
                    .Select(dict => dict.ContainsKey("DisplayName") ? dict["DisplayName"].ToString() : null)
                    .Where(nodeId => nodeId != null);
                return Task.FromResult(results);
            }
        }

        public Task<string?> GetValueAsync(string nodeid)
        {
            ReadValueId nodeToRead = new ReadValueId
            {
                NodeId = (NodeId)nodeid,
                AttributeId = Attributes.Value
            };

            DataValueCollection results;
            DiagnosticInfoCollection diagnosticInfos;
            _session!.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                new ReadValueIdCollection { nodeToRead },
                out results,
                out diagnosticInfos);

            DataValue dataValue = results[0];
            return Task.FromResult<string?>(dataValue.ToString());
        }

        public Task SetValueAsync(string nodeid, string value)
        {
            if (_session == null) throw new InvalidOperationException("Session is not connected.");

            // Read DataType and ValueRank attributes from the server for the node
            ReadValueId dataTypeRead = new ReadValueId { NodeId = (NodeId)nodeid, AttributeId = Attributes.DataType };
            ReadValueId valueRankRead = new ReadValueId { NodeId = (NodeId)nodeid, AttributeId = Attributes.ValueRank };

            DataValueCollection readResults;
            DiagnosticInfoCollection diag;

            _session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                new ReadValueIdCollection { dataTypeRead, valueRankRead },
                out readResults,
                out diag);

            NodeId? dataTypeId = null;
            int valueRank = -2; // unknown

            if (readResults != null && readResults.Count > 0)
            {
                var dtVal = readResults[0];
                if (dtVal != null && dtVal.Value is NodeId nid)
                    dataTypeId = nid;
            }
            if (readResults != null && readResults.Count > 1)
            {
                var vrVal = readResults[1];
                if (vrVal != null && vrVal.Value is int vr)
                    valueRank = vr;
            }

            bool isArray = valueRank >= 0;

            object variantValue;

            if (dataTypeId != null)
            {
                // Map common DataTypeIds to CLR types
                if (dataTypeId == DataTypeIds.Boolean)
                {
                    if (isArray)
                        variantValue = ParseArray<bool>(value, bool.TryParse);
                    else
                        variantValue = bool.Parse(value);
                }
                else if (dataTypeId == DataTypeIds.SByte)
                {
                    if (isArray)
                        variantValue = ParseArray<sbyte>(value, (string s, out sbyte r) => sbyte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = sbyte.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Byte)
                {
                    if (isArray)
                        variantValue = ParseArray<byte>(value, (string s, out byte r) => byte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = byte.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Int16)
                {
                    if (isArray)
                        variantValue = ParseArray<short>(value, (string s, out short r) => short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = short.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.UInt16)
                {
                    if (isArray)
                        variantValue = ParseArray<ushort>(value, (string s, out ushort r) => ushort.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = ushort.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Int32)
                {
                    if (isArray)
                        variantValue = ParseArray<int>(value, (string s, out int r) => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.UInt32)
                {
                    if (isArray)
                        variantValue = ParseArray<uint>(value, (string s, out uint r) => uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = uint.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Int64)
                {
                    if (isArray)
                        variantValue = ParseArray<long>(value, (string s, out long r) => long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = long.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.UInt64)
                {
                    if (isArray)
                        variantValue = ParseArray<ulong>(value, (string s, out ulong r) => ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = ulong.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Float)
                {
                    if (isArray)
                        variantValue = ParseArray<float>(value, (string s, out float r) => float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = float.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.Double)
                {
                    if (isArray)
                        variantValue = ParseArray<double>(value, (string s, out double r) => double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out r));
                    else
                        variantValue = double.Parse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                }
                else if (dataTypeId == DataTypeIds.String)
                {
                    if (isArray)
                        variantValue = value.Split(',').Select(s => s.Trim()).ToArray();
                    else
                        variantValue = value;
                }
                else if (dataTypeId == DataTypeIds.DateTime)
                {
                    if (isArray)
                        variantValue = ParseArray<DateTime>(value, (string s, out DateTime r) => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out r));
                    else
                        variantValue = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                }
                else if (dataTypeId == DataTypeIds.Guid)
                {
                    if (isArray)
                        variantValue = ParseArray<Guid>(value, (string s, out Guid r) => Guid.TryParse(s, out r));
                    else
                        variantValue = Guid.Parse(value);
                }
                else if (dataTypeId == DataTypeIds.ByteString)
                {
                    // expect base64 for byte strings
                    if (isArray)
                    {
                        // not supported array of bytestrings
                        throw new InvalidOperationException("Array of ByteString not supported via comma-separated input.");
                    }
                    variantValue = Convert.FromBase64String(value);
                }
                else
                {
                    // fallback to best-effort numeric/boolean parsing
                    if (bool.TryParse(value, out var vb))
                        variantValue = vb;
                    else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vi))
                        variantValue = vi;
                    else if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var vd))
                        variantValue = vd;
                    else
                        variantValue = value;
                }
            }
            else
            {
                // No DataType available, fall back to previous heuristics
                if (bool.TryParse(value, out var vb))
                {
                    variantValue = vb;
                }
                else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vi))
                {
                    variantValue = vi;
                }
                else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vl))
                {
                    variantValue = vl;
                }
                else if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var vd))
                {
                    variantValue = vd;
                }
                else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var vdt))
                {
                    variantValue = vdt;
                }
                else
                {
                    variantValue = value;
                }
            }

            var writeValue = new WriteValue
            {
                NodeId = (NodeId)nodeid,
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(variantValue))
            };

            var writeValues = new WriteValueCollection { writeValue };
            StatusCodeCollection results;
            DiagnosticInfoCollection diagnosticInfos;

            try
            {
                _session.Write(
                    null,
                    writeValues,
                    out results,
                    out diagnosticInfos);

                var status = results[0];
                if (StatusCode.IsBad(status))
                {
                    throw new InvalidOperationException($"Write failed for node {nodeid}: {status}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error writing node {nodeid}: {ex.Message}", ex);
            }

            return Task.CompletedTask;
        }

        // helper to parse comma-separated arrays
        private static T[] ParseArray<T>(string input, TryParseDelegate<T> tryParse)
        {
            var parts = input.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToArray();
            var list = new List<T>(parts.Length);
            foreach (var part in parts)
            {
                if (tryParse(part, out var parsed))
                    list.Add(parsed);
                else
                    throw new FormatException($"Failed to parse '{part}' as {typeof(T).Name}");
            }
            return list.ToArray();
        }

        private delegate bool TryParseDelegate<T>(string s, out T result);

        void BrowseNode(Session session, NodeId nodeId, string Folder)
        {
            ReferenceDescriptionCollection references;
            byte[] continuationPoint;

            do
            {
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
                    NodeId nodeIdServer = ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris);

                    if (nodeIdServer == ObjectIds.Server)
                    {
                        continue;
                    }

                    if (rd.NodeClass == NodeClass.Variable)
                    {
                        var variableProperties = new Dictionary<string, object>();

                        try
                        {
                            variableProperties["DisplayName"] = Folder + rd.DisplayName.Text;
                            variableProperties["DataType"] = rd.TypeId;
                            variableProperties["NodeId"] = rd.NodeId;

                            lock (_variables)
                                _variables.Add(variableProperties);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading node {rd.NodeId}: {ex.Message}");
                            continue;
                        }
                    }

                    if (rd.NodeClass == NodeClass.Object)
                    {
                        BrowseNode(session, (NodeId)rd.NodeId, String.IsNullOrEmpty(Folder) ?
                                                            rd.DisplayName.Text + "." :
                                                            Folder + rd.DisplayName.Text + ".");
                    }
                }
            } while (continuationPoint != null && continuationPoint.Length > 0);
        }

        public async Task ConnectToServerAsync()
        {
            var serverUrl = _settings.ListServers.FirstOrDefault()?.ServerUrl;

            if (string.IsNullOrEmpty(serverUrl))
                throw new InvalidOperationException("No server configured");

            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationName = _settings.ApplicationName;
            application.ApplicationType = ApplicationType.Client;
            application.ConfigSectionName = "OPCUAClient";

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
            _session = await Session.Create(config, endpoint, false, "MinimalClient", (uint)_settings.ListServers.First().TimeoutMs, null, null);

            BrowseNode(_session, ObjectIds.ObjectsFolder, String.Empty);
        }
    }
}
