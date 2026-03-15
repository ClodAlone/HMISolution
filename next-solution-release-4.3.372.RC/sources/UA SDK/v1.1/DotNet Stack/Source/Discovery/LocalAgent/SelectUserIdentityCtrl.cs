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
using System.Windows.Forms;
using System.IO;
using System.Security.Principal;
using CubicOrange.Windows.Forms.ActiveDirectory;

namespace Opc.Ua.Client.Controls
{
    /// <summary>
    /// A control with button that displays an open file dialog.
    /// </summary>
    public partial class SelectUserIdentityCtrl : UserControl
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of the control.
        /// </summary>
        public SelectUserIdentityCtrl()
        {
            InitializeComponent();
        }
        #endregion
        
        #region Private Fields
        private IdentityReference m_identity;
        private event EventHandler m_IdentitySelected;
        #endregion
        
        #region Public Interface
        /// <summary>
        /// Gets the currently selected identity.
        /// </summary>
        public IdentityReference Identity 
        {
            get
            {
                return m_identity;
            }

            private set
            {
                if (IdentityControl != null)
                {
                    if (value != null)
                    {
                        IdentityControl.Text = value.ToString();
                    }
                    else
                    {
                        IdentityControl.Text = null;
                    }
                }

                m_identity = value;
            }
        }

        /// <summary>
        /// Gets or sets the control that is stores with the current file path.
        /// </summary>
        public Control IdentityControl { get; set; }

        /// <summary>
        /// Raised when a new identity is selected.
        /// </summary>
        public event EventHandler IdentitySelected
        {
            add { m_IdentitySelected += value; }
            remove { m_IdentitySelected -= value; }
        }
        #endregion

        #region Event Handlers
        private void BrowseBTN_Click(object sender, EventArgs e)
        {
            DirectoryObjectPickerDialog dialog = new DirectoryObjectPickerDialog();

            dialog.AllowedObjectTypes = CubicOrange.Windows.Forms.ActiveDirectory.ObjectTypes.Computers | CubicOrange.Windows.Forms.ActiveDirectory.ObjectTypes.BuiltInGroups | CubicOrange.Windows.Forms.ActiveDirectory.ObjectTypes.Groups | CubicOrange.Windows.Forms.ActiveDirectory.ObjectTypes.Users | CubicOrange.Windows.Forms.ActiveDirectory.ObjectTypes.WellKnownPrincipals;
            dialog.DefaultObjectTypes = dialog.AllowedObjectTypes;
            dialog.AllowedLocations = CubicOrange.Windows.Forms.ActiveDirectory.Locations.All;
            dialog.DefaultLocations = CubicOrange.Windows.Forms.ActiveDirectory.Locations.All;
            dialog.MultiSelect = false;
            dialog.TargetComputer = null;

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CubicOrange.Windows.Forms.ActiveDirectory.DirectoryObject[] results = dialog.SelectedObjects;

            if (results == null || results.Length != 1)
            {
                return;
            }

            if (!String.IsNullOrEmpty(results[0].Path))
            {
                string path = results[0].Path;
                string[] fields = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                string domain = fields[fields.Length - 2];
                string account = fields[fields.Length - 1];

                if (String.Compare(domain, System.Net.Dns.GetHostName(), StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    Identity = new NTAccount(account);
                }
                else
                {
                    Identity = new NTAccount(domain, account);
                }
            }
            else
            {
                Identity = new NTAccount(results[0].Name);
            }

            if (m_IdentitySelected != null)
            {
                m_IdentitySelected(this, new EventArgs());
            }
        }
        #endregion
    }
}
