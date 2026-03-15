using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua.Configuration;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using UFUACertificateChecker.Properties;

namespace UFUACertificateChecker
{
    class CertificateMng : ApplicationInstance
    {
        internal X509Certificate2 FindCertificate(string sectionname, string filepath, ApplicationType at = ApplicationType.Server, string appname = null)
        {
            ApplicationType = at;// ApplicationType.Server;
            ConfigSectionName = sectionname;

            ApplicationConfiguration configuration = ApplicationInstance.LoadAppConfig(true,
                filepath, ApplicationType, ConfigurationType, true);

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            var config = ApplicationConfiguration.LoadWithNoValidation(new System.IO.FileInfo(filepath), ConfigurationType ?? typeof(ApplicationConfiguration));
            if (config != null && !String.IsNullOrEmpty(appname))
                configuration.ApplicationUri = String.Format("{0}:{1}", config.ApplicationUri, appname);
            configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);
            ApplicationConfiguration = configuration;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Configuration file does not specify a certificate.");
            }

            return id.Find(true);
        }

        public void CheckCertificate(string sectionname, string filepath, ApplicationType at = ApplicationType.Server, string appname = null)
        {
            ApplicationType = at;// ApplicationType.Server;
            ConfigSectionName = sectionname;

            ApplicationConfiguration configuration = ApplicationInstance.LoadAppConfig(true,
                filepath, ApplicationType, ConfigurationType, true);

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            var config = ApplicationConfiguration.LoadWithNoValidation(new System.IO.FileInfo(filepath), ConfigurationType ?? typeof(ApplicationConfiguration));
            if (config != null && !String.IsNullOrEmpty(appname))
                configuration.ApplicationUri = String.Format("{0}:{1}", config.ApplicationUri, appname);
            configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);
            ApplicationConfiguration = configuration;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Configuration file does not specify a certificate.");
            }

            X509Certificate2 certificate = id.Find(true);

            // check that it is ok.
            if (certificate != null)
            {
                int minimumKeySize = 0;

                Utils.Trace(Utils.TraceMasks.Information, "Checking application instance certificate. {0}", certificate.Subject);

                try
                {
                    // validate certificate.
                    configuration.CertificateValidator.Validate(certificate);
                }
                catch (Exception ex)
                {
                    if (System.Windows.MessageBox.Show(String.Format(Properties.Resources.BadCertificate, ex.Message),
                       Properties.Resources.AppTitle, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                        CreateNewCertificate(sectionname, filepath, appname: appname);
                    return;
                }

                // check key size.
                if (minimumKeySize > certificate.PublicKey.Key.KeySize)
                {
                    string message = Utils.Format(
                        "The key size ({0}) in the certificate is less than the minimum provided ({1}). Update certificate?",
                        certificate.PublicKey.Key.KeySize,
                        minimumKeySize);

                    Utils.Trace(message);

                    if (System.Windows.MessageBox.Show(Properties.Resources.InvalidKeyCertificate,
                        Properties.Resources.AppTitle, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                        CreateNewCertificate(sectionname, filepath, appname: appname);
                    return;
                }
                System.Windows.MessageBox.Show(Properties.Resources.ValidCertificate);
                return;
            }
            if(System.Windows.MessageBox.Show(Properties.Resources.InvalidOrNullCertificate,
                        Properties.Resources.AppTitle, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                CreateNewCertificate(sectionname, filepath, appname: appname);

        }
        public int CheckAndCreate(string sectionname, string filepath, ApplicationType at = ApplicationType.Server, string appname = null)
        {
            ApplicationType = at; //ApplicationType.Server;
            ConfigSectionName = sectionname;

            ApplicationConfiguration configuration = ApplicationInstance.LoadAppConfig(true,
                filepath, ApplicationType, ConfigurationType, true);

            if (configuration == null)
            {
                return -1;
                //throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            var config = ApplicationConfiguration.LoadWithNoValidation(new System.IO.FileInfo(filepath), ConfigurationType ?? typeof(ApplicationConfiguration));
            if (config != null && !String.IsNullOrEmpty(appname))
                configuration.ApplicationUri = String.Format("{0}:{1}", config.ApplicationUri, appname);
            configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);
            ApplicationConfiguration = configuration;

            try
            {
                CheckApplicationInstanceCertificate(false, 0);
            }
            catch (Exception ex)
            {
                return -2;
            }
            
            return 0;
        }

        public void CreateNewCertificate(string sectionname, string filepath, ApplicationType at = ApplicationType.Server, string appname = null)
        {

            ApplicationType = at;
            ConfigSectionName = sectionname;

            ApplicationConfiguration configuration = ApplicationInstance.LoadAppConfig(true,
                filepath, ApplicationType, ConfigurationType, true);

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            var config = ApplicationConfiguration.LoadWithNoValidation(new System.IO.FileInfo(filepath), ConfigurationType ?? typeof(ApplicationConfiguration));
            if (config != null && !String.IsNullOrEmpty(appname))
                configuration.ApplicationUri = String.Format("{0}:{1}", config.ApplicationUri, appname);
            configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);

            X509Certificate2 certificate = CreateNewApplicationInstanceCertificate(configuration, 0, 600);
        }

        private X509Certificate2 CreateNewApplicationInstanceCertificate(ApplicationConfiguration configuration,
            ushort keySize,
            ushort lifetimeInMonths)
        {
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id != null)
            {
                // delete private key.
                X509Certificate2 delcertificate = id.Find();

                // delete trusted peer certificate.
                if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
                {
                    string thumbprint = id.Thumbprint;

                    if (delcertificate != null)
                    {
                        thumbprint = delcertificate.Thumbprint;
                    }

                    using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                    {
                        store.Delete(thumbprint);
                    }
                }

                // delete private key.
                if (delcertificate != null)
                {
                    using (ICertificateStore store = id.OpenStore())
                    {
                        store.Delete(delcertificate.Thumbprint);
                    }
                }

            }
            
            CertificateIdentifier cid = configuration.SecurityConfiguration.ApplicationCertificate;

            // get the domains from the configuration file.
            IList<string> serverDomainNames = configuration.GetServerDomainNames();

            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(System.Net.Dns.GetHostName());
            }

            // ensure the certificate store directory exists.
            if (cid.StoreType == CertificateStoreType.Directory)
            {
                Utils.GetAbsoluteDirectoryPath(cid.StorePath, true, true, true);
            }

            X509Certificate2 certificate = Opc.Ua.CertificateFactory.CreateCertificate(
                cid.StoreType,
                cid.StorePath,
                configuration.ApplicationUri,
                configuration.ApplicationName,
                null,
                serverDomainNames,
                keySize,
                lifetimeInMonths);

            cid.Certificate = certificate;

            AddToStore(configuration, certificate);

            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);

            Utils.Trace(Utils.TraceMasks.Information, "Certificate created. Thumbprint={0}", certificate.Thumbprint);

            if (configuration.ApplicationType == ApplicationType.Server || configuration.ApplicationType == ApplicationType.ClientAndServer)
            {
                try
                {
                    AddToDiscoveryServerTrustList(certificate, null, null, configuration.SecurityConfiguration.TrustedPeerCertificates);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Could not add certificate to LDS trust list.");
                }
            }

            // reload the certificate from disk.
            return configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(null);
        }


        public static void AddToStore(ApplicationConfiguration configuration, X509Certificate2 certificate)
        {
            string storePath = null;

            if (configuration != null && configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                storePath = configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            }

            if (String.IsNullOrEmpty(storePath))
            {
                Utils.Trace(Utils.TraceMasks.Information, "WARNING: Trusted peer store not specified.");
                return;
            }

            try
            {
                ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

                if (store == null)
                {
                    Utils.Trace("Could not open trusted peer store. StorePath={0}", storePath);
                    return;
                }

                try
                {
                    // check if it already exists.
                    X509Certificate2 certificate2 = store.FindByThumbprint(certificate.Thumbprint);

                    if (certificate2 != null)
                    {
                        return;
                    }

                    Utils.Trace(Utils.TraceMasks.Information, "Adding certificate to trusted peer store. StorePath={0}", storePath);

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

                    //copy the certificate in the trusted directory of LDS
                    configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = Settings.Default.TrustListPath;
                    store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();
                    store.Add(publicKey);
                }
                finally
                {
                    store.Close();
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not add certificate to trusted peer store. StorePath={0}", storePath);
            }
        }

        public void installCertificate(String certificateFile, string sectionname, string filepath, ApplicationType at = ApplicationType.Server, string appname = null)
        {
            ApplicationType = at;
            ConfigSectionName = sectionname;

            ApplicationConfiguration configuration = ApplicationInstance.LoadAppConfig(true,
                filepath, ApplicationType, ConfigurationType, true);

            if (configuration == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Could not load configuration file.");
            }

            var config = ApplicationConfiguration.LoadWithNoValidation(new System.IO.FileInfo(filepath), ConfigurationType ?? typeof(ApplicationConfiguration));
            if (config != null && !String.IsNullOrEmpty(appname))
                configuration.ApplicationUri = String.Format("{0}:{1}", config.ApplicationUri, appname);
            configuration.ApplicationUri = Utils.ReplaceLocalhost(configuration.ApplicationUri);

            X509Certificate2 cert = new X509Certificate2(certificateFile);
            AddToStore(configuration, cert);
        }

    }
}
