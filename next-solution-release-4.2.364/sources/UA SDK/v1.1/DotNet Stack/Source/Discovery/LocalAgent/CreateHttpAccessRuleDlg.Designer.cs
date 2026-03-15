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
    partial class CreateHttpAccessRuleDlg
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
            this.LayoutPN = new System.Windows.Forms.TableLayoutPanel();
            this.IdentityNameTB = new System.Windows.Forms.TextBox();
            this.RuleTypeLB = new System.Windows.Forms.Label();
            this.UrlTB = new System.Windows.Forms.TextBox();
            this.UrlLB = new System.Windows.Forms.Label();
            this.IdentityNameLB = new System.Windows.Forms.Label();
            this.RuleTypeCB = new System.Windows.Forms.ComboBox();
            this.IdentityNameBTN = new Opc.Ua.Client.Controls.SelectUserIdentityCtrl();
            this.PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteBindingMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonsPN.SuspendLayout();
            this.MainPN.SuspendLayout();
            this.LayoutPN.SuspendLayout();
            this.PopupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.Controls.Add(this.OkBTN);
            this.ButtonsPN.Controls.Add(this.CancelBTN);
            this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPN.Location = new System.Drawing.Point(0, 85);
            this.ButtonsPN.Name = "ButtonsPN";
            this.ButtonsPN.Size = new System.Drawing.Size(473, 31);
            this.ButtonsPN.TabIndex = 0;
            // 
            // OkBTN
            // 
            this.OkBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.CancelBTN.Location = new System.Drawing.Point(394, 4);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 0;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            // 
            // MainPN
            // 
            this.MainPN.Controls.Add(this.LayoutPN);
            this.MainPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPN.Location = new System.Drawing.Point(0, 0);
            this.MainPN.Name = "MainPN";
            this.MainPN.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.MainPN.Size = new System.Drawing.Size(473, 85);
            this.MainPN.TabIndex = 1;
            // 
            // LayoutPN
            // 
            this.LayoutPN.ColumnCount = 3;
            this.LayoutPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.LayoutPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LayoutPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.LayoutPN.Controls.Add(this.IdentityNameTB, 1, 1);
            this.LayoutPN.Controls.Add(this.RuleTypeLB, 0, 2);
            this.LayoutPN.Controls.Add(this.UrlTB, 1, 0);
            this.LayoutPN.Controls.Add(this.UrlLB, 0, 0);
            this.LayoutPN.Controls.Add(this.IdentityNameLB, 0, 1);
            this.LayoutPN.Controls.Add(this.RuleTypeCB, 1, 2);
            this.LayoutPN.Controls.Add(this.IdentityNameBTN, 2, 1);
            this.LayoutPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPN.Location = new System.Drawing.Point(3, 3);
            this.LayoutPN.Name = "LayoutPN";
            this.LayoutPN.RowCount = 4;
            this.LayoutPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LayoutPN.Size = new System.Drawing.Size(467, 82);
            this.LayoutPN.TabIndex = 0;
            // 
            // IdentityNameTB
            // 
            this.IdentityNameTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IdentityNameTB.Location = new System.Drawing.Point(81, 29);
            this.IdentityNameTB.Name = "IdentityNameTB";
            this.IdentityNameTB.Size = new System.Drawing.Size(359, 20);
            this.IdentityNameTB.TabIndex = 3;
            // 
            // RuleTypeLB
            // 
            this.RuleTypeLB.AutoSize = true;
            this.RuleTypeLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RuleTypeLB.Location = new System.Drawing.Point(3, 52);
            this.RuleTypeLB.Name = "RuleTypeLB";
            this.RuleTypeLB.Size = new System.Drawing.Size(72, 27);
            this.RuleTypeLB.TabIndex = 4;
            this.RuleTypeLB.Text = "Rule Type";
            this.RuleTypeLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UrlTB
            // 
            this.UrlTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UrlTB.Location = new System.Drawing.Point(81, 3);
            this.UrlTB.Name = "UrlTB";
            this.UrlTB.Size = new System.Drawing.Size(359, 20);
            this.UrlTB.TabIndex = 1;
            // 
            // UrlLB
            // 
            this.UrlLB.AutoSize = true;
            this.UrlLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UrlLB.Location = new System.Drawing.Point(3, 0);
            this.UrlLB.Name = "UrlLB";
            this.UrlLB.Size = new System.Drawing.Size(72, 26);
            this.UrlLB.TabIndex = 0;
            this.UrlLB.Text = "URL";
            this.UrlLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IdentityNameLB
            // 
            this.IdentityNameLB.AutoSize = true;
            this.IdentityNameLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IdentityNameLB.Location = new System.Drawing.Point(3, 26);
            this.IdentityNameLB.Name = "IdentityNameLB";
            this.IdentityNameLB.Size = new System.Drawing.Size(72, 26);
            this.IdentityNameLB.TabIndex = 2;
            this.IdentityNameLB.Text = "Identity Name";
            this.IdentityNameLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RuleTypeCB
            // 
            this.RuleTypeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RuleTypeCB.FormattingEnabled = true;
            this.RuleTypeCB.Location = new System.Drawing.Point(81, 55);
            this.RuleTypeCB.Name = "RuleTypeCB";
            this.RuleTypeCB.Size = new System.Drawing.Size(121, 21);
            this.RuleTypeCB.TabIndex = 5;
            // 
            // IdentityNameBTN
            // 
            this.IdentityNameBTN.IdentityControl = this.IdentityNameTB;
            this.IdentityNameBTN.Location = new System.Drawing.Point(443, 26);
            this.IdentityNameBTN.Margin = new System.Windows.Forms.Padding(0);
            this.IdentityNameBTN.Name = "IdentityNameBTN";
            this.IdentityNameBTN.Size = new System.Drawing.Size(24, 24);
            this.IdentityNameBTN.TabIndex = 6;
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
            // CreateHttpAccessRuleDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 116);
            this.Controls.Add(this.MainPN);
            this.Controls.Add(this.ButtonsPN);
            this.MaximumSize = new System.Drawing.Size(1024, 184);
            this.MinimumSize = new System.Drawing.Size(489, 154);
            this.Name = "CreateHttpAccessRuleDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create HTTP Access Rule";
            this.ButtonsPN.ResumeLayout(false);
            this.MainPN.ResumeLayout(false);
            this.LayoutPN.ResumeLayout(false);
            this.LayoutPN.PerformLayout();
            this.PopupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ButtonsPN;
        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Panel MainPN;
        private System.Windows.Forms.ContextMenuStrip PopupMenu;
        private System.Windows.Forms.ToolStripMenuItem DeleteBindingMI;
        private System.Windows.Forms.ToolStripMenuItem NewBindingMI;
        private System.Windows.Forms.TableLayoutPanel LayoutPN;
        private System.Windows.Forms.TextBox UrlTB;
        private System.Windows.Forms.Label UrlLB;
        private System.Windows.Forms.Label IdentityNameLB;
        private System.Windows.Forms.Label RuleTypeLB;
        private System.Windows.Forms.TextBox IdentityNameTB;
        private System.Windows.Forms.ComboBox RuleTypeCB;
        private Opc.Ua.Client.Controls.SelectUserIdentityCtrl IdentityNameBTN;
    }
}
