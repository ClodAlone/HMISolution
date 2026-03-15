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
    partial class GdsAgentApplicationsListCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ApplicationsLV = new System.Windows.Forms.ListView();
            this.ApplicationNameCH = new System.Windows.Forms.ColumnHeader();
            this.ApplicationTypeCH = new System.Windows.Forms.ColumnHeader();
            this.ConfigurationTypeCH = new System.Windows.Forms.ColumnHeader();
            this.CertificateTypeCH = new System.Windows.Forms.ColumnHeader();
            this.RegisteredCH = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // ApplicationsLV
            // 
            this.ApplicationsLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ApplicationNameCH,
            this.RegisteredCH,
            this.ApplicationTypeCH,
            this.ConfigurationTypeCH,
            this.CertificateTypeCH});
            this.ApplicationsLV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationsLV.FullRowSelect = true;
            this.ApplicationsLV.Location = new System.Drawing.Point(0, 0);
            this.ApplicationsLV.Name = "ApplicationsLV";
            this.ApplicationsLV.Size = new System.Drawing.Size(647, 291);
            this.ApplicationsLV.TabIndex = 0;
            this.ApplicationsLV.UseCompatibleStateImageBehavior = false;
            this.ApplicationsLV.View = System.Windows.Forms.View.Details;
            // 
            // ApplicationNameCH
            // 
            this.ApplicationNameCH.Text = "Name";
            this.ApplicationNameCH.Width = 113;
            // 
            // ApplicationTypeCH
            // 
            this.ApplicationTypeCH.Text = "Application Type";
            this.ApplicationTypeCH.Width = 86;
            // 
            // ConfigurationTypeCH
            // 
            this.ConfigurationTypeCH.Text = "Configuration Type";
            this.ConfigurationTypeCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ConfigurationTypeCH.Width = 115;
            // 
            // CertificateTypeCH
            // 
            this.CertificateTypeCH.Text = "Certificate Type";
            this.CertificateTypeCH.Width = 99;
            // 
            // RegisteredCH
            // 
            this.RegisteredCH.Text = "Registered";
            this.RegisteredCH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RegisteredCH.Width = 117;
            // 
            // GdsAgentApplicationsListCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ApplicationsLV);
            this.Name = "GdsAgentApplicationsListCtrl";
            this.Size = new System.Drawing.Size(647, 291);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ApplicationsLV;
        private System.Windows.Forms.ColumnHeader ApplicationNameCH;
        private System.Windows.Forms.ColumnHeader ApplicationTypeCH;
        private System.Windows.Forms.ColumnHeader CertificateTypeCH;
        private System.Windows.Forms.ColumnHeader RegisteredCH;
        private System.Windows.Forms.ColumnHeader ConfigurationTypeCH;
    }
}
