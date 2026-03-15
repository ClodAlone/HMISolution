using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Client;
using System.Threading;

#if !WINDOWS_UWP && !NET_STANDARD
using System.IdentityModel.Tokens;
using System.IdentityModel.Claims;
using System.Windows;
#endif
#if !WINDOWS_UWP
using System.Reflection;
using System.Diagnostics;
#endif

namespace OPCUAViewModel
{
    public static class Helpers
    {
        internal static ApplicationInstance application = new ApplicationInstance();
        internal static ApplicationConfiguration configuration;

        /// <summary>
        /// Creates a minimal application configuration for a client.
        /// </summary>
        /// <remarks>
        /// In most cases the application configuration will be loaded from an XML file. 
        /// This example populates the configuration in code.
        /// </remarks>
        /// 
        public static ApplicationConfiguration CreateClientConfiguration()
        {
            if (configuration != null)
                return application.ApplicationConfiguration;

#if !WINDOWS_UWP && !NET_STANDARD
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {
                    configuration = new ApplicationConfiguration();
                    Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(callingMainAssembly.Location);

                        application.ApplicationName = fvi.ProductName; // callingMainAssembly.GetName().Name
                    }
                    else
                        application.ApplicationName = "UFSolution";

                    application.ApplicationType = ApplicationType.Client;
                    application.ConfigSectionName = "OPCUAClient";

                    // process and command line arguments.
                    //if (application.ProcessCommandLine())
                    //{
                    //    return;
                    //}

                    // load the application configuration.
                    application.LoadApplicationConfiguration(false);

                    // check the application certificate.
                    try
                    {
                        application.CheckApplicationInstanceCertificate(false, 0);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, name, MessageBoxButton.OK,
                                    MessageBoxImage.Error, MessageBoxResult.OK,
                                    MessageBoxOptions.ServiceNotification);
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }

//#if DEBUG
//            application.ApplicationConfiguration.TransportQuotas.OperationTimeout = Int32.MaxValue;
//            configuration.ClientConfiguration = new ClientConfiguration();
//            configuration.ClientConfiguration.DefaultSessionTimeout = Int32.MaxValue;
//#endif
            return application.ApplicationConfiguration;

