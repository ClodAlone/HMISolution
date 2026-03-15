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
using System.Security.Cryptography.X509Certificates;
using System.IO;

using Opc.Ua.Client.Controls;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Prompts the user to edit a ApplicationDescription.
    /// </summary>
    public partial class CertificateListDlg : Form
    {
        public CertificateListDlg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public CertificateIdentifier ShowDialog(CertificateStoreIdentifier store, bool allowStoreChange)
        {
            CertificateStoreCTRL.StoreType = CertificateStoreType.Directory;
            CertificateStoreCTRL.StorePath = String.Empty;
            CertificateStoreCTRL.ReadOnly = !allowStoreChange;
            CertificatesCTRL.Initialize(null);
            OkBTN.Enabled = false;

            if (store != null)
            {
                CertificateStoreCTRL.StoreType = store.StoreType;
                CertificateStoreCTRL.StorePath = store.StorePath;
            }

            if (ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            CertificateIdentifier id = new CertificateIdentifier();
            id.StoreType = CertificateStoreCTRL.StoreType;
            id.StorePath = CertificateStoreCTRL.StorePath;
            id.Certificate = CertificatesCTRL.SelectedCertificate;
            return id;
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

        private void FilterBTN_Click(object sender, EventArgs e)
        {
            try
            {
                CertificateListFilter filter = new CertificateListFilter();
                filter.SubjectName = SubjectNameTB.Text.Trim();
                filter.IssuerName = IssuerNameTB.Text.Trim();
                filter.Domain = DomainTB.Text.Trim();
                filter.PrivateKey = PrivateKeyCK.Checked;

                List<CertificateListFilterType> types = new List<CertificateListFilterType>();

                if (ApplicationCK.Checked)
                {
                    types.Add(CertificateListFilterType.Application);
                }

                if (CaCK.Checked)
                {
                    types.Add(CertificateListFilterType.CA);
                }

                if (SelfSignedCK.Checked)
                {
                    types.Add(CertificateListFilterType.SelfSigned);
                }

                if (IssuedCK.Checked)
                {
                    types.Add(CertificateListFilterType.Issued);
                }

                if (types.Count > 0)
                {
                    filter.CertificateTypes = types.ToArray();
                }

                CertificatesCTRL.SetFilter(filter);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void CertificatesCTRL_ItemsSelected(object sender, ListItemActionEventArgs e)
        {
            try
            {
                OkBTN.Enabled = e.Items.Count == 1;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void CertificateStoreCTRL_StoreChanged(object sender, EventArgs e)
        {
            try
            {
                CertificateStoreIdentifier store = new CertificateStoreIdentifier();
                store.StoreType = CertificateStoreCTRL.StoreType;
                store.StorePath = CertificateStoreCTRL.StorePath;
                CertificatesCTRL.Initialize(store, null);

                if (!CertificatesCTRL.IsEmptyStore)
                {
                    Utils.UpdateRecentFileList("CertificateStores:" + store.StoreType, store.StorePath, 16);
                }

                FilterBTN_Click(sender, e);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
    }
}
