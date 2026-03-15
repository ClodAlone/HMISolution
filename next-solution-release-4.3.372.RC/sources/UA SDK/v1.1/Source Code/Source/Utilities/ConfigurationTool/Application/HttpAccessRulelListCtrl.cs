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
using System.Reflection;
using System.Windows.Forms;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.Configuration
{
    public partial class HttpAccessRulelListCtrl : Opc.Ua.Client.Controls.BaseListCtrl
    {
        #region Constructors
        public HttpAccessRulelListCtrl()
        {
            InitializeComponent();

            SetColumns(m_ColumnNames);
        }
        #endregion
       
        #region Private Fields
		// The columns to display in the control.		
		private readonly object[][] m_ColumnNames = new object[][]
		{ 
			new object[] { "Url", HorizontalAlignment.Left, null },
			new object[] { "User", HorizontalAlignment.Left, null },
			new object[] { "Rule", HorizontalAlignment.Left, null }
		};
        #endregion

        #region Public Interface
        /// <summary>
        /// The currently selected certificate.
        /// </summary>
        public CertificateIdentifier SelectedCertificate
        {
            get
            {
                return SelectedTag as CertificateIdentifier;
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
        /// Displays the applications in the control.
        /// </summary>
        internal void Initialize()
        {
            ItemsLV.Items.Clear();

            try
            {
                IList<HttpAccessRule> accessRules = HttpAccessRule.GetAccessRules(null);

                if (accessRules == null || accessRules.Count == 0)
                {
                    Instructions = "No HTTP access rules defined.";
                    AdjustColumns();
                    return;
                }

                // display the list.
                foreach (HttpAccessRule accessRule in accessRules)
                {
                    AddItem(accessRule);
                }

                AdjustColumns();
            }
            catch (Exception e)
            {
                Instructions = e.Message;
                AdjustColumns();
            }
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
            HttpAccessRule value = item as HttpAccessRule;

            if (value == null)
            {
                base.UpdateItem(listItem, item);
                return;
            }

            listItem.SubItems[0].Text = value.UrlPrefix;
            listItem.SubItems[1].Text = value.IdentityName;
            listItem.SubItems[2].Text = value.Right.ToString();

            listItem.ImageKey = GuiUtils.Icons.Method;
            listItem.Tag = item;
        }

        /// <summary>
        /// Enables the menu items based on the current selection.
        /// </summary>
        protected override void EnableMenuItems(ListViewItem clickedItem)
        {
            base.EnableMenuItems(clickedItem);
            DeleteMI.Enabled = ItemsLV.SelectedItems.Count > 0;
        }
        #endregion

        #region Event Handlers
        private void ViewMI_Click(object sender, EventArgs e)
        {
            try
            {
                string value = SelectedTag as string;

                if (value == null)
                {
                    return;
                }

                value = new StringValueEditDlg().ShowDialog(value);

                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                value = new Uri(value).ToString();
                // UpdateItem(ItemsLV.SelectedItems[0], value);
                AdjustColumns();
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
                    "Are you sure you wish to delete the rules from the list?", 
                    "Delete Certificate",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                // remove the items.
                Dictionary<string, List<HttpAccessRule>> rulesToDelete = new Dictionary<string, List<HttpAccessRule>>();

                while (ItemsLV.SelectedItems.Count > 0)
                {
                    HttpAccessRule rule = ItemsLV.SelectedItems[0].Tag as HttpAccessRule;

                    List<HttpAccessRule> rules = null;

                    if (!rulesToDelete.TryGetValue(rule.UrlPrefix, out rules))
                    {
                        rulesToDelete[rule.UrlPrefix] = rules = new List<HttpAccessRule>();
                    }

                    rules.Add(rule);
                    ItemsLV.SelectedItems[0].Remove();
                }

                // delete rules.
                foreach (KeyValuePair<string, List<HttpAccessRule>> pair in rulesToDelete)
                {
                    IList<HttpAccessRule> existingRules = HttpAccessRule.GetAccessRules(pair.Key);
                    
                    List<HttpAccessRule> remainingRules = new List<HttpAccessRule>();

                    for (int ii = 0; ii < existingRules.Count; ii++)
                    {
                        HttpAccessRule existingRule = existingRules[ii];

                        bool found = false;

                        for (int jj = 0; jj < pair.Value.Count; jj++)
                        {
                            HttpAccessRule ruleToDelete = pair.Value[jj];

                            if (ruleToDelete.IdentityName == existingRule.IdentityName)
                            {
                                continue;
                            }

                            if (ruleToDelete.Right == existingRule.Right)
                            {
                                continue;
                            }

                            found = true;
                            break;
                        }

                        if (!found)
                        {
                            remainingRules.Add(existingRule);
                        }
                    }

                    HttpAccessRule.SetAccessRules(pair.Key, remainingRules, true);
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