            /*
            // Step 1 - Specify the server identity.
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(callingMainAssembly.Location);
                configuration.ApplicationName = fvi.ProductName; // callingMainAssembly.GetName().Name
                configuration.ApplicationUri = String.Format("http://localhost/{0}/{1}", fvi.CompanyName.Trim(), configuration.ApplicationName);
                // product uri : vendorid, productid, versionid
                configuration.ProductUri = String.Format("http://{0}/{1}/{2}", fvi.CompanyName.Trim(), configuration.ApplicationName, fvi.FileVersion);
            }
            else
                configuration.ApplicationName = "ProgeaClient"; // callingMainAssembly.GetName().Name

            configuration.ApplicationType = ApplicationType.Client;
            // Application uri : vendorid, applicationid, instanceid

            configuration.SecurityConfiguration = new SecurityConfiguration();

            // Step 2 - Specify the server's application instance certificate.

            // Application instance certificates must be placed in a windows certficate store because that is 
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.
            //
            // In this example the following values are used:
            // 
            //   LocalMachine    - use the machine wide certificate store.
            //   Personal        - use the store for individual certificates.
            //   ApplicationName - use the application name as a search key.   

            configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
            configuration.SecurityConfiguration.ApplicationCertificate.StoreType = CertificateStoreType.Windows;
            configuration.SecurityConfiguration.ApplicationCertificate.StorePath = "LocalMachine\\My";
            configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = configuration.ApplicationName;

            // trust all applications installed on the same machine.
            configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = CertificateStoreType.Windows;
            configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = "LocalMachine\\My";

            // find the certificate in the store.
            X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

            // create a new certificate if one not found.
            if (clientCertificate == null)
            {
                // this code would normally be called as part of the installer - called here to illustrate.
                // create a new certificate an place it in the LocalMachine/Personal store.
                clientCertificate = Opc.Ua.CertificateFactory.CreateCertificate(
                    configuration.SecurityConfiguration.ApplicationCertificate.StoreType,
                    configuration.SecurityConfiguration.ApplicationCertificate.StorePath,
                    configuration.ApplicationUri,
                    configuration.ApplicationName,
                    null,
                    null,
                    1024,
                    120);

                Console.WriteLine("Created client certificate: {0}", clientCertificate.Subject);
            }

            // Step 3 - Specify the supported transport configurations.

            // The SDK requires Binding objects which are sub-types of Opc.Ua.Bindings.BaseBinding
            // These two lines add support for SOAP/HTTP w/ WS-* and UA-TCP. Support for other protocols 
            // such as .NET TCP can be added but they would not be considered interoperable across different vendors. 
            // Only one binding per URL scheme is allowed.
            configuration.TransportConfigurations.Add(new TransportConfiguration(Utils.UriSchemeOpcTcp, typeof(Opc.Ua.Bindings.UaTcpBinding)));
            configuration.TransportConfigurations.Add(new TransportConfiguration(Utils.UriSchemeHttp, typeof(Opc.Ua.Bindings.UaSoapXmlBinding)));

            // Step 4 - Specify the supported transport quotas.

            // The transport quotas are used to set limits on the contents of messages and are
            // used to protect against DOS attacks and rogue clients. They should be set to
            // reasonable values.
          //<TransportQuotas>
          //  <OperationTimeout>600000</OperationTimeout>
          //  <MaxStringLength>1048576</MaxStringLength>
          //  <MaxByteStringLength>1048576</MaxByteStringLength>
          //  <MaxArrayLength>65535</MaxArrayLength>
          //  <MaxMessageSize>4194304</MaxMessageSize>
          //  <MaxBufferSize>65535</MaxBufferSize>
          //  <ChannelLifetime>300000</ChannelLifetime>
          //  <SecurityTokenLifetime>3600000</SecurityTokenLifetime>
          //</TransportQuotas>
            configuration.TransportQuotas = new TransportQuotas();
            configuration.TransportQuotas.OperationTimeout = 600000;
            configuration.TransportQuotas.MaxStringLength = 1048576;
            configuration.TransportQuotas.MaxByteStringLength = 4194304;
            configuration.TransportQuotas.MaxArrayLength = 65535;
            configuration.TransportQuotas.MaxMessageSize = 4194304;
            configuration.TransportQuotas.MaxBufferSize = 65535;
            configuration.TransportQuotas.ChannelLifetime = 300000;
            configuration.TransportQuotas.SecurityTokenLifetime = 3600000;


            configuration.ServerConfiguration = new ServerConfiguration();

            // Step 5 - Specify the client specific configuration.
            configuration.ClientConfiguration = new ClientConfiguration();
            configuration.ClientConfiguration.DefaultSessionTimeout = 600000;

            // Step 6 - Turn on tracing.
            configuration.TraceConfiguration = new TraceConfiguration();
            configuration.TraceConfiguration.DeleteOnLoad = true;
            configuration.TraceConfiguration.OutputFilePath = "%MyDocuments%\\MyClientTraceFile.txt";
            configuration.TraceConfiguration.TraceMasks = 0x07;

            // Step 7 - Validate the configuration.

            // This step checks if the configuration is consistent and assigns a few internal variables
            // that are used by the SDK. This is called automatically if the configuration is loaded from
            // a file using the ApplicationConfiguration.Load() method.          
            configuration.Validate(ApplicationType.Client);

            CheckApplicationInstanceCertificate(configuration);

            return configuration;
             * */
#else
            InitApplicationConfiguration();
            return configuration;
#endif
        }

#if WINDOWS_UWP || NET_STANDARD
        static async void InitApplicationConfiguration()
        {
            application.ApplicationName = "OPCUAClient";

            application.ApplicationType = ApplicationType.Client;
            application.ConfigSectionName = "OPCUAClient";
            await application.LoadApplicationConfiguration(false);
            await application.CheckApplicationInstanceCertificate(false, 0);
            configuration = application.ApplicationConfiguration;
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Handles a certificate validation error.
        /// </summary>
        /// 
        /*
        public static void HandleCertificateValidationError(Window caller, CertificateValidator validator, CertificateValidationEventArgs e)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.AppendFormat("Certificate could not validated: {0}\r\n\r\n", e.Error.StatusCode);
            buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
            buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
            buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
            buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
            buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);

            buffer.AppendFormat("Accept anyways?");

            if (MessageBox.Show(buffer.ToString(), caller.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Accept = true;
            }
        }
        */

        /// <summary>
        /// Does any configuration checks before starting up.
        /// </summary>
        public static ApplicationConfiguration LoadConfiguration(
            string configSectionName,
            ApplicationType applicationType,
            string defaultConfigFile,
            bool interactive)
        {
            // get the location of the config file.
            string filePath = ApplicationConfiguration.GetFilePathFromAppConfig(configSectionName);

            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                filePath = Utils.GetAbsoluteFilePath(defaultConfigFile, false, false, false);
            }

            try
            {
                // load the configuration file.
                ApplicationConfiguration configuration = ApplicationConfiguration.Load(new System.IO.FileInfo(filePath), applicationType, null);

                if (configuration == null)
                {
                    return null;
                }

                return configuration;
            }
            catch (Exception e)
            {
                // warn user.
                if (interactive)
                {
                    StringBuilder message = new StringBuilder();

                    message.Append("Could not load configuration file.\r\n");
                    message.Append(filePath);
                    message.Append("\r\n");
                    message.Append("\r\n");
                    message.Append(e.Message);

                    MessageBox.Show(message.ToString(), "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Utils.Trace(e, "Could not load configuration file. {0}", filePath);
                return null;
            }
        }


        /// <summary>
        /// Does any configuration checks before starting up.
        /// </summary>
        public static ApplicationConfiguration DoStartupChecks(
            string configSectionName,
            ApplicationType applicationType,
            string defaultConfigFile,
            bool interactive)
        {
            // load the configuration file.
            ApplicationConfiguration configuration = LoadConfiguration(configSectionName, applicationType, defaultConfigFile, interactive);

            if (configuration == null)
            {
                return null;
            }

            // check the certificate.
            X509Certificate2 certificate = CheckApplicationInstanceCertificate(configuration, 2048, interactive, true);

            if (certificate == null)
            {
                return null;
            }

            // ensure the application uri matches the certificate.
            string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

            if (applicationUri != null)
            {
                configuration.ApplicationUri = applicationUri;
            }

            return configuration;
        }

        /// <summary>
        /// Deletes an existing application instance certificate.
        /// </summary>
        /// <param name="configuration">The configuration instance that stores the configurable information for a UA application.</param>
        public static void DeleteApplicationInstanceCertificate(ApplicationConfiguration configuration)
        {
            // create a default certificate id none specified.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                return;
            }

            // delete private key.
            X509Certificate2 certificate = id.Find();

            // delete trusted peer certificate.
            if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                string thumbprint = id.Thumbprint;

                if (certificate != null)
                {
                    thumbprint = certificate.Thumbprint;
                }

                if (!String.IsNullOrEmpty(thumbprint))
                {
                    using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                    {
                        store.Delete(thumbprint);
                    }
                }
            }

            // delete private key.
            if (certificate != null)
            {
                using (ICertificateStore store = id.OpenStore())
                {
                    store.Delete(certificate.Thumbprint);
                }
            }
        }

