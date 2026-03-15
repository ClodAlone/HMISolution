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
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Net;

using Opc.Ua.Configuration;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    /// <summary>
    /// Prompts the user to edit a ApplicationDescription.
    /// </summary>
    public partial class CreateHttpAccessRuleDlg : Form
    {
        public CreateHttpAccessRuleDlg()
        {
            InitializeComponent();

            RuleTypeCB.Items.Add(ApplicationAccessRight.Run);
            RuleTypeCB.Items.Add(ApplicationAccessRight.Update);
            RuleTypeCB.Items.Add(ApplicationAccessRight.Configure);
            RuleTypeCB.SelectedIndex = 0;
        }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public HttpAccessRule ShowDialog(HttpAccessRule rule)
        {
            if (rule != null)
            {
                UrlTB.Text = rule.UrlPrefix;
                IdentityNameTB.Text = rule.IdentityName;
                RuleTypeCB.SelectedItem = rule;
            }

            if (base.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            HttpAccessRule result = new HttpAccessRule();
            result.UrlPrefix = UrlTB.Text.Trim();
            result.IdentityName = IdentityNameTB.Text.Trim();
            result.Right = (ApplicationAccessRight)RuleTypeCB.SelectedItem;
            return result;
        }

        private bool Ask(string message)
        {
            DialogResult result = MessageBox.Show(
                message,
                this.Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }

        private void OkBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(UrlTB.Text))
                {
                    throw new ArgumentException("A URL must be provided.");
                }

                if (String.IsNullOrEmpty(IdentityNameTB.Text))
                {
                    throw new ArgumentException("A user name, group name or account SID must be provided.");
                }

                if (IdentityNameTB.Text.StartsWith("S-"))
                {
                    if (ApplicationAccessRule.SidToAccountName(IdentityNameTB.Text.Trim()) == null)
                    {
                        throw new ArgumentException("The SID does not represent a valid Windows Account.");
                    }
                }
                else
                {
                    if (ApplicationAccessRule.AccountNameToSid(IdentityNameTB.Text.Trim()) == null)
                    {
                        throw new ArgumentException("The user or group name does not a valid Windows Account.");
                    }
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
    }
}
