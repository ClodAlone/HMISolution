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
    partial class ApplicationWizardCtrl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationWizardCtrl));
            this.XmlConfigRB = new System.Windows.Forms.RadioButton();
            this.ImportExportRB = new System.Windows.Forms.RadioButton();
            this.Stage1LB = new System.Windows.Forms.Label();
            this.ManualConfigRB = new System.Windows.Forms.RadioButton();
            this.ApplicationTypeTP = new System.Windows.Forms.ToolTip(this.components);
            this.ConfigurationFileLB = new System.Windows.Forms.Label();
            this.ApplicationNameLB = new System.Windows.Forms.Label();
            this.PublicKeyLB = new System.Windows.Forms.Label();
            this.PrivateKeyLB = new System.Windows.Forms.Label();
            this.TrustListLB = new System.Windows.Forms.Label();
            this.IssuerListLB = new System.Windows.Forms.Label();
            this.BaseAddressesLB = new System.Windows.Forms.Label();
            this.ExportArgumentsLB = new System.Windows.Forms.Label();
            this.ImportExportUtilityLB = new System.Windows.Forms.Label();
            this.ApplicationUriLB = new System.Windows.Forms.Label();
            this.ImportArgumentsLB = new System.Windows.Forms.Label();
            this.ApplicationTypeLB = new System.Windows.Forms.Label();
            this.ApplicationCertificateStoreLB = new System.Windows.Forms.Label();
            this.SecurityProfilesLB = new System.Windows.Forms.Label();
            this.MachineNameLB = new System.Windows.Forms.Label();
            this.ProductUriLB = new System.Windows.Forms.Label();
            this.SubjectNameLB = new System.Windows.Forms.Label();
            this.Stage1PN = new System.Windows.Forms.Panel();
            this.Stage1BottomPN = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonsPN = new System.Windows.Forms.FlowLayoutPanel();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.DoneBTN = new System.Windows.Forms.Button();
            this.NextBTN = new System.Windows.Forms.Button();
            this.BackBTN = new System.Windows.Forms.Button();
            this.Stage2BottomPN = new System.Windows.Forms.TableLayoutPanel();
            this.SubjectNameTB = new System.Windows.Forms.TextBox();
            this.ProductUriTB = new System.Windows.Forms.TextBox();
            this.MachineNameTB = new System.Windows.Forms.TextBox();
            this.SecurityProfilesBTN = new Opc.Ua.Client.Controls.SelectProfileCtrl();
            this.SecurityProfilesTB = new System.Windows.Forms.TextBox();
            this.ApplicationCertificateStoreBTN = new Opc.Ua.Client.Controls.SelectCertificateStoreCtrl();
            this.ApplicationCertificateStoreTB = new System.Windows.Forms.TextBox();
            this.ImportArgumentsTB = new System.Windows.Forms.TextBox();
            this.ImportExportUtilityBTN = new Opc.Ua.Client.Controls.SelectFileCtrl();
            this.ImportExportUtilityTB = new System.Windows.Forms.TextBox();
            this.ExportArgumentsTB = new System.Windows.Forms.TextBox();
            this.IssuerListBTN = new Opc.Ua.Client.Controls.SelectCertificateStoreCtrl();
            this.IssuerListTB = new System.Windows.Forms.TextBox();
            this.TrustListBTN = new Opc.Ua.Client.Controls.SelectCertificateStoreCtrl();
            this.TrustListTB = new System.Windows.Forms.TextBox();
            this.PrivateKeyBTN = new Opc.Ua.Client.Controls.SelectFileCtrl();
            this.PrivateKeyTB = new System.Windows.Forms.TextBox();
            this.PublicKeyBTN = new Opc.Ua.Client.Controls.SelectFileCtrl();
            this.PublicKeyTB = new System.Windows.Forms.TextBox();
            this.ConfigurationFileBTN = new Opc.Ua.Client.Controls.SelectFileCtrl();
            this.ConfigurationFileTB = new System.Windows.Forms.TextBox();
            this.BaseAddressesTB = new System.Windows.Forms.TextBox();
            this.ApplicationUriTB = new System.Windows.Forms.TextBox();
            this.ApplicationNameTB = new System.Windows.Forms.TextBox();
            this.ApplicationTypeCB = new System.Windows.Forms.ComboBox();
            this.BaseAddressesBTN = new Opc.Ua.Client.Controls.SelectUrlsCtrl();
            this.Stage2PN = new System.Windows.Forms.Panel();
            this.Stage2LB = new System.Windows.Forms.Label();
            this.Stage1PN.SuspendLayout();
            this.Stage1BottomPN.SuspendLayout();
            this.ButtonsPN.SuspendLayout();
            this.Stage2BottomPN.SuspendLayout();
            this.Stage2PN.SuspendLayout();
            this.SuspendLayout();
            // 
            // XmlConfigRB
            // 
            this.XmlConfigRB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.XmlConfigRB.AutoSize = true;
            this.XmlConfigRB.Location = new System.Drawing.Point(3, 3);
            this.XmlConfigRB.Name = "XmlConfigRB";
            this.XmlConfigRB.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.XmlConfigRB.Size = new System.Drawing.Size(315, 17);
            this.XmlConfigRB.TabIndex = 0;
            this.XmlConfigRB.TabStop = true;
            this.XmlConfigRB.Text = "An XML file which can be edited directly by the GDS agent.";
            this.ApplicationTypeTP.SetToolTip(this.XmlConfigRB, "Most applications built with the UA .NET SDK support this choice.\r\nApplications t" +
                    "hat use the SecuredApplication schema in their configuration files can also choo" +
                    "se this option.");
            this.XmlConfigRB.UseVisualStyleBackColor = true;
            this.XmlConfigRB.CheckedChanged += new System.EventHandler(this.XmlConfigRB_CheckedChanged);
            // 
            // ImportExportRB
            // 
            this.ImportExportRB.Enabled = false;
            this.ImportExportRB.Location = new System.Drawing.Point(3, 26);
            this.ImportExportRB.Name = "ImportExportRB";
            this.ImportExportRB.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.ImportExportRB.Size = new System.Drawing.Size(573, 17);
            this.ImportExportRB.TabIndex = 1;
            this.ImportExportRB.TabStop = true;
            this.ImportExportRB.Text = "An import/export utility.";
            this.ApplicationTypeTP.SetToolTip(this.ImportExportRB, "Applications with a command line utility that updates the security configuration " +
                    "can choose this.\r\nThe command line utility must support the SecuredApplication s" +
                    "chema as the input/output format.");
            this.ImportExportRB.UseVisualStyleBackColor = true;
            this.ImportExportRB.CheckedChanged += new System.EventHandler(this.ImportExportRB_CheckedChanged);
            // 
            // Stage1LB
            // 
            this.Stage1LB.AutoSize = true;
            this.Stage1LB.Dock = System.Windows.Forms.DockStyle.Top;
            this.Stage1LB.Location = new System.Drawing.Point(0, 0);
            this.Stage1LB.Name = "Stage1LB";
            this.Stage1LB.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.Stage1LB.Size = new System.Drawing.Size(120, 29);
            this.Stage1LB.TabIndex = 0;
            this.Stage1LB.Text = "IDS_STAGE1_SELECT";
            // 
            // ManualConfigRB
            // 
            this.ManualConfigRB.Location = new System.Drawing.Point(3, 49);
            this.ManualConfigRB.Name = "ManualConfigRB";
            this.ManualConfigRB.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.ManualConfigRB.Size = new System.Drawing.Size(573, 17);
            this.ManualConfigRB.TabIndex = 2;
            this.ManualConfigRB.TabStop = true;
            this.ManualConfigRB.Text = "Manual configuration.";
            this.ApplicationTypeTP.SetToolTip(this.ManualConfigRB, "Applications that require manual configuration by an administrator must choose th" +
                    "is.");
            this.ManualConfigRB.UseVisualStyleBackColor = true;
            this.ManualConfigRB.CheckedChanged += new System.EventHandler(this.ManualConfigRB_CheckedChanged);
            // 
            // ConfigurationFileLB
            // 
            this.ConfigurationFileLB.AutoSize = true;
            this.ConfigurationFileLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ConfigurationFileLB.Location = new System.Drawing.Point(3, 0);
            this.ConfigurationFileLB.Name = "ConfigurationFileLB";
            this.ConfigurationFileLB.Size = new System.Drawing.Size(88, 26);
            this.ConfigurationFileLB.TabIndex = 0;
            this.ConfigurationFileLB.Text = "Configuration File";
            this.ConfigurationFileLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ConfigurationFileLB, "This the full path to a configuration file that can be edited directly.");
            // 
            // ApplicationNameLB
            // 
            this.ApplicationNameLB.AutoSize = true;
            this.ApplicationNameLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ApplicationNameLB.Location = new System.Drawing.Point(3, 104);
            this.ApplicationNameLB.Name = "ApplicationNameLB";
            this.ApplicationNameLB.Size = new System.Drawing.Size(90, 26);
            this.ApplicationNameLB.TabIndex = 8;
            this.ApplicationNameLB.Text = "Application Name";
            this.ApplicationNameLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ApplicationNameLB, "The name that identifies the application. This is the name that appears in the GD" +
                    "S.");
            // 
            // PublicKeyLB
            // 
            this.PublicKeyLB.AutoSize = true;
            this.PublicKeyLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.PublicKeyLB.Location = new System.Drawing.Point(3, 287);
            this.PublicKeyLB.Name = "PublicKeyLB";
            this.PublicKeyLB.Size = new System.Drawing.Size(107, 26);
            this.PublicKeyLB.TabIndex = 12;
            this.PublicKeyLB.Text = "Certificate Public Key";
            this.PublicKeyLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.PublicKeyLB, "The path to a file containing the public key. This file will be updated by the GD" +
                    "S agent.");
            // 
            // PrivateKeyLB
            // 
            this.PrivateKeyLB.AutoSize = true;
            this.PrivateKeyLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.PrivateKeyLB.Location = new System.Drawing.Point(3, 313);
            this.PrivateKeyLB.Name = "PrivateKeyLB";
            this.PrivateKeyLB.Size = new System.Drawing.Size(111, 26);
            this.PrivateKeyLB.TabIndex = 15;
            this.PrivateKeyLB.Text = "Certificate Private Key";
            this.PrivateKeyLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.PrivateKeyLB, "The path to a file containing the application\'s private key. This file will be up" +
                    "dated by the GDS agent.");
            // 
            // TrustListLB
            // 
            this.TrustListLB.AutoSize = true;
            this.TrustListLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrustListLB.Location = new System.Drawing.Point(3, 339);
            this.TrustListLB.Name = "TrustListLB";
            this.TrustListLB.Size = new System.Drawing.Size(137, 26);
            this.TrustListLB.TabIndex = 18;
            this.TrustListLB.Text = "Trust List";
            this.TrustListLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.TrustListLB, resources.GetString("TrustListLB.ToolTip"));
            // 
            // IssuerListLB
            // 
            this.IssuerListLB.AutoSize = true;
            this.IssuerListLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.IssuerListLB.Location = new System.Drawing.Point(3, 365);
            this.IssuerListLB.Name = "IssuerListLB";
            this.IssuerListLB.Size = new System.Drawing.Size(104, 26);
            this.IssuerListLB.TabIndex = 21;
            this.IssuerListLB.Text = "Issuer Certificate List";
            this.IssuerListLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.IssuerListLB, resources.GetString("IssuerListLB.ToolTip"));
            // 
            // BaseAddressesLB
            // 
            this.BaseAddressesLB.AutoSize = true;
            this.BaseAddressesLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.BaseAddressesLB.Location = new System.Drawing.Point(3, 417);
            this.BaseAddressesLB.Name = "BaseAddressesLB";
            this.BaseAddressesLB.Size = new System.Drawing.Size(83, 26);
            this.BaseAddressesLB.TabIndex = 24;
            this.BaseAddressesLB.Text = "Base Addresses";
            this.BaseAddressesLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.BaseAddressesLB, "The base addresses for the endpoints exposed by a server application.");
            // 
            // ExportArgumentsLB
            // 
            this.ExportArgumentsLB.AutoSize = true;
            this.ExportArgumentsLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ExportArgumentsLB.Location = new System.Drawing.Point(3, 78);
            this.ExportArgumentsLB.Name = "ExportArgumentsLB";
            this.ExportArgumentsLB.Size = new System.Drawing.Size(90, 26);
            this.ExportArgumentsLB.TabIndex = 6;
            this.ExportArgumentsLB.Text = "Export Arguments";
            this.ExportArgumentsLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ExportArgumentsLB, "This is the command line that needs to be passed to the utility. The {0} is place" +
                    " holder for the import/export file path.");
            // 
            // ImportExportUtilityLB
            // 
            this.ImportExportUtilityLB.AutoSize = true;
            this.ImportExportUtilityLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ImportExportUtilityLB.Location = new System.Drawing.Point(3, 26);
            this.ImportExportUtilityLB.Name = "ImportExportUtilityLB";
            this.ImportExportUtilityLB.Size = new System.Drawing.Size(99, 26);
            this.ImportExportUtilityLB.TabIndex = 3;
            this.ImportExportUtilityLB.Text = "Import/Export Utility";
            this.ImportExportUtilityLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ImportExportUtilityLB, "This is the full path to a command line utility that can be called to import and " +
                    "export the application security configuration using the SecureApplication schema" +
                    " defined in Part 6.");
            // 
            // ApplicationUriLB
            // 
            this.ApplicationUriLB.AutoSize = true;
            this.ApplicationUriLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ApplicationUriLB.Location = new System.Drawing.Point(3, 130);
            this.ApplicationUriLB.Name = "ApplicationUriLB";
            this.ApplicationUriLB.Size = new System.Drawing.Size(81, 26);
            this.ApplicationUriLB.TabIndex = 27;
            this.ApplicationUriLB.Text = "Application URI";
            this.ApplicationUriLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ApplicationUriLB, "The globally unique identifier for the application. It may be overridden by the G" +
                    "DS.");
            // 
            // ImportArgumentsLB
            // 
            this.ImportArgumentsLB.AutoSize = true;
            this.ImportArgumentsLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ImportArgumentsLB.Location = new System.Drawing.Point(3, 52);
            this.ImportArgumentsLB.Name = "ImportArgumentsLB";
            this.ImportArgumentsLB.Size = new System.Drawing.Size(89, 26);
            this.ImportArgumentsLB.TabIndex = 29;
            this.ImportArgumentsLB.Text = "Import Arguments";
            this.ImportArgumentsLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ImportArgumentsLB, "This is the command line that needs to be passed to the utility. The {0} is place" +
                    " holder for the import/export file path.");
            // 
            // ApplicationTypeLB
            // 
            this.ApplicationTypeLB.AutoSize = true;
            this.ApplicationTypeLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ApplicationTypeLB.Location = new System.Drawing.Point(3, 234);
            this.ApplicationTypeLB.Name = "ApplicationTypeLB";
            this.ApplicationTypeLB.Size = new System.Drawing.Size(86, 27);
            this.ApplicationTypeLB.TabIndex = 10;
            this.ApplicationTypeLB.Text = "Application Type";
            this.ApplicationTypeLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ApplicationTypeLB, "The globally unique identifier for the application. It may be overridden by the G" +
                    "DS.");
            // 
            // ApplicationCertificateStoreLB
            // 
            this.ApplicationCertificateStoreLB.AutoSize = true;
            this.ApplicationCertificateStoreLB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationCertificateStoreLB.Location = new System.Drawing.Point(3, 261);
            this.ApplicationCertificateStoreLB.Name = "ApplicationCertificateStoreLB";
            this.ApplicationCertificateStoreLB.Size = new System.Drawing.Size(137, 26);
            this.ApplicationCertificateStoreLB.TabIndex = 31;
            this.ApplicationCertificateStoreLB.Text = "Application Certificate Store";
            this.ApplicationCertificateStoreLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ApplicationCertificateStoreLB, "The path to the certificate store containing the application certificate. This ca" +
                    "n be a Windows store or a directory based store.");
            // 
            // SecurityProfilesLB
            // 
            this.SecurityProfilesLB.AutoSize = true;
            this.SecurityProfilesLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.SecurityProfilesLB.Location = new System.Drawing.Point(3, 391);
            this.SecurityProfilesLB.Name = "SecurityProfilesLB";
            this.SecurityProfilesLB.Size = new System.Drawing.Size(82, 26);
            this.SecurityProfilesLB.TabIndex = 35;
            this.SecurityProfilesLB.Text = "Security Profiles";
            this.SecurityProfilesLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.SecurityProfilesLB, "The security profiles used by the application.");
            // 
            // MachineNameLB
            // 
            this.MachineNameLB.AutoSize = true;
            this.MachineNameLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.MachineNameLB.Location = new System.Drawing.Point(3, 156);
            this.MachineNameLB.Name = "MachineNameLB";
            this.MachineNameLB.Size = new System.Drawing.Size(79, 26);
            this.MachineNameLB.TabIndex = 38;
            this.MachineNameLB.Text = "Machine Name";
            this.MachineNameLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.MachineNameLB, "The name of the machine which hosts the application.");
            // 
            // ProductUriLB
            // 
            this.ProductUriLB.AutoSize = true;
            this.ProductUriLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ProductUriLB.Location = new System.Drawing.Point(3, 208);
            this.ProductUriLB.Name = "ProductUriLB";
            this.ProductUriLB.Size = new System.Drawing.Size(66, 26);
            this.ProductUriLB.TabIndex = 39;
            this.ProductUriLB.Text = "Product URI";
            this.ProductUriLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.ProductUriLB, "A globally unique URI for the product which the application belongs to.");
            // 
            // SubjectNameLB
            // 
            this.SubjectNameLB.AutoSize = true;
            this.SubjectNameLB.Dock = System.Windows.Forms.DockStyle.Left;
            this.SubjectNameLB.Location = new System.Drawing.Point(3, 182);
            this.SubjectNameLB.Name = "SubjectNameLB";
            this.SubjectNameLB.Size = new System.Drawing.Size(74, 26);
            this.SubjectNameLB.TabIndex = 42;
            this.SubjectNameLB.Text = "Subject Name";
            this.SubjectNameLB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ApplicationTypeTP.SetToolTip(this.SubjectNameLB, "The SubjectName to use with the Application Certificate. It must not include a Co" +
                    "mmonName (CN=)");
            // 
            // Stage1PN
            // 
            this.Stage1PN.Controls.Add(this.Stage1BottomPN);
            this.Stage1PN.Controls.Add(this.Stage1LB);
            this.Stage1PN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stage1PN.Location = new System.Drawing.Point(0, 0);
            this.Stage1PN.Name = "Stage1PN";
            this.Stage1PN.Size = new System.Drawing.Size(503, 476);
            this.Stage1PN.TabIndex = 4;
            // 
            // Stage1BottomPN
            // 
            this.Stage1BottomPN.Controls.Add(this.XmlConfigRB);
            this.Stage1BottomPN.Controls.Add(this.ImportExportRB);
            this.Stage1BottomPN.Controls.Add(this.ManualConfigRB);
            this.Stage1BottomPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stage1BottomPN.Location = new System.Drawing.Point(0, 29);
            this.Stage1BottomPN.Name = "Stage1BottomPN";
            this.Stage1BottomPN.Size = new System.Drawing.Size(503, 447);
            this.Stage1BottomPN.TabIndex = 1;
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.Controls.Add(this.CancelBTN);
            this.ButtonsPN.Controls.Add(this.DoneBTN);
            this.ButtonsPN.Controls.Add(this.NextBTN);
            this.ButtonsPN.Controls.Add(this.BackBTN);
            this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPN.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonsPN.Location = new System.Drawing.Point(0, 476);
            this.ButtonsPN.Name = "ButtonsPN";
            this.ButtonsPN.Size = new System.Drawing.Size(503, 29);
            this.ButtonsPN.TabIndex = 0;
            // 
            // CancelBTN
            // 
            this.CancelBTN.Location = new System.Drawing.Point(425, 3);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 2;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
            // 
            // DoneBTN
            // 
            this.DoneBTN.Location = new System.Drawing.Point(344, 3);
            this.DoneBTN.Name = "DoneBTN";
            this.DoneBTN.Size = new System.Drawing.Size(75, 23);
            this.DoneBTN.TabIndex = 3;
            this.DoneBTN.Text = "Done";
            this.DoneBTN.UseVisualStyleBackColor = true;
            this.DoneBTN.Click += new System.EventHandler(this.DoneBTN_Click);
            // 
            // NextBTN
            // 
            this.NextBTN.Location = new System.Drawing.Point(263, 3);
            this.NextBTN.Name = "NextBTN";
            this.NextBTN.Size = new System.Drawing.Size(75, 23);
            this.NextBTN.TabIndex = 0;
            this.NextBTN.Text = "Next";
            this.NextBTN.UseVisualStyleBackColor = true;
            this.NextBTN.Click += new System.EventHandler(this.NextBTN_Click);
            // 
            // BackBTN
            // 
            this.BackBTN.Location = new System.Drawing.Point(182, 3);
            this.BackBTN.Name = "BackBTN";
            this.BackBTN.Size = new System.Drawing.Size(75, 23);
            this.BackBTN.TabIndex = 1;
            this.BackBTN.Text = "Back";
            this.BackBTN.UseVisualStyleBackColor = true;
            this.BackBTN.Click += new System.EventHandler(this.BackBTN_Click);
            // 
            // Stage2BottomPN
            // 
            this.Stage2BottomPN.ColumnCount = 3;
            this.Stage2BottomPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Stage2BottomPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Stage2BottomPN.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Stage2BottomPN.Controls.Add(this.SubjectNameTB, 1, 7);
            this.Stage2BottomPN.Controls.Add(this.SubjectNameLB, 0, 7);
            this.Stage2BottomPN.Controls.Add(this.ProductUriTB, 1, 8);
            this.Stage2BottomPN.Controls.Add(this.MachineNameTB, 1, 6);
            this.Stage2BottomPN.Controls.Add(this.ProductUriLB, 0, 8);
            this.Stage2BottomPN.Controls.Add(this.MachineNameLB, 0, 6);
            this.Stage2BottomPN.Controls.Add(this.SecurityProfilesBTN, 2, 15);
            this.Stage2BottomPN.Controls.Add(this.SecurityProfilesTB, 1, 15);
            this.Stage2BottomPN.Controls.Add(this.SecurityProfilesLB, 0, 15);
            this.Stage2BottomPN.Controls.Add(this.ApplicationCertificateStoreBTN, 2, 10);
            this.Stage2BottomPN.Controls.Add(this.ApplicationCertificateStoreTB, 1, 10);
            this.Stage2BottomPN.Controls.Add(this.ApplicationCertificateStoreLB, 0, 10);
            this.Stage2BottomPN.Controls.Add(this.ImportArgumentsTB, 1, 2);
            this.Stage2BottomPN.Controls.Add(this.ImportArgumentsLB, 0, 2);
            this.Stage2BottomPN.Controls.Add(this.ApplicationUriLB, 0, 5);
            this.Stage2BottomPN.Controls.Add(this.ImportExportUtilityBTN, 2, 1);
            this.Stage2BottomPN.Controls.Add(this.ImportExportUtilityTB, 1, 1);
            this.Stage2BottomPN.Controls.Add(this.ImportExportUtilityLB, 0, 1);
            this.Stage2BottomPN.Controls.Add(this.ExportArgumentsTB, 1, 3);
            this.Stage2BottomPN.Controls.Add(this.ExportArgumentsLB, 0, 3);
            this.Stage2BottomPN.Controls.Add(this.IssuerListBTN, 2, 14);
            this.Stage2BottomPN.Controls.Add(this.TrustListBTN, 2, 13);
            this.Stage2BottomPN.Controls.Add(this.PrivateKeyBTN, 2, 12);
            this.Stage2BottomPN.Controls.Add(this.PublicKeyBTN, 2, 11);
            this.Stage2BottomPN.Controls.Add(this.ConfigurationFileBTN, 2, 0);
            this.Stage2BottomPN.Controls.Add(this.BaseAddressesTB, 1, 16);
            this.Stage2BottomPN.Controls.Add(this.IssuerListTB, 1, 14);
            this.Stage2BottomPN.Controls.Add(this.TrustListTB, 1, 13);
            this.Stage2BottomPN.Controls.Add(this.PrivateKeyTB, 1, 12);
            this.Stage2BottomPN.Controls.Add(this.PublicKeyTB, 1, 11);
            this.Stage2BottomPN.Controls.Add(this.ApplicationUriTB, 1, 5);
            this.Stage2BottomPN.Controls.Add(this.ApplicationNameTB, 1, 4);
            this.Stage2BottomPN.Controls.Add(this.BaseAddressesLB, 0, 16);
            this.Stage2BottomPN.Controls.Add(this.IssuerListLB, 0, 14);
            this.Stage2BottomPN.Controls.Add(this.TrustListLB, 0, 13);
            this.Stage2BottomPN.Controls.Add(this.PrivateKeyLB, 0, 12);
            this.Stage2BottomPN.Controls.Add(this.PublicKeyLB, 0, 11);
            this.Stage2BottomPN.Controls.Add(this.ApplicationTypeLB, 0, 9);
            this.Stage2BottomPN.Controls.Add(this.ApplicationNameLB, 0, 4);
            this.Stage2BottomPN.Controls.Add(this.ConfigurationFileLB, 0, 0);
            this.Stage2BottomPN.Controls.Add(this.ConfigurationFileTB, 1, 0);
            this.Stage2BottomPN.Controls.Add(this.ApplicationTypeCB, 1, 9);
            this.Stage2BottomPN.Controls.Add(this.BaseAddressesBTN, 2, 16);
            this.Stage2BottomPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stage2BottomPN.Location = new System.Drawing.Point(0, 29);
            this.Stage2BottomPN.Name = "Stage2BottomPN";
            this.Stage2BottomPN.RowCount = 19;
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Stage2BottomPN.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Stage2BottomPN.Size = new System.Drawing.Size(503, 447);
            this.Stage2BottomPN.TabIndex = 1;
            // 
            // SubjectNameTB
            // 
            this.SubjectNameTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SubjectNameTB.Location = new System.Drawing.Point(146, 185);
            this.SubjectNameTB.Name = "SubjectNameTB";
            this.SubjectNameTB.Size = new System.Drawing.Size(328, 20);
            this.SubjectNameTB.TabIndex = 43;
            // 
            // ProductUriTB
            // 
            this.ProductUriTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProductUriTB.Location = new System.Drawing.Point(146, 211);
            this.ProductUriTB.Name = "ProductUriTB";
            this.ProductUriTB.Size = new System.Drawing.Size(328, 20);
            this.ProductUriTB.TabIndex = 41;
            // 
            // MachineNameTB
            // 
            this.MachineNameTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MachineNameTB.Location = new System.Drawing.Point(146, 159);
            this.MachineNameTB.Name = "MachineNameTB";
            this.MachineNameTB.Size = new System.Drawing.Size(328, 20);
            this.MachineNameTB.TabIndex = 40;
            // 
            // SecurityProfilesBTN
            // 
            this.SecurityProfilesBTN.CurrentProfilesControl = this.SecurityProfilesTB;
            this.SecurityProfilesBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.SecurityProfilesBTN.Location = new System.Drawing.Point(478, 392);
            this.SecurityProfilesBTN.Margin = new System.Windows.Forms.Padding(1);
            this.SecurityProfilesBTN.Name = "SecurityProfilesBTN";
            this.SecurityProfilesBTN.Profiles = null;
            this.SecurityProfilesBTN.Size = new System.Drawing.Size(24, 24);
            this.SecurityProfilesBTN.TabIndex = 37;
            // 
            // SecurityProfilesTB
            // 
            this.SecurityProfilesTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SecurityProfilesTB.Location = new System.Drawing.Point(146, 394);
            this.SecurityProfilesTB.Name = "SecurityProfilesTB";
            this.SecurityProfilesTB.Size = new System.Drawing.Size(328, 20);
            this.SecurityProfilesTB.TabIndex = 36;
            // 
            // ApplicationCertificateStoreBTN
            // 
            this.ApplicationCertificateStoreBTN.CertificateStoreControl = this.ApplicationCertificateStoreTB;
            this.ApplicationCertificateStoreBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.ApplicationCertificateStoreBTN.Location = new System.Drawing.Point(478, 262);
            this.ApplicationCertificateStoreBTN.Margin = new System.Windows.Forms.Padding(1);
            this.ApplicationCertificateStoreBTN.Name = "ApplicationCertificateStoreBTN";
            this.ApplicationCertificateStoreBTN.Size = new System.Drawing.Size(24, 24);
            this.ApplicationCertificateStoreBTN.TabIndex = 33;
            // 
            // ApplicationCertificateStoreTB
            // 
            this.ApplicationCertificateStoreTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationCertificateStoreTB.Location = new System.Drawing.Point(146, 264);
            this.ApplicationCertificateStoreTB.Name = "ApplicationCertificateStoreTB";
            this.ApplicationCertificateStoreTB.Size = new System.Drawing.Size(328, 20);
            this.ApplicationCertificateStoreTB.TabIndex = 32;
            // 
            // ImportArgumentsTB
            // 
            this.ImportArgumentsTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImportArgumentsTB.Location = new System.Drawing.Point(146, 55);
            this.ImportArgumentsTB.Name = "ImportArgumentsTB";
            this.ImportArgumentsTB.Size = new System.Drawing.Size(328, 20);
            this.ImportArgumentsTB.TabIndex = 30;
            // 
            // ImportExportUtilityBTN
            // 
            this.ImportExportUtilityBTN.CurrentDirectory = null;
            this.ImportExportUtilityBTN.DefaultExt = null;
            this.ImportExportUtilityBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.ImportExportUtilityBTN.FilePathControl = this.ImportExportUtilityTB;
            this.ImportExportUtilityBTN.Filter = null;
            this.ImportExportUtilityBTN.Location = new System.Drawing.Point(478, 27);
            this.ImportExportUtilityBTN.Margin = new System.Windows.Forms.Padding(1);
            this.ImportExportUtilityBTN.Name = "ImportExportUtilityBTN";
            this.ImportExportUtilityBTN.Size = new System.Drawing.Size(24, 24);
            this.ImportExportUtilityBTN.TabIndex = 5;
            // 
            // ImportExportUtilityTB
            // 
            this.ImportExportUtilityTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImportExportUtilityTB.Location = new System.Drawing.Point(146, 29);
            this.ImportExportUtilityTB.Name = "ImportExportUtilityTB";
            this.ImportExportUtilityTB.Size = new System.Drawing.Size(328, 20);
            this.ImportExportUtilityTB.TabIndex = 4;
            // 
            // ExportArgumentsTB
            // 
            this.ExportArgumentsTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExportArgumentsTB.Location = new System.Drawing.Point(146, 81);
            this.ExportArgumentsTB.Name = "ExportArgumentsTB";
            this.ExportArgumentsTB.Size = new System.Drawing.Size(328, 20);
            this.ExportArgumentsTB.TabIndex = 7;
            // 
            // IssuerListBTN
            // 
            this.IssuerListBTN.CertificateStoreControl = this.IssuerListTB;
            this.IssuerListBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.IssuerListBTN.Location = new System.Drawing.Point(478, 366);
            this.IssuerListBTN.Margin = new System.Windows.Forms.Padding(1);
            this.IssuerListBTN.Name = "IssuerListBTN";
            this.IssuerListBTN.Size = new System.Drawing.Size(24, 24);
            this.IssuerListBTN.TabIndex = 23;
            // 
            // IssuerListTB
            // 
            this.IssuerListTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IssuerListTB.Location = new System.Drawing.Point(146, 368);
            this.IssuerListTB.Name = "IssuerListTB";
            this.IssuerListTB.Size = new System.Drawing.Size(328, 20);
            this.IssuerListTB.TabIndex = 22;
            // 
            // TrustListBTN
            // 
            this.TrustListBTN.CertificateStoreControl = this.TrustListTB;
            this.TrustListBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.TrustListBTN.Location = new System.Drawing.Point(478, 340);
            this.TrustListBTN.Margin = new System.Windows.Forms.Padding(1);
            this.TrustListBTN.Name = "TrustListBTN";
            this.TrustListBTN.Size = new System.Drawing.Size(24, 24);
            this.TrustListBTN.TabIndex = 20;
            // 
            // TrustListTB
            // 
            this.TrustListTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrustListTB.Location = new System.Drawing.Point(146, 342);
            this.TrustListTB.Name = "TrustListTB";
            this.TrustListTB.Size = new System.Drawing.Size(328, 20);
            this.TrustListTB.TabIndex = 19;
            // 
            // PrivateKeyBTN
            // 
            this.PrivateKeyBTN.CurrentDirectory = null;
            this.PrivateKeyBTN.DefaultExt = null;
            this.PrivateKeyBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.PrivateKeyBTN.FilePathControl = this.PrivateKeyTB;
            this.PrivateKeyBTN.Filter = null;
            this.PrivateKeyBTN.Location = new System.Drawing.Point(478, 314);
            this.PrivateKeyBTN.Margin = new System.Windows.Forms.Padding(1);
            this.PrivateKeyBTN.Name = "PrivateKeyBTN";
            this.PrivateKeyBTN.Size = new System.Drawing.Size(24, 24);
            this.PrivateKeyBTN.TabIndex = 17;
            // 
            // PrivateKeyTB
            // 
            this.PrivateKeyTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PrivateKeyTB.Location = new System.Drawing.Point(146, 316);
            this.PrivateKeyTB.Name = "PrivateKeyTB";
            this.PrivateKeyTB.Size = new System.Drawing.Size(328, 20);
            this.PrivateKeyTB.TabIndex = 16;
            // 
            // PublicKeyBTN
            // 
            this.PublicKeyBTN.CurrentDirectory = null;
            this.PublicKeyBTN.DefaultExt = null;
            this.PublicKeyBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.PublicKeyBTN.FilePathControl = this.PublicKeyTB;
            this.PublicKeyBTN.Filter = null;
            this.PublicKeyBTN.Location = new System.Drawing.Point(478, 288);
            this.PublicKeyBTN.Margin = new System.Windows.Forms.Padding(1);
            this.PublicKeyBTN.Name = "PublicKeyBTN";
            this.PublicKeyBTN.Size = new System.Drawing.Size(24, 24);
            this.PublicKeyBTN.TabIndex = 14;
            this.PublicKeyBTN.FileSelected += new System.EventHandler(this.PublicKeyBTN_FileSelected);
            // 
            // PublicKeyTB
            // 
            this.PublicKeyTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PublicKeyTB.Location = new System.Drawing.Point(146, 290);
            this.PublicKeyTB.Name = "PublicKeyTB";
            this.PublicKeyTB.Size = new System.Drawing.Size(328, 20);
            this.PublicKeyTB.TabIndex = 13;
            // 
            // ConfigurationFileBTN
            // 
            this.ConfigurationFileBTN.CurrentDirectory = null;
            this.ConfigurationFileBTN.DefaultExt = null;
            this.ConfigurationFileBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.ConfigurationFileBTN.FilePathControl = this.ConfigurationFileTB;
            this.ConfigurationFileBTN.Filter = null;
            this.ConfigurationFileBTN.Location = new System.Drawing.Point(478, 1);
            this.ConfigurationFileBTN.Margin = new System.Windows.Forms.Padding(1);
            this.ConfigurationFileBTN.Name = "ConfigurationFileBTN";
            this.ConfigurationFileBTN.Size = new System.Drawing.Size(24, 24);
            this.ConfigurationFileBTN.TabIndex = 2;
            this.ConfigurationFileBTN.FileSelected += new System.EventHandler(this.ConfigurationFileBTN_FileSelected);
            // 
            // ConfigurationFileTB
            // 
            this.ConfigurationFileTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigurationFileTB.Location = new System.Drawing.Point(146, 3);
            this.ConfigurationFileTB.Name = "ConfigurationFileTB";
            this.ConfigurationFileTB.Size = new System.Drawing.Size(328, 20);
            this.ConfigurationFileTB.TabIndex = 1;
            // 
            // BaseAddressesTB
            // 
            this.BaseAddressesTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseAddressesTB.Location = new System.Drawing.Point(146, 420);
            this.BaseAddressesTB.Name = "BaseAddressesTB";
            this.BaseAddressesTB.Size = new System.Drawing.Size(328, 20);
            this.BaseAddressesTB.TabIndex = 25;
            // 
            // ApplicationUriTB
            // 
            this.ApplicationUriTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationUriTB.Location = new System.Drawing.Point(146, 133);
            this.ApplicationUriTB.Name = "ApplicationUriTB";
            this.ApplicationUriTB.Size = new System.Drawing.Size(328, 20);
            this.ApplicationUriTB.TabIndex = 11;
            // 
            // ApplicationNameTB
            // 
            this.ApplicationNameTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationNameTB.Location = new System.Drawing.Point(146, 107);
            this.ApplicationNameTB.Name = "ApplicationNameTB";
            this.ApplicationNameTB.Size = new System.Drawing.Size(328, 20);
            this.ApplicationNameTB.TabIndex = 9;
            // 
            // ApplicationTypeCB
            // 
            this.ApplicationTypeCB.Dock = System.Windows.Forms.DockStyle.Left;
            this.ApplicationTypeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ApplicationTypeCB.FormattingEnabled = true;
            this.ApplicationTypeCB.Location = new System.Drawing.Point(146, 237);
            this.ApplicationTypeCB.Name = "ApplicationTypeCB";
            this.ApplicationTypeCB.Size = new System.Drawing.Size(151, 21);
            this.ApplicationTypeCB.TabIndex = 28;
            // 
            // BaseAddressesBTN
            // 
            this.BaseAddressesBTN.CurrentUrlsControl = this.BaseAddressesTB;
            this.BaseAddressesBTN.Dock = System.Windows.Forms.DockStyle.Right;
            this.BaseAddressesBTN.Location = new System.Drawing.Point(478, 418);
            this.BaseAddressesBTN.Margin = new System.Windows.Forms.Padding(1);
            this.BaseAddressesBTN.Name = "BaseAddressesBTN";
            this.BaseAddressesBTN.Size = new System.Drawing.Size(24, 24);
            this.BaseAddressesBTN.TabIndex = 34;
            this.BaseAddressesBTN.Urls = null;
            // 
            // Stage2PN
            // 
            this.Stage2PN.Controls.Add(this.Stage2BottomPN);
            this.Stage2PN.Controls.Add(this.Stage2LB);
            this.Stage2PN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stage2PN.Location = new System.Drawing.Point(0, 0);
            this.Stage2PN.Name = "Stage2PN";
            this.Stage2PN.Size = new System.Drawing.Size(503, 476);
            this.Stage2PN.TabIndex = 7;
            // 
            // Stage2LB
            // 
            this.Stage2LB.AutoSize = true;
            this.Stage2LB.Dock = System.Windows.Forms.DockStyle.Top;
            this.Stage2LB.Location = new System.Drawing.Point(0, 0);
            this.Stage2LB.Margin = new System.Windows.Forms.Padding(3);
            this.Stage2LB.Name = "Stage2LB";
            this.Stage2LB.Padding = new System.Windows.Forms.Padding(3, 8, 0, 8);
            this.Stage2LB.Size = new System.Drawing.Size(131, 29);
            this.Stage2LB.TabIndex = 0;
            this.Stage2LB.Text = "IDS_STAGE2_CONFIRM";
            this.Stage2LB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ApplicationWizardCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.Stage2PN);
            this.Controls.Add(this.Stage1PN);
            this.Controls.Add(this.ButtonsPN);
            this.Name = "ApplicationWizardCtrl";
            this.Size = new System.Drawing.Size(503, 505);
            this.Stage1PN.ResumeLayout(false);
            this.Stage1PN.PerformLayout();
            this.Stage1BottomPN.ResumeLayout(false);
            this.Stage1BottomPN.PerformLayout();
            this.ButtonsPN.ResumeLayout(false);
            this.Stage2BottomPN.ResumeLayout(false);
            this.Stage2BottomPN.PerformLayout();
            this.Stage2PN.ResumeLayout(false);
            this.Stage2PN.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton XmlConfigRB;
        private System.Windows.Forms.RadioButton ImportExportRB;
        private System.Windows.Forms.Label Stage1LB;
        private System.Windows.Forms.RadioButton ManualConfigRB;
        private System.Windows.Forms.ToolTip ApplicationTypeTP;
        private System.Windows.Forms.Panel Stage1PN;
        private System.Windows.Forms.FlowLayoutPanel ButtonsPN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Button BackBTN;
        private System.Windows.Forms.Button NextBTN;
        private System.Windows.Forms.TableLayoutPanel Stage2BottomPN;
        private System.Windows.Forms.Label ConfigurationFileLB;
        private System.Windows.Forms.Label ApplicationNameLB;
        private System.Windows.Forms.Label PublicKeyLB;
        private System.Windows.Forms.Label IssuerListLB;
        private System.Windows.Forms.Label TrustListLB;
        private System.Windows.Forms.Label PrivateKeyLB;
        private System.Windows.Forms.TextBox BaseAddressesTB;
        private System.Windows.Forms.TextBox IssuerListTB;
        private System.Windows.Forms.TextBox TrustListTB;
        private System.Windows.Forms.TextBox PrivateKeyTB;
        private System.Windows.Forms.TextBox PublicKeyTB;
        private System.Windows.Forms.TextBox ApplicationUriTB;
        private System.Windows.Forms.TextBox ApplicationNameTB;
        private System.Windows.Forms.Label BaseAddressesLB;
        private System.Windows.Forms.TextBox ConfigurationFileTB;
        private Opc.Ua.Client.Controls.SelectCertificateStoreCtrl IssuerListBTN;
        private Opc.Ua.Client.Controls.SelectCertificateStoreCtrl TrustListBTN;
        private Opc.Ua.Client.Controls.SelectFileCtrl PrivateKeyBTN;
        private Opc.Ua.Client.Controls.SelectFileCtrl ConfigurationFileBTN;
        private Opc.Ua.Client.Controls.SelectFileCtrl ImportExportUtilityBTN;
        private System.Windows.Forms.TextBox ImportExportUtilityTB;
        private System.Windows.Forms.Label ImportExportUtilityLB;
        private System.Windows.Forms.TextBox ExportArgumentsTB;
        private System.Windows.Forms.Label ExportArgumentsLB;
        private System.Windows.Forms.FlowLayoutPanel Stage1BottomPN;
        private System.Windows.Forms.Panel Stage2PN;
        private System.Windows.Forms.Label Stage2LB;
        private System.Windows.Forms.Label ApplicationUriLB;
        private System.Windows.Forms.ComboBox ApplicationTypeCB;
        private System.Windows.Forms.TextBox ImportArgumentsTB;
        private System.Windows.Forms.Label ImportArgumentsLB;
        private Opc.Ua.Client.Controls.SelectFileCtrl PublicKeyBTN;
        private System.Windows.Forms.Button DoneBTN;
        private Opc.Ua.Client.Controls.SelectCertificateStoreCtrl ApplicationCertificateStoreBTN;
        private System.Windows.Forms.TextBox ApplicationCertificateStoreTB;
        private System.Windows.Forms.Label ApplicationCertificateStoreLB;
        private System.Windows.Forms.Label ApplicationTypeLB;
        private Opc.Ua.Client.Controls.SelectUrlsCtrl BaseAddressesBTN;
        private Opc.Ua.Client.Controls.SelectProfileCtrl SecurityProfilesBTN;
        private System.Windows.Forms.TextBox SecurityProfilesTB;
        private System.Windows.Forms.Label SecurityProfilesLB;
        private System.Windows.Forms.TextBox ProductUriTB;
        private System.Windows.Forms.TextBox MachineNameTB;
        private System.Windows.Forms.Label ProductUriLB;
        private System.Windows.Forms.Label MachineNameLB;
        private System.Windows.Forms.TextBox SubjectNameTB;
        private System.Windows.Forms.Label SubjectNameLB;
    }
}
