// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcStatusCodes = Opc.Ua.StatusCodes;
using OpcSession = Opc.Ua.Client.ISession;

namespace ServerEditorWeb.Services;

/// <summary>
/// Describes a certificate stored in a PKI directory store.
/// </summary>
public class CertificateInfo
{
    public string Thumbprint { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Issuer { get; set; } = "";
    public DateTime NotBefore { get; set; }
    public DateTime NotAfter { get; set; }
    public bool IsExpired => DateTime.UtcNow > NotAfter;
    public bool IsNotYetValid => DateTime.UtcNow < NotBefore;
    public string SerialNumber { get; set; } = "";
    public string ApplicationUri { get; set; } = "";
    public List<string> DomainNames { get; set; } = new();
    public string StoreName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public int KeySizeBits { get; set; }
}

/// <summary>
/// Result of a GDS operation.
/// </summary>
public class GdsOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}

/// <summary>
/// Manages OPC UA PKI certificate stores for the Server and Editor,
/// and provides GDS (Global Discovery Server) client support via raw OPC UA sessions.
/// Works with directory-based certificate stores (DER files).
/// </summary>
public class CertificateService
{
    private readonly string _serverPkiRoot;
    private readonly string _editorPkiRoot;

    public string ServerPkiRoot => _serverPkiRoot;
    public string EditorPkiRoot => _editorPkiRoot;

    public CertificateService()
    {
        _serverPkiRoot = Utils.ReplaceSpecialFolderNames("%LocalApplicationData%/SimpleOpcFileServer/pki");
        _editorPkiRoot = Utils.ReplaceSpecialFolderNames("%LocalApplicationData%/ServerEditorWeb/pki");
    }

    /// <summary>
    /// Lists certificates in a specific store directory.
    /// </summary>
    public List<CertificateInfo> ListCertificates(string storePath, string storeName)
    {
        var results = new List<CertificateInfo>();
        var resolvedPath = ResolvePath(storePath);
        if (!Directory.Exists(resolvedPath))
            return results;

        var extensions = new[] { "*.der", "*.crt", "*.pem", "*.cer" };
        foreach (var dir in GetSearchDirs(resolvedPath))
        {
            foreach (var ext in extensions)
            {
                foreach (var file in Directory.EnumerateFiles(dir, ext))
                {
                    try
                    {
                        var cert = X509CertificateLoader.LoadCertificateFromFile(file);
                        var info = BuildCertificateInfo(cert, storeName, file);
                        if (!results.Any(r => r.Thumbprint == info.Thumbprint))
                            results.Add(info);
                        cert.Dispose();
                    }
                    catch { }
                }
            }
        }

        return results.OrderBy(c => c.Subject).ToList();
    }

    public List<CertificateInfo> GetServerTrusted() => ListCertificates(Path.Combine(_serverPkiRoot, "trusted"), "Trusted");
    public List<CertificateInfo> GetServerRejected() => ListCertificates(Path.Combine(_serverPkiRoot, "rejected"), "Rejected");
    public List<CertificateInfo> GetServerIssuers() => ListCertificates(Path.Combine(_serverPkiRoot, "issuer"), "Issuer");
    public List<CertificateInfo> GetServerOwn() => ListCertificates(Path.Combine(_serverPkiRoot, "own"), "Own");

    public List<CertificateInfo> GetEditorTrusted() => ListCertificates(Path.Combine(_editorPkiRoot, "trusted"), "Trusted");
    public List<CertificateInfo> GetEditorRejected() => ListCertificates(Path.Combine(_editorPkiRoot, "rejected"), "Rejected");

