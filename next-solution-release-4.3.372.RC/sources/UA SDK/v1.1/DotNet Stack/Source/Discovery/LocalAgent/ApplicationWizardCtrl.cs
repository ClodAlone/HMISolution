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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Security;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    public partial class ApplicationWizardCtrl : UserControl
    {
        #region Constructors
        public ApplicationWizardCtrl()
        {
            InitializeComponent();

            foreach (ApplicationType value in Enum.GetValues(typeof(ApplicationType)))
            {
                ApplicationTypeCB.Items.Add(value);
            }

            SetPermissionAndAccess(Stage.SelectConfigurationType);
        }
        #endregion

        #region Private Fields
        private Stage m_stage;
        private ConfigurationType m_configurationType;
        private event EventHandler m_WizardCancelled;
        private event EventHandler m_WizardComplete;
        #endregion

        #region Public Interface
        /// <summary>
        /// Raised if the wizard is cancelled.
        /// </summary>
        public event EventHandler WizardCancelled
        {
            add { m_WizardCancelled += value; }
            remove { m_WizardCancelled -= value; }
        }

        /// <summary>
        /// Raised when an application is created.
        /// </summary>
        public event EventHandler WizardComplete
        {
            add { m_WizardComplete += value; }
            remove { m_WizardComplete -= value; }
        }

        /// <summary>
        /// Starts the wizard.
        /// </summary>
        public void Start(GdsAgentApplication application)
        {
            this.Visible = true;

            ClearControls();

            if (application != null)
            {
                m_configurationType = application.ConfigurationType;
                CopyToControls(application);
            }
            else
            {
                m_configurationType = ConfigurationType.Xml;
            }

            SetPermissionAndAccess(Stage.SelectConfigurationType);
        }

        /// <summary>
        /// Ends the wizard.
        /// </summary>
        public void Finish(GdsAgentApplication application)
        {
            if (application != null)
            {
                CopyFromControls(application);

                if (application.ConfigurationType == ConfigurationType.Xml)
                {
                    SaveXmlConfiguration(application);
                }
            }
            
            this.Visible = false;
        }
        #endregion

        #region Stage Enumeration
        private enum Stage
        {
            SelectConfigurationType,
            Configuration,
            ConfirmConfiguration
        }
        #endregion

        #region Stage Enumeration
        private void ClearControls()
        {
            ConfigurationFileTB.Text = null;
            ImportExportUtilityTB.Text = null;
            ImportArgumentsTB.Text = "/import {0}";
            ExportArgumentsTB.Text = "/export {0}";
            PublicKeyTB.Text = null;
            PrivateKeyTB.Text = null;
            ApplicationNameTB.Text = null;
            ApplicationTypeCB.SelectedItem = ApplicationType.Server;
            ApplicationUriTB.Text = null;
            ProductUriTB.Text = null;
            MachineNameTB.Text = null;
            SubjectNameTB.Text = null;
            TrustListTB.Text = null;
            IssuerListTB.Text = null;
            PublicKeyTB.Text = null;
            PrivateKeyTB.Text = null;
            SecurityProfilesTB.Text = null;
            SecurityProfilesBTN.Profiles = null;
            BaseAddressesTB.Text = null;
            BaseAddressesBTN.Urls = null;
        }

        private void CopyFromControls(GdsAgentApplication application)
        {
            application.ConfigurationType = m_configurationType;
            application.ApplicationName = ApplicationNameTB.Text.Trim();
            application.MachineName = MachineNameTB.Text.Trim();
            application.SubjectName = SubjectNameTB.Text.Trim();

            switch (m_configurationType)
            {
                case ConfigurationType.Xml:
                {
                    application.ConfigurationFile = ConfigurationFileTB.Text.Trim();
                    application.ImportExportUtility = null;
                    application.ImportArguments = null;
                    application.ExportArguments = null;
                    application.PublicKeyFilePath = null;
                    application.PrivateKeyFilePath = null;
                    break;
                }

                case ConfigurationType.ImportExport:
                {
                    application.ConfigurationFile = null;
                    application.ImportExportUtility = ImportExportUtilityTB.Text.Trim();
                    application.ImportArguments = ImportArgumentsTB.Text.Trim();
                    application.ExportArguments = ExportArgumentsTB.Text.Trim();
                    application.PublicKeyFilePath = null;
                    application.PrivateKeyFilePath = null;
                    break;
                }

                case ConfigurationType.Manual:
                {
                    application.ConfigurationFile = null;
                    application.ImportExportUtility = null;
                    application.ImportArguments = null;
                    application.ExportArguments = null;
                    application.PublicKeyFilePath = PublicKeyTB.Text.Trim();
                    application.PrivateKeyFilePath = PrivateKeyTB.Text.Trim();
                    break;
                }
            }

            if (application.SecuritySettings == null)
            {
                application.SecuritySettings = new SecuredApplication();
            }

            CopyFromControls(application.SecuritySettings);
        }

        private void CopyToControls(GdsAgentApplication application)
        {
            ClearControls();

            m_configurationType = application.ConfigurationType;
            ApplicationNameTB.Text = application.ApplicationName;
            MachineNameTB.Text = application.MachineName;
            SubjectNameTB.Text = application.SubjectName;
            ConfigurationFileTB.Text = application.ConfigurationFile;
            ImportExportUtilityTB.Text = application.ImportExportUtility;
            ImportArgumentsTB.Text = application.ImportArguments;
            ExportArgumentsTB.Text = application.ExportArguments;
            PublicKeyTB.Text = application.PublicKeyFilePath;
            PrivateKeyTB.Text = application.PrivateKeyFilePath;

            if (String.IsNullOrEmpty(MachineNameTB.Text))
            {
                MachineNameTB.Text = System.Net.Dns.GetHostName();
            }

            if (application.SecuritySettings != null)
            {
                CopyToControls(application.SecuritySettings);
            }
        }

        /// <summary>
        /// Updates the SecuredApplication with the values in the controls.
        /// </summary>
        private void CopyFromControls(SecuredApplication settings)
        {
            settings.ProductName       = null;
            settings.ApplicationName   = ApplicationNameTB.Text.Trim();
            settings.ApplicationType   = SecuredApplication.ToApplicationType((ApplicationType)ApplicationTypeCB.SelectedItem);
            settings.ApplicationUri    = ApplicationUriTB.Text.Trim();
            settings.ProductName       = ProductUriTB.Text.Trim();
            settings.ConfigurationFile = ConfigurationFileTB.Text.Trim();

            ApplicationCertificateStoreTB.Text = ApplicationCertificateStoreTB.Text.Trim();

            if (!String.IsNullOrEmpty(ApplicationCertificateStoreTB.Text))
            {
                if (settings.ApplicationCertificate == null)
                {
                    settings.ApplicationCertificate = new Opc.Ua.Security.CertificateIdentifier();
                }

                settings.ApplicationCertificate.StoreType = CertificateStoreIdentifier.DetermineStoreType(ApplicationCertificateStoreTB.Text);
                settings.ApplicationCertificate.StorePath = ApplicationCertificateStoreTB.Text.Trim();
            }
            else
            {
                settings.ApplicationCertificate = null;
            }

            TrustListTB.Text = TrustListTB.Text.Trim();

            if (!String.IsNullOrEmpty(TrustListTB.Text))
            {
                settings.TrustedCertificateStore = new Opc.Ua.Security.CertificateStoreIdentifier();
                settings.TrustedCertificateStore.StoreType = CertificateStoreIdentifier.DetermineStoreType(TrustListTB.Text);
                settings.TrustedCertificateStore.StorePath = TrustListTB.Text.Trim();
            }
            else
            {
                settings.TrustedCertificateStore = null;
            }

            IssuerListTB.Text = IssuerListTB.Text.Trim();

            if (!String.IsNullOrEmpty(IssuerListTB.Text))
            {
                settings.IssuerCertificateStore = new Opc.Ua.Security.CertificateStoreIdentifier();
                settings.IssuerCertificateStore.StoreType = CertificateStoreIdentifier.DetermineStoreType(IssuerListTB.Text);
                settings.IssuerCertificateStore.StorePath = IssuerListTB.Text.Trim();
            }
            else
            {
                settings.IssuerCertificateStore = null;
            }

            if (SecurityProfilesBTN.Profiles != null)
            {
                settings.SecurityProfiles = SecurityProfilesBTN.Profiles;
            }

            settings.BaseAddresses = new ListOfBaseAddresses();

            if (BaseAddressesBTN.Urls != null)
            {
                foreach (Uri url in BaseAddressesBTN.Urls)
                {
                    settings.BaseAddresses.Add(url.ToString());
                }
            }
        }

        /// <summary>
        /// Updates the controls with the SecuredApplication.
        /// </summary>
        private void CopyToControls(SecuredApplication settings)
        {
            ApplicationNameTB.Text = settings.ApplicationName;
            ApplicationTypeCB.SelectedItem = SecuredApplication.FromApplicationType(settings.ApplicationType);
            ApplicationUriTB.Text = settings.ApplicationUri;
            ProductUriTB.Text = settings.ProductName;
            TrustListTB.Text = null;
            IssuerListTB.Text = null;
            SecurityProfilesTB.Text = null;
            SecurityProfilesBTN.Profiles = null;
            BaseAddressesTB.Text = null;
            BaseAddressesBTN.Urls = null;

            if (settings.ApplicationCertificate != null)
            {
                ApplicationCertificateStoreTB.Text = settings.ApplicationCertificate.StorePath;
            }

            if (settings.TrustedCertificateStore != null)
            {
                TrustListTB.Text = settings.TrustedCertificateStore.StorePath;
            }

            if (settings.IssuerCertificateStore != null)
            {
                IssuerListTB.Text = settings.IssuerCertificateStore.StorePath;
            }

            if (settings.SecurityProfiles != null)
            {
                SecurityProfilesBTN.Profiles = settings.SecurityProfiles;
            }

            if (settings.BaseAddresses != null)
            {
                List<Uri> urls = new List<Uri>();

                foreach (string baseAddress in settings.BaseAddresses)
                {
                    Uri url = Utils.ParseUri(baseAddress);
                    urls.Add(url);
                }

                BaseAddressesBTN.Urls = urls;
            }
        }

        private void SetXmlConfiguration()
        {
            ConfigurationFileLB.Visible = true;
            ConfigurationFileTB.Visible = true;
            ConfigurationFileBTN.Visible = true;
            ImportExportUtilityLB.Visible = false;
            ImportExportUtilityTB.Visible = false;
            ImportExportUtilityBTN.Visible = false;
            ImportArgumentsLB.Visible = false;
            ImportArgumentsTB.Visible = false;
            ExportArgumentsLB.Visible = false;
            ExportArgumentsTB.Visible = false;
            ApplicationNameLB.Visible = false;
            ApplicationNameTB.Visible = false;
            ApplicationTypeLB.Visible = false;
            ApplicationTypeCB.Visible = false;
            ApplicationUriLB.Visible = false;
            ApplicationUriTB.Visible = false;
            MachineNameLB.Visible = false;
            MachineNameTB.Visible = false;
            SubjectNameLB.Visible = false;
            SubjectNameTB.Visible = false;
            ProductUriLB.Visible = false;
            ProductUriTB.Visible = false;
            ApplicationCertificateStoreLB.Visible = false;
            ApplicationCertificateStoreTB.Visible = false;
            ApplicationCertificateStoreBTN.Visible = false;
            PublicKeyLB.Visible = false;
            PublicKeyTB.Visible = false;
            PublicKeyBTN.Visible = false;
            PrivateKeyLB.Visible = false;
            PrivateKeyTB.Visible = false;
            PrivateKeyBTN.Visible = false;
            TrustListLB.Visible = false;
            TrustListTB.Visible = false;
            TrustListBTN.Visible = false;
            IssuerListLB.Visible = false;
            IssuerListTB.Visible = false;
            IssuerListBTN.Visible = false;
            SecurityProfilesLB.Visible = false;
            SecurityProfilesTB.Visible = false;
            SecurityProfilesBTN.Visible = false;
            BaseAddressesLB.Visible = false;
            BaseAddressesTB.Visible = false;
            BaseAddressesBTN.Visible = false;
        }

        private void SetImportExportConfiguration()
        {
            ConfigurationFileLB.Visible = false;
            ConfigurationFileTB.Visible = false;
            ConfigurationFileBTN.Visible = false;
            ImportExportUtilityLB.Visible = true;
            ImportExportUtilityTB.Visible = true;
            ImportExportUtilityBTN.Visible = true;
            ImportArgumentsLB.Visible = true;
            ImportArgumentsTB.Visible = true;
            ExportArgumentsLB.Visible = true;
            ExportArgumentsTB.Visible = true;
            ApplicationNameLB.Visible = false;
            ApplicationNameTB.Visible = false;
            ApplicationTypeLB.Visible = false;
            ApplicationTypeCB.Visible = false;
            ApplicationUriLB.Visible = false;
            ApplicationUriTB.Visible = false;
            MachineNameLB.Visible = false;
            MachineNameTB.Visible = false;
            SubjectNameLB.Visible = false;
            SubjectNameTB.Visible = false;
            ProductUriLB.Visible = false;
            ProductUriTB.Visible = false;
            ApplicationCertificateStoreLB.Visible = false;
            ApplicationCertificateStoreTB.Visible = false;
            ApplicationCertificateStoreBTN.Visible = false;
            PublicKeyLB.Visible = false;
            PublicKeyTB.Visible = false;
            PublicKeyBTN.Visible = false;
            PrivateKeyLB.Visible = false;
            PrivateKeyTB.Visible = false;
            PrivateKeyBTN.Visible = false;
            TrustListLB.Visible = false;
            TrustListTB.Visible = false;
            TrustListBTN.Visible = false;
            IssuerListLB.Visible = false;
            IssuerListTB.Visible = false;
            IssuerListBTN.Visible = false;
            SecurityProfilesLB.Visible = false;
            SecurityProfilesTB.Visible = false;
            SecurityProfilesBTN.Visible = false;
            BaseAddressesLB.Visible = false;
            BaseAddressesTB.Visible = false;
            BaseAddressesBTN.Visible = false;
        }

        private void SetManualConfiguration()
        {
            ConfigurationFileLB.Visible = false;
            ConfigurationFileTB.Visible = false;
            ConfigurationFileBTN.Visible = false;
            ImportExportUtilityLB.Visible = false;
            ImportExportUtilityTB.Visible = false;
            ImportExportUtilityBTN.Visible = false;
            ImportArgumentsLB.Visible = false;
            ImportArgumentsTB.Visible = false;
            ExportArgumentsLB.Visible = false;
            ExportArgumentsTB.Visible = false;
            ApplicationNameLB.Visible = true;
            ApplicationNameTB.Visible = true;
            ApplicationTypeLB.Visible = true;
            ApplicationTypeCB.Visible = true;
            ApplicationUriLB.Visible = true;
            ApplicationUriTB.Visible = true;
            MachineNameLB.Visible = true;
            MachineNameTB.Visible = true;
            SubjectNameLB.Visible = true;
            SubjectNameTB.Visible = true;
            ProductUriLB.Visible = true;
            ProductUriTB.Visible = true;
            ApplicationCertificateStoreLB.Visible = false;
            ApplicationCertificateStoreTB.Visible = false;
            ApplicationCertificateStoreBTN.Visible = false;
            PublicKeyLB.Visible = true;
            PublicKeyTB.Visible = true;
            PublicKeyBTN.Visible = true;
            PrivateKeyLB.Visible = true;
            PrivateKeyTB.Visible = true;
            PrivateKeyBTN.Visible = true;
            TrustListLB.Visible = true;
            TrustListTB.Visible = true;
            TrustListBTN.Visible = true;
            IssuerListLB.Visible = true;
            IssuerListTB.Visible = true;
            IssuerListBTN.Visible = true;
            SecurityProfilesLB.Visible = false;
            SecurityProfilesTB.Visible = false;
            SecurityProfilesBTN.Visible = false;
            BaseAddressesLB.Visible = true;
            BaseAddressesTB.Visible = true;
            BaseAddressesBTN.Visible = true;
        }

        private void SetConfirmConfiguration()
        {
            ConfigurationFileLB.Visible = false;
            ConfigurationFileTB.Visible = false;
            ConfigurationFileBTN.Visible = false;
            ImportExportUtilityLB.Visible = false;
            ImportExportUtilityTB.Visible = false;
            ImportExportUtilityBTN.Visible = false;
            ExportArgumentsLB.Visible = false;
            ExportArgumentsTB.Visible = false;
            ApplicationNameLB.Visible = true;
            ApplicationNameTB.Visible = true;
            ApplicationTypeLB.Visible = true;
            ApplicationTypeCB.Visible = true;
            ApplicationUriLB.Visible = true;
            ApplicationUriTB.Visible = true;
            MachineNameLB.Visible = true;
            MachineNameTB.Visible = true;
            SubjectNameLB.Visible = true;
            SubjectNameTB.Visible = true;
            ProductUriLB.Visible = true;
            ProductUriTB.Visible = true;
            ApplicationCertificateStoreLB.Visible = XmlConfigRB.Checked;
            ApplicationCertificateStoreTB.Visible = XmlConfigRB.Checked;
            ApplicationCertificateStoreBTN.Visible = XmlConfigRB.Checked;
            PublicKeyLB.Visible = false;
            PublicKeyTB.Visible = false;
            PublicKeyBTN.Visible = false;
            PrivateKeyLB.Visible = false;
            PrivateKeyTB.Visible = false;
            PrivateKeyBTN.Visible = false;
            TrustListLB.Visible = true;
            TrustListTB.Visible = true;
            TrustListBTN.Visible = true;
            IssuerListLB.Visible = true;
            IssuerListTB.Visible = true;
            IssuerListBTN.Visible = true;
            SecurityProfilesLB.Visible = true;
            SecurityProfilesTB.Visible = true;
            SecurityProfilesBTN.Visible = true;
            BaseAddressesLB.Visible = true;
            BaseAddressesTB.Visible = true;
            BaseAddressesBTN.Visible = true;
        }

        private void SetPermissionAndAccess(Stage stage)
        {
            m_stage = stage;

            switch (stage)
            {
                case Stage.SelectConfigurationType:
                {
                    Stage1LB.Text = Properties.Resources.IDS_STAGE1_SELECT;
                    NextBTN.Visible = true;
                    BackBTN.Visible = false;
                    DoneBTN.Visible = false;
                    Stage1PN.Visible = true;
                    Stage2PN.Visible = false;

                    switch (m_configurationType)
                    {
                        case ConfigurationType.Xml: { XmlConfigRB.Checked = true; break; }
                        case ConfigurationType.Manual: { ManualConfigRB.Checked = true; break; }
                        case ConfigurationType.ImportExport: { ImportExportRB.Checked = true; break; }
                    }

                    break;
                }

                case Stage.Configuration:
                {
                    Stage2LB.Text = Properties.Resources.IDS_STAGE2_SPECIFY;

                    if (m_configurationType == ConfigurationType.Manual)
                    {
                        NextBTN.Visible = false;
                        BackBTN.Visible = true;
                        DoneBTN.Visible = true;
                        Stage1PN.Visible = false;
                        Stage2PN.Visible = true;
                    }
                    else
                    {
                        NextBTN.Visible = true;
                        BackBTN.Visible = true;
                        DoneBTN.Visible = false;
                        Stage1PN.Visible = false;
                        Stage2PN.Visible = true;
                    }

                    SetConfiguration(m_configurationType);
                    break;
                }

                case Stage.ConfirmConfiguration:
                {
                    Stage2LB.Text = Properties.Resources.IDS_STAGE2_CONFIRM;
                    NextBTN.Visible = false;
                    BackBTN.Visible = true;
                    DoneBTN.Visible = true;
                    Stage1PN.Visible = false;
                    Stage2PN.Visible = true;

                    SetConfirmConfiguration();
                    break;
                }
            }
        }

        private void ValidateXmlConfiguration()
        {
            FileInfo file = new FileInfo(Utils.GetAbsoluteFilePath(ConfigurationFileTB.Text));

            if (!file.Exists)
            {
                throw new ArgumentException("Please specify a valid path to a configuration file.");
            }

            SecuredApplication configuration = null;

            try
            {
                configuration = new SecurityConfigurationManager().ReadConfiguration(file.FullName);
            }
            catch (Exception)
            {
                throw new ArgumentException("The configuration file does not appear to be valid.");
            }

            // copy to controls.
            CopyToControls(configuration);

            // ensure the machine name is non-null.
            if (String.IsNullOrEmpty(MachineNameTB.Text))
            {
                MachineNameTB.Text = System.Net.Dns.GetHostName();
            }
        }

        private void SaveXmlConfiguration(GdsAgentApplication application)
        {
            FileInfo file = new FileInfo(Utils.GetAbsoluteFilePath(ConfigurationFileTB.Text));

            try
            {
                new SecurityConfigurationManager().WriteConfiguration(file.FullName, application.SecuritySettings);
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not update the configuration file.");
            }
        }

        private void ValidateImportExportConfiguration()
        {
            // TBD
        }

        /// <summary>
        /// Validates a certificate store path.
        /// </summary>
        private bool ValidateCertificateStore(string storePath)
        {
            string storeType = CertificateStoreIdentifier.DetermineStoreType(storePath);

            // attempt to open a windows store.
            if (storeType == CertificateStoreType.Windows)
            {
                Opc.Ua.Security.CertificateStoreIdentifier id = new Opc.Ua.Security.CertificateStoreIdentifier();
                id.StoreType = CertificateStoreType.Windows;
                id.StorePath = storePath;

                try
                {
                    // validate a windows store (just checks syntax).
                    id.OpenStore();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            // validate a directory (just checks syntax).
            Utils.GetAbsoluteDirectoryPath(storePath, false, false, false);
            return true;
        }

        /// <summary>
        /// Validates the configuration after it has been confirmed.
        /// </summary>
        private void ValidateConfirmedConfiguration()
        {
            if (String.IsNullOrEmpty(ApplicationNameTB.Text))
            {
                throw new ServiceResultException("An Application Name must be specified.");
            }

            if (!String.IsNullOrEmpty(ApplicationCertificateStoreTB.Text))
            {
                if (!ValidateCertificateStore(ApplicationCertificateStoreTB.Text))
                {
                    throw new ServiceResultException("The Application Certificate Store is not valid.");
                }
            }

            if (!String.IsNullOrEmpty(SubjectNameTB.Text))
            {
                List<string> names = Utils.ParseDistinguishedName(SubjectNameTB.Text);

                if (names.Count == 0)
                {
                    throw new ServiceResultException("The SubjectName contains no valid fields.");
                }

                StringBuilder buffer = new StringBuilder();

                foreach (string name in names)
                {
                    if (name.StartsWith("CN=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ServiceResultException("The SubjectName must not contain a CommonName (CN=).");
                    }

                    if (buffer.Length > 0)
                    {
                        buffer.Append('/');
                    }

                    buffer.Append(name);
                }

                SubjectNameTB.Text = buffer.ToString();
            }

            if (!String.IsNullOrEmpty(PublicKeyTB.Text))
            {
                if (!File.Exists(PublicKeyTB.Text))
                {
                    throw new ServiceResultException("The Public Key File Path is not valid.");
                }
            }

            if (!String.IsNullOrEmpty(PrivateKeyTB.Text))
            {
                if (!File.Exists(PrivateKeyTB.Text))
                {
                    throw new ServiceResultException("The Private Key File Path is not valid.");
                }
            }

            if (!String.IsNullOrEmpty(TrustListTB.Text))
            {
                if (!ValidateCertificateStore(TrustListTB.Text))
                {
                    throw new ServiceResultException("The Trusted List is not valid.");
                }
            }

            if (!String.IsNullOrEmpty(IssuerListTB.Text))
            {
                if (!ValidateCertificateStore(IssuerListTB.Text))
                {
                    throw new ServiceResultException("The Issuer List is not valid.");
                }
            }
        }

        private void SetConfiguration(ConfigurationType configurationType)
        {
            m_configurationType = configurationType;

            switch (configurationType)
            {
                case ConfigurationType.Xml: { SetXmlConfiguration(); break; }
                case ConfigurationType.ImportExport: { SetImportExportConfiguration(); break; }
                case ConfigurationType.Manual: { SetManualConfiguration(); break; }
            }
        }

        private void ValidateConfiguration()
        {
            switch (m_configurationType)
            {
                case ConfigurationType.Xml: { ValidateXmlConfiguration(); break; }
                case ConfigurationType.ImportExport: {  ValidateImportExportConfiguration(); break; }
            }
        }
        #endregion

        private void NextBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_stage == Stage.Configuration)
                {
                    ValidateConfiguration();
                }

                SetPermissionAndAccess(m_stage+1);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            try
            {
                SetPermissionAndAccess(m_stage-1);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void DoneBTN_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateConfirmedConfiguration();

                if (m_WizardComplete != null)
                {
                    m_WizardComplete(this, null);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void CancelBTN_Click(object sender, EventArgs e)
        {
            try
            {
                Finish(null);

                if (m_WizardCancelled != null)
                {
                    m_WizardCancelled(this, null);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void XmlConfigRB_CheckedChanged(object sender, EventArgs e)
        {
            if (XmlConfigRB.Checked)
            {
                m_configurationType = ConfigurationType.Xml;
                SetXmlConfiguration();
            }
        }

        private void ImportExportRB_CheckedChanged(object sender, EventArgs e)
        {
            if (ImportExportRB.Checked)
            {
                m_configurationType = ConfigurationType.ImportExport;
                SetImportExportConfiguration();
            }
        }

        private void ManualConfigRB_CheckedChanged(object sender, EventArgs e)
        {
            if (ManualConfigRB.Checked)
            {
                m_configurationType = ConfigurationType.Manual;
                SetManualConfiguration();
            }
        }

        private void ConfigurationFileBTN_FileSelected(object sender, EventArgs e)
        {
            try
            {
                string file = ConfigurationFileTB.Text;

                if (file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    file += ".config";
                }

                if (file.EndsWith(".config", StringComparison.OrdinalIgnoreCase))
                {
                    XElement root = XElement.Load(file);

                    IEnumerable<XElement> locations =
                        from element in root.Descendants(XName.Get("ConfigurationLocation", Opc.Ua.Namespaces.OpcUaConfig))
                        select element;

                    bool found = false;

                    foreach (XElement element in locations)
                    {
                        if (found)
                        {
                            throw new ArgumentException("The app.config file references multiple UA configuration files. Please select the exact configuration file that you would like to use.");
                        }

                        XElement child = element.Element(XName.Get("FilePath", Opc.Ua.Namespaces.OpcUaConfig));

                        if (child != null)
                        {
                            found = true;

                            using (XmlReader reader = child.CreateReader())
                            {
                                reader.MoveToContent();
                                string filePath = reader.ReadElementContentAsString();

                                FileInfo exePath = new FileInfo(file);
                                string currentDirectory = Environment.CurrentDirectory;

                                try
                                {
                                    Environment.CurrentDirectory = exePath.DirectoryName;
                                    file = Utils.GetAbsoluteFilePath(filePath, true, true, false);
                                    ConfigurationFileTB.Text = file;
                                }
                                finally
                                {
                                    Environment.CurrentDirectory = currentDirectory;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void PublicKeyBTN_FileSelected(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(PublicKeyTB.Text))
                {
                    X509Certificate2 certificate = new X509Certificate2(PublicKeyTB.Text);

                    if (String.IsNullOrEmpty(ApplicationUriTB.Text))
                    {
                        ApplicationUriTB.Text = Utils.GetApplicationUriFromCertficate(certificate);
                    }

                    List<string> fields = Utils.ParseDistinguishedName(certificate.Subject);

                    StringBuilder builder = new StringBuilder();

                    foreach (string field in fields)
                    {
                        if (field.StartsWith("CN="))
                        {
                            if (String.IsNullOrEmpty(ApplicationNameTB.Text))
                            {
                                ApplicationNameTB.Text = field.Substring(3);
                            }

                            continue;
                        }

                        if (field.StartsWith("DC="))
                        {
                            if (String.IsNullOrEmpty(MachineNameTB.Text))
                            {
                                MachineNameTB.Text = field.Substring(3);
                                continue;
                            }
                        }

                        if (builder.Length > 0)
                        {
                            builder.Append("/");
                        }

                        builder.Append(field);
                    }

                    if (String.IsNullOrEmpty(SubjectNameTB.Text))
                    {
                        SubjectNameTB.Text = builder.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
    }
}
