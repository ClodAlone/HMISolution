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
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;

using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;

namespace Opc.Ua.Configuration
{
    public partial class AccountAccessRightsListCtrl : Opc.Ua.Client.Controls.BaseListCtrl
    {
        #region Constructors
        /// <summary>
        /// Initalize the control.
        /// </summary>
        public AccountAccessRightsListCtrl()
        {
            InitializeComponent();
            SetColumns(m_ColumnNames);
            ItemsLV.MultiSelect = false;
        }
        #endregion
        
        #region Private Fields 
        // The columns to display in the control.		
		private readonly object[][] m_ColumnNames = new object[][]
		{ 
			new object[] { "Name",            HorizontalAlignment.Left, null },
			new object[] { "Type",            HorizontalAlignment.Center, null },
			new object[] { "Secured Objects", HorizontalAlignment.Left, null },
		};

        private class Item
        {
            public Item(SecuredObjectAccessRights accessRights, bool denied)
            {
                Identity = accessRights.Identity;
                Denied = denied;
                Rights = (denied) ? accessRights.DeniedObjects : accessRights.AllowedObjects;
            }

            public IdentityReference Identity;
            public bool Denied;
            public SecuredObject Rights;
        }
        #endregion
        
        #region Public Interface
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
        /// Displays the access rights.
        /// </summary>
        public void Initialize(IEnumerable<SecuredObjectAccessRights> accessRights)
        {
            ItemsLV.Items.Clear();
            Instructions = "No access rights are granted.";

            if (accessRights != null)
            {
                foreach (SecuredObjectAccessRights accessRight in accessRights)
                {
                    if (accessRight.AllowedObjects != 0)
                    {
                        AddItem(new Item(accessRight, false));
                    }

                    if (accessRight.DeniedObjects != 0)
                    {
                        AddItem(new Item(accessRight, true));
                    }
                }
            }

            AdjustColumns();
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Updates an item in the control.
        /// </summary>
        protected override void UpdateItem(ListViewItem listItem, object item)
        {
            Item accessRight = item as Item;

            if (accessRight == null)
            {
                base.UpdateItem(listItem, item);
                return;
            }

            listItem.SubItems[0].Text = accessRight.Identity.Value;
            listItem.SubItems[1].Text = (accessRight.Denied) ? "Deny" : "Allow";

            StringBuilder buffer = new StringBuilder();

            foreach (SecuredObject securedObject in Enum.GetValues(typeof(SecuredObject)))
            {
                if ((securedObject & accessRight.Rights) == securedObject)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append(',');
                    }

                    buffer.Append(securedObject);
                }
            }

            listItem.SubItems[2].Text = buffer.ToString();

            listItem.ImageKey = (!accessRight.Denied) ? GuiUtils.Icons.Users : GuiUtils.Icons.UsersRedCross;
            listItem.Tag = item;
        }
        #endregion
        
        #region Event Handlers
        #endregion
    }
}