    /// <summary>
    /// Trust a certificate: move from rejected to trusted store.
    /// </summary>
    public bool TrustCertificate(string pkiRoot, string thumbprint)
    {
        try
        {
            var rejectedDir = ResolvePath(Path.Combine(pkiRoot, "rejected"));
            var trustedDir = ResolvePath(Path.Combine(pkiRoot, "trusted", "certs"));
            Directory.CreateDirectory(trustedDir);

            var file = FindCertFile(rejectedDir, thumbprint);
            if (file == null) return false;

            var destFile = Path.Combine(trustedDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
            File.Delete(file);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Reject a certificate: move from trusted to rejected store.
    /// </summary>
    public bool RejectCertificate(string pkiRoot, string thumbprint)
    {
        try
        {
            var trustedDir = ResolvePath(Path.Combine(pkiRoot, "trusted"));
            var rejectedDir = ResolvePath(Path.Combine(pkiRoot, "rejected", "certs"));
            Directory.CreateDirectory(rejectedDir);

            var file = FindCertFile(trustedDir, thumbprint);
            if (file == null) return false;

            var destFile = Path.Combine(rejectedDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
            File.Delete(file);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Delete a certificate from any store.
    /// </summary>
    public bool DeleteCertificate(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return false;
            File.Delete(filePath);
            // Also delete matching private key if it exists
            var privateKeyPath = filePath.Replace("/certs/", "/private/").Replace("\\certs\\", "\\private\\");
            privateKeyPath = Path.ChangeExtension(privateKeyPath, ".pfx");
            if (File.Exists(privateKeyPath))
                File.Delete(privateKeyPath);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Export a certificate as DER bytes.
    /// </summary>
    public byte[]? ExportCertificate(string filePath)
    {
        try
        {
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }
        catch { return null; }
    }

    /// <summary>
    /// Import a certificate into a target store.
    /// </summary>
    public bool ImportCertificate(string pkiRoot, string store, byte[] certData, string fileName)
    {
        try
        {
            var cert = X509CertificateLoader.LoadCertificate(certData);
            var targetDir = ResolvePath(Path.Combine(pkiRoot, store, "certs"));
            Directory.CreateDirectory(targetDir);

            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext)) ext = ".der";
            File.WriteAllBytes(Path.Combine(targetDir, $"{cert.Thumbprint}{ext}"), certData);
            cert.Dispose();
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Connect to a GDS, register the application, and request a signed certificate.
    /// Uses raw OPC UA session calls (Part 12 services) to avoid tight coupling with
    /// a specific GDS client library version.
    /// </summary>
    public async Task<GdsOperationResult> RegisterWithGdsAsync(
        string gdsEndpointUrl, string applicationName, string applicationUri,
        string? gdsUserName, string? gdsPassword)
    {
        try
        {
            var (session, config) = await CreateGdsSessionAsync(gdsEndpointUrl, gdsUserName, gdsPassword);

            try
            {
                // GDS well-known NodeIds (OPC UA Part 12)
                var gdsObjectId = new NodeId(12637);          // Directory object
                var registerMethodId = new NodeId(12613);     // RegisterApplication
                var startKeyPairMethodId = new NodeId(12620); // StartNewKeyPairRequest

                // Register the application by calling the GDS RegisterApplication method.
                // Input args: ApplicationRecordDataType encoded as an ExtensionObject.
                // We build the record manually via EncodeableObject to avoid dependency
                // on the GDS-specific type if it is not available at compile time.
                var registerResult = session.Call(
                    gdsObjectId,
                    registerMethodId,
                    applicationName,
                    applicationUri,
                    (byte)ApplicationType.Server,
                    new StringCollection());

                var applicationId = registerResult?.Count > 0 ? registerResult[0]?.ToString() : "unknown";

                // Request a new key pair
                var subjectName = $"CN={applicationName}";
                var domainNames = new StringCollection { System.Net.Dns.GetHostName() };

                try
                {
                    session.Call(
                        gdsObjectId,
                        startKeyPairMethodId,
                        applicationId,
                        NodeId.Null,
                        NodeId.Null,
                        subjectName,
                        domainNames,
                        "PFX",
                        "");

                    return new GdsOperationResult
                    {
                        Success = true,
                        Message = $"Registered with GDS. Application ID: {applicationId}. " +
                                  "Certificate request submitted (may require GDS admin approval)."
                    };
                }
                catch (ServiceResultException)
                {
                    return new GdsOperationResult
                    {
                        Success = true,
                        Message = $"Registered with GDS. Application ID: {applicationId}. " +
                                  "Certificate signing not available — contact GDS administrator."
                    };
                }
            }
            finally
            {
                session.Close();
                session.Dispose();
            }
        }
        catch (Exception ex)
        {
            return new GdsOperationResult
            {
                Success = false,
                Message = $"GDS registration failed: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Pull the trust list from a GDS and import certificates into the Server's stores.
    /// </summary>
    public async Task<GdsOperationResult> PullTrustListAsync(
        string gdsEndpointUrl, string applicationName, string applicationUri,
        string? gdsUserName, string? gdsPassword)
    {
        try
        {
            var (session, config) = await CreateGdsSessionAsync(gdsEndpointUrl, gdsUserName, gdsPassword);
            int importedCount = 0;

            try
            {
                // Read the default trust list object
                // GDS GetTrustList method: the trust list is typically a FileType node
                // We browse the GDS to find the TrustList node and read its content

                var gdsObjectId = new NodeId(12637); // GDS Directory object
                var getTrustListMethodId = new NodeId(12625); // GetTrustList

                try
                {
                    var trustListResult = session.Call(
                        gdsObjectId,
                        getTrustListMethodId,
                        NodeId.Null,    // applicationId
                        NodeId.Null);   // certificateGroupId

                    if (trustListResult?.Count > 0 && trustListResult[0] is NodeId trustListNodeId)
                    {
                        // Read the trust list using the OPC UA File Transfer (Open, Read, Close)
                        importedCount = await ReadAndImportTrustListFileAsync(session, trustListNodeId);
                    }
                }
                catch (ServiceResultException ex) when (ex.StatusCode == OpcStatusCodes.BadNotFound ||
                                                         ex.StatusCode == OpcStatusCodes.BadMethodInvalid)
                {
                    return new GdsOperationResult
                    {
                        Success = false,
                        Message = "GDS does not support GetTrustList method or trust list not found."
                    };
                }
            }
            finally
            {
                session.Close();
                session.Dispose();
            }

            return new GdsOperationResult
            {
                Success = true,
                Message = $"Trust list updated. {importedCount} certificate(s) imported from GDS."
            };
        }
        catch (Exception ex)
        {
            return new GdsOperationResult
            {
                Success = false,
                Message = $"Pull trust list failed: {ex.Message}"
            };
        }
    }

    private async Task<(OpcSession session, ApplicationConfiguration config)> CreateGdsSessionAsync(
        string gdsEndpointUrl, string? userName, string? password)
    {
        var config = new ApplicationConfiguration
        {
            ApplicationName = "ServerEditorWebGdsClient",
            ApplicationUri = Utils.Format("urn:{0}:ServerEditorWebGdsClient", System.Net.Dns.GetHostName()),
            ApplicationType = Opc.Ua.ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_editorPkiRoot, "own"),
                    SubjectName = "ServerEditorWebGdsClient"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_editorPkiRoot, "trusted")
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_editorPkiRoot, "issuer")
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_editorPkiRoot, "rejected")
                }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 30000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

        await config.Validate(Opc.Ua.ApplicationType.Client);
        config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = true; };

        var app = new ApplicationInstance
        {
            ApplicationName = "ServerEditorWebGdsClient",
            ApplicationType = Opc.Ua.ApplicationType.Client,
            ApplicationConfiguration = config
        };

        await app.CheckApplicationInstanceCertificates(false, 2048);

        var discoveryClient = DiscoveryClient.Create(new Uri(gdsEndpointUrl));
        var endpoints = discoveryClient.GetEndpoints(null);
        discoveryClient.Dispose();

        var selectedEndpoint = endpoints
            .FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
            ?? endpoints.FirstOrDefault();

        if (selectedEndpoint == null)
            throw new Exception("No GDS endpoints found");

        var endpointConfig = EndpointConfiguration.Create(config);
        var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfig);

        UserIdentity identity;
        if (string.IsNullOrEmpty(userName))
        {
            identity = new UserIdentity(new AnonymousIdentityToken());
        }
        else
        {
            var token = new UserNameIdentityToken
            {
                UserName = userName,
                DecryptedPassword = System.Text.Encoding.UTF8.GetBytes(password ?? "")
            };
            identity = new UserIdentity(token);
        }

        var session = await Session.Create(
            config, endpoint, false,
            "ServerEditorWebGdsClient", 60000, identity, null);

        return (session, config);
    }

    private async Task<int> ReadAndImportTrustListFileAsync(OpcSession session, NodeId trustListNodeId)
    {
        // Use OPC UA file transfer: Open → Read → Close
        int count = 0;
        try
        {
            // Read the Size property to know how much data to expect
            var sizeNodeId = new NodeId(trustListNodeId.Identifier + "/Size", trustListNodeId.NamespaceIndex);

            // Open the file (mode 1 = read)
            var openMethod = new NodeId(trustListNodeId.Identifier + "/Open", trustListNodeId.NamespaceIndex);
            var openResult = session.Call(trustListNodeId, openMethod, (byte)1);
            if (openResult?.Count > 0 && openResult[0] is uint fileHandle)
            {
                try
                {
                    // Read data in chunks
                    var readMethod = new NodeId(trustListNodeId.Identifier + "/Read", trustListNodeId.NamespaceIndex);
                    var allData = new List<byte>();
                    while (true)
                    {
                        var readResult = session.Call(trustListNodeId, readMethod, fileHandle, 65536);
                        if (readResult?.Count > 0 && readResult[0] is byte[] chunk && chunk.Length > 0)
                        {
                            allData.AddRange(chunk);
                            if (chunk.Length < 65536) break;
                        }
                        else break;
                    }

                    // Close the file
                    var closeMethod = new NodeId(trustListNodeId.Identifier + "/Close", trustListNodeId.NamespaceIndex);
                    session.Call(trustListNodeId, closeMethod, fileHandle);

                    // Parse the trust list (UA Binary encoded TrustListDataType)
                    if (allData.Count > 0)
                    {
                        count = ImportTrustListData(allData.ToArray());
                    }
                }
                catch
                {
                    // Try to close the file handle on error
                    try
                    {
                        var closeMethod = new NodeId(trustListNodeId.Identifier + "/Close", trustListNodeId.NamespaceIndex);
                        session.Call(trustListNodeId, closeMethod, fileHandle);
                    }
                    catch { }
                    throw;
                }
            }
        }
        catch
        {
            // File transfer approach not supported; return 0
        }

        await Task.CompletedTask;
        return count;
    }

    private int ImportTrustListData(byte[] data)
    {
        // TrustListDataType is UA Binary encoded; try to decode individual DER certificates
        int count = 0;
        var trustedDir = ResolvePath(Path.Combine(_serverPkiRoot, "trusted", "certs"));
        var issuerDir = ResolvePath(Path.Combine(_serverPkiRoot, "issuer", "certs"));
        Directory.CreateDirectory(trustedDir);
        Directory.CreateDirectory(issuerDir);

        // Try to decode as OPC UA binary using the SDK
        try
        {
            using var decoder = new BinaryDecoder(data, ServiceMessageContext.GlobalContext);
            var specifiedLists = decoder.ReadUInt32("SpecifiedLists");

            // Bit 0 = TrustedCertificates
            if ((specifiedLists & 0x01) != 0)
            {
                var certCount = decoder.ReadInt32("NoOfTrustedCertificates");
                for (int i = 0; i < certCount; i++)
                {
                    var certData = decoder.ReadByteString("TrustedCertificates");
                    if (certData != null && certData.Length > 0)
                    {
                        try
                        {
                            var cert = X509CertificateLoader.LoadCertificate(certData);
                            File.WriteAllBytes(Path.Combine(trustedDir, $"{cert.Thumbprint}.der"), certData);
                            cert.Dispose();
                            count++;
                        }
                        catch { }
                    }
                }
            }

            // Bit 1 = TrustedCRLs (skip)
            if ((specifiedLists & 0x02) != 0)
            {
                var crlCount = decoder.ReadInt32("NoOfTrustedCrls");
                for (int i = 0; i < crlCount; i++)
                    decoder.ReadByteString("TrustedCrls");
            }

            // Bit 2 = IssuerCertificates
            if ((specifiedLists & 0x04) != 0)
            {
                var certCount = decoder.ReadInt32("NoOfIssuerCertificates");
                for (int i = 0; i < certCount; i++)
                {
                    var certData = decoder.ReadByteString("IssuerCertificates");
                    if (certData != null && certData.Length > 0)
                    {
                        try
                        {
                            var cert = X509CertificateLoader.LoadCertificate(certData);
                            File.WriteAllBytes(Path.Combine(issuerDir, $"{cert.Thumbprint}.der"), certData);
                            cert.Dispose();
                            count++;
                        }
                        catch { }
                    }
                }
            }
        }
        catch { }

        return count;
    }

    private static CertificateInfo BuildCertificateInfo(X509Certificate2 cert, string storeName, string filePath)
    {
        var info = new CertificateInfo
        {
            Thumbprint = cert.Thumbprint,
            Subject = cert.Subject,
            Issuer = cert.Issuer,
            NotBefore = cert.NotBefore.ToUniversalTime(),
            NotAfter = cert.NotAfter.ToUniversalTime(),
            SerialNumber = cert.SerialNumber,
            StoreName = storeName,
            FilePath = filePath,
            KeySizeBits = cert.PublicKey.GetRSAPublicKey()?.KeySize
                          ?? cert.PublicKey.GetECDsaPublicKey()?.KeySize
                          ?? 0
        };

        // Extract Application URI and DNS names from Subject Alternative Name
        try
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext is X509SubjectAlternativeNameExtension sanExt)
                {
                    var formatted = sanExt.Format(true);
                    foreach (var line in formatted.Split('\n', '\r'))
                    {
                        var trimmed = line.Trim();
                        if (trimmed.StartsWith("URL=", StringComparison.OrdinalIgnoreCase) ||
                            trimmed.StartsWith("URI:", StringComparison.OrdinalIgnoreCase))
                        {
                            var val = trimmed.Substring(trimmed.IndexOf('=') + 1).Trim();
                            if (val.Length == 0 && trimmed.Contains(':'))
                                val = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                            if (!string.IsNullOrEmpty(val))
                                info.ApplicationUri = val;
                        }
                        else if (trimmed.StartsWith("DNS Name=", StringComparison.OrdinalIgnoreCase) ||
                                 trimmed.StartsWith("DNS:", StringComparison.OrdinalIgnoreCase))
                        {
                            var val = trimmed.Substring(trimmed.IndexOf('=') + 1).Trim();
                            if (val.Length == 0 && trimmed.Contains(':'))
                                val = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                            if (!string.IsNullOrEmpty(val))
                                info.DomainNames.Add(val);
                        }
                    }
                }
            }
        }
        catch { }

        return info;
    }

    private static string? FindCertFile(string storeDir, string thumbprint)
    {
        if (!Directory.Exists(storeDir)) return null;
        var extensions = new[] { "*.der", "*.crt", "*.pem", "*.cer" };

        foreach (var dir in GetSearchDirs(storeDir))
        {
            foreach (var ext in extensions)
            {
                foreach (var file in Directory.EnumerateFiles(dir, ext))
                {
                    try
                    {
                        var cert = X509CertificateLoader.LoadCertificateFromFile(file);
                        var match = cert.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase);
                        cert.Dispose();
                        if (match) return file;
                    }
                    catch { }
                }
            }
        }
        return null;
    }

    private static IEnumerable<string> GetSearchDirs(string rootDir)
    {
        yield return rootDir;
        var certsSubDir = Path.Combine(rootDir, "certs");
        if (Directory.Exists(certsSubDir))
            yield return certsSubDir;
    }

    private static string ResolvePath(string path)
    {
        return Utils.ReplaceSpecialFolderNames(path);
    }
}
