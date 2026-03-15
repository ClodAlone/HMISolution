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
    partial class MainForm
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
            this.TopMenuStrip = new System.Windows.Forms.MenuStrip();
            this.GdsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Gds_ConnectMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Gds_DisconnectMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Gds_LoginMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Gds_LogoutMI = new System.Windows.Forms.ToolStripMenuItem();
            this.File_Seperator01 = new System.Windows.Forms.ToolStripSeparator();
            this.Gds_BrowseMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Gds_SearchMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.File_ExitMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMI = new System.Windows.Forms.ToolStripMenuItem();
            this.View_CertificateStoresMI = new System.Windows.Forms.ToolStripMenuItem();
            this.View_CertificateRequestsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.View_ComPseudoServersMI = new System.Windows.Forms.ToolStripMenuItem();
            this.View_ManageHttpAccessRulesMI = new System.Windows.Forms.ToolStripMenuItem();
            this.View_ManagedSslBindingsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.View_MessagesMI = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Help_AboutMI = new System.Windows.Forms.ToolStripMenuItem();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.GdsStatusLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.GdsServerStatusTB = new System.Windows.Forms.ToolStripStatusLabel();
            this.GdsUpdateTimeTB = new System.Windows.Forms.ToolStripStatusLabel();
            this.GdsUserLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.GdsUserTB = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainPN = new System.Windows.Forms.Panel();
            this.ApplicationsCTRL = new Opc.Ua.GdsLocalAgent.GdsAgentApplicationsListCtrl();
            this.ApplicationMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Application_NewMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_EditMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_DeleteMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_Separator01 = new System.Windows.Forms.ToolStripSeparator();
            this.Application_ViewTrustMatrixMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_CreateCertificateMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_FirewallMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_RestartServiceMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_InstallHttpsRootMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_Separator02 = new System.Windows.Forms.ToolStripSeparator();
            this.Application_RegisterMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_RequestCertificateMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_AbandonRequestMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_RequestHttpsCertificateMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_CheckRequestStatusMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_UpdateTrustListsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.Application_RevokeCertificateMI = new System.Windows.Forms.ToolStripMenuItem();
            this.ApplicationWizardCTRL = new Opc.Ua.GdsLocalAgent.ApplicationWizardCtrl();
            this.SynchTimer = new System.Windows.Forms.Timer(this.components);
            this.ContainerPN = new System.Windows.Forms.SplitContainer();
            this.LogCTRL = new System.Windows.Forms.RichTextBox();
            this.ServerCTRL = new Opc.Ua.Client.Controls.ConnectServerCtrl();
            this.TopMenuStrip.SuspendLayout();
            this.MainStatusStrip.SuspendLayout();
            this.MainPN.SuspendLayout();
            this.ApplicationMenuStrip.SuspendLayout();
            this.ContainerPN.Panel1.SuspendLayout();
            this.ContainerPN.Panel2.SuspendLayout();
            this.ContainerPN.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopMenuStrip
            // 
            this.TopMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GdsMI,
            this.ViewMI,
            this.HelpMI});
            this.TopMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.TopMenuStrip.Name = "TopMenuStrip";
            this.TopMenuStrip.Size = new System.Drawing.Size(802, 24);
            this.TopMenuStrip.TabIndex = 0;
            // 
            // GdsMI
            // 
            this.GdsMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Gds_ConnectMI,
            this.Gds_DisconnectMI,
            this.Gds_LoginMI,
            this.Gds_LogoutMI,
            this.File_Seperator01,
            this.Gds_BrowseMI,
            this.Gds_SearchMI,
            this.toolStripSeparator2,
            this.File_ExitMI});
            this.GdsMI.Name = "GdsMI";
            this.GdsMI.Size = new System.Drawing.Size(41, 20);
            this.GdsMI.Text = "GDS";
            this.GdsMI.DropDownOpening += new System.EventHandler(this.GdsMI_DropDownOpening);
            // 
            // Gds_ConnectMI
            // 
            this.Gds_ConnectMI.Name = "Gds_ConnectMI";
            this.Gds_ConnectMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_ConnectMI.Text = "Connect...";
            this.Gds_ConnectMI.Click += new System.EventHandler(this.Gds_ConnectMI_Click);
            // 
            // Gds_DisconnectMI
            // 
            this.Gds_DisconnectMI.Name = "Gds_DisconnectMI";
            this.Gds_DisconnectMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_DisconnectMI.Text = "Disconnect";
            this.Gds_DisconnectMI.Click += new System.EventHandler(this.Gds_DisconnectMI_Click);
            // 
            // Gds_LoginMI
            // 
            this.Gds_LoginMI.Name = "Gds_LoginMI";
            this.Gds_LoginMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_LoginMI.Text = "Login As User...";
            this.Gds_LoginMI.Click += new System.EventHandler(this.Gds_LoginMI_Click);
            // 
            // Gds_LogoutMI
            // 
            this.Gds_LogoutMI.Name = "Gds_LogoutMI";
            this.Gds_LogoutMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_LogoutMI.Text = "Logout";
            this.Gds_LogoutMI.Click += new System.EventHandler(this.Gds_LogoutMI_Click);
            // 
            // File_Seperator01
            // 
            this.File_Seperator01.Name = "File_Seperator01";
            this.File_Seperator01.Size = new System.Drawing.Size(152, 6);
            // 
            // Gds_BrowseMI
            // 
            this.Gds_BrowseMI.Name = "Gds_BrowseMI";
            this.Gds_BrowseMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_BrowseMI.Text = "Browse...";
            this.Gds_BrowseMI.Click += new System.EventHandler(this.Gds_BrowseMI_Click);
            // 
            // Gds_SearchMI
            // 
            this.Gds_SearchMI.Name = "Gds_SearchMI";
            this.Gds_SearchMI.Size = new System.Drawing.Size(155, 22);
            this.Gds_SearchMI.Text = "Search...";
            this.Gds_SearchMI.Click += new System.EventHandler(this.Gds_SearchMI_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
            // 
            // File_ExitMI
            // 
            this.File_ExitMI.Name = "File_ExitMI";
            this.File_ExitMI.Size = new System.Drawing.Size(155, 22);
            this.File_ExitMI.Text = "Exit";
            // 
            // ViewMI
            // 
            this.ViewMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.View_CertificateStoresMI,
            this.View_CertificateRequestsMI,
            this.View_ComPseudoServersMI,
            this.View_ManageHttpAccessRulesMI,
            this.View_ManagedSslBindingsMI,
            this.toolStripSeparator1,
            this.View_MessagesMI});
            this.ViewMI.Name = "ViewMI";
            this.ViewMI.Size = new System.Drawing.Size(44, 20);
            this.ViewMI.Text = "View";
            this.ViewMI.DropDownOpening += new System.EventHandler(this.ViewMI_DropDownOpening);
            // 
            // View_CertificateStoresMI
            // 
            this.View_CertificateStoresMI.Name = "View_CertificateStoresMI";
            this.View_CertificateStoresMI.Size = new System.Drawing.Size(203, 22);
            this.View_CertificateStoresMI.Text = "Local Certificate Stores...";
            this.View_CertificateStoresMI.Click += new System.EventHandler(this.View_CertificateStoresMI_Click);
            // 
            // View_CertificateRequestsMI
            // 
            this.View_CertificateRequestsMI.Name = "View_CertificateRequestsMI";
            this.View_CertificateRequestsMI.Size = new System.Drawing.Size(203, 22);
            this.View_CertificateRequestsMI.Text = "Certficate Requests...";
            this.View_CertificateRequestsMI.Click += new System.EventHandler(this.View_CertificateRequestsMI_Click);
            // 
            // View_ComPseudoServersMI
            // 
            this.View_ComPseudoServersMI.Name = "View_ComPseudoServersMI";
            this.View_ComPseudoServersMI.Size = new System.Drawing.Size(203, 22);
            this.View_ComPseudoServersMI.Text = "COM Pseudo Servers...";
            this.View_ComPseudoServersMI.Click += new System.EventHandler(this.View_ComPseudoServersMI_Click);
            // 
            // View_ManageHttpAccessRulesMI
            // 
            this.View_ManageHttpAccessRulesMI.Name = "View_ManageHttpAccessRulesMI";
            this.View_ManageHttpAccessRulesMI.Size = new System.Drawing.Size(203, 22);
            this.View_ManageHttpAccessRulesMI.Text = "HTTP Access Rules...";
            this.View_ManageHttpAccessRulesMI.Click += new System.EventHandler(this.View_ManageHttpAccessRulesMI_Click);
            // 
            // View_ManagedSslBindingsMI
            // 
            this.View_ManagedSslBindingsMI.Name = "View_ManagedSslBindingsMI";
            this.View_ManagedSslBindingsMI.Size = new System.Drawing.Size(203, 22);
            this.View_ManagedSslBindingsMI.Text = "SSL Bindings...";
            this.View_ManagedSslBindingsMI.Click += new System.EventHandler(this.View_ManagedSslBindingsMI_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // View_MessagesMI
            // 
            this.View_MessagesMI.Checked = true;
            this.View_MessagesMI.CheckOnClick = true;
            this.View_MessagesMI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.View_MessagesMI.Name = "View_MessagesMI";
            this.View_MessagesMI.Size = new System.Drawing.Size(203, 22);
            this.View_MessagesMI.Text = "Show Message Pane";
            this.View_MessagesMI.Click += new System.EventHandler(this.View_MessagesMI_Click);
            // 
            // HelpMI
            // 
            this.HelpMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Help_AboutMI});
            this.HelpMI.Name = "HelpMI";
            this.HelpMI.Size = new System.Drawing.Size(44, 20);
            this.HelpMI.Text = "Help";
            // 
            // Help_AboutMI
            // 
            this.Help_AboutMI.Name = "Help_AboutMI";
            this.Help_AboutMI.Size = new System.Drawing.Size(152, 22);
            this.Help_AboutMI.Text = "About...";
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GdsStatusLB,
            this.GdsServerStatusTB,
            this.GdsUpdateTimeTB,
            this.GdsUserLB,
            this.GdsUserTB});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 537);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(802, 22);
            this.MainStatusStrip.TabIndex = 1;
            // 
            // GdsStatusLB
            // 
            this.GdsStatusLB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.GdsStatusLB.Name = "GdsStatusLB";
            this.GdsStatusLB.Size = new System.Drawing.Size(101, 17);
            this.GdsStatusLB.Text = "GDS Connection:";
            // 
            // GdsServerStatusTB
            // 
            this.GdsServerStatusTB.Name = "GdsServerStatusTB";
            this.GdsServerStatusTB.Size = new System.Drawing.Size(48, 17);
            this.GdsServerStatusTB.Text = "<state>";
            // 
            // GdsUpdateTimeTB
            // 
            this.GdsUpdateTimeTB.Name = "GdsUpdateTimeTB";
            this.GdsUpdateTimeTB.Size = new System.Drawing.Size(47, 17);
            this.GdsUpdateTimeTB.Text = "<time>";
            // 
            // GdsUserLB
            // 
            this.GdsUserLB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.GdsUserLB.Name = "GdsUserLB";
            this.GdsUserLB.Size = new System.Drawing.Size(64, 17);
            this.GdsUserLB.Text = "GDS User:";
            // 
            // GdsUserTB
            // 
            this.GdsUserTB.Name = "GdsUserTB";
            this.GdsUserTB.Size = new System.Drawing.Size(45, 17);
            this.GdsUserTB.Text = "<user>";
            // 
            // MainPN
            // 
            this.MainPN.Controls.Add(this.ApplicationsCTRL);
            this.MainPN.Controls.Add(this.ApplicationWizardCTRL);
            this.MainPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPN.Location = new System.Drawing.Point(0, 0);
            this.MainPN.Name = "MainPN";
            this.MainPN.Size = new System.Drawing.Size(802, 371);
            this.MainPN.TabIndex = 2;
            // 
            // ApplicationsCTRL
            // 
            this.ApplicationsCTRL.ContextMenuStrip = this.ApplicationMenuStrip;
            this.ApplicationsCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationsCTRL.Location = new System.Drawing.Point(0, 0);
            this.ApplicationsCTRL.Name = "ApplicationsCTRL";
            this.ApplicationsCTRL.Size = new System.Drawing.Size(802, 371);
            this.ApplicationsCTRL.TabIndex = 2;
            // 
            // ApplicationMenuStrip
            // 
            this.ApplicationMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Application_NewMI,
            this.Application_EditMI,
            this.Application_DeleteMI,
            this.Application_Separator01,
            this.Application_ViewTrustMatrixMI,
            this.Application_CreateCertificateMI,
            this.Application_FirewallMI,
            this.Application_RestartServiceMI,
            this.Application_InstallHttpsRootMI,
            this.Application_Separator02,
            this.Application_RegisterMI,
            this.Application_RequestCertificateMI,
            this.Application_AbandonRequestMI,
            this.Application_RequestHttpsCertificateMI,
            this.Application_CheckRequestStatusMI,
            this.Application_UpdateTrustListsMI,
            this.Application_RevokeCertificateMI});
            this.ApplicationMenuStrip.Name = "ApplicationMenuStrip";
            this.ApplicationMenuStrip.Size = new System.Drawing.Size(244, 346);
            this.ApplicationMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ApplicationMenuStrip_Opening);
            // 
            // Application_NewMI
            // 
            this.Application_NewMI.Name = "Application_NewMI";
            this.Application_NewMI.Size = new System.Drawing.Size(243, 22);
            this.Application_NewMI.Text = "New...";
            this.Application_NewMI.Click += new System.EventHandler(this.Application_NewMI_Click);
            // 
            // Application_EditMI
            // 
            this.Application_EditMI.Name = "Application_EditMI";
            this.Application_EditMI.Size = new System.Drawing.Size(243, 22);
            this.Application_EditMI.Text = "Edit...";
            this.Application_EditMI.Click += new System.EventHandler(this.Application_EditMI_Click);
            // 
            // Application_DeleteMI
            // 
            this.Application_DeleteMI.Name = "Application_DeleteMI";
            this.Application_DeleteMI.Size = new System.Drawing.Size(243, 22);
            this.Application_DeleteMI.Text = "Delete...";
            this.Application_DeleteMI.Click += new System.EventHandler(this.Application_DeleteMI_Click);
            // 
            // Application_Separator01
            // 
            this.Application_Separator01.Name = "Application_Separator01";
            this.Application_Separator01.Size = new System.Drawing.Size(240, 6);
            // 
            // Application_ViewTrustMatrixMI
            // 
            this.Application_ViewTrustMatrixMI.Name = "Application_ViewTrustMatrixMI";
            this.Application_ViewTrustMatrixMI.Size = new System.Drawing.Size(243, 22);
            this.Application_ViewTrustMatrixMI.Text = "View Trust Matrix...";
            this.Application_ViewTrustMatrixMI.Click += new System.EventHandler(this.Application_ViewTrustMatrixMI_Click);
            // 
            // Application_CreateCertificateMI
            // 
            this.Application_CreateCertificateMI.Name = "Application_CreateCertificateMI";
            this.Application_CreateCertificateMI.Size = new System.Drawing.Size(243, 22);
            this.Application_CreateCertificateMI.Text = "Create Self-Signed Certificate...";
            this.Application_CreateCertificateMI.Click += new System.EventHandler(this.Application_CreateCertificateMI_Click);
            // 
            // Application_FirewallMI
            // 
            this.Application_FirewallMI.Name = "Application_FirewallMI";
            this.Application_FirewallMI.Size = new System.Drawing.Size(243, 22);
            this.Application_FirewallMI.Text = "Set Permissions...";
            this.Application_FirewallMI.Click += new System.EventHandler(this.Application_FirewallMI_Click);
            // 
            // Application_RestartServiceMI
            // 
            this.Application_RestartServiceMI.Name = "Application_RestartServiceMI";
            this.Application_RestartServiceMI.Size = new System.Drawing.Size(243, 22);
            this.Application_RestartServiceMI.Text = "Restart Service...";
            this.Application_RestartServiceMI.Click += new System.EventHandler(this.Application_RestartServiceMI_Click);
            // 
            // Application_InstallHttpsRootMI
            // 
            this.Application_InstallHttpsRootMI.Name = "Application_InstallHttpsRootMI";
            this.Application_InstallHttpsRootMI.Size = new System.Drawing.Size(243, 22);
            this.Application_InstallHttpsRootMI.Text = "Install/Update HTTPS Root CA...";
            this.Application_InstallHttpsRootMI.Click += new System.EventHandler(this.Application_InstallHttpsRootMI_Click);
            // 
            // Application_Separator02
            // 
            this.Application_Separator02.Name = "Application_Separator02";
            this.Application_Separator02.Size = new System.Drawing.Size(240, 6);
            // 
            // Application_RegisterMI
            // 
            this.Application_RegisterMI.Name = "Application_RegisterMI";
            this.Application_RegisterMI.Size = new System.Drawing.Size(243, 22);
            this.Application_RegisterMI.Text = "Register";
            this.Application_RegisterMI.Click += new System.EventHandler(this.Application_RegisterMI_Click);
            // 
            // Application_RequestCertificateMI
            // 
            this.Application_RequestCertificateMI.Name = "Application_RequestCertificateMI";
            this.Application_RequestCertificateMI.Size = new System.Drawing.Size(243, 22);
            this.Application_RequestCertificateMI.Text = "Request Certificate";
            this.Application_RequestCertificateMI.Click += new System.EventHandler(this.Application_RequestCertificateMI_Click);
            // 
            // Application_AbandonRequestMI
            // 
            this.Application_AbandonRequestMI.Name = "Application_AbandonRequestMI";
            this.Application_AbandonRequestMI.Size = new System.Drawing.Size(243, 22);
            this.Application_AbandonRequestMI.Text = "Abandon Certificate Request";
            this.Application_AbandonRequestMI.Click += new System.EventHandler(this.Application_AbandonRequestMI_Click);
            // 
            // Application_RequestHttpsCertificateMI
            // 
            this.Application_RequestHttpsCertificateMI.Name = "Application_RequestHttpsCertificateMI";
            this.Application_RequestHttpsCertificateMI.Size = new System.Drawing.Size(243, 22);
            this.Application_RequestHttpsCertificateMI.Text = "Request HTTPS Certfiicate...";
            this.Application_RequestHttpsCertificateMI.Click += new System.EventHandler(this.Application_RequestCertificateMI_Click);
            // 
            // Application_CheckRequestStatusMI
            // 
            this.Application_CheckRequestStatusMI.Name = "Application_CheckRequestStatusMI";
            this.Application_CheckRequestStatusMI.Size = new System.Drawing.Size(243, 22);
            this.Application_CheckRequestStatusMI.Text = "Check Request Status";
            this.Application_CheckRequestStatusMI.Click += new System.EventHandler(this.Application_CheckRequestStatusMI_Click);
            // 
            // Application_UpdateTrustListsMI
            // 
            this.Application_UpdateTrustListsMI.Name = "Application_UpdateTrustListsMI";
            this.Application_UpdateTrustListsMI.Size = new System.Drawing.Size(243, 22);
            this.Application_UpdateTrustListsMI.Text = "Update Trust Lists...";
            this.Application_UpdateTrustListsMI.Click += new System.EventHandler(this.Application_UpdateTrustListsMI_Click);
            // 
            // Application_RevokeCertificateMI
            // 
            this.Application_RevokeCertificateMI.Name = "Application_RevokeCertificateMI";
            this.Application_RevokeCertificateMI.Size = new System.Drawing.Size(243, 22);
            this.Application_RevokeCertificateMI.Text = "Revoke Certificate...";
            this.Application_RevokeCertificateMI.Click += new System.EventHandler(this.Application_RevokeCertificateMI_Click);
            // 
            // ApplicationWizardCTRL
            // 
            this.ApplicationWizardCTRL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ApplicationWizardCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ApplicationWizardCTRL.Location = new System.Drawing.Point(0, 0);
            this.ApplicationWizardCTRL.Name = "ApplicationWizardCTRL";
            this.ApplicationWizardCTRL.Size = new System.Drawing.Size(802, 371);
            this.ApplicationWizardCTRL.TabIndex = 1;
            this.ApplicationWizardCTRL.WizardComplete += new System.EventHandler(this.ApplicationWizardCTRL_WizardComplete);
            this.ApplicationWizardCTRL.WizardCancelled += new System.EventHandler(this.ApplicationWizardCTRL_WizardCancelled);
            // 
            // SynchTimer
            // 
            this.SynchTimer.Interval = 20000;
            // 
            // ContainerPN
            // 
            this.ContainerPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainerPN.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.ContainerPN.Location = new System.Drawing.Point(0, 47);
            this.ContainerPN.Name = "ContainerPN";
            this.ContainerPN.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ContainerPN.Panel1
            // 
            this.ContainerPN.Panel1.Controls.Add(this.MainPN);
            // 
            // ContainerPN.Panel2
            // 
            this.ContainerPN.Panel2.Controls.Add(this.LogCTRL);
            this.ContainerPN.Size = new System.Drawing.Size(802, 490);
            this.ContainerPN.SplitterDistance = 371;
            this.ContainerPN.TabIndex = 3;
            // 
            // LogCTRL
            // 
            this.LogCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogCTRL.Location = new System.Drawing.Point(0, 0);
            this.LogCTRL.Name = "LogCTRL";
            this.LogCTRL.Size = new System.Drawing.Size(802, 115);
            this.LogCTRL.TabIndex = 0;
            this.LogCTRL.Text = "";
            // 
            // ServerCTRL
            // 
            this.ServerCTRL.Configuration = null;
            this.ServerCTRL.DisableDomainCheck = false;
            this.ServerCTRL.Dock = System.Windows.Forms.DockStyle.Top;
            this.ServerCTRL.Location = new System.Drawing.Point(0, 24);
            this.ServerCTRL.MaximumSize = new System.Drawing.Size(2048, 23);
            this.ServerCTRL.MinimumSize = new System.Drawing.Size(500, 23);
            this.ServerCTRL.Name = "ServerCTRL";
            this.ServerCTRL.PreferredLocales = null;
            this.ServerCTRL.ServerStatusControl = this.GdsServerStatusTB;
            this.ServerCTRL.ServerUrl = "";
            this.ServerCTRL.SessionName = null;
            this.ServerCTRL.Size = new System.Drawing.Size(802, 23);
            this.ServerCTRL.StatusStrip = null;
            this.ServerCTRL.StatusUpateTimeControl = this.GdsUpdateTimeTB;
            this.ServerCTRL.TabIndex = 3;
            this.ServerCTRL.UserIdentity = null;
            this.ServerCTRL.UseSecurity = true;
            this.ServerCTRL.Visible = false;
            this.ServerCTRL.ConnectComplete += new System.EventHandler(this.ServerCTRL_ConnectComplete);
            this.ServerCTRL.ReconnectStarting += new System.EventHandler(this.ServerCTRL_ReconnectStarting);
            this.ServerCTRL.ReconnectComplete += new System.EventHandler(this.ServerCTRL_ReconnectComplete);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 559);
            this.Controls.Add(this.ContainerPN);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.ServerCTRL);
            this.Controls.Add(this.TopMenuStrip);
            this.Name = "MainForm";
            this.Text = "Global Directory Service Local Agent";
            this.TopMenuStrip.ResumeLayout(false);
            this.TopMenuStrip.PerformLayout();
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.MainPN.ResumeLayout(false);
            this.ApplicationMenuStrip.ResumeLayout(false);
            this.ContainerPN.Panel1.ResumeLayout(false);
            this.ContainerPN.Panel2.ResumeLayout(false);
            this.ContainerPN.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem GdsMI;
        private System.Windows.Forms.ToolStripMenuItem Gds_LoginMI;
        private System.Windows.Forms.ToolStripMenuItem Gds_LogoutMI;
        private System.Windows.Forms.ToolStripSeparator File_Seperator01;
        private System.Windows.Forms.ToolStripMenuItem File_ExitMI;
        private System.Windows.Forms.ToolStripMenuItem HelpMI;
        private System.Windows.Forms.ToolStripMenuItem Help_AboutMI;
        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.Panel MainPN;
        private System.Windows.Forms.ContextMenuStrip ApplicationMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Application_NewMI;
        private ApplicationWizardCtrl ApplicationWizardCTRL;
        private GdsAgentApplicationsListCtrl ApplicationsCTRL;
        private System.Windows.Forms.ToolStripMenuItem Application_EditMI;
        private System.Windows.Forms.ToolStripMenuItem Application_DeleteMI;
        private System.Windows.Forms.ToolStripMenuItem Application_RegisterMI;
        private System.Windows.Forms.ToolStripSeparator Application_Separator01;
        private System.Windows.Forms.ToolStripStatusLabel GdsUpdateTimeTB;
        private System.Windows.Forms.Timer SynchTimer;
        private System.Windows.Forms.SplitContainer ContainerPN;
        private System.Windows.Forms.RichTextBox LogCTRL;
        private System.Windows.Forms.ToolStripMenuItem Gds_ConnectMI;
        private System.Windows.Forms.ToolStripMenuItem Gds_DisconnectMI;
        private System.Windows.Forms.ToolStripStatusLabel GdsUserLB;
        private System.Windows.Forms.ToolStripStatusLabel GdsUserTB;
        private Opc.Ua.Client.Controls.ConnectServerCtrl ServerCTRL;
        private System.Windows.Forms.ToolStripStatusLabel GdsServerStatusTB;
        private System.Windows.Forms.ToolStripStatusLabel GdsStatusLB;
        private System.Windows.Forms.ToolStripMenuItem ViewMI;
        private System.Windows.Forms.ToolStripMenuItem View_CertificateRequestsMI;
        private System.Windows.Forms.ToolStripMenuItem View_MessagesMI;
        private System.Windows.Forms.ToolStripMenuItem Application_RequestCertificateMI;
        private System.Windows.Forms.ToolStripMenuItem Application_CheckRequestStatusMI;
        private System.Windows.Forms.ToolStripMenuItem Application_UpdateTrustListsMI;
        private System.Windows.Forms.ToolStripMenuItem View_CertificateStoresMI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Application_RevokeCertificateMI;
        private System.Windows.Forms.ToolStripMenuItem Gds_BrowseMI;
        private System.Windows.Forms.ToolStripMenuItem Gds_SearchMI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Application_FirewallMI;
        private System.Windows.Forms.ToolStripMenuItem Application_RequestHttpsCertificateMI;
        private System.Windows.Forms.ToolStripMenuItem View_ManagedSslBindingsMI;
        private System.Windows.Forms.ToolStripMenuItem Application_RestartServiceMI;
        private System.Windows.Forms.ToolStripMenuItem Application_AbandonRequestMI;
        private System.Windows.Forms.ToolStripMenuItem View_ManageHttpAccessRulesMI;
        private System.Windows.Forms.ToolStripMenuItem View_ComPseudoServersMI;
        private System.Windows.Forms.ToolStripMenuItem Application_CreateCertificateMI;
        private System.Windows.Forms.ToolStripMenuItem Application_ViewTrustMatrixMI;
        private System.Windows.Forms.ToolStripSeparator Application_Separator02;
        private System.Windows.Forms.ToolStripMenuItem Application_InstallHttpsRootMI;
    }
}
