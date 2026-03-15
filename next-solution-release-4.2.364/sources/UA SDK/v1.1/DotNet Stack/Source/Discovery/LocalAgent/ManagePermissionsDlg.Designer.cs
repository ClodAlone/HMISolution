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
    partial class ManagePermissionsDlg
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
            this.OkBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.MainPN = new System.Windows.Forms.Panel();
            this.PortsDV = new System.Windows.Forms.DataGridView();
            this.ProtocolCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EnabledCH = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BlankCH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DataSet = new System.Data.DataSet();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OpenFirewallPortsLB = new System.Windows.Forms.Label();
            this.ExecutablePathLB = new System.Windows.Forms.Label();
            this.ExecutablePathTB = new System.Windows.Forms.TextBox();
            this.PermissionTemplateLB = new System.Windows.Forms.Label();
            this.PermissionTemplateCB = new System.Windows.Forms.ComboBox();
            this.OpenFirewallPortsCK = new System.Windows.Forms.CheckBox();
            this.ExecutablePathBTN = new Opc.Ua.Client.Controls.SelectFileCtrl();
            this.ButtonsPN.SuspendLayout();
            this.MainPN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PortsDV)).BeginInit();
            this.PopupMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.Controls.Add(this.OkBTN);
            this.ButtonsPN.Controls.Add(this.CancelBTN);
            this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPN.Location = new System.Drawing.Point(0, 214);
            this.ButtonsPN.Name = "ButtonsPN";
            this.ButtonsPN.Size = new System.Drawing.Size(581, 31);
            this.ButtonsPN.TabIndex = 0;
            // 
            // OkBTN
            // 
            this.OkBTN.Location = new System.Drawing.Point(4, 4);
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.Size = new System.Drawing.Size(75, 23);
            this.OkBTN.TabIndex = 1;
            this.OkBTN.Text = "OK";
            this.OkBTN.UseVisualStyleBackColor = true;
            this.OkBTN.Click += new System.EventHandler(this.OkBTN_Click);
            // 
            // CancelBTN
            // 
            this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Location = new System.Drawing.Point(502, 4);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 0;
            this.CancelBTN.Text = "Close";
            this.CancelBTN.UseVisualStyleBackColor = true;
            // 
            // MainPN
            // 
            this.MainPN.Controls.Add(this.PortsDV);
            this.MainPN.Controls.Add(this.tableLayoutPanel1);
            this.MainPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPN.Location = new System.Drawing.Point(0, 0);
            this.MainPN.Name = "MainPN";
            this.MainPN.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.MainPN.Size = new System.Drawing.Size(581, 214);
            this.MainPN.TabIndex = 1;
            // 
            // PortsDV
            // 
            this.PortsDV.AllowUserToAddRows = false;
            this.PortsDV.AllowUserToDeleteRows = false;
            this.PortsDV.AllowUserToOrderColumns = true;
            this.PortsDV.AllowUserToResizeRows = false;
            this.PortsDV.AutoGenerateColumns = false;
            this.PortsDV.BackgroundColor = System.Drawing.SystemColors.Control;
            this.PortsDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PortsDV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProtocolCH,
            this.PortCH,
            this.EnabledCH,
            this.BlankCH});
            this.PortsDV.ContextMenuStrip = this.PopupMenu;
            this.PortsDV.DataSource = this.DataSet;
            this.PortsDV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PortsDV.Location = new System.Drawing.Point(3, 79);
            this.PortsDV.Name = "PortsDV";
            this.PortsDV.RowHeadersVisible = false;
            this.PortsDV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.PortsDV.Size = new System.Drawing.Size(575, 135);
            this.PortsDV.TabIndex = 0;
            // 
            // ProtocolCH
            // 
            this.ProtocolCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ProtocolCH.DataPropertyName = "Protocol";
            this.ProtocolCH.HeaderText = "Protocol";
            this.ProtocolCH.Name = "ProtocolCH";
            this.ProtocolCH.Width = 71;
            // 
            // PortCH
            // 
            this.PortCH.DataPropertyName = "Port";
            this.PortCH.HeaderText = "Port";
            this.PortCH.Name = "PortCH";
            // 
            // EnabledCH
            // 
            this.EnabledCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.EnabledCH.DataPropertyName = "Enabled";
            this.EnabledCH.HeaderText = "Enabled";
            this.EnabledCH.Name = "EnabledCH";
            this.EnabledCH.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EnabledCH.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EnabledCH.Width = 71;
            // 
            // BlankCH
            // 
            this.BlankCH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BlankCH.HeaderText = "";
            this.BlankCH.Name = "BlankCH";
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
            // 
            // DeleteBindingMI
            // 
            this.DeleteBindingMI.Name = "DeleteBindingMI";
            this.DeleteBindingMI.Size = new System.Drawing.Size(116, 22);
            this.DeleteBindingMI.Text = "Delete...";
            // 
            // DataSet
            // 
            this.DataSet.DataSetName = "NewDataSet";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.OpenFirewallPortsLB, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ExecutablePathLB, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ExecutablePathTB, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.PermissionTemplateLB, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.PermissionTemplateCB, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ExecutablePathBTN, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.OpenFirewallPortsCK, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(575, 76);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // OpenFirewallPortsLB
            // 
            this.OpenFirewallPortsLB.AutoSize = true;
            this.OpenFirewallPortsLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenFirewallPortsLB.Location = new System.Drawing.Point(3, 52);
            this.OpenFirewallPortsLB.Name = "OpenFirewallPortsLB";
            this.OpenFirewallPortsLB.Size = new System.Drawing.Size(104, 23);
            this.OpenFirewallPortsLB.TabIndex = 5;
            this.OpenFirewallPortsLB.Text = "Open Firewall Ports";
            this.OpenFirewallPortsLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExecutablePathLB
            // 
            this.ExecutablePathLB.AutoSize = true;
            this.ExecutablePathLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExecutablePathLB.Location = new System.Drawing.Point(3, 0);
            this.ExecutablePathLB.Name = "ExecutablePathLB";
            this.ExecutablePathLB.Size = new System.Drawing.Size(104, 26);
            this.ExecutablePathLB.TabIndex = 0;
            this.ExecutablePathLB.Text = "Executable Path";
            this.ExecutablePathLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExecutablePathTB
            // 
            this.ExecutablePathTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExecutablePathTB.Location = new System.Drawing.Point(113, 3);
            this.ExecutablePathTB.Name = "ExecutablePathTB";
            this.ExecutablePathTB.Size = new System.Drawing.Size(435, 20);
            this.ExecutablePathTB.TabIndex = 1;
            // 
            // PermissionTemplateLB
            // 
            this.PermissionTemplateLB.AutoSize = true;
            this.PermissionTemplateLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PermissionTemplateLB.Location = new System.Drawing.Point(3, 26);
            this.PermissionTemplateLB.Name = "PermissionTemplateLB";
            this.PermissionTemplateLB.Size = new System.Drawing.Size(104, 26);
            this.PermissionTemplateLB.TabIndex = 3;
            this.PermissionTemplateLB.Text = "Permission Template";
            this.PermissionTemplateLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PermissionTemplateCB
            // 
            this.PermissionTemplateCB.Dock = System.Windows.Forms.DockStyle.Left;
            this.PermissionTemplateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PermissionTemplateCB.FormattingEnabled = true;
            this.PermissionTemplateCB.Location = new System.Drawing.Point(113, 29);
            this.PermissionTemplateCB.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.PermissionTemplateCB.Name = "PermissionTemplateCB";
            this.PermissionTemplateCB.Size = new System.Drawing.Size(145, 21);
            this.PermissionTemplateCB.TabIndex = 4;
            // 
            // OpenFirewallPortsCK
            // 
            this.OpenFirewallPortsCK.AutoSize = true;
            this.OpenFirewallPortsCK.Location = new System.Drawing.Point(113, 58);
            this.OpenFirewallPortsCK.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.OpenFirewallPortsCK.Name = "OpenFirewallPortsCK";
            this.OpenFirewallPortsCK.Size = new System.Drawing.Size(15, 14);
            this.OpenFirewallPortsCK.TabIndex = 6;
            this.OpenFirewallPortsCK.UseVisualStyleBackColor = true;
            this.OpenFirewallPortsCK.CheckedChanged += new System.EventHandler(this.OpenFirewallPortsCK_CheckedChanged);
            // 
            // ExecutablePathBTN
            // 
            this.ExecutablePathBTN.CurrentDirectory = null;
            this.ExecutablePathBTN.DefaultExt = null;
            this.ExecutablePathBTN.FilePathControl = this.ExecutablePathTB;
            this.ExecutablePathBTN.Filter = null;
            this.ExecutablePathBTN.Location = new System.Drawing.Point(551, 0);
            this.ExecutablePathBTN.Margin = new System.Windows.Forms.Padding(0);
            this.ExecutablePathBTN.Name = "ExecutablePathBTN";
            this.ExecutablePathBTN.Size = new System.Drawing.Size(24, 24);
            this.ExecutablePathBTN.TabIndex = 2;
            this.ExecutablePathBTN.FileSelected += new System.EventHandler(this.ExecutablePathBTN_FileSelected);
            // 
            // ManageLaunchPermissionsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 245);
            this.Controls.Add(this.MainPN);
            this.Controls.Add(this.ButtonsPN);
            this.MaximumSize = new System.Drawing.Size(2048, 2048);
            this.MinimumSize = new System.Drawing.Size(300, 128);
            this.Name = "ManageLaunchPermissionsDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Application Permissions";
            this.ButtonsPN.ResumeLayout(false);
            this.MainPN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PortsDV)).EndInit();
            this.PopupMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataSet)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ButtonsPN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Panel MainPN;
        private System.Windows.Forms.DataGridView PortsDV;
        private System.Windows.Forms.ContextMenuStrip PopupMenu;
        private System.Windows.Forms.ToolStripMenuItem DeleteBindingMI;
        private System.Windows.Forms.ToolStripMenuItem NewBindingMI;
        private System.Data.DataSet DataSet;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ExecutablePathLB;
        private System.Windows.Forms.TextBox ExecutablePathTB;
        private System.Windows.Forms.Label PermissionTemplateLB;
        private System.Windows.Forms.ComboBox PermissionTemplateCB;
        private Opc.Ua.Client.Controls.SelectFileCtrl ExecutablePathBTN;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProtocolCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortCH;
        private System.Windows.Forms.DataGridViewCheckBoxColumn EnabledCH;
        private System.Windows.Forms.DataGridViewTextBoxColumn BlankCH;
        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Label OpenFirewallPortsLB;
        private System.Windows.Forms.CheckBox OpenFirewallPortsCK;
    }
}
