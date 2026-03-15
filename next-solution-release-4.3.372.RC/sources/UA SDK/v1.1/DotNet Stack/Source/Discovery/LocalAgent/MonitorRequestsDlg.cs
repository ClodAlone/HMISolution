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
    public partial class MonitorRequestsDlg : Form
    {
        public MonitorRequestsDlg()
        {
            InitializeComponent();
        }

        private Session m_session;

        /// <summary>
        /// Updates the application after connecting to or disconnecting from the server.
        /// </summary>
        public void Show(Session session)
        {
            try
            {
                base.Show();

                m_session = session;

                EventsCTRL.IsSubscribed = false;
                EventsCTRL.DisplayConditions = true;
                EventsCTRL.ChangeArea(Opc.Ua.ObjectIds.Server, false);

                FilterDeclaration filter = new FilterDeclaration();

                ushort namespaceIndex = (ushort)m_session.NamespaceUris.GetIndex(Namespaces.OpcUaGds);

                filter.EventTypeId = ExpandedNodeId.ToNodeId(Opc.Ua.Gds.ObjectTypeIds.CertificateRequestType, m_session.NamespaceUris);
                filter.AddSimpleField(Opc.Ua.BrowseNames.EventId, BuiltInType.ByteString, false);
                filter.AddSimpleField(Opc.Ua.BrowseNames.EventType, BuiltInType.NodeId, false);
                filter.AddSimpleField(Opc.Ua.BrowseNames.ConditionName, BuiltInType.String, true);
                filter.AddSimpleField(Opc.Ua.BrowseNames.DialogState, BuiltInType.LocalizedText, true);
                filter.AddSimpleField(Opc.Ua.BrowseNames.Prompt, BuiltInType.LocalizedText, false);
                filter.AddSimpleField(Opc.Ua.BrowseNames.ResponseOptionSet, BuiltInType.LocalizedText, ValueRanks.OneDimension, false);
                filter.AddSimpleField(Opc.Ua.BrowseNames.OkResponse, BuiltInType.Int32, false);
                filter.AddSimpleField(Opc.Ua.BrowseNames.CancelResponse, BuiltInType.Int32, false);
                filter.AddSimpleField(new QualifiedName(Opc.Ua.Gds.BrowseNames.ApplicationType, namespaceIndex), BuiltInType.String, true);
                filter.AddSimpleField(new QualifiedName(Opc.Ua.Gds.BrowseNames.ApplicationUri, namespaceIndex), BuiltInType.String, true);
                filter.AddSimpleField(new QualifiedName(Opc.Ua.Gds.BrowseNames.IsHttpsCertificate, namespaceIndex), BuiltInType.Boolean, true);
                filter.AddSimpleField(new QualifiedName(Opc.Ua.Gds.BrowseNames.DomainNames, namespaceIndex), BuiltInType.String, ValueRanks.OneDimension, true);

                EventsCTRL.ChangeFilter(filter, true);

                EventsCTRL.IsSubscribed = true;
                EventsCTRL.ChangeSession(m_session, false);
                EventsCTRL.ConditionRefresh();
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
            EventsCTRL.SessionReconnected(m_session);
            EventsCTRL.ConditionRefresh();
        }

        private void MonitorRequestsDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                EventsCTRL.ChangeSession(null, false);
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
                VariantCollection fields = EventsCTRL.GetSelectedEvent(0);

                if (fields != null)
                {
                    new ViewEventDetailsDlg().ShowDialog(EventsCTRL.Filter, fields);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void PopupMenu_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                while (PopupMenu.Items.Count > 2)
                {
                    PopupMenu.Items.RemoveAt(PopupMenu.Items.Count - 1);
                }

                PopupMenu.Items[1].Visible = false;

                VariantCollection fields = EventsCTRL.GetSelectedEvent(0);

                if (fields != null)
                {
                    LocalizedText[] options = EventsCTRL.Filter.GetValue<LocalizedText[]>(Opc.Ua.BrowseNames.ResponseOptionSet, fields, null);

                    if (options != null)
                    {
                        PopupMenu.Items[1].Visible = true;

                        for (int ii = 0; ii < options.Length; ii++)
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem(options[ii].ToString());
                            item.Click += new EventHandler(Respond_Click);
                            item.Tag = ii;
                            PopupMenu.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        void Respond_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                VariantCollection fields = EventsCTRL.GetSelectedEvent(0);
                CallMethodRequestCollection methodsToCall = new CallMethodRequestCollection();

                for (int ii = 0; fields != null; ii++)
                {
                    CallMethodRequest request = new CallMethodRequest();
                    request.ObjectId = fields[0].Value as NodeId;
                    request.MethodId = Opc.Ua.Methods.DialogConditionType_Respond;
                    request.InputArguments.Add(new Variant(item.Tag));
                    request.Handle = fields;
                    methodsToCall.Add(request);

                    fields = EventsCTRL.GetSelectedEvent(ii+1);
                }

                CallMethodResultCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                ResponseHeader responseHeader = m_session.Call(
                    null,
                    methodsToCall,
                    out results,
                    out diagnosticInfos);

                Session.ValidateResponse(results, methodsToCall);
                Session.ValidateDiagnosticInfos(diagnosticInfos, methodsToCall);

                StringBuilder buffer = new StringBuilder();

                for (int ii = 0; ii < methodsToCall.Count; ii++)
                {
                    if (StatusCode.IsBad(results[ii].StatusCode))
                    {
                        ServiceResult error = Session.GetResult(results[ii].StatusCode, ii, diagnosticInfos, responseHeader);

                        if (buffer.Length > 0)
                        {
                            buffer.Append("\r\n");
                        }
                        
                        fields = methodsToCall[ii].Handle as VariantCollection;
                        buffer.Append(EventsCTRL.Filter.GetValue<string>(Opc.Ua.BrowseNames.ConditionName, fields, "<unknown>"));
                        buffer.Append(": ");
                        buffer.Append(error);
                    }
                }

                if (buffer.Length > 0)
                {
                    MessageBox.Show(buffer.ToString(), "Error Responding to Dialog Conditions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
    }
}
