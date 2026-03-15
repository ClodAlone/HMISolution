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

using Opc.Ua.Client.Controls;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Prompts the user to edit a ApplicationDescription.
    /// </summary>
    public partial class CertificateDlg : Form
    {
        public CertificateDlg()
        {
            InitializeComponent();

            PrivateKeyCB.Items.Add("No");
            PrivateKeyCB.Items.Add("Yes");
            PrivateKeyCB.SelectedIndex = 0;
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(CertificateIdentifier certificateIdentifier)
        {
            CertificateStoreCTRL.StoreType = null;
            CertificateStoreCTRL.StorePath = null;
            PrivateKeyCB.SelectedIndex = 0;
            PropertiesCTRL.Initialize((X509Certificate2)null);

            if (certificateIdentifier != null)
            {
                X509Certificate2 certificate = certificateIdentifier.Find();

                CertificateStoreCTRL.StoreType = certificateIdentifier.StoreType;
                CertificateStoreCTRL.StorePath = certificateIdentifier.StorePath;

                if (certificate != null && certificateIdentifier.Find(true) != null)
                {
                    PrivateKeyCB.SelectedIndex = 1;
                }
                else
                {
                    PrivateKeyCB.SelectedIndex = 0;
                }

                PropertiesCTRL.Initialize(certificate);
            }

            if (ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(X509Certificate2 certificate)
        {
            CertificateStoreCTRL.StoreType = null;
            CertificateStoreCTRL.StorePath = null;
            PrivateKeyCB.SelectedIndex = 0;
            PropertiesCTRL.Initialize(certificate);

            if (ShowDialog() != DialogResult.OK)
            {
                return false;
            }
  
            return true;
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
    }
}
