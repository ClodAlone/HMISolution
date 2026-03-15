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
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Net;
using System.IO;

using Opc.Ua.Configuration;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    /// <summary>
    /// Allows the use to manage the SSL certificate bindings.
    /// </summary>
    public partial class ManagePermissionsDlg : Form
    {
        #region Constructors
        /// <summary>
        /// Constructs the object.
        /// </summary>
        public ManagePermissionsDlg()
        {
            InitializeComponent();
            PortsDV.AutoGenerateColumns = false;
            PortsDV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataTable table = new DataTable();
            
            table.Columns.Add("Protocol", typeof(string));
            table.Columns.Add("Port", typeof(int));
            table.Columns.Add("Enabled", typeof(bool));
            table.Columns.Add("Url", typeof(string));

            DataSet.Tables.Add(table);
            PortsDV.DataSource = DataSet.Tables[0];
        }
        #endregion

        #region Private Fields
        AccessTemplateManager m_templateManager;
        GdsAgentApplication m_application;
        #endregion

        #region Public Interface
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(GdsAgentConfiguration configuration, GdsAgentApplication application)
        {
            m_templateManager = new AccessTemplateManager(configuration.AccessTemplateDirectory);
            m_application = application;

            PermissionTemplateCB.Items.Clear();
            PermissionTemplateCB.Items.Add("<do not change>");
            PermissionTemplateCB.Items.AddRange(m_templateManager.EnumerateTemplates());
            PermissionTemplateCB.SelectedIndex = 0;

            if (application != null)
            {
                ExecutablePathTB.Text = application.ExecutableFile;
                ExecutablePathBTN_FileSelected(this, null);
            }

            if (base.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        private void UpdateFirewallPermissions(string executablePath)
        {
            List<string> addressesToAdd = new List<string>();
            List<int> portsToRemove = new List<int>();

            foreach (DataRow row in DataSet.Tables[0].Rows)
            {
                if ((int)row[1] == -1)
                {
                    continue;
                }

                if (OpenFirewallPortsCK.Checked && (bool)row[2])
                {
                    addressesToAdd.Add((string)row[3]);
                }
                else
                {
                    portsToRemove.Add((int)row[1]);
                }
            }

            if (portsToRemove.Count > 0)
            {
                ConfigUtils.RemoveFirewallAccess(portsToRemove.ToArray());
            }

            if (OpenFirewallPortsCK.Checked)
            {
                if (addressesToAdd.Count > 0)
                {
                    ConfigUtils.SetFirewallAccess(m_application.ApplicationName, executablePath, addressesToAdd);
                }
            }
            else
            {
                ConfigUtils.RemoveFirewallAccess(executablePath, null);
            }
        }

        private void UpdatePermissions(string executablePath)
        {
            if (PermissionTemplateCB.SelectedIndex <= 0)
            {
                return;
            }

            List<string> httpAddresses = new List<string>();

            foreach (DataRow row in DataSet.Tables[0].Rows)
            {
                string url = ApplicationInstance.GetHttpUrlForAccessRule((string)row[3]);

                if (url != null)
                {
                    httpAddresses.Add(url);
                }
            }

            string role = (string)PermissionTemplateCB.SelectedItem;

            if (httpAddresses.Count > 0)
            {
                foreach (string httpAddress in httpAddresses)
                {
                    m_templateManager.SetPermissions(role, new Uri(httpAddress), false);
                }
            }

            m_templateManager.SetPermissions(role, new FileInfo(m_application.ConfigurationFile));
            m_templateManager.SetPermissions(role, new FileInfo(executablePath));

            if (m_application.SecuritySettings.ApplicationCertificate != null)
            {
                ICertificateStore store = m_application.SecuritySettings.ApplicationCertificate.OpenStore();

                try
                {
                    X509Certificate2 certificate = m_application.SecuritySettings.ApplicationCertificate.Find(false);
                    string filePath = store.GetPrivateKeyFilePath(certificate.Thumbprint);

                    if (filePath != null)
                    {
                        m_templateManager.SetPermissions(role, new FileInfo(filePath));
                    }
                }
                catch
                {
                    store.Close();
                }
            }
        }
        #endregion

        #region Event Handlers
        private void ExecutablePathBTN_FileSelected(object sender, EventArgs e)
        {
            try
            {
                int[] ports = ConfigUtils.GetFirewallAccess(ExecutablePathTB.Text);
                OpenFirewallPortsCK.Checked = ports != null;
                PortsDV.Enabled = OpenFirewallPortsCK.Checked;

                if (m_application.SecuritySettings.BaseAddresses != null)
                {
                    DataSet.Tables[0].Rows.Clear();

                    foreach (string baseAddress in m_application.SecuritySettings.BaseAddresses)
                    {
                        Uri url = Utils.ParseUri(baseAddress);

                        if (url != null)
                        {
                            DataRow row = DataSet.Tables[0].NewRow();

                            int port = url.Port;

                            if (port == -1)
                            {
                                switch (url.Scheme)
                                {
                                    case Utils.UriSchemeHttp: { port = 80; break; }
                                    case Utils.UriSchemeHttps: { port = 443; break; }
                                    case Utils.UriSchemeOpcTcp: { port = 4840; break; }
                                }
                            }

                            row[0] = url.Scheme;
                            row[1] = url.Port;
                            row[2] = false;
                            row[3] = baseAddress;

                            if (ports != null)
                            {
                                for (int jj = 0; jj < ports.Length; jj++)
                                {
                                    if (ports[jj] == url.Port)
                                    {
                                        row[2] = true;
                                    }
                                }
                            }

                            DataSet.Tables[0].Rows.Add(row);
                        }
                    }

                    DataSet.AcceptChanges();
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void OpenFirewallPortsCK_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                PortsDV.Enabled = OpenFirewallPortsCK.Checked;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void OkBTN_Click(object sender, EventArgs e)
        {
            try
            {
                string executablePath = Utils.GetAbsoluteFilePath(ExecutablePathTB.Text, false, false, false);

                if (executablePath == null)
                {
                    throw new ArgumentException("The executable path does not point to a valid file.");
                }

                UpdatePermissions(executablePath);
                UpdateFirewallPermissions(executablePath);

                m_application.ExecutableFile = executablePath;

                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
        #endregion
    }
}
