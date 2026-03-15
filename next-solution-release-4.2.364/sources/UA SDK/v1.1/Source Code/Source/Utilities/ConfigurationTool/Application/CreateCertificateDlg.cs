/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Opc.Ua.Client.Controls;
using Opc.Ua.Security;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Prompts the user to specify a new access rule for a file.
    /// </summary>
    public partial class CreateCertificateDlg : Form
    {
        #region Constructors
        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        public CreateCertificateDlg()
        {
            InitializeComponent();

            KeySizeCB.Items.Add("1024");
            KeySizeCB.Items.Add("2048");
            KeySizeCB.SelectedIndex = 0;

            KeyFormatCB.Items.Add("PFX");
            KeyFormatCB.Items.Add("PEM");
            KeyFormatCB.SelectedIndex = 0;
        }
        #endregion

        #region Private Fields
        private CertificateIdentifier m_certificate;
        private string m_currentDirectory;
        #endregion
        
        #region Public Interface
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public CertificateIdentifier ShowDialog(SecuredApplication configuration)
        {
            CertificateStoreCTRL.StoreType = null;
            CertificateStoreCTRL.StorePath = null;
            IssuerKeyFilePathTB.Text = null;
            IssuerPasswordTB.Text = null;
            ApplicationNameTB.Text = null;
            ApplicationUriTB.Text = null;
            SubjectNameTB.Text = null;
            DomainsTB.Text = System.Net.Dns.GetHostName();
            KeySizeCB.SelectedIndex = 0;
            LifeTimeInMonthsUD.Value = 60;

            if (configuration != null)
            {
                ApplicationNameTB.Text = configuration.ApplicationName;
                ApplicationUriTB.Text = configuration.ApplicationUri;

                if (configuration.ApplicationCertificate != null)
                {
                    CertificateStoreCTRL.StoreType = configuration.ApplicationCertificate.StoreType;
                    CertificateStoreCTRL.StorePath = configuration.ApplicationCertificate.StorePath;
                    UpdateWithCertificate(configuration.ApplicationCertificate.Find());
                }
            }

            if (ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return m_certificate;
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public CertificateIdentifier ShowDialog(CertificateStoreIdentifier store, string issuerKeyFilePath, X509Certificate2 certificate)
        {
            CertificateStoreCTRL.StoreType = null;
            CertificateStoreCTRL.StorePath = null;
            IssuerKeyFilePathTB.Text = null;
            IssuerPasswordTB.Text = null;
            ApplicationNameTB.Text = null;
            ApplicationUriTB.Text = null;
            SubjectNameTB.Text = null;
            DomainsTB.Text = System.Net.Dns.GetHostName();
            KeySizeCB.SelectedIndex = 0;
            LifeTimeInMonthsUD.Value = 60;

            if (store != null)
            {
                CertificateStoreCTRL.StoreType = store.StoreType;
                CertificateStoreCTRL.StorePath = store.StorePath;
            }

            if (issuerKeyFilePath != null)
            {
                IssuerKeyFilePathTB.Text = issuerKeyFilePath;
            }

            UpdateWithCertificate(certificate);

            if (ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return m_certificate;
        }

        /// <summary>
        /// Updates controls with the certificate.
        /// </summary>
        private void UpdateWithCertificate(X509Certificate2 certificate)
        {
            int index = 0;

            if (certificate != null)
            {
                StringBuilder buffer = new StringBuilder();

                foreach (string element in Utils.ParseDistinguishedName(certificate.Subject))
                {
                    string key = element;
                    string value = String.Empty;

                    index = key.IndexOf('=');

                    if (index >= 0)
                    {
                        key = element.Substring(0, index);
                        value = element.Substring(index + 1);
                    }

                    if (key == "CN")
                    {
                        ApplicationNameTB.Text = value;
                    }

                    if (key == "O")
                    {
                        OrganizationTB.Text = value;
                    }

                    // work around to deal with invalid fields in some certificates.
                    if (key == "S")
                    {
                        key = "ST";
                    }

                    if (buffer.Length > 0)
                    {
                        buffer.Append('/');
                    }

                    buffer.Append(key);
                    buffer.Append('=');

                    if (value.IndexOf('/') != -1)
                    {
                        buffer.Append('"');
                        buffer.Append(value);
                        buffer.Append('"');
                    }
                    else
                    {
                        buffer.Append(value);
                    }
                }

                if (buffer.Length > 0)
                {
                    SubjectNameCK.Checked = true;
                    SubjectNameTB.Text = buffer.ToString();
                }

                string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

                if (!String.IsNullOrEmpty(applicationUri))
                {
                    ApplicationUriCK.Checked = true;
                    ApplicationUriTB.Text = applicationUri;
                }

                buffer = new StringBuilder();

                foreach (string domain in Utils.GetDomainsFromCertficate(certificate))
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(',');
                    }

                    buffer.Append(domain);
                }

                if (buffer.Length > 0)
                {
                    DomainsCK.Checked = true;
                    DomainsTB.Text = buffer.ToString();
                }

                index = KeySizeCB.FindStringExact(certificate.PublicKey.Key.KeySize.ToString());

                if (index >= 0)
                {
                    KeySizeCB.SelectedIndex = index;
                }
            }
        }
        #endregion

        #region Event Handlers
        private void OkBTN_Click(object sender, EventArgs e)
        {
            try
            {
                string storeType = null;
                string storePath = null;
                string applicationName = ApplicationNameTB.Text.Trim();
                string applicationUri = ApplicationUriTB.Text.Trim();
                string subjectName = SubjectNameTB.Text.Trim();
                string[] domainNames = null;

                string issuerKeyFilePath = IssuerKeyFilePathTB.Text.Trim();
                string issuerKeyFilePassword = IssuerPasswordTB.Text.Trim();

                if (!String.IsNullOrEmpty(issuerKeyFilePath))
                {
                    // verify certificate.
                    X509Certificate2 issuer = new X509Certificate2(
                        issuerKeyFilePath,
                        issuerKeyFilePassword,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                    if (!issuer.HasPrivateKey)
                    {
                        throw new ApplicationException("Issuer certificate does not have a private key.");
                    }

                    // determine certificate type.
                    foreach (X509Extension extension in issuer.Extensions)
                    {
                        X509BasicConstraintsExtension basicContraints = extension as X509BasicConstraintsExtension;

                        if (basicContraints != null)
                        {
                            if (!basicContraints.CertificateAuthority)
                            {
                                throw new ApplicationException("Certificate cannot be used to issue new certificates.");
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(CertificateStoreCTRL.StorePath))
                {
                    storeType = CertificateStoreCTRL.StoreType;
                    storePath = CertificateStoreCTRL.StorePath;
                }

                domainNames = DomainsTB.Text.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (String.IsNullOrEmpty(storePath))
                {
                    throw new ApplicationException("Please specify a store path.");
                }

                if (String.IsNullOrEmpty(applicationName))
                {
                    throw new ApplicationException("Please specify an application name.");
                }

                X509Certificate2 certificate = Opc.Ua.CertificateFactory.CreateCertificate(
                    storeType,
                    storePath,
                    null,
                    applicationUri,
                    applicationName,
                    subjectName,
                    domainNames,
                    Convert.ToUInt16(KeySizeCB.SelectedItem.ToString()),
                    (ushort)LifeTimeInMonthsUD.Value,
                    false,
                    (string)KeyFormatCB.SelectedItem == "PEM",
                    issuerKeyFilePath,
                    issuerKeyFilePassword);

                m_certificate = new CertificateIdentifier();
                m_certificate.StoreType = storeType;
                m_certificate.StorePath = storePath;
                m_certificate.Certificate = certificate;
                
                // close the dialog.
                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, System.Reflection.MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void DomainsCK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ApplicationUriTB.Enabled = ApplicationUriCK.Checked;
                DomainsTB.Enabled = DomainsCK.Checked;
                SubjectNameTB.Enabled = SubjectNameCK.Checked;
                ApplicationNameTB_TextChanged(sender, e);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, System.Reflection.MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ApplicationNameTB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!DomainsCK.Checked)
                {
                    DomainsTB.Text = System.Net.Dns.GetHostName();
                }

                // get the domain name.
                string domainName = DomainsTB.Text;
                int index = domainName.IndexOfAny(new char[] { ',', ';' });

                if (index > 0)
                {
                    domainName = DomainsTB.Text.Substring(0, index);
                }

                // update subject name.
                if (!SubjectNameCK.Checked)
                {
                    StringBuilder buffer = new StringBuilder();
                    buffer.Append("CN=");
                    buffer.Append(ApplicationNameTB.Text);

                    if (!String.IsNullOrEmpty(OrganizationTB.Text))
                    {
                        buffer.Append("/O=");
                        buffer.Append(OrganizationTB.Text);
                    }

                    buffer.Append("/DC=");
                    buffer.Append(domainName);

                    SubjectNameTB.Text = buffer.ToString();
                }

                // update application uri.
                if (!ApplicationUriCK.Checked)
                {
                    StringBuilder buffer = new StringBuilder();
                    
                    buffer.Append("urn:");
                    buffer.Append(domainName);

                    if (!String.IsNullOrEmpty(OrganizationTB.Text))
                    {
                        buffer.Append(":");
                        buffer.Append(OrganizationTB.Text);
                    }

                    buffer.Append(":");
                    buffer.Append(ApplicationNameTB.Text);

                    ApplicationUriTB.Text = buffer.ToString();
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, System.Reflection.MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void BrowseBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // set current directory.
                if (m_currentDirectory == null)
                {
                    m_currentDirectory = Utils.GetAbsoluteDirectoryPath("%CommonApplicationData%\\OPC Foundation\\CertificateStores\\UA Certificate Authorities\\private", false, false);
                }

                // open file dialog.
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.DefaultExt = ".pfx";
                dialog.Filter = "PKCS#12 Files (*.pfx)|*.pfx|All Files (*.*)|*.*";
                dialog.Multiselect = false;
                dialog.ValidateNames = true;
                dialog.Title = "Open Issuer (CA) Private Key File";
                dialog.FileName = null;
                dialog.InitialDirectory = m_currentDirectory;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                FileInfo fileInfo = new FileInfo(dialog.FileName);
                m_currentDirectory = fileInfo.Directory.FullName;
                IssuerKeyFilePathTB.Text = fileInfo.FullName;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, System.Reflection.MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion

    }
}
