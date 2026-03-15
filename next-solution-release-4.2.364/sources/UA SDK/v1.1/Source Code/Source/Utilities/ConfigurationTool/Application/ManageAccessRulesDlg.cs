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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

using Opc.Ua.Client.Controls;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Prompts the user to edit a ApplicationDescription.
    /// </summary>
    public partial class ManageAccessRulesDlg : Form
    {
        public ManageAccessRulesDlg()
        {
            InitializeComponent();
        }

        private string m_executablePath;
        private string m_configurationFile;
        private string m_privateKeyFilePath;
        private string m_trustListPath;
        private ManagedApplication m_application;

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(ManagedApplication application)
        {
            InstructionsTB.Text =
                 "It is not possible to set permissions on trust lists which are Windows certificate stores. " +
                 "It also may not be possible to permissions on private keys stored in a Windows certificate store on some machines. " +
                 "In these cases, the application should be configured to use directory stores.";

            ChangeApplicationPermissionBTN.Enabled = false;
            ChangeTrustListPermissionBTN.Enabled = false;

            Update(application);

            if (ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the dialog.
        /// </summary>
        private void Update(ManagedApplication application)
        {
            m_application = application;

            Dictionary<string, SecuredObjectAccessRights> read = new Dictionary<string, SecuredObjectAccessRights>();
            Dictionary<string, SecuredObjectAccessRights> write = new Dictionary<string, SecuredObjectAccessRights>();
            Dictionary<string, SecuredObjectAccessRights> configure = new Dictionary<string, SecuredObjectAccessRights>();

            m_executablePath = application.ExecutablePath;

            if (!String.IsNullOrEmpty(m_executablePath))
            {
                ChangeApplicationPermissionBTN.Enabled = true;
                GetAccountAccessRights(m_executablePath, SecuredObject.ExecutableFile, read, write, configure);

                string appConfigFile = m_executablePath + ".config";

                if (File.Exists(appConfigFile))
                {
                    GetAccountAccessRights(appConfigFile, SecuredObject.DotNetAppConfigFile, read, write, configure);
                }
            }

            m_configurationFile = application.ConfigurationPath;

            if (!String.IsNullOrEmpty(m_configurationFile))
            {
                ChangeApplicationPermissionBTN.Enabled = true;
                GetAccountAccessRights(m_configurationFile, SecuredObject.ConfigurationFile, read, write, configure);
            }

            if (application.Certificate != null)
            {
                m_privateKeyFilePath = application.Certificate.GetPrivateKeyFilePath();

                if (!String.IsNullOrEmpty(m_privateKeyFilePath))
                {
                    GetAccountAccessRights(m_privateKeyFilePath, SecuredObject.PrivateKey, read, write, configure);
                }
            }

            if (application.TrustList != null)
            {
                if (application.TrustList.StoreType == CertificateStoreType.Directory)
                {
                    m_trustListPath = Utils.GetAbsoluteDirectoryPath(application.TrustList.StorePath, true, false);

                    if (!String.IsNullOrEmpty(m_trustListPath))
                    {
                        ChangeTrustListPermissionBTN.Enabled = true;
                        GetAccountAccessRights(m_trustListPath, SecuredObject.TrustList, read, write, configure);
                    }
                }
            }

            ConfigureRightsCTRL.Initialize(configure.Values);
            WriteRightsCTRL.Initialize(write.Values);
            ReadRightsCTRL.Initialize(read.Values);
        }

        /// <summary>
        /// Gets the access rights granted to each account.
        /// </summary>
        private void GetAccountAccessRights(
            string path,
            SecuredObject objectToSecure,
            Dictionary<string, SecuredObjectAccessRights> read,
            Dictionary<string, SecuredObjectAccessRights> write,
            Dictionary<string, SecuredObjectAccessRights> configure)
        {
            AuthorizationRuleCollection authorizationRules = null;

            // determine if a file or directory.
            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                FileSystemSecurity security = fileInfo.GetAccessControl(AccessControlSections.Access);
                authorizationRules = security.GetAccessRules(true, true, typeof(NTAccount));
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (directoryInfo.Exists)
                {
                    FileSystemSecurity security = directoryInfo.GetAccessControl(AccessControlSections.Access);
                    authorizationRules = security.GetAccessRules(true, true, typeof(NTAccount));
                }
            }

            // check if no rules to add.
            if (authorizationRules == null || authorizationRules.Count == 0)
            {
                return;
            }

            // process the access rules.
            for (int ii = 0; ii < authorizationRules.Count; ii++)
            {
                // check for file system rule.
                FileSystemAccessRule accessRule = authorizationRules[ii] as FileSystemAccessRule;

                if (accessRule == null)
                {
                    continue;
                }

                // check the type of rule.
                bool denied = (accessRule.AccessControlType == System.Security.AccessControl.AccessControlType.Deny);

                // check for right to take ownership.
                if (!denied)
                {
                    if ((FileSystemRights.TakeOwnership & accessRule.FileSystemRights) != 0)
                    {
                        UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, configure);
                    }
                }

                // check if the rule affects configuration rights.
                if ((FileSystemRights.ChangePermissions & accessRule.FileSystemRights) != 0)
                {
                    UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, configure);
                }

                // check if the rule affects write rights.
                if ((FileSystemRights.WriteData & accessRule.FileSystemRights) != 0)
                {
                    UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, write);
                }

                // check if the rule affects read rights.
                if ((FileSystemRights.ReadData & accessRule.FileSystemRights) != 0)
                {
                    UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, read);
                }

                // check if the rule affects read rights.
                if (objectToSecure == SecuredObject.ExecutableFile)
                {
                    if ((FileSystemRights.ExecuteFile & accessRule.FileSystemRights) != 0)
                    {
                        UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, read);
                    }
                }
                else
                {
                    if ((FileSystemRights.ReadData & accessRule.FileSystemRights) != 0)
                    {
                        UpdateAccessRightSet(objectToSecure, accessRule.IdentityReference, denied, read);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the access right set with the permissions for the specified object.
        /// </summary>
        private void UpdateAccessRightSet(
            SecuredObject objectToSecure,
            IdentityReference identity,
            bool denied,
            Dictionary<string, SecuredObjectAccessRights> setToUpdate)
        {
            SecuredObjectAccessRights accountRights = null;

            if (!setToUpdate.TryGetValue(identity.Value, out accountRights))
            {
                accountRights = new SecuredObjectAccessRights();
                accountRights.Identity = identity;
                setToUpdate.Add(identity.Value, accountRights);
            }

            if (denied)
            {
                accountRights.DeniedObjects |= objectToSecure;
            }
            else
            {
                accountRights.AllowedObjects |= objectToSecure;
            }
        }

        private void OkBTN_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ChangeApplicationPermissionBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(m_configurationFile))
                {
                    return;
                }

                List<SecuredObject> objectTypes = new List<SecuredObject>();
                List<string> objectPaths = new List<string>();

                if (!String.IsNullOrEmpty(m_configurationFile))
                {
                    objectTypes.Add(SecuredObject.ConfigurationFile);
                    objectPaths.Add(m_configurationFile);
                }

                if (!String.IsNullOrEmpty(m_executablePath))
                {
                    objectTypes.Add(SecuredObject.ExecutableFile);
                    objectPaths.Add(m_executablePath);
                }

                if (!String.IsNullOrEmpty(m_privateKeyFilePath))
                {
                    objectTypes.Add(SecuredObject.PrivateKey);
                    objectPaths.Add(m_privateKeyFilePath);
                }

                new AccessRuleListDlg().ShowDialog(objectTypes, objectPaths);
                
                Update(m_application);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ChangeTrustListPermissionBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(m_configurationFile))
                {
                    return;
                }

                List<SecuredObject> objectTypes = new List<SecuredObject>();
                List<string> objectPaths = new List<string>();

                if (!String.IsNullOrEmpty(m_trustListPath))
                {
                    objectTypes.Add(SecuredObject.PrivateKey);
                    objectPaths.Add(m_privateKeyFilePath);
                }

                new AccessRuleListDlg().ShowDialog(objectTypes, objectPaths);

                Update(m_application);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
    }
}