        /// <summary>
        /// Creates an application instance certificate if one does not already exist.
        /// </summary>
        public static X509Certificate2 CheckApplicationInstanceCertificate(ApplicationConfiguration configuration)
        {
            return CheckApplicationInstanceCertificate(configuration, 2048, Environment.UserInteractive, true);
        }

        /// <summary>
        /// Creates an application instance certificate if one does not already exist.
        /// </summary>
        public static X509Certificate2 CheckApplicationInstanceCertificate(ApplicationConfiguration configuration,
                                                                            ushort keySize, 
                                                                            bool interactive,
                                                                            bool updateFile)
        {
            // create a default certificate id none specified.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                id = new CertificateIdentifier();
                id.StoreType = Utils.DefaultStoreType;
                id.StorePath = Utils.DefaultStorePath;
                id.SubjectName = configuration.ApplicationName;
            }

            bool createNewCertificate = false;
            IList<string> serverDomainNames = configuration.GetServerDomainNames();

            // check for private key.
            X509Certificate2 certificate = id.Find(true);

            if (certificate == null)
            {
                // check if config file has wrong thumprint.
                if (!String.IsNullOrEmpty(id.SubjectName) && !String.IsNullOrEmpty(id.Thumbprint))
                {
                    CertificateIdentifier id2 = new CertificateIdentifier();
                    id2.StoreType = id.StoreType;
                    id2.StorePath = id.StorePath;
                    id2.SubjectName = id.SubjectName;
                    id = id2;

                    certificate = id2.Find(true);

                    if (certificate != null)
                    {
                        string message = Utils.Format(
                            "Matching certificate with SubjectName={0} found but with a different thumbprint. Use certificate?",
                            id.SubjectName);

                        if (interactive)
                        {
                            if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                certificate = null;
                            }
                        }
                    }
                }
            }

            // check if private key is missing.
            if (certificate == null)
            {
                certificate = id.Find(false);

                if (certificate != null)
                {
                    string message = Utils.Format(
                        "Matching certificate with SubjectName={0} found but without a private key. Create a new certificate?",
                        id.SubjectName);

                    if (interactive)
                    {
                        if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            certificate = null;
                        }
                    }
                }
            }

            // check domains.
            if (certificate != null)
            {
                IList<string> certificateDomainNames = Utils.GetDomainsFromCertficate(certificate);

                for (int ii = 0; ii < serverDomainNames.Count; ii++)
                {
                    if (Utils.FindStringIgnoreCase(certificateDomainNames, serverDomainNames[ii]))
                    {
                        continue;
                    }

                    if (String.Compare(serverDomainNames[ii], "localhost", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // check computer name.
                        string computerName = System.Net.Dns.GetHostName();

                        if (Utils.FindStringIgnoreCase(certificateDomainNames, computerName))
                        {
                            continue;
                        }

                        // check for aliases.
                        System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(computerName);

                        bool found = false;

                        for (int jj = 0; jj < entry.Aliases.Length; jj++)
                        {
                            if (Utils.FindStringIgnoreCase(certificateDomainNames, entry.Aliases[jj]))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }

                        // check for ip addresses.
                        for (int jj = 0; jj < entry.AddressList.Length; jj++)
                        {
                            if (Utils.FindStringIgnoreCase(certificateDomainNames, entry.AddressList[jj].ToString()))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }
                    }

                    string message = Utils.Format(
                        "The server is configured to use domain '{0}' which does not appear in the certificate. Update certificate?",
                        serverDomainNames[ii]);

                    createNewCertificate = true;

                    if (interactive)
                    {
                        if (MessageBox.Show(message, configuration.ApplicationName, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            createNewCertificate = false;
                            continue;
                        }
                    }

                    Utils.Trace(message);
                    break;
                }

                if (!createNewCertificate)
                {
                    // check if key size matches.
                    if (keySize == certificate.PublicKey.Key.KeySize)
                    {
                        try
                        {
                            AddToTrustedPeerStore(configuration, certificate);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Application does not have administrative privilege to access the certificate store. Please, restart the application as administrator the fist time", configuration.ApplicationName, MessageBoxButton.OK);
                            return null;
                        }
                        return certificate;
                    }
                }
            }

            // prompt user.
            if (interactive)
            {
                if (!createNewCertificate)
                {
                    MessageBox.Show(Properties.Resource.MissingApplicationCertificate, configuration.ApplicationName);
                    return null;
                }
            }

            // delete existing certificate.
            if (certificate != null)
            {
                DeleteApplicationInstanceCertificate(configuration);
            }

            // add the localhost.
            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(System.Net.Dns.GetHostName());
            }

            certificate = Opc.Ua.CertificateFactory.CreateCertificate(
                id.StoreType,
                id.StorePath,
                configuration.ApplicationUri,
                configuration.ApplicationName,
                null,
                serverDomainNames,
                keySize,
                300);

            id.Certificate = certificate;
            try
            {
                AddToTrustedPeerStore(configuration, certificate);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application does not have administrative privilege to access the certificate store. Please, restart the application as administrator the fist time", configuration.ApplicationName, MessageBoxButton.OK);
                return null;
            }

            if (updateFile && !String.IsNullOrEmpty(configuration.SourceFilePath))
            {
                configuration.SaveToFile(configuration.SourceFilePath);
            }

            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);

            return certificate;
        }

        /// <summary>
        /// Adds the certificate to the Trusted Peer Certificate Store
        /// </summary>
        /// <param name="configuration">The application's configuration which specifies the location of the TrustedPeerStore.</param>
        /// <param name="certificate">The certificate to register.</param>
        public static void AddToTrustedPeerStore(ApplicationConfiguration configuration, X509Certificate2 certificate)
        {
            ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

            try
            {
                // check if it already exists.
                X509Certificate2 certificate2 = store.FindByThumbprint(certificate.Thumbprint);

                if (certificate2 != null)
                {
                    return;
                }

                List<string> subjectName = Utils.ParseDistinguishedName(certificate.Subject);

                // check for old certificate.
                X509Certificate2Collection certificates = store.Enumerate();

                for (int ii = 0; ii < certificates.Count; ii++)
                {
                    if (Utils.CompareDistinguishedName(certificates[ii], subjectName))
                    {
                        if (certificates[ii].Thumbprint == certificate.Thumbprint)
                        {
                            return;
                        }

                        store.Delete(certificates[ii].Thumbprint);
                        break;
                    }
                }

                // add new certificate.
                X509Certificate2 publicKey = new X509Certificate2(certificate.GetRawCertData());
                store.Add(publicKey);
            }
            finally
            {
                store.Close();
            }
        }
