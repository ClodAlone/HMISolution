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
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;

namespace Opc.Ua.Configuration
{
    public partial class CertificateStoreCtrl : UserControl
    {
        #region Constructors
        public CertificateStoreCtrl()
        {
            InitializeComponent();

            StoreTypeCB.Items.Add(CertificateStoreType.Directory);
            StoreTypeCB.Items.Add(CertificateStoreType.Windows);
            StoreTypeCB.SelectedIndex = 0;
        }
        #endregion
       
        #region Private Fields
        private event EventHandler m_StoreChanged;
        #endregion

        public event EventHandler StoreChanged
        {
            add { m_StoreChanged += value; }
            remove { m_StoreChanged -= value; }
        }

        [DefaultValue(75)]
        public int LabelPosition
        {
            get
            {
                return LeftPN.Width;
            }

            set
            {
                LeftPN.Width = value;
            }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return !StoreTypeCB.Enabled;
            }

            set
            {
                StoreTypeCB.Enabled = !value;
                StorePathCB.Enabled = !value;
                BrowseBTN.Enabled = !value;
            }
        }

        [DefaultValue(Utils.DefaultStoreType)]
        public string StoreType
        {
            get 
            { 
                return StoreTypeCB.SelectedItem as string; 
            }

            set
            {
                if (value == null || StoreTypeCB.FindStringExact(value) == -1)
                {
                    StoreTypeCB.SelectedIndex = 0;
                    return;
                }

                StoreTypeCB.SelectedItem = value;
            }
        }


        [DefaultValue(Utils.DefaultStorePath)]
        public string StorePath
        {
            get
            {
                if (StorePathCB.SelectedItem == null)
                {
                    return StorePathCB.Text;
                }

                return StorePathCB.SelectedItem as string;
            }

            set
            {
                StorePathCB.SelectedIndex = -1;
                StorePathCB.Text = value;
            }
        }

        #region Private Methods
        private List<string> GetListOfStores(string storeType)
        {
            List<string> stores = Utils.GetRecentFileList("CertificateStores:" + storeType);

            if (CertificateStoreType.Directory == storeType)
            {
                stores.Add("%CommonApplicationData%\\OPC Foundation\\CertificateStores\\MachineDefault");
                stores.Add("%CommonApplicationData%\\OPC Foundation\\CertificateStores\\UA Applications");
                stores.Add("%CommonApplicationData%\\OPC Foundation\\CertificateStores\\UA Certificate Authorities");
                stores.Add("%CommonApplicationData%\\OPC Foundation\\RejectedCertificates");
            }

            if (CertificateStoreType.Windows == storeType)
            {
                stores.Add("LocalMachine\\My");
                stores.Add("LocalMachine\\UA Applications");
                stores.Add("LocalMachine\\UA Certificate Authorities");
            }

            return stores;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Populates the drop down with the recent file list.
        /// </summary>
        private void StorePathCB_DropDown(object sender, EventArgs e)
        {
            try
            {
                StorePathCB.Items.Clear();

                foreach (string storePath in GetListOfStores(StoreTypeCB.SelectedItem as string))
                {
                    // ignore duplicates.
                    bool found = false;

                    foreach (string item in StorePathCB.Items)
                    {
                        if (String.Compare(storePath, item, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            found = true;
                            break;
                        }
                    }

                    // add list.
                    if (!found)
                    {
                        StorePathCB.Items.Add(storePath);
                    }
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        /// <summary>
        /// Browses for new stores to manage.
        /// </summary>
        private void BrowseStoreBTN_Click(object sender, EventArgs e)
        {
            try
            {
                string storeType = StoreTypeCB.SelectedItem as string;
                string storePath = null;

                if (storeType == CertificateStoreType.Directory)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();

                    dialog.Description = "Select Certificate Store Directory";
                    dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    storePath = dialog.SelectedPath;
                }

                if (storeType == CertificateStoreType.Windows)
                {
                    CertificateStoreIdentifier store = new CertificateStoreTreeDlg().ShowDialog(null);

                    if (store == null)
                    {
                        return;
                    }

                    storePath = store.StorePath;
                }

                if (String.IsNullOrEmpty(storePath))
                {
                    return;
                }

                bool found = false;

                for (int ii = 0; ii < StorePathCB.Items.Count; ii++)
                {
                    if (String.Compare(storePath, StorePathCB.Items[ii] as string, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        StorePathCB.SelectedIndex = ii;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    StorePathCB.SelectedIndex = StorePathCB.Items.Add(storePath);
                }

                Utils.UpdateRecentFileList("CertificateStores:" + storeType, storePath, 16);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void StoreTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                StorePathCB_DropDown(sender, e);

                if (StorePathCB.Items.Count > 0)
                {
                    StorePathCB.SelectedIndex = 0;
                }

                if (m_StoreChanged != null)
                {
                    m_StoreChanged(null, e);
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void StorePathCB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_StoreChanged != null)
                {
                    m_StoreChanged(null, e);
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }

        }

        private void StorePathCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CertificateStoreIdentifier store = new CertificateStoreIdentifier();
                store.StoreType = StoreTypeCB.SelectedItem as string;
                store.StorePath = StorePathCB.Text;

                if (StorePathCB.SelectedIndex != -1)
                {
                    store.StorePath = StorePathCB.SelectedItem as string;
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
    }
}
