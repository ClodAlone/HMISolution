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

namespace Opc.Ua.GdsLocalAgent
{
    partial class ManageHttpAccessRulesDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ButtonsPN = new System.Windows.Forms.Panel();
            this.DeleteBTN = new System.Windows.Forms.Button();
            this.NewBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.MainPN = new System.Windows.Forms.Panel();
            this.RulesDV = new System.Windows.Forms.DataGridView();
            this.UrlCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserNameCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RuleTypeCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DataSet = new System.Data.DataSet();
            this.ButtonsPN.SuspendLayout();
            this.MainPN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RulesDV)).BeginInit();
            this.PopupMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.Controls.Add(this.DeleteBTN);
            this.ButtonsPN.Controls.Add(this.NewBTN);
            this.ButtonsPN.Controls.Add(this.CancelBTN);
            this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPN.Location = new System.Drawing.Point(0, 306);
            this.ButtonsPN.Name = "ButtonsPN";
            this.ButtonsPN.Size = new System.Drawing.Size(833, 31);
            this.ButtonsPN.TabIndex = 0;
            // 
            // DeleteBTN
            // 
            this.DeleteBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeleteBTN.Location = new System.Drawing.Point(85, 4);
            this.DeleteBTN.Name = "DeleteBTN";
            this.DeleteBTN.Size = new System.Drawing.Size(75, 23);
            this.DeleteBTN.TabIndex = 2;
            this.DeleteBTN.Text = "Delete...";
            this.DeleteBTN.UseVisualStyleBackColor = true;
            this.DeleteBTN.Click += new System.EventHandler(this.DeleteRuleMI_Click);
            // 
            // NewBTN
            // 
            this.NewBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NewBTN.Location = new System.Drawing.Point(4, 4);
            this.NewBTN.Name = "NewBTN";
            this.NewBTN.Size = new System.Drawing.Size(75, 23);
            this.NewBTN.TabIndex = 1;
            this.NewBTN.Text = "New...";
            this.NewBTN.UseVisualStyleBackColor = true;
            this.NewBTN.Click += new System.EventHandler(this.NewRuleMI_Click);
            // 
            // CancelBTN
            // 
            this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Location = new System.Drawing.Point(754, 4);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 0;
            this.CancelBTN.Text = "Close";
            this.CancelBTN.UseVisualStyleBackColor = true;
            // 
            // MainPN
            // 
            this.MainPN.Controls.Add(this.RulesDV);
            this.MainPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPN.Location = new System.Drawing.Point(0, 0);
            this.MainPN.Name = "MainPN";
            this.MainPN.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.MainPN.Size = new System.Drawing.Size(833, 306);
            this.MainPN.TabIndex = 1;
            // 
            // RulesDV
            // 
            this.RulesDV.AllowUserToAddRows = false;
            this.RulesDV.AllowUserToDeleteRows = false;
            this.RulesDV.AllowUserToResizeRows = false;
            this.RulesDV.AutoGenerateColumns = false;
            this.RulesDV.BackgroundColor = System.Drawing.SystemColors.Control;
            this.RulesDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RulesDV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UrlCH,
            this.UserNameCH,
            this.RuleTypeCH});
            this.RulesDV.ContextMenuStrip = this.PopupMenu;
            this.RulesDV.DataSource = this.DataSet;
            this.RulesDV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RulesDV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.RulesDV.Location = new System.Drawing.Point(3, 3);
            this.RulesDV.Name = "RulesDV";
            this.RulesDV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.RulesDV.Size = new System.Drawing.Size(827, 303);
            this.RulesDV.TabIndex = 0;
            // 
            // UrlCH
            // 
            this.UrlCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.UrlCH.DataPropertyName = "Url";
            this.UrlCH.HeaderText = "URL";
            this.UrlCH.Name = "UrlCH";
            this.UrlCH.Width = 54;
            // 
            // UserNameCH
            // 
            this.UserNameCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.UserNameCH.DataPropertyName = "UserName";
            this.UserNameCH.HeaderText = "User Name";
            this.UserNameCH.Name = "UserNameCH";
            this.UserNameCH.Width = 85;
            // 
            // RuleTypeCH
            // 
            this.RuleTypeCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RuleTypeCH.DataPropertyName = "RuleType";
            this.RuleTypeCH.HeaderText = "Rule Type";
            this.RuleTypeCH.Name = "RuleTypeCH";
            // 
            // PopupMenu
            // 
            this.PopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewBindingMI,
            this.DeleteBindingMI});
            this.PopupMenu.Name = "PopupMenu";
            this.PopupMenu.Size = new System.Drawing.Size(117, 48);
            // 
            // NewBindingMI
            // 
            this.NewBindingMI.Name = "NewBindingMI";
            this.NewBindingMI.Size = new System.Drawing.Size(116, 22);
            this.NewBindingMI.Text = "New...";
            this.NewBindingMI.Click += new System.EventHandler(this.NewRuleMI_Click);
            // 
            // DeleteBindingMI
            // 
            this.DeleteBindingMI.Name = "DeleteBindingMI";
            this.DeleteBindingMI.Size = new System.Drawing.Size(116, 22);
            this.DeleteBindingMI.Text = "Delete...";
            this.DeleteBindingMI.Click += new System.EventHandler(this.DeleteRuleMI_Click);
            // 
            // DataSet
            // 
            this.DataSet.DataSetName = "NewDataSet";
            // 
            // ManageHttpAccessRulesDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 337);
            this.Controls.Add(this.MainPN);
            this.Controls.Add(this.ButtonsPN);
            this.MaximumSize = new System.Drawing.Size(2048, 2048);
            this.MinimumSize = new System.Drawing.Size(300, 128);
            this.Name = "ManageHttpAccessRulesDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage HTTP/HTTPS Access Rules";
            this.Shown += new System.EventHandler(this.ManageHttpAccessRulesDlg_Shown);
            this.ButtonsPN.ResumeLayout(false);
            this.MainPN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RulesDV)).EndInit();
            this.PopupMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ButtonsPN;
        private System.Windows.Forms.Button NewBTN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Panel MainPN;
        private System.Windows.Forms.DataGridView RulesDV;
        private System.Windows.Forms.ContextMenuStrip PopupMenu;
        private System.Windows.Forms.ToolStripMenuItem DeleteBindingMI;
        private System.Windows.Forms.ToolStripMenuItem NewBindingMI;
        private System.Windows.Forms.Button DeleteBTN;
        private System.Windows.Forms.DataGridViewTextBoxColumn UrlCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserNameCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn RuleTypeCH;
        private System.Data.DataSet DataSet;
    }
}
