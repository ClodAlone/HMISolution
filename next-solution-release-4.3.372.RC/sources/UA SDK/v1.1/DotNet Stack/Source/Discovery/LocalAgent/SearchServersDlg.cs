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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    public partial class SearchServersDlg : Form
    {
        public SearchServersDlg()
        {
            InitializeComponent();

            List<object> items = new  List<object>();

            foreach (object value in Enum.GetValues(typeof(Match)))
            {
                items.Add(value);
            }

            ApplicationNameCB.Items.AddRange(items.ToArray());
            MachineNameCB.Items.AddRange(items.ToArray());
            ApplicationUriCB.Items.AddRange(items.ToArray());
            ProductUriCB.Items.AddRange(items.ToArray());

            ApplicationNameCB.SelectedIndex = 0;
            MachineNameCB.SelectedIndex = 0;
            ApplicationUriCB.SelectedIndex = 0;
            ProductUriCB.SelectedIndex = 0;
        }

        private enum Match
        {
            StartsWith,
            IsExactly,
            EndsWith,
            Contains
        }

        private Session m_session;

        /// <summary>
        /// Updates the application after connecting to or disconnecting from the server.
        /// </summary>
        public void Show(Session session, string caption)
        {
            try
            {
                if (!String.IsNullOrEmpty(caption))
                {
                    this.Text = caption;
                }

                m_session = session;

                OkBTN.Visible = false;
                CancelBTN.Visible = false;
                CloseBTN.Visible = true;

                SystemElementBTN.Session = m_session;
                SystemElementBTN.RootId = ExpandedNodeId.ToNodeId(Opc.Ua.Gds.ObjectIds.Directory_Applications, m_session.NamespaceUris);
                SystemElementBTN.ReferenceTypeIds = new NodeId[] { Opc.Ua.ReferenceTypeIds.Organizes };
                
                base.Show();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        /// <summary>
        /// Handles a session reconnect.
        /// </summary>
        public void Reconnect(Session session)
        {
            m_session = session;
            SystemElementBTN.Session = m_session;
        }

        /// <summary>
        /// Adds the results to the control.
        /// </summary>
        private void UpdateResults(ApplicationDescription[] descriptions)
        {
            ServersLV.Items.Clear();

            if (descriptions == null)
            {
                return;
            }

            for (int ii = 0; ii < descriptions.Length; ii++)
            {
                ApplicationDescription description = descriptions[ii];

                if (description == null)
                {
                    continue;
                }

                ListViewItem item = new ListViewItem();
                item.Text = Utils.Format("{0}", description.ApplicationName);
                item.SubItems.Add(new ListViewItem.ListViewSubItem());
                item.SubItems.Add(new ListViewItem.ListViewSubItem());
                item.Tag = description;
                ServersLV.Items.Add(item);

                if (description.DiscoveryUrls == null)
                {
                    continue;
                }
                
                // collect the domains and protocols.
                List<string> domains = new List<string>();
                List<string> protocols = new List<string>();

                foreach (string discoveryUrl in description.DiscoveryUrls)
                {
                    Uri url = Utils.ParseUri(discoveryUrl);

                    if (url != null)
                    {
                        if (!domains.Contains(url.DnsSafeHost))
                        {
                            domains.Add(url.DnsSafeHost);
                        }

                        if (!protocols.Contains(url.Scheme))
                        {
                            protocols.Add(url.Scheme);
                        }
                    }
                }

                // format the domains.
                StringBuilder buffer = new StringBuilder();

                foreach (string domain in domains)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    buffer.Append(domain);
                }

                item.SubItems[1].Text = buffer.ToString();

                // format the protocols.
                buffer = new StringBuilder();

                foreach (string protocol in protocols)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(", ");
                    }

                    buffer.Append(protocol);
                }

                item.SubItems[2].Text = buffer.ToString();
            }

            // adjust column widths.
            for (int ii = 0; ii < ServersLV.Columns.Count; ii++)
            {
                ServersLV.Columns[ii].Width = -2;
            }
        }

        /// <summary>
        /// Adds wildcards to the filter.
        /// </summary>
        private string ProcessFilter(ComboBox selection, TextBox filter)
        {
            if (String.IsNullOrEmpty(filter.Text))
            {
                return String.Empty;
            }
                    
            string text = filter.Text;
            Match match = (Match)selection.SelectedItem;

            if (match == Match.Contains || match == Match.StartsWith)
            {
                if (!text.EndsWith("%"))
                {
                    text = text + "%";
                }
            }

            if (match == Match.Contains || match == Match.EndsWith)
            {
                if (!text.StartsWith("%"))
                {
                    text = "%" + text;
                }
            }

            return text;
        }

        /// <summary>
        /// Searches the server for servers.
        /// </summary>
        private void Search()
        {
            NodeId elementId = null;            
            ReferenceDescription reference = SystemElementBTN.SelectedReference;

            if (reference != null && !reference.NodeId.IsAbsolute)
            {
                elementId = (NodeId)reference.NodeId;
            }

            ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

            IList<object> outputArguments = m_session.Call(
                new NodeId(Opc.Ua.Gds.Objects.Directory, namespaceIndex),
                new NodeId(Opc.Ua.Gds.Methods.RootDirectoryEntryType_QueryServers, namespaceIndex),
                elementId,
                ProcessFilter(ApplicationNameCB, ApplicationNameTB),
                ProcessFilter(MachineNameCB, MachineNameTB),
                ProcessFilter(ApplicationUriCB, ApplicationUriTB),
                ProcessFilter(ProductUriCB, ProductUriTB));

            if (outputArguments != null && outputArguments.Count == 1)
            {
                ExtensionObject[] extensions = outputArguments[0] as ExtensionObject[];
                ApplicationDescription[] descriptions = (ApplicationDescription[])ExtensionObject.ToArray(extensions, typeof(ApplicationDescription));
                UpdateResults(descriptions);
            }
        }

        private void BrowseServersDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SystemElementBTN.Session = null;;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void SearchBTN_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void CloseBTN_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void DetailsMI_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
    }
}
