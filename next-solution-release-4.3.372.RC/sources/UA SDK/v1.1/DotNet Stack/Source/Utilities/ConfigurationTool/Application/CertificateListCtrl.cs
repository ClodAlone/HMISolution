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
    public partial class CertificateListCtrl : Opc.Ua.Client.Controls.BaseListCtrl
    {
        #region Constructors
        public CertificateListCtrl()
        {
            InitializeComponent();

            SetColumns(m_ColumnNames);
        }
        #endregion
       
        #region Private Fields
		// The columns to display in the control.		
		private readonly object[][] m_ColumnNames = new object[][]
		{ 
			new object[] { "Name",        HorizontalAlignment.Left, null },
			new object[] { "Type",        HorizontalAlignment.Left, null },
			new object[] { "Private Key", HorizontalAlignment.Center, null },
			new object[] { "Domains",     HorizontalAlignment.Left, null },  
			new object[] { "Uri ",        HorizontalAlignment.Left, null },  
			new object[] { "Valid Until", HorizontalAlignment.Left, null }
		};

        private CertificateStoreIdentifier m_storeId;
        private CertificateIdentifierCollection m_certificates;
        private IList<string> m_thumbprints;
        private List<ListViewItem> m_items;
        #endregion

        #region Public Interface
        /// <summary>
        /// The currently selected certificate.
        /// </summary>
        public X509Certificate2 SelectedCertificate
        {
            get
            {
                return SelectedTag as X509Certificate2;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty store.
        /// </summary>
        public bool IsEmptyStore
        {
            get
            {
                if (m_items == null || m_items.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Removes all items in the list.
        /// </summary>
        internal void Clear()
        {
            ItemsLV.Items.Clear();
            Instructions = String.Empty;
            AdjustColumns();            
        }

        /// <summary>
        /// Sets the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        internal void SetFilter(CertificateListFilter filter)
        {
            if (m_items == null || m_items.Count == 0)
            {
                return;
            }

            if (ItemsLV.View == View.List)
            {
                ItemsLV.Items.Clear();
                ItemsLV.View = View.Details;
            }

            for (int ii = 0; ii < m_items.Count; ii++)
            {
                ListViewItem item = m_items[ii];

                X509Certificate2 certificate = item.Tag as X509Certificate2;
                
                if (certificate == null)
                {
                    continue;
                }

                if (item.ListView != null)
                {
                    if (!filter.Match(certificate))
                    {
                        item.Remove();
                    }
                }
                else
                {
                    if (filter.Match(certificate))
                    {
                        ItemsLV.Items.Add(item);
                    }
                }
            }

            if (ItemsLV.Items.Count == 0)
            {
                Instructions = "No certificates meet the current filter criteria.";
                AdjustColumns();
                return;
            }
        }

        /// <summary>
        /// Displays the applications in the control.
        /// </summary>
        internal void Initialize(CertificateIdentifierCollection certificates)
        {
            ItemsLV.Items.Clear();

            m_certificates = certificates;

            if (m_certificates == null || m_certificates.Count == 0)
            {
                Instructions = "No certificates are in the store.";
                AdjustColumns();
                return;
            }

            // display the list.
            foreach (CertificateIdentifier certificate in certificates)
            {
                AddItem(certificate);
            }

            // save the unfiltered list.
            m_items = new List<ListViewItem>(ItemsLV.Items.Count);
            
            foreach (ListViewItem item in ItemsLV.Items)
            {
                m_items.Add(item);
            }

            AdjustColumns();
        }

        /// <summary>
        /// Displays the applications in the control.
        /// </summary>
        internal void Initialize(CertificateStoreIdentifier id, IList<string> thumbprints)
        {
            ItemsLV.Items.Clear();

            m_storeId = id;
            m_thumbprints = thumbprints;

            if (m_storeId == null || String.IsNullOrEmpty(m_storeId.StoreType) || String.IsNullOrEmpty(m_storeId.StorePath))
            {
                Instructions = "No certificates are in the store.";
                AdjustColumns();
                return;
            }

            try
            {
                // get the store.
                using (ICertificateStore store = m_storeId.OpenStore())
                {
                    // only show certificates with the specified thumbprint.
                    if (thumbprints != null)
                    {
                        Instructions = "None of the selected certificates can be found in the store.";

                        foreach (string thumbprint in thumbprints)
                        {
                            X509Certificate2 certificate = store.FindByThumbprint(thumbprint);

                            if (certificate != null)
                            {
                                AddItem(certificate);
                            }
                        }
                    }

                    // show all certificates.
                    else
                    {
                        Instructions = "No certificates are in the store.";

                        foreach (X509Certificate2 certificate in store.Enumerate())
                        {
                            AddItem(certificate);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Instructions = "An error occurred opening the store: " + e.Message;
            }
            
            // save the unfiltered list.
            m_items = new List<ListViewItem>(ItemsLV.Items.Count);

            foreach (ListViewItem item in ItemsLV.Items)
            {
                m_items.Add(item);
            }

            AdjustColumns();

        }
        #endregion
        
        #region Overridden Methods
        /// <summary>
        /// Handles a double click event.
        /// </summary>
        protected override void PickItems()
        {
            base.PickItems();
            ViewMI_Click(this, null);
        }

        /// <summary>
        /// Updates an item in the view.
        /// </summary>
        protected override void UpdateItem(ListViewItem listItem, object item)
        {
            X509Certificate2 certificate = item as X509Certificate2;

            if (certificate == null)
            {
                base.UpdateItem(listItem, item);
                return;
            }

			listItem.SubItems[0].Text = null;
            listItem.SubItems[1].Text = null;
            listItem.SubItems[2].Text = null;
            listItem.SubItems[3].Text = null;
            listItem.SubItems[4].Text = null;
            listItem.SubItems[5].Text = null;

            if (certificate != null)
            {
                List<string> fields = Utils.ParseDistinguishedName(certificate.Subject);

                for (int ii = 0; ii < fields.Count; ii++)
                {
                    if (fields[ii].StartsWith("CN="))
                    {
                        listItem.SubItems[0].Text = fields[ii].Substring(3);
                    }

                    if (fields[ii].StartsWith("DC="))
                    {
                        listItem.SubItems[1].Text = fields[ii].Substring(3);
                    }
                }

                if (String.IsNullOrEmpty(listItem.SubItems[0].Text))
                {
                    listItem.SubItems[0].Text = String.Format("{0}", certificate.Subject);
                }

                // determine certificate type.
                foreach (X509Extension extension in certificate.Extensions)
                {
                    X509BasicConstraintsExtension basicContraints = extension as X509BasicConstraintsExtension;

                    if (basicContraints != null)
                    {
                        if (basicContraints.CertificateAuthority)
                        {
                            listItem.SubItems[1].Text = "CA";
                        }
                        else
                        {
                            listItem.SubItems[1].Text = "End-Entity";
                        }

                        break;
                    }
                }

                // check if a private key is available.
                if (certificate.HasPrivateKey)
                {
                    listItem.SubItems[2].Text = "Yes";
                }
                else
                {
                    listItem.SubItems[2].Text = "No";

                    if (m_storeId != null)
                    {
                        ICertificateStore store = m_storeId.OpenStore();

                        try
                        {
                            if (store.GetPrivateKeyFilePath(certificate.Thumbprint) != null)
                            {
                                listItem.SubItems[2].Text = "Yes (No Access)";
                            }
                        }
                        catch (Exception e)
                        {
                            listItem.SubItems[2].Text = e.Message;
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                }

                // look up domains.
                IList<string> domains = Utils.GetDomainsFromCertficate(certificate);

                StringBuilder buffer = new StringBuilder();

                for (int ii = 0; ii < domains.Count; ii++)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(";");
                    }

                    buffer.Append(domains[ii]);
                }

                listItem.SubItems[3].Text = buffer.ToString();
                listItem.SubItems[4].Text = Utils.GetApplicationUriFromCertficate(certificate);
                listItem.SubItems[5].Text = String.Format("{0:yyyy-MM-dd}", certificate.NotAfter);
            }

            listItem.ImageKey = GuiUtils.Icons.Certificate;
            listItem.Tag = item;
        }

        /// <summary>
        /// Enables the menu items based on the current selection.
        /// </summary>
        protected override void EnableMenuItems(ListViewItem clickedItem)
        {
            base.EnableMenuItems(clickedItem);

            DeleteMI.Enabled = ItemsLV.SelectedItems.Count > 0;

            X509Certificate2 certificate = SelectedTag as X509Certificate2;

            if (certificate != null)
            {
                ViewMI.Enabled = true;
                CopyMI.Enabled = true;
            }

            IDataObject clipboardData = Clipboard.GetDataObject();
         
            if (clipboardData.GetDataPresent(DataFormats.Text)) 
            {
                PasteMI.Enabled = true;
            }
        }
        #endregion

        private void ViewMI_Click(object sender, EventArgs e)
        {
            try
			{                
                X509Certificate2 certificate = SelectedTag as X509Certificate2;

                if (certificate != null)
                {
                    CertificateIdentifier id = new CertificateIdentifier();
                    id.Certificate = certificate;

                    if (m_storeId != null)
                    {
                        id.StoreType = m_storeId.StoreType;
                        id.StorePath = m_storeId.StorePath;
                    }

                    new ViewCertificateDlg().ShowDialog(id);
                }
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void DeleteMI_Click(object sender, EventArgs e)
        {
            try
			{            
                if (ItemsLV.SelectedItems.Count < 1)
                {
                    return;
                }    

                DialogResult result = MessageBox.Show(
                    "Are you sure you wish to delete the certificates from the store?", 
                    "Delete Certificate",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                // remove the certificates.
                List<ListViewItem> itemsToDelete = new List<ListViewItem>();

                using (ICertificateStore store = m_storeId.OpenStore())
                {
                    for (int ii = 0; ii < ItemsLV.SelectedItems.Count; ii++)
                    {
                        X509Certificate2 certificate = ItemsLV.SelectedItems[ii].Tag as X509Certificate2;

                        // check for private key.
                        X509Certificate2 certificate2 = store.FindByThumbprint(certificate.Thumbprint);

                        if (certificate2.HasPrivateKey)
                        {
                            StringBuilder buffer = new StringBuilder();
                            buffer.Append("Certificate '");
                            buffer.Append(certificate2.Subject);
                            buffer.Append("'");
                            buffer.Append("Deleting it may cause applications to stop working.");
                            buffer.Append("\r\n");
                            buffer.Append("\r\n");
                            buffer.Append("Are you sure you wish to continue?.");

                            if (new YesNoDlg().ShowDialog(buffer.ToString(), "Delete Private Key") != DialogResult.Yes)
                            {
                                continue;
                            }
                        }

                        if (certificate != null)
                        {
                            store.Delete(certificate.Thumbprint);
                            itemsToDelete.Add(ItemsLV.SelectedItems[ii]);
                        }
                    }
                }

                // remove the items.
                foreach (ListViewItem itemToDelete in itemsToDelete)
                {
                    itemToDelete.Remove();
                }
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
                Initialize(m_storeId, m_thumbprints);
            }
        }

        private void CopyMI_Click(object sender, EventArgs e)
        {
            try
			{                
                X509Certificate2 certificate = SelectedTag as X509Certificate2;

                if (certificate == null)
                {
                    return;
                }

                StringBuilder builder = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(builder);

                try
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CertificateIdentifier));
                    CertificateIdentifier id = new CertificateIdentifier();
                    id.Certificate = certificate;
                    serializer.WriteObject(writer, id);
                }
                finally
                {
                    writer.Close();
                }

                ClipboardHack.SetData(DataFormats.Text, builder.ToString());
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void PasteMI_Click(object sender, EventArgs e)
        {
            try
			{ 
                string xml = (string)ClipboardHack.GetData(DataFormats.Text);

                if (String.IsNullOrEmpty(xml))
                {
                    return;
                }
                    
                // deserialize the data.
                CertificateIdentifier id = null;

                using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CertificateIdentifier));                    
                    id = (CertificateIdentifier)serializer.ReadObject(reader, false);
                }
                
                if (id.Certificate != null)
                {
                    using (ICertificateStore store = m_storeId.OpenStore())
                    {
                        store.Add(id.Certificate);
                    }

                    AddItem(id.Certificate);
                }
            }
            catch (Exception exception)
            {
				GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
    }
}
