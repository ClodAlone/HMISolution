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
    partial class ViewTrustMatrixDlg
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
            this.MainPN = new System.Windows.Forms.Panel();
            this.TrustMatrixDV = new System.Windows.Forms.DataGridView();
            this.ApplicationNameCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrustsMeCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrustedByMeCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ViewMyTrustListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewTargetTrustListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.EnableMutualTrustMI = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrustMatrixDV)).BeginInit();
            this.PopupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPN
            // 
            this.MainPN.Controls.Add(this.TrustMatrixDV);
            this.MainPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPN.Location = new System.Drawing.Point(0, 0);
            this.MainPN.Name = "MainPN";
            this.MainPN.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.MainPN.Size = new System.Drawing.Size(464, 208);
            this.MainPN.TabIndex = 1;
            // 
            // TrustMatrixDV
            // 
            this.TrustMatrixDV.AllowUserToAddRows = false;
            this.TrustMatrixDV.AllowUserToDeleteRows = false;
            this.TrustMatrixDV.AllowUserToResizeRows = false;
            this.TrustMatrixDV.BackgroundColor = System.Drawing.SystemColors.Control;
            this.TrustMatrixDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TrustMatrixDV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ApplicationNameCH,
            this.TrustsMeCH,
            this.TrustedByMeCH});
            this.TrustMatrixDV.ContextMenuStrip = this.PopupMenu;
            this.TrustMatrixDV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrustMatrixDV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.TrustMatrixDV.Location = new System.Drawing.Point(3, 3);
            this.TrustMatrixDV.Name = "TrustMatrixDV";
            this.TrustMatrixDV.RowHeadersVisible = false;
            this.TrustMatrixDV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.TrustMatrixDV.Size = new System.Drawing.Size(458, 205);
            this.TrustMatrixDV.TabIndex = 0;
            // 
            // ApplicationNameCH
            // 
            this.ApplicationNameCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ApplicationNameCH.DataPropertyName = "ApplicationName";
            this.ApplicationNameCH.HeaderText = "Application Name";
            this.ApplicationNameCH.Name = "ApplicationNameCH";
            this.ApplicationNameCH.Width = 106;
            // 
            // TrustsMeCH
            // 
            this.TrustsMeCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.TrustsMeCH.DataPropertyName = "TrustsMe";
            this.TrustsMeCH.HeaderText = "Trusts Me";
            this.TrustsMeCH.Name = "TrustsMeCH";
            this.TrustsMeCH.Width = 73;
            // 
            // TrustedByMeCH
            // 
            this.TrustedByMeCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TrustedByMeCH.DataPropertyName = "TrustedByMe";
            this.TrustedByMeCH.HeaderText = "Trusted By Me";
            this.TrustedByMeCH.Name = "TrustedByMeCH";
            // 
            // PopupMenu
            // 
            this.PopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewMyTrustListMI,
            this.ViewTargetTrustListMI,
            this.EnableMutualTrustMI});
            this.PopupMenu.Name = "PopupMenu";
            this.PopupMenu.Size = new System.Drawing.Size(229, 92);
            // 
            // ViewMyTrustListMI
            // 
            this.ViewMyTrustListMI.Name = "ViewMyTrustListMI";
            this.ViewMyTrustListMI.Size = new System.Drawing.Size(228, 22);
            this.ViewMyTrustListMI.Text = "View My Trust List...";
            this.ViewMyTrustListMI.Click += new System.EventHandler(this.ViewMyTrustListMI_Click);
            // 
            // ViewTargetTrustListMI
            // 
            this.ViewTargetTrustListMI.Name = "ViewTargetTrustListMI";
            this.ViewTargetTrustListMI.Size = new System.Drawing.Size(228, 22);
            this.ViewTargetTrustListMI.Text = "View Trust List for Selection...";
            this.ViewTargetTrustListMI.Click += new System.EventHandler(this.ViewTargetTrustListMI_Click);
            // 
            // EnableMutualTrustMI
            // 
            this.EnableMutualTrustMI.Name = "EnableMutualTrustMI";
            this.EnableMutualTrustMI.Size = new System.Drawing.Size(228, 22);
            this.EnableMutualTrustMI.Text = "Enable Mutual Trust";
            this.EnableMutualTrustMI.Click += new System.EventHandler(this.EnableMutualTrustMI_Click);
            // 
            // ViewTrustMatrixDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 208);
            this.Controls.Add(this.MainPN);
            this.MaximumSize = new System.Drawing.Size(2048, 2048);
            this.MinimumSize = new System.Drawing.Size(300, 128);
            this.Name = "ViewTrustMatrixDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Trust Matrix";
            this.Shown += new System.EventHandler(this.ManageHttpAccessRulesDlg_Shown);
            this.MainPN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrustMatrixDV)).EndInit();
            this.PopupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPN;
        private System.Windows.Forms.DataGridView TrustMatrixDV;
        private System.Windows.Forms.DataGridViewTextBoxColumn ApplicationNameCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrustsMeCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrustedByMeCH;
        private System.Windows.Forms.ContextMenuStrip PopupMenu;
        private System.Windows.Forms.ToolStripMenuItem ViewMyTrustListMI;
        private System.Windows.Forms.ToolStripMenuItem ViewTargetTrustListMI;
        private System.Windows.Forms.ToolStripMenuItem EnableMutualTrustMI;
    }
}
