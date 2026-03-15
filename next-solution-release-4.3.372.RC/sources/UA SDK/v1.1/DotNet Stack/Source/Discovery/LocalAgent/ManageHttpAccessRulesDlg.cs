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
    /// Allows the use to manage the SSL certificate bindings.
    /// </summary>
    public partial class ManageHttpAccessRulesDlg : Form
    {
        #region Constructors
        /// <summary>
        /// Constructs the object.
        /// </summary>
        public ManageHttpAccessRulesDlg()
        {
            InitializeComponent();
            RulesDV.AutoGenerateColumns = false;
            RulesDV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            m_dataset = DataSet;

            DataTable table = new DataTable();
            
            table.Columns.Add("Url", typeof(string));
            table.Columns.Add("UserName", typeof(string));
            table.Columns.Add("RuleType", typeof(ApplicationAccessRight));
            table.Columns.Add("Rule", typeof(HttpAccessRule));

            m_dataset.Tables.Add(table);
        }
        #endregion

        #region Private Fields
        private DataSet m_dataset;
        #endregion

        #region Public Interface
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(string url)
        {
            IList<HttpAccessRule> accessRules = HttpAccessRule.GetAccessRules(url);

            if (accessRules != null && accessRules.Count > 0)
            {
                foreach (HttpAccessRule accessRule in accessRules)
                {
                    AddRow(accessRule);
                }
            }

            m_dataset.AcceptChanges();
            RulesDV.DataSource = m_dataset.Tables[0];
            RulesDV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            if (base.ShowDialog() == DialogResult.Cancel)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds a certificate binding to to list.
        /// </summary>
        private void AddRow(HttpAccessRule rule)
        {
            DataRow row = m_dataset.Tables[0].NewRow();

            row[0] = rule.UrlPrefix;
            row[1] = rule.IdentityName;
            row[2] = rule.Right;
            row[3] = rule;

            m_dataset.Tables[0].Rows.Add(row);
        }

        /// <summary>
        /// Asks a question.
        /// </summary>
        private bool Ask(string message)
        {
            DialogResult result = MessageBox.Show(
                message,
                this.Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }
        #endregion

        #region Event Handlers
        private void NewRuleMI_Click(object sender, EventArgs e)
        {
            try
            {
                HttpAccessRule template = null;

                foreach (DataGridViewRow row in RulesDV.SelectedRows)
                {
                    DataRowView source = row.DataBoundItem as DataRowView;
                    template = (HttpAccessRule)source.Row[3];
                    break;
                }

                HttpAccessRule rule = new CreateHttpAccessRuleDlg().ShowDialog(template);

                if (rule != null)
                {
                    List<HttpAccessRule> accessRules = new List<HttpAccessRule>();
                    accessRules.Add(rule);
                    HttpAccessRule.SetAccessRules(rule.UrlPrefix, accessRules, false);
                    AddRow(rule);
                    m_dataset.AcceptChanges();
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void DeleteRuleMI_Click(object sender, EventArgs e)
        {
            try
            {
                if (RulesDV.SelectedRows.Count <= 0)
                {
                    return;
                }

                if (Ask("Are you sure you want to delete these access rules?"))
                {
                    foreach (DataGridViewRow row in RulesDV.SelectedRows)
                    {
                        DataRowView source = row.DataBoundItem as DataRowView;
                        string url = (string)source.Row[0];

                        List<HttpAccessRule> accessRules = new List<HttpAccessRule>();

                        foreach (HttpAccessRule existingRule in HttpAccessRule.GetAccessRules(url))
                        {
                            if (existingRule.Right == (ApplicationAccessRight)source.Row[2])
                            {
                                if (existingRule.IdentityName == (string)source.Row[1])
                                {
                                    continue;
                                }
                            }

                            accessRules.Add(existingRule);
                        }

                        HttpAccessRule.SetAccessRules(url, accessRules, true);
                        source.Row.Delete();
                    }

                    m_dataset.AcceptChanges();
                   // RulesDV.DataSource = m_dataset.Tables[0];
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ManageHttpAccessRulesDlg_Shown(object sender, EventArgs e)
        {
            RulesDV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }
        #endregion
    }
}
