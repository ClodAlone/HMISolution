/* ========================================================================
 * Copyright (c) 2005-2011 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;

namespace Opc.Ua.GdsLocalAgent
{
    /// <summary>
    /// A class which manages the GDS local agent application.
    /// </summary>
    public class GdsAgent
    {
        public GdsAgent(ApplicationConfiguration configuration)
        {
            m_configuration = configuration;
            m_applications = new List<GdsAgentApplication>();
            Applications = new ReadOnlyList<GdsAgentApplication>(m_applications);
        }

        public ReadOnlyList<GdsAgentApplication> Applications { get; private set; }

        public event LogMessageEventHandler LogMessage
        {
            add { m_LogMessage += value; }
            remove { m_LogMessage -= value; }
        }

        public void ChangeSession(Session session)
        {
            m_session = session;
        }

        public void Log(string format, params object[] args)
        {
            if (m_LogMessage != null)
            {
                string message = format;

                if (args != null && args.Length > 0)
                {
                    message = Utils.Format(format, args);
                }

                m_LogMessage(this, new LogMessageEventArg() { Message = message });
            }
        }

        public void LoadApplications()
        {
            string filePath = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\GDS\\Applications", true, false, true);

            if (!Directory.Exists(filePath))
            {
                m_persistentFilePath = Directory.CreateDirectory(filePath);
            }
            else
            {
                m_persistentFilePath = new DirectoryInfo(filePath);
            }

            foreach (FileInfo file in m_persistentFilePath.GetFiles("*.xml"))
            {
                try
                {
                    GdsAgentApplication application = LoadApplication(file.FullName);

                    if (application != null)
                    {
                        if (String.IsNullOrEmpty(application.MachineName))
                        {
                            application.MachineName = System.Net.Dns.GetHostName();
                        }

                        m_applications.Add(application);
                    }
                }
                catch (Exception exception)
                {
                    Log("Could not load GDS application file: {1} '{0}'.", exception.Message, file.Name); 
                }
            }
        }

        public GdsAgentApplication LoadApplication(string filePath)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Opc.Ua.Security.SecuredApplication));

            using (FileStream istrm = File.Open(filePath, FileMode.Open))
            {
                Opc.Ua.Security.SecuredApplication application = serializer.ReadObject(istrm) as Opc.Ua.Security.SecuredApplication;         
                Log("Application Loaded: {0}.", application.ApplicationName);
                
                GdsAgentApplication extension = Utils.ParseExtension<GdsAgentApplication>(application.Extensions, null);

                if (extension == null)
                {
                    extension = new GdsAgentApplication();
                    FileInfo fileInfo = new FileInfo(filePath);
                    extension.UniqueId = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                    extension.ConfigurationType = ConfigurationType.Xml;

                    if (application.BaseAddresses == null)
                    {
                        application.BaseAddresses = new Opc.Ua.Security.ListOfBaseAddresses();
                    }
                }

                extension.SecuritySettings = application;

                return extension;
            }
        }

        public void SaveApplication(GdsAgentApplication application)
        {
            if (String.IsNullOrEmpty(application.UniqueId))
            {
                application.UniqueId = Guid.NewGuid().ToString();
            }

            if (!m_applications.Contains(application))
            {
                m_applications.Add(application);
            }

            Opc.Ua.Security.SecuredApplication container = application.SecuritySettings;

            XmlElementCollection extensions = (container.Extensions != null)?new XmlElementCollection(container.Extensions):new XmlElementCollection();
            Utils.UpdateExtension<GdsAgentApplication>(ref extensions, null, application);
            container.Extensions = new Opc.Ua.Security.ListOfExtensions();
            container.Extensions.AddRange(extensions);

            DataContractSerializer serializer = new DataContractSerializer(typeof(Opc.Ua.Security.SecuredApplication));

            using (FileStream ostrm = File.Open(m_persistentFilePath.FullName + "\\" + application.UniqueId + ".xml", FileMode.Create))
            {
                serializer.WriteObject(ostrm, container);
                Log("Application Saved: {0}.", application.ApplicationName);
            }
        }

        public void DeleteApplication(GdsAgentApplication application)
        {
            if (application != null)
            {
                for (int ii = 0; ii < m_applications.Count; ii++)
                {
                    if (Object.ReferenceEquals(application, m_applications[ii]))
                    {
                        m_applications.RemoveAt(ii);
                        break;
                    }
                }

                File.Delete(m_persistentFilePath.FullName + "\\" + application.UniqueId + ".xml");
                Log("Application Deleted: {0}.", application.ApplicationName);
            }
        }

        public void RegisterApplication(GdsAgentApplication application)
        {
            if (m_session == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotConnected);
            }

            // get the current host name.
            string hostName = application.MachineName;

            if (String.IsNullOrEmpty(hostName))
            {
                hostName = System.Net.Dns.GetHostName();
            }

            List<string> discoveryUrls = new List<string>();

            // create the discovery urls.
            if (application.SecuritySettings.BaseAddresses != null)
            {
                foreach (string baseAddress in application.SecuritySettings.BaseAddresses)
                {
                    Uri url = Utils.ParseUri(baseAddress);

                    if (url != null)
                    {
                        UriBuilder builder = new UriBuilder(url);

                        if (String.Compare("localhost", url.DnsSafeHost, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            builder.Host = hostName;
                        }

                        if (url.Scheme == Utils.UriSchemeHttp)
                        {
                            builder.Path += "/discovery";
                        }

                        if (url.Scheme == Utils.UriSchemeNoSecurityHttp)
                        {
                            builder.Scheme = Utils.UriSchemeHttp;
                        }

                        discoveryUrls.Add(builder.ToString());
                    }
                }
            }

            // create application.
            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = m_session.Call(
                new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                new NodeId(Opc.Ua.Gds.Methods.Directory_RegisterApplication, namespaceIndex),
                application.SecuritySettings.ApplicationUri,
                hostName,
                application.SecuritySettings.ApplicationName,
                application.SecuritySettings.ApplicationType,
                application.SecuritySettings.ProductName,
                null,
                discoveryUrls);

            if (outputArguments != null && outputArguments.Count == 1)
            {
                application.SecuritySettings.ApplicationUri = outputArguments[0] as string;
            }

            // persist the updated application.
            application.RegisteredWithGds = true;
            SaveApplication(application);
            Log("Registered Application: {0}.", application.ApplicationName);
        }

        /// <summary>
        /// Creates a new certificate for the application.
        /// </summary>
        public void CreateCertificate(GdsAgentApplication application)
        {
            // determine the type of key.
            string keyType = "PFX";

            if (application.ConfigurationType == ConfigurationType.Manual)
            {
                if (!String.IsNullOrEmpty(application.PrivateKeyFilePath) && application.PrivateKeyFilePath.EndsWith(".pem", StringComparison.InvariantCultureIgnoreCase))
                {
                    keyType = "PEM";
                }
            }

            // build list of domains.]
            List<string> domains = new List<string>();
            string hostName = application.MachineName;

            if (application.SecuritySettings.BaseAddresses != null)
            {
                foreach (string baseAddress in application.SecuritySettings.BaseAddresses)
                {
                    Uri url = Utils.ParseUri(baseAddress);

                    if (url != null)
                    {
                        string targetHost = url.DnsSafeHost;

                        if (String.Compare("localhost", url.DnsSafeHost, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            targetHost = hostName;
                        }

                        if (!domains.Contains(targetHost))
                        {
                            domains.Add(targetHost);
                        }
                    }
                }
            }

            // set the subject name based on the type of certificate.
            string subjectName = ConstructSubjectName(application, false);

            CertificateStoreIdentifier csid = new CertificateStoreIdentifier();

            if (application.SecuritySettings.ApplicationCertificate != null)
            {
                csid.StoreType = application.SecuritySettings.ApplicationCertificate.StoreType;
                csid.StorePath = application.SecuritySettings.ApplicationCertificate.StorePath;
            }
            else
            {
                csid.StoreType = Utils.DefaultStoreType;
                csid.StorePath = Utils.DefaultStorePath;
            }
            
            X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                csid.StoreType,
                csid.StorePath,
                null,
                application.SecuritySettings.ApplicationUri,
                application.ApplicationName,
                subjectName,
                domains.ToArray(),
                1024,
                60,
                false,
                keyType == "PEM",
                null,
                null);

            string thumbprintToDelete = application.SecuritySettings.ApplicationCertificate.Thumbprint;

            // update certificate for XML configurations.
            if (application.ConfigurationType == ConfigurationType.Xml)
            {
                application.SecuritySettings.ApplicationCertificate.SubjectName = certificate.Subject;
                application.SecuritySettings.ApplicationCertificate.Thumbprint = certificate.Thumbprint;
                application.SecuritySettings.ApplicationCertificate.RawData = null;
            }

            // update certificate for manual configurations.
            else if (application.ConfigurationType == ConfigurationType.Manual)
            {
                ICertificateStore store = csid.OpenStore();

                try
                {
                    string filePath = store.GetPrivateKeyFilePath(certificate.Thumbprint);
                    byte[] privateKey = File.ReadAllBytes(filePath);
                    InstallCertificateForManualConfiguration(application, null, certificate.RawData, privateKey);
                    store.Delete(certificate.Thumbprint);
                }
                finally
                {
                    store.Close();
                }
            }

            // ensure LDS can communicate with a server.
            if (application.SecuritySettings.ApplicationType == Opc.Ua.Security.ApplicationType.Server_0 || application.SecuritySettings.ApplicationType == Opc.Ua.Security.ApplicationType.ClientAndServer_2)
            {
                ApplicationInstance.AddToDiscoveryServerTrustList(
                    certificate,
                    thumbprintToDelete,
                    null,
                    Opc.Ua.Security.SecuredApplication.FromCertificateStoreIdentifier(application.SecuritySettings.TrustedCertificateStore));
            }

            // persist the updated application.
            SaveApplication(application);
            Log("Requested Application Certificate: {0}.", application.ApplicationName);
        }

        /// <summary>
        /// Constructs a subject name.
        /// </summary>
        private string ConstructSubjectName(GdsAgentApplication application, bool isHttpsCertificate)
        {
            StringBuilder buffer = new StringBuilder();

            if (isHttpsCertificate)
            {
                buffer.Append("CN=");
                buffer.Append(application.MachineName);
            }
            else
            {
                string name = application.ApplicationName;
                bool escapeRequired = name.IndexOfAny(new char[] { '/', ',', '=' }) != -1;

                buffer.Append("CN=");

                if (escapeRequired)
                {
                    buffer.Append('"');
                }

                buffer.Append(application.ApplicationName);

                if (escapeRequired)
                {
                    buffer.Append('"');
                }

                buffer.Append("/DC=");
                buffer.Append(application.MachineName);
            }

            if (!String.IsNullOrEmpty(application.SubjectName))
            {
                List<string> names = Utils.ParseDistinguishedName(application.SubjectName);

                foreach (string name in names)
                {
                    // common name is set by the caller.
                    if (name.StartsWith("CN=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    // append names.
                    if (buffer.Length > 0)
                    {
                        buffer.Append('/');
                    }

                    buffer.Append(name);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Requests a new certificate from the server.
        /// </summary>
        public void RequestCertificate(GdsAgentApplication application, bool isHttpsCertificate)
        {
            if (m_session == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotConnected);
            }

            // determine the type of key.
            string keyType = "PFX";

            if (application.ConfigurationType == ConfigurationType.Manual)
            {
                if (!String.IsNullOrEmpty(application.PrivateKeyFilePath) && application.PrivateKeyFilePath.EndsWith(".pem", StringComparison.InvariantCultureIgnoreCase))
                {
                    keyType = "PEM";
                }
            }

            // build list of domains.]
            string httpsDomainName = null;
            List<string> domains = new List<string>();
            string hostName = application.MachineName;

            if (application.SecuritySettings.BaseAddresses != null)
            {
                foreach (string baseAddress in application.SecuritySettings.BaseAddresses)
                {
                    Uri url = Utils.ParseUri(baseAddress);

                    if (url != null)
                    {
                        string targetHost = url.DnsSafeHost;

                        if (String.Compare("localhost", url.DnsSafeHost, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            targetHost = hostName;
                        }

                        if (!domains.Contains(targetHost))
                        {
                            domains.Add(targetHost);
                        }

                        if (isHttpsCertificate)
                        {
                            if (httpsDomainName == null && url.Scheme == Utils.UriSchemeHttps)
                            {
                                httpsDomainName = targetHost;
                            }
                        }
                    }
                }
            }

            if (domains.Count == 0)
            {
                domains.Add(application.MachineName);
            }

            if (isHttpsCertificate)
            {
                if (httpsDomainName == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Server must have an HTTPS endpoint.");
                }

                if (String.Compare(httpsDomainName, application.MachineName, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "HTTPS endpoint domain name does not match the MachineName.");
                }
            }

            // set the subject name based on the type of certificate.
            string subjectName = ConstructSubjectName(application, isHttpsCertificate);

            // request certificate.
            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = m_session.Call(
                new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                new NodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_RequestCertificate, namespaceIndex),
                application.SecuritySettings.ApplicationUri,
                subjectName,
                domains.ToArray(),
                keyType,
                String.Empty,
                isHttpsCertificate);

            application.LastCertificateRequestId = null;

            if (outputArguments != null && outputArguments.Count == 1)
            {
                application.LastCertificateRequestId = outputArguments[0] as NodeId;
            }

            // persist the updated application.
            SaveApplication(application);
            Log("Requested Application Certificate: {0}.", application.ApplicationName);
        }

        public void CheckCertificateStatus(GdsAgentApplication application)
        {
            if (m_session == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotConnected);
            }

            // request certificate.
            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = null;

            try
            {
                outputArguments = m_session.Call(
                     new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                     new NodeId(Opc.Ua.Gds.Methods.Directory_CheckRequestStatus, namespaceIndex),
                     application.LastCertificateRequestId);
            }
            catch (ServiceResultException e)
            {
                if (e.StatusCode == StatusCodes.BadNoEntryExists)
                {
                    application.LastCertificateRequestId = null;
                    SaveApplication(application);
                }

                throw;
            }

            if (outputArguments != null && outputArguments.Count == 3)
            {
                Log("Received Application Certificate: {0}.", application.ApplicationName);
                
                application.LastCertificateRequestId = null;

                byte[] publicKey = outputArguments[0] as byte[];
                byte[] privateKey = outputArguments[1] as byte[];
                byte[][] issuerCertificates = outputArguments[2] as byte[][];

                // check if it is an HTTPS certificate.
                if (!InstallHttpsCertificate(application, privateKey, issuerCertificates))
                {
                    // get the thumbprint of the existing certificate.
                    string thumbprintToDelete = null;

                    if (application.ConfigurationType != ConfigurationType.Manual)
                    {
                        thumbprintToDelete = (application.SecuritySettings.ApplicationCertificate != null) ? application.SecuritySettings.ApplicationCertificate.Thumbprint : null;
                    }

                    // must validate the certificates before installing them.
                    CertificateTrustList trustedStore = new CertificateTrustList();
                    CertificateValidator validator = new CertificateValidator();
                    validator.CertificateValidation += new CertificateValidationEventHandler(IssuerCertificate_CertificateValidation);
                    validator.Update(null, trustedStore, null);

                    // installs the issuer certificates.
                    InstallIssuerCertificates(application, validator, trustedStore, issuerCertificates);

                    // update certificate for XML configurations.
                    if (application.ConfigurationType == ConfigurationType.Xml)
                    {
                        InstallCertificateForXmlConfiguration(application, validator, privateKey);
                    }

                    // update certificate for manual configurations.
                    else if (application.ConfigurationType == ConfigurationType.Manual)
                    {
                        InstallCertificateForManualConfiguration(application, validator, publicKey, privateKey);
                    }

                    // ensure LDS can communicate with a server.
                    if (application.SecuritySettings.ApplicationType == Opc.Ua.Security.ApplicationType.Server_0 || application.SecuritySettings.ApplicationType == Opc.Ua.Security.ApplicationType.ClientAndServer_2)
                    {
                        List<X509Certificate2> issuers = new List<X509Certificate2>();

                        if (issuerCertificates != null)
                        {
                            foreach (byte[] issuerCertificate in issuerCertificates)
                            {
                                issuers.Add(new X509Certificate2(issuerCertificate));
                            }
                        }

                        ApplicationInstance.AddToDiscoveryServerTrustList(
                            new X509Certificate2(publicKey),
                            thumbprintToDelete,
                            issuers,
                            Opc.Ua.Security.SecuredApplication.FromCertificateStoreIdentifier(application.SecuritySettings.TrustedCertificateStore));
                    }
                }
            }

            // persist the updated application.
            SaveApplication(application);
        }

        /// <summary>
        /// Checks if the certificate is an HTTPS certificate and installs it.
        /// </summary>
        private bool InstallHttpsCertificate(
            GdsAgentApplication application, 
            byte[] privateKey,
            byte[][] issuerCertificates)
        {
            // check for PEM file.
            if (application.ConfigurationType == ConfigurationType.Manual)
            {
                if (application.PrivateKeyFilePath == null || !application.PrivateKeyFilePath.EndsWith(".pfx"))
                {
                    return false;
                }
            }

            // check that the CommonName matches the MachineName.                
            X509Certificate2 certificate = new X509Certificate2(privateKey, String.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            List<string> names = Utils.ParseDistinguishedName(certificate.Subject);

            foreach (string name in names)
            {
                if (name.StartsWith("CN=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string commonName = name.Substring(3);

                    if (String.Compare(commonName, application.MachineName) != 0)
                    {
                        return false;
                    }

                    break;
                }
            }
            
            // setup policy chain.
            X509ChainPolicy policy = new X509ChainPolicy();

            policy.RevocationFlag = X509RevocationFlag.EntireChain;
            policy.RevocationMode = X509RevocationMode.NoCheck;
            policy.VerificationFlags = X509VerificationFlags.NoFlag;
            
            policy.VerificationFlags |= X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown;
            policy.VerificationFlags |= X509VerificationFlags.IgnoreCtlSignerRevocationUnknown;
            policy.VerificationFlags |= X509VerificationFlags.IgnoreEndRevocationUnknown;
            policy.VerificationFlags |= X509VerificationFlags.IgnoreRootRevocationUnknown;

            for (int ii = 0; ii < issuerCertificates.Length; ii++)            
            {
                X509Certificate2 issuer = new X509Certificate2(issuerCertificates[ii]);
                policy.ExtraStore.Add(issuer);
            }

            // build chain.
            X509Chain chain = new X509Chain();
            chain.ChainPolicy = policy;
            chain.Build(certificate);

            for (int ii = 0; ii < chain.ChainElements.Count; ii++)
            {
                X509ChainElement element = chain.ChainElements[ii];

                foreach (X509ChainStatus status in element.ChainElementStatus)
                {
                    if (status.Status != X509ChainStatusFlags.UntrustedRoot)
                    {
                        throw new ServiceResultException(StatusCodes.BadCertificateInvalid, status.StatusInformation);
                    }
                }
            }

            // must install the issuers in the trusted CA store.
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = CertificateStoreType.Windows;
            id.StorePath = "LocalMachine\\CA";

            ICertificateStore store = id.OpenStore();

            try
            {
                for (int ii = 0; ii < issuerCertificates.Length; ii++)
                {
                    X509Certificate2 issuer = new X509Certificate2(issuerCertificates[ii]);

                    if (ii == issuerCertificates.Length-1 && Utils.CompareDistinguishedName(issuer.Subject, issuer.Issuer))
                    {
                        store.Close();
                        id.StorePath = "LocalMachine\\Root";
                        store = id.OpenStore();
                    }

                    if (store.FindByThumbprint(issuer.Thumbprint) == null)
                    {
                        store.Add(issuer);
                    }
                }
            }
            finally
            {
                store.Close();
            }

            // must install it in the LocalMachine/My store.
            id = new CertificateStoreIdentifier();
            id.StoreType = CertificateStoreType.Windows;
            id.StorePath = "LocalMachine\\My";

            store = id.OpenStore();

            try
            {
                if (store.FindByThumbprint(certificate.Thumbprint) == null)
                {
                    store.Add(certificate);
                }
            }
            finally
            {
                store.Close();
            }

            Log("Installed an HTTPS Certificate: {0}.", application.ApplicationName);
            return true;
        }

        /// <summary>
        /// Installs the certificate for an XML configuration.
        /// </summary>
        private void InstallCertificateForXmlConfiguration(GdsAgentApplication application, CertificateValidator validator, byte[] privateKey)
        {
            X509Certificate2 certificate = new X509Certificate2(privateKey, String.Empty, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            validator.Validate(certificate);

            // ensure there is a valid store.
            if (application.SecuritySettings.ApplicationCertificate == null)
            {
                application.SecuritySettings.ApplicationCertificate = new Opc.Ua.Security.CertificateIdentifier();
                application.SecuritySettings.ApplicationCertificate.StoreType = Utils.DefaultStoreType;
                application.SecuritySettings.ApplicationCertificate.StorePath = Utils.DefaultStorePath;
            }

            // decode private key.
            ICertificateStore store = application.SecuritySettings.ApplicationCertificate.OpenStore();

            try
            {
                string thumbprintToDelete = application.SecuritySettings.ApplicationCertificate.Thumbprint;

                if (thumbprintToDelete != null)
                {
                    store.Delete(thumbprintToDelete);

                    if (application.SecuritySettings.TrustedCertificateStore != null)
                    {
                        ICertificateStore store2 = application.SecuritySettings.TrustedCertificateStore.OpenStore();
                        store2.Delete(thumbprintToDelete);
                        store2.Close();
                    }
                }

                // delete any existing certificates with the same application URI.
                foreach (X509Certificate2 target in store.Enumerate())
                {
                    if (Utils.CompareDistinguishedName(target.Subject, certificate.Subject))
                    {
                        if (Utils.GetApplicationUriFromCertficate(target) == application.SecuritySettings.ApplicationUri)
                        {
                            store.Delete(target.Thumbprint);
                        }
                    }
                }

                if (store.FindByThumbprint(certificate.Thumbprint) == null)
                {
                    store.Add(certificate);
                }

                application.SecuritySettings.ApplicationCertificate.SubjectName = certificate.Subject;
                application.SecuritySettings.ApplicationCertificate.Thumbprint = certificate.Thumbprint;
                application.SecuritySettings.ApplicationCertificate.RawData = null;
            }
            finally
            {
                store.Close();
            }

            // save configuration.
            new Opc.Ua.Security.SecurityConfigurationManager().WriteConfiguration(application.ConfigurationFile, application.SecuritySettings);
            Log("Updated Application Settings: {0}.", application.ApplicationName);
        }

        /// <summary>
        /// Installs the certificate for a manual configuration.
        /// </summary>
        private void InstallCertificateForManualConfiguration(GdsAgentApplication application, CertificateValidator validator, byte[] publicKey, byte[] privateKey)
        {
            X509Certificate2 certificate = new X509Certificate2(publicKey);

            if (validator != null)
            {
                validator.Validate(certificate);
            }

            if (!String.IsNullOrEmpty(application.PublicKeyFilePath))
            {
                File.Copy(application.PublicKeyFilePath, application.PublicKeyFilePath + ".bak", true);
                File.WriteAllBytes(application.PublicKeyFilePath, publicKey);
            }

            if (!String.IsNullOrEmpty(application.PrivateKeyFilePath))
            {
                File.Copy(application.PrivateKeyFilePath, application.PrivateKeyFilePath + ".bak", true);
                File.WriteAllBytes(application.PrivateKeyFilePath, privateKey);
            }

            Log("Updated Application Certificate Files: {0}.", application.ApplicationName);
        }

        /// <summary>
        /// Installs the issuer certificates.
        /// </summary>
        private void InstallIssuerCertificates(GdsAgentApplication application, CertificateValidator validator, CertificateTrustList issuerStore, byte[][] issuerCertificates)
        {
            Opc.Ua.Security.CertificateStoreIdentifier storeId = application.SecuritySettings.IssuerCertificateStore;

            if (storeId == null)
            {
                storeId = application.SecuritySettings.TrustedCertificateStore;
            }

            InstallIssuerCertificates(storeId, validator, issuerStore, issuerCertificates);
            Log("Installed the Issuer Certificates {0}.", application.ApplicationName);
        }

        /// <summary>
        /// Installs the issuer certificates.
        /// </summary>
        private void InstallIssuerCertificates(Opc.Ua.Security.CertificateStoreIdentifier storeId, CertificateValidator validator, CertificateTrustList issuerStore, byte[][] issuerCertificates)
        {
            if (storeId != null)
            {
                ICertificateStore store = storeId.OpenStore();

                try
                {
                    for (int ii = issuerCertificates.Length-1; ii >= 0; ii--)
                    {
                        X509Certificate2 certificate = new X509Certificate2(issuerCertificates[ii]);
                        validator.Validate(certificate);
                        issuerStore.TrustedCertificates.Add(certificate);
                        validator.Update(null, issuerStore, null);

                        if (store.FindByThumbprint(certificate.Thumbprint) == null)
                        {
                            store.Add(certificate);
                        }
                    }
                }
                finally
                {
                    store.Close();
                }
            }
        }

        void IssuerCertificate_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (e.Error.Code == StatusCodes.BadCertificateUntrusted)
            {
                if (Utils.CompareDistinguishedName(e.Certificate.Subject, e.Certificate.Issuer))
                {
                    e.Accept = true;
                }
            }
        }

        public void UpdateTrustLists(GdsAgentApplication application, bool returnHttpsLists, bool deleteExisting)
        {
            if (m_session == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotConnected);
            }

            // request certificate.
            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = m_session.Call(
                new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                new NodeId(Opc.Ua.Gds.Methods.Directory_GetTrustList, namespaceIndex),
                (application != null)?application.SecuritySettings.ApplicationUri:null,
                returnHttpsLists);

            if (outputArguments != null && outputArguments.Count == 5)
            {
                byte[][] trustedCertificates = outputArguments[1] as byte[][];
                byte[][] trustedCertificateRevocationLists = outputArguments[2] as byte[][];
                byte[][] issuerCertificates = outputArguments[3] as byte[][];
                byte[][] issuerCertificateRevocationLists = outputArguments[4] as byte[][];

                if (returnHttpsLists)
                {
                    UpdateHttpsTrustLists(trustedCertificates, trustedCertificateRevocationLists, issuerCertificates, issuerCertificateRevocationLists);
                    return;
                }
                
                Log("Received Application Trust Lists: {0}.", application.ApplicationName);
                
                application.LastCertificateRequestId = null;
                
                    
                if (application.SecuritySettings.TrustedCertificateStore != null)
                {
                    UpdateCertificates(application.SecuritySettings.TrustedCertificateStore, trustedCertificates, deleteExisting);
                    UpdateCRLs(application.SecuritySettings.TrustedCertificateStore, trustedCertificateRevocationLists, deleteExisting);
                }

                Opc.Ua.Security.CertificateStoreIdentifier issuerStore = application.SecuritySettings.IssuerCertificateStore;

                if (issuerStore == null || String.IsNullOrEmpty(issuerStore.StorePath))
                {
                    issuerStore = application.SecuritySettings.TrustedCertificateStore;
                }

                if (issuerStore != null)
                {
                    // make sure we are not deleting the certs that were just added to the trust list.
                    if (application.SecuritySettings.TrustedCertificateStore != null && String.Compare(issuerStore.StorePath, application.SecuritySettings.TrustedCertificateStore.StorePath, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        deleteExisting = false;
                    }

                    UpdateCertificates(issuerStore, issuerCertificates, deleteExisting);
                    UpdateCRLs(issuerStore, issuerCertificateRevocationLists, deleteExisting);
                }

                Log("Updated Application Trust Lists: {0}.", application.ApplicationName);
            }

            // persist the updated application.
            SaveApplication(application);
        }

        /// <summary>
        /// Updates the HTTPS trust lists.
        /// </summary>
        private void UpdateHttpsTrustLists(
            byte[][] trustedCertificates,
            byte[][] trustedCertificateRevocationLists,
            byte[][] issuerCertificates,
            byte[][] issuerCertificateRevocationLists)
        {
            Log("Received HTTPS Trust Lists.");

            CertificateStoreIdentifier csid1 = new CertificateStoreIdentifier();
            csid1.StoreType = CertificateStoreType.Windows;
            csid1.StorePath = "LocalMachine\\CA";

            CertificateStoreIdentifier csid2 = new CertificateStoreIdentifier();
            csid2.StoreType = CertificateStoreType.Windows;
            csid2.StorePath = "LocalMachine\\Root";

            ICertificateStore store1 = csid1.OpenStore();
            ICertificateStore store2 = csid2.OpenStore();

            UpdateHttpsTrustLists(store1, store2, trustedCertificates, trustedCertificateRevocationLists);
            UpdateHttpsTrustLists(store1, store2, issuerCertificates, issuerCertificateRevocationLists);

            store1.Close();
            store2.Close();

            Log("Updated HTTPS Trust Lists.");
        }

        /// <summary>
        /// Adds the HTTPS CA certificates to the appropriate Windows store.
        /// </summary>
        private void UpdateHttpsTrustLists(
            ICertificateStore caStore,
            ICertificateStore rootStore,
            byte[][] certificates,
            byte[][] crls)
        {
            if (certificates != null)
            {
                for (int ii = 0; ii < certificates.Length; ii++)
                {
                    try
                    {
                        X509Certificate2 certificate = new X509Certificate2(certificates[ii]);

                        foreach (X509Certificate2 target in caStore.Enumerate())
                        {
                            if (Utils.CompareDistinguishedName(target.Subject, certificate.Subject))
                            {
                                caStore.Delete(target.Thumbprint);
                            }
                        }

                        foreach (X509Certificate2 target in rootStore.Enumerate())
                        {
                            if (Utils.CompareDistinguishedName(target.Subject, certificate.Subject))
                            {
                                rootStore.Delete(target.Thumbprint);
                            }
                        }

                        if (Utils.CompareDistinguishedName(certificate.Subject, certificate.Issuer))
                        {
                            if (rootStore.FindByThumbprint(certificate.Thumbprint) == null)
                            {
                                rootStore.Add(certificate);
                            }
                        }
                        else
                        {
                            if (caStore.FindByThumbprint(certificate.Thumbprint) == null)
                            {
                                caStore.Add(certificate);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log("Could not install HTTPS certificate in trust lists. " + e.Message);
                    }
                }
            }
        }

        public void RevokeCertificate(GdsAgentApplication application)
        {
            if (m_session == null)
            {
                throw new ServiceResultException(StatusCodes.BadNotConnected);
            }

            X509Certificate2 certificate = application.SecuritySettings.ApplicationCertificate.Find(false);

            if (certificate == null)
            {
                throw new ServiceResultException(StatusCodes.BadCertificateInvalid);
            }

            // request certificate.
            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = m_session.Call(
                new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                new NodeId(Opc.Ua.Gds.Methods.Directory_RevokeCertificate, namespaceIndex),
                certificate.Thumbprint);

            if (outputArguments != null)
            {
                Log("Revoked Application Certificate: {0}.", application.ApplicationName);
            }

            // persist the updated application.
            SaveApplication(application);
        }

        /// <summary>
        /// Updates the certificates in the certificate store.
        /// </summary>
        private void UpdateCertificates(Opc.Ua.Security.CertificateStoreIdentifier storeId, byte[][] certificates, bool deleteExisting)
        {
            ICertificateStore store = storeId.OpenStore();

            try
            {
                if (deleteExisting)
                {
                    X509Certificate2Collection certificatesToDelete = store.Enumerate();

                    foreach (X509Certificate2 certificate in certificatesToDelete)
                    {
                        store.Delete(certificate.Thumbprint);
                    }

                    store.Close();
                    store = storeId.OpenStore();
                }

                for (int ii = 0; ii < certificates.Length; ii++)
                {
                    X509Certificate2 certificate = new X509Certificate2(certificates[ii]);

                    if (store.FindByThumbprint(certificate.Thumbprint) == null)
                    {
                        store.Add(certificate);
                    }
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Updates the CTRs in the certificate store.
        /// </summary>
        private void UpdateCRLs(Opc.Ua.Security.CertificateStoreIdentifier storeId, byte[][] crls, bool deleteExisting)
        {
            ICertificateStore store = storeId.OpenStore();

            if (!store.SupportsCRLs)
            {
                return;
            }

            try
            {
                if (deleteExisting)
                {
                    List<X509CRL> crlsToDelete = store.EnumerateCRLs();

                    foreach (X509CRL crl in crlsToDelete)
                    {
                        store.DeleteCRL(crl);
                    }

                    store.Close();
                    store = storeId.OpenStore();
                }

                for (int ii = 0; ii < crls.Length; ii++)
                {
                    X509CRL crl = new X509CRL(crls[ii]);
                    store.AddCRL(crl);
                }
            }
            finally
            {
                store.Close();
            }
        }

        private DirectoryInfo m_persistentFilePath;
        private List<GdsAgentApplication> m_applications;
        private ApplicationConfiguration m_configuration;
        private Session m_session;
        private LogMessageEventHandler m_LogMessage;
    }

    public class LogMessageEventArg : EventArgs
    {
        public string Message { get; set; }
    }

    public delegate void LogMessageEventHandler(object sender, LogMessageEventArg args);

}