#endif

        public static object GetDefaultValue(NodeId datatypeId, int valueRank)
        {
            Type type = Opc.Ua.TypeInfo.GetSystemType(datatypeId, EncodeableFactory.GlobalFactory);
            return GetDefaultValue(type, valueRank);
        }

        /// <summary>
        /// Returns a default value for the data type.
        /// </summary>
        public static object GetDefaultValue(Type type, int valueRank)
        {
            if (type == null)
            {
                return null;
            }

            if (valueRank < 0)
            {
                if (type == typeof(String))
                {
                    return System.String.Empty;
                }

                if (type == typeof(byte[]))
                {
                    return new byte[0];
                }

                if (type == typeof(NodeId))
                {
                    return Opc.Ua.NodeId.Null;
                }

                if (type == typeof(ExpandedNodeId))
                {
                    return Opc.Ua.ExpandedNodeId.Null;
                }

                if (type == typeof(QualifiedName))
                {
                    return Opc.Ua.QualifiedName.Null;
                }

                if (type == typeof(LocalizedText))
                {
                    return Opc.Ua.LocalizedText.Null;
                }

                if (type == typeof(Guid))
                {
                    return System.Guid.Empty;
                }

                if (type == typeof(System.Xml.XmlElement))
                {
                    System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                    document.InnerXml = "<Null/>";
                    return document.DocumentElement;
                }

                return Activator.CreateInstance(type);
            }

            return Array.CreateInstance(type, new int[valueRank]);
        }

        /*
        /// <summary>
        /// Displays a dialog that allows a use to edit a value.
        /// </summary>
        public static object EditValue(Session session, object value)
        {
            TypeInfo typeInfo = TypeInfo.Construct(value);

            if (typeInfo != null)
            {
                return EditValue(session, value, (uint)typeInfo.BuiltInType, typeInfo.ValueRank);
            }

            return null;
        }

        /// <summary>
        /// Displays a dialog that allows a use to edit a value.
        /// </summary>
        public static object EditValue(Session session, object value, NodeId datatypeId, int valueRank)
        {
            if (value == null)
            {
                value = GetDefaultValue(datatypeId, valueRank);
            }

            if (valueRank >= 0)
            {
                return new ComplexValueEditDlg().ShowDialog(value);
            }

            BuiltInType builtinType = TypeInfo.GetBuiltInType(datatypeId, session.TypeTree);

            switch (builtinType)
            {
                case BuiltInType.Boolean:
                case BuiltInType.Byte:
                case BuiltInType.SByte:
                case BuiltInType.Int16:
                case BuiltInType.UInt16:
                case BuiltInType.Int32:
                case BuiltInType.UInt32:
                case BuiltInType.Int64:
                case BuiltInType.UInt64:
                case BuiltInType.Float:
                case BuiltInType.Double:
                    {
                        return new NumericValueEditDlg().ShowDialog(value, TypeInfo.GetSystemType(builtinType, valueRank));
                    }

                case BuiltInType.NodeId:
                    {
                        return new NodeIdValueEditDlg().ShowDialog(session, (NodeId)value);
                    }

                case BuiltInType.ExpandedNodeId:
                    {
                        return new NodeIdValueEditDlg().ShowDialog(session, (ExpandedNodeId)value);
                    }

                case BuiltInType.DateTime:
                    {
                        DateTime datetime = (DateTime)value;

                        if (new DateTimeValueEditDlg().ShowDialog(ref datetime))
                        {
                            return datetime;
                        }

                        return null;
                    }

                case BuiltInType.QualifiedName:
                    {
                        QualifiedName qname = (QualifiedName)value;

                        string name = new StringValueEditDlg().ShowDialog(qname.Name);

                        if (name != null)
                        {
                            return new QualifiedName(name, qname.NamespaceIndex);
                        }

                        return null;
                    }

                case BuiltInType.String:
                    {
                        return new StringValueEditDlg().ShowDialog((string)value);
                    }

                case BuiltInType.LocalizedText:
                    {
                        LocalizedText ltext = (LocalizedText)value;

                        string text = new StringValueEditDlg().ShowDialog(ltext.Text);

                        if (text != null)
                        {
                            return new LocalizedText(text, ltext.Locale);
                        }

                        return null;
                    }
            }

            return new ComplexValueEditDlg().ShowDialog(value);
        }
        */

        /// <summary>
        /// Returns a discovery url that can be used to get endpoints.
        /// </summary>
        public static string GetDiscoveryUrl(string endpointUrl)
        {
            var securityPolicies = SecurityPolicies.GetDisplayNames();
            if (securityPolicies != null && securityPolicies.Length > 1)
            {
                for (int ii = 0; ii < securityPolicies.Length; ii++)
                {
                    if (securityPolicies[ii] == "BaseUri")
                        continue;

                    var policies = String.Format("/{0}", securityPolicies[ii]);
                    if (endpointUrl.EndsWith(policies))
                    {
                        endpointUrl = endpointUrl.Substring(0, endpointUrl.Length - policies.Length);
                        break;
                    }
                }
            }

            // needs to add the '/discovery' back onto non-UA TCP URLs.
            if (!endpointUrl.StartsWith(Utils.UriSchemeOpcTcp))
            {
                if (!endpointUrl.EndsWith("/discovery"))
                {
                    endpointUrl += "/discovery";
                }
            }

            return endpointUrl;
        }
    }
}
