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
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;

namespace Opc.Ua.GdsLocalAgent
{
    public partial class GdsAgentApplicationsListCtrl : UserControl
    {
        public GdsAgentApplicationsListCtrl()
        {
            InitializeComponent();
        }

        public void Initialize(IList<GdsAgentApplication> applications)
        {
            foreach (GdsAgentApplication application in applications)
            {
                if (application != null)
                {
                    AddItem(application);
                }
            }

            foreach (ColumnHeader header in ApplicationsLV.Columns)
            {
                header.Width = -2;
            }
        }

        public GdsAgentApplication GetSelectedApplication(int index)
        {
            if (ApplicationsLV.SelectedItems.Count > index)
            {
                return ApplicationsLV.SelectedItems[index].Tag as GdsAgentApplication;
            }

            return null;
        }

        public bool Remove(GdsAgentApplication application)
        {
            foreach (ListViewItem item in ApplicationsLV.Items)
            {
                if (Object.ReferenceEquals(item.Tag, application))
                {
                    item.Remove();
                    return true;
                }
            }

            return false;
        }

        public void AddOrUpdate(GdsAgentApplication application)
        {
            if (application != null)
            {
                ListViewItem existingItem = null;

                foreach (ListViewItem item in ApplicationsLV.Items)
                {
                    if (Object.ReferenceEquals(item.Tag, application))
                    {
                        existingItem = item;
                        break;
                    }
                }

                if (existingItem == null)
                {
                    AddItem(application);
                }
                else
                {
                    UpdateItem(existingItem, application);
                }
            }

            foreach (ColumnHeader header in ApplicationsLV.Columns)
            {
                header.Width = -2;
            }
        }

        private void AddItem(GdsAgentApplication application)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems.Add(String.Empty);
            item.SubItems.Add(String.Empty);
            item.SubItems.Add(String.Empty);
            item.SubItems.Add(String.Empty);

            UpdateItem(item, application);
            ApplicationsLV.Items.Add(item);     
        }

        private void UpdateItem(ListViewItem item, GdsAgentApplication application)
        {
            item.SubItems[0].Text = application.ApplicationName;
            item.SubItems[1].Text = application.RegisteredWithGds.ToString();
            item.SubItems[3].Text = application.ConfigurationType.ToString();
            item.SubItems[4].Text = "None";

            switch (application.SecuritySettings.ApplicationType)
            {
                case Opc.Ua.Security.ApplicationType.Server_0: { item.SubItems[2].Text = "Server"; break; }
                case Opc.Ua.Security.ApplicationType.Client_1: { item.SubItems[2].Text = "Client"; break; }
                case Opc.Ua.Security.ApplicationType.ClientAndServer_2: { item.SubItems[2].Text = "Client and Server"; break; }
                case Opc.Ua.Security.ApplicationType.DiscoveryServer_3: { item.SubItems[2].Text = "Discovery Server"; break; }
            }

            // determine the type of certificate.
            X509Certificate2 certificate = null;

            try
            {
                if (application.SecuritySettings.ApplicationCertificate != null)
                {
                    certificate = application.SecuritySettings.ApplicationCertificate.Find(false);
                }

                else if (!String.IsNullOrEmpty(application.PublicKeyFilePath))
                {
                    string filePath = Utils.GetAbsoluteFilePath(application.PublicKeyFilePath, true, false, false);

                    if (filePath != null)
                    {
                        certificate = new X509Certificate2(filePath);
                    }
                }

                if (certificate != null)
                {
                    if (Utils.CompareDistinguishedName(certificate.Subject, certificate.Issuer))
                    {
                        item.SubItems[4].Text = "Self-Signed";
                    }
                    else
                    {
                        item.SubItems[4].Text = "Issued";
                    }
                }
            }
            catch (Exception)
            {
                item.SubItems[4].Text = "Error";
            }

            item.Tag = application;
        }
    }
}
