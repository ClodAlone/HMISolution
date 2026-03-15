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
using Opc.Ua.Com.Client;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Prompts the user to specify a new access rule for a file.
    /// </summary>
    public partial class EditComServerDlg : Form
    {
        #region Constructors
        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        public EditComServerDlg()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Fields
        private ComClientConfiguration m_configuration;
        #endregion
        
        #region Public Interface
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(ComClientConfiguration configuration)
        {
            m_configuration = configuration;

            if (configuration != null)
            {
                ServerTypeTB.Text = "DA";

                switch (configuration.GetType().Name)
                {
                    case "ComAeClientConfiguration": { ServerTypeTB.Text = "AE"; break; }
                    case "ComHdaClientConfiguration": { ServerTypeTB.Text = "HDA"; break; }
                }

                BrowseNameTB.Text = configuration.ServerName;
                SeperatorsTB.Text = configuration.SeperatorChars;

                int reconnectTime = configuration.MaxReconnectWait/1000;

                if (ReconnectTimeUD.Minimum <= reconnectTime && ReconnectTimeUD.Maximum >= reconnectTime)
                {
                    ReconnectTimeUD.Value = reconnectTime;
                }
                else
                {
                    ReconnectTimeUD.Value = ReconnectTimeUD.Maximum;
                }

                Uri url = Utils.ParseUri(configuration.ServerUrl);

                if (url != null)
                {
                    HostNameTB.Text = url.DnsSafeHost;

                    string[] parts = url.PathAndQuery.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 0)
                    {
                        ProgIdTB.Text = parts[0];
                    }

                    if (parts.Length > 1)
                    {
                        ClsidTB.Text = parts[1];
                    }
                }
            }

            if (ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Event Handlers
        private void OkBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // set the server name.
                m_configuration.ServerName = BrowseNameTB.Text;

                if (String.IsNullOrEmpty(m_configuration.ServerName))
                {
                    m_configuration.ServerName = ProgIdTB.Text;
                }

                if (String.IsNullOrEmpty(m_configuration.ServerName))
                {
                    m_configuration.ServerName = "COM Server";
                }

                // construct the server URL.
                StringBuilder buffer = new StringBuilder();
                buffer.Append("opc.com://");

                if (String.IsNullOrEmpty(HostNameTB.Text))
                {
                    buffer.Append("localhost");
                }
                else
                {
                    buffer.Append(HostNameTB.Text);
                }

                if (!String.IsNullOrEmpty(ProgIdTB.Text))
                {
                    buffer.Append("/");
                    buffer.Append(ProgIdTB.Text);
                }

                if (!String.IsNullOrEmpty(ClsidTB.Text))
                {
                    buffer.Append("/");
                    buffer.Append(ClsidTB.Text);
                }

                m_configuration.ServerUrl = buffer.ToString();
                m_configuration.SeperatorChars = SeperatorsTB.Text;
                m_configuration.MaxReconnectWait = (int)ReconnectTimeUD.Value*1000;
                
                // close the dialog.
                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, System.Reflection.MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
    }
}
