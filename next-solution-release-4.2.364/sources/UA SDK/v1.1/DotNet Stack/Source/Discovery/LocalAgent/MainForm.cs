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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Configuration;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    public partial class MainForm : Form
    {
        public MainForm(ApplicationConfiguration configuration)
        {
            InitializeComponent();
            
            m_LogMessageHandler = new LogMessageEventHandler(GdsAgent_LogMessage);

            m_configuration = configuration;
            ServerCTRL.Configuration = configuration;

            m_agent = new GdsAgent(configuration);
            m_agent.LogMessage += m_LogMessageHandler;
            m_agent.LoadApplications();

            // get the GDS server url.
            m_agentConfiguration = m_configuration.ParseExtension<GdsAgentConfiguration>();
            ServerCTRL.ServerUrl = m_agentConfiguration.ServerUrl;
            ServerCTRL.Disconnect();

            ApplicationsCTRL.Initialize(m_agent.Applications);
            Gds_LogoutMI_Click(this, null);

            this.Text = "OPC UA Global Directory Service Agent";
            this.Icon = ConfigUtils.GetAppIcon();
        }

        private ApplicationConfiguration m_configuration;
        private GdsAgent m_agent;
        private GdsAgentApplication m_application;
        private GdsAgentConfiguration m_agentConfiguration;
        private LogMessageEventHandler m_LogMessageHandler;
        private MonitorRequestsDlg MonitorRequestsDLG;
        private SearchServersDlg SearchServersDLG;

        private bool IsConnected
        {
            get
            {
                return (ServerCTRL.Session != null && ServerCTRL.Session.Connected);
            }
        }

        private bool IsLoggedIn
        {
            get
            {
                return (ServerCTRL.Session != null && ServerCTRL.Session.Identity != null && ServerCTRL.Session.Identity.TokenType != UserTokenType.Anonymous);
            }
        }

        private void GdsAgent_LogMessage(object sender, LogMessageEventArg args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(m_LogMessageHandler, sender, args);
                return;
            }

            LogCTRL.AppendText(Utils.Format("{0:hh:mm:ss} {1}\r\n", DateTime.Now, args.Message));
            LogCTRL.ScrollToCaret();
        }

        private void Application_NewMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = null;
                ApplicationsCTRL.Visible = false;
                ApplicationWizardCTRL.Start(null);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_EditMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null)
                {
                    ApplicationsCTRL.Visible = false;
                    ApplicationWizardCTRL.Start(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_DeleteMI_Click(object sender, EventArgs e)
        {
            try
            {
                while ((m_application = ApplicationsCTRL.GetSelectedApplication(0)) != null)
                {
                    ApplicationsCTRL.Remove(m_application);
                    m_agent.DeleteApplication(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ApplicationWizardCTRL_WizardCancelled(object sender, EventArgs e)
        {
            ApplicationsCTRL.Visible = true;
        }

        private void ApplicationWizardCTRL_WizardComplete(object sender, EventArgs e)
        {
            try
            {
                if (m_application == null)
                {
                    m_application = new GdsAgentApplication();
                }

                ApplicationWizardCTRL.Finish(m_application);
                ApplicationsCTRL.AddOrUpdate(m_application);
                ApplicationsCTRL.Visible = true;

                m_agent.SaveApplication(m_application);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_RegisterMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null)
                {
                    if (!IsConnected)
                    {
                        ServerCTRL.Connect();
                    }

                    if (!IsLoggedIn)
                    {
                        Gds_LoginMI_Click(sender, e);
                    }

                    m_agent.RegisterApplication(m_application);
                    ApplicationsCTRL.AddOrUpdate(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_RequestCertificateMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null && m_application.RegisteredWithGds)
                {
                    if (!IsConnected)
                    {
                        ServerCTRL.Connect();
                    }

                    if (!IsLoggedIn)
                    {
                        Gds_LoginMI_Click(sender, e);
                    }

                    m_agent.RequestCertificate(m_application, Object.ReferenceEquals(sender, Application_RequestHttpsCertificateMI));
                    ApplicationsCTRL.AddOrUpdate(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_CheckRequestStatusMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null && !NodeId.IsNull(m_application.LastCertificateRequestId))
                {
                    if (!IsConnected)
                    {
                        ServerCTRL.Connect();
                    }

                    m_agent.CheckCertificateStatus(m_application);
                    ApplicationsCTRL.AddOrUpdate(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private bool StoreSupportsCRLs(Opc.Ua.Security.CertificateStoreIdentifier store)
        {
            if (store == null)
            {
                return true;
            }

            if (store.StoreType != CertificateStoreType.Directory)
            {
                return false;
            }
                
            return true;
        }

        private void Application_UpdateTrustListsMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null && m_application.RegisteredWithGds)
                {
                    if (!IsConnected)
                    {
                        ServerCTRL.Connect();
                    }

                    DialogResult result = new YesNoDlg().ShowDialog(
                        "Would you like to delete the existing contents of the trust lists?\r\nIt may break other applications.",
                        "Update Trust Lists");

                    bool deleteExisting = result == DialogResult.Yes;

                    if (!StoreSupportsCRLs(m_application.SecuritySettings.TrustedCertificateStore) || !StoreSupportsCRLs(m_application.SecuritySettings.IssuerCertificateStore))
                    {
                        result = new YesNoDlg().ShowDialog(
                            "If the GDS provides CRLs they cannot be used because CRLS in Windows stores are not supported at this time.\r\nWould you like to continue?",
                            "Update Trust Lists");

                        if (result != DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    m_agent.UpdateTrustLists(m_application, false, deleteExisting);
                    ApplicationsCTRL.AddOrUpdate(m_application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_RevokeCertificateMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (m_application != null && m_application.RegisteredWithGds)
                {
                    if (!IsConnected)
                    {
                        ServerCTRL.Connect();
                    }

                    if (!IsLoggedIn)
                    {
                        Gds_LoginMI_Click(sender, e);
                    }

                    DialogResult result = new YesNoDlg().ShowDialog(
                        "Are you sure you would like to revoke the application's certificate?",
                        "Revoke Certificate");

                    if (result == DialogResult.Yes)
                    {
                        m_agent.RevokeCertificate(m_application);
                        ApplicationsCTRL.AddOrUpdate(m_application);
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_ConnectMI_Click(object sender, EventArgs e)
        {
            try
            {
                Uri serverUrl = new SetUrlDlg().ShowDialog(new Uri(ServerCTRL.ServerUrl), null);

                if (serverUrl != null)
                {
                    // save the GDS server url.
                    GdsAgentConfiguration agentConfiguration = m_configuration.ParseExtension<GdsAgentConfiguration>();
                    ServerCTRL.ServerUrl = agentConfiguration.ServerUrl;
                    m_configuration.UpdateExtension<GdsAgentConfiguration>(null, agentConfiguration);
                    m_configuration.SaveToFile(m_configuration.SourceFilePath);
          
                    ServerCTRL.Connect(serverUrl.ToString(), true);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_DisconnectMI_Click(object sender, EventArgs e)
        {
            try
            {
                ServerCTRL.Disconnect();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_LoginMI_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsConnected)
                {
                    UserIdentity identity = new UserNamePasswordDlg().ShowDialog(ServerCTRL.UserIdentity, null);

                    if (identity != null)
                    {
                        ServerCTRL.UserIdentity = identity;
                        ServerCTRL.Session.UpdateSession(identity, ServerCTRL.PreferredLocales);
                        GdsUserTB.Text = identity.DisplayName;
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_LogoutMI_Click(object sender, EventArgs e)
        {
            try
            {
                ServerCTRL.UserIdentity = new UserIdentity();

                if (IsLoggedIn)
                {
                    ServerCTRL.Session.UpdateSession(ServerCTRL.UserIdentity, ServerCTRL.PreferredLocales);
                }

                GdsUserTB.Text = ServerCTRL.UserIdentity.DisplayName;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ServerCTRL_ConnectComplete(object sender, EventArgs e)
        {
            try
            {
                if (ServerCTRL.Session != null)
                {
                    ServerCTRL.Session.ReturnDiagnostics = DiagnosticsMasks.SymbolicIdAndText;
                }

                m_agent.ChangeSession(ServerCTRL.Session);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ServerCTRL_ReconnectStarting(object sender, EventArgs e)
        {
            try
            {
                m_agent.ChangeSession(null);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ServerCTRL_ReconnectComplete(object sender, EventArgs e)
        {
            try
            {
                m_agent.ChangeSession(ServerCTRL.Session);

                if (MonitorRequestsDLG != null)
                {
                    MonitorRequestsDLG.Reconnect(ServerCTRL.Session);
                }

                if (SearchServersDLG != null)
                {
                    SearchServersDLG.Reconnect(ServerCTRL.Session);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void View_MessagesMI_Click(object sender, EventArgs e)
        {
            ContainerPN.Panel2Collapsed = !View_MessagesMI.Checked;
        }

        private void View_CertificateRequestsMI_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                if (MonitorRequestsDLG == null)
                {
                    if (!IsLoggedIn)
                    {
                        Gds_LoginMI_Click(sender, e);
                    }

                    MonitorRequestsDLG = new MonitorRequestsDlg();
                    MonitorRequestsDLG.FormClosing += new FormClosingEventHandler(MonitorRequestsDLG_FormClosing);
                }

                MonitorRequestsDLG.Show(ServerCTRL.Session);
                MonitorRequestsDLG.BringToFront();
            }
        }

        private void MonitorRequestsDLG_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MonitorRequestsDLG = null;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void GdsMI_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                Gds_DisconnectMI.Enabled = IsConnected;
                Gds_LoginMI.Enabled = IsConnected;
                Gds_LogoutMI.Enabled = IsLoggedIn;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ViewMI_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                View_CertificateRequestsMI.Enabled = IsConnected;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void ApplicationMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                Application_EditMI.Visible = application != null;
                Application_DeleteMI.Visible = application != null;
                Application_Separator01.Visible = application != null;
                Application_FirewallMI.Visible = application != null;
                Application_ViewTrustMatrixMI.Visible = application != null;
                Application_CreateCertificateMI.Visible = application != null; 
                Application_RestartServiceMI.Visible = application != null && ServiceManager.ServiceExists(application.ApplicationName);
                Application_Separator02.Visible = application != null;
                Application_RegisterMI.Visible = application != null;
                Application_RequestCertificateMI.Visible = application != null && application.RegisteredWithGds && NodeId.IsNull(application.LastCertificateRequestId);
                Application_RequestHttpsCertificateMI.Visible = application != null && application.RegisteredWithGds && NodeId.IsNull(application.LastCertificateRequestId);
                Application_AbandonRequestMI.Visible = application != null && !NodeId.IsNull(application.LastCertificateRequestId);
                Application_CheckRequestStatusMI.Visible = application != null && !NodeId.IsNull(application.LastCertificateRequestId);
                Application_UpdateTrustListsMI.Visible = application != null && application.RegisteredWithGds;
                Application_RevokeCertificateMI.Visible = application != null && application.RegisteredWithGds;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void View_CertificateStoresMI_Click(object sender, EventArgs e)
        {
            try
            {
                CertificateStoreIdentifier csid = new CertificateStoreIdentifier();
                csid.StoreType = Utils.DefaultStoreType;
                csid.StorePath = Utils.DefaultStorePath;
                new CertificateListDlg().ShowDialog(csid, true);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_BrowseMI_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsConnected)
                {
                    ServerCTRL.Connect();
                }

                new SelectNodeDlg().ShowDialog(
                    ServerCTRL.Session,
                    ExpandedNodeId.ToNodeId(Opc.Ua.Gds.ObjectIds.Directory_Applications, ServerCTRL.Session.NamespaceUris),
                    null,
                    "Browse Global Directory Service",
                    Opc.Ua.ReferenceTypeIds.Organizes);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Gds_SearchMI_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsConnected)
                {
                    ServerCTRL.Connect();
                }

                if (SearchServersDLG == null)
                {
                    SearchServersDLG = new SearchServersDlg();
                    SearchServersDLG.FormClosing += new FormClosingEventHandler(SearchServersDLG_FormClosing);
                }

                SearchServersDLG.Show(ServerCTRL.Session, "Search Global Directory Service");
                SearchServersDLG.BringToFront();
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void SearchServersDLG_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SearchServersDLG = null;
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void View_ManagedSslBindingsMI_Click(object sender, EventArgs e)
        {
            try
            {
                CertificateStoreIdentifier store = new CertificateStoreIdentifier()
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "LocalMachine\\My"
                };

                new ManageSslBindingsDlg().ShowDialog(store);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_RestartServiceMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                if (application != null && ServiceManager.ServiceExists(application.ApplicationName))
                {
                    if (!ServiceManager.IsServiceStopped(application.ApplicationName))
                    {
                        ServiceManager.StopService(application.ApplicationName);
                        m_agent.Log("Service stopped: {0}", application.ApplicationName);
                        System.Threading.Thread.Sleep(5000);
                    }

                    ServiceManager.StartService(application.ApplicationName);
                    m_agent.Log("Service started: {0}", application.ApplicationName);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_AbandonRequestMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                if (application != null && !NodeId.IsNull(application.LastCertificateRequestId))
                {
                    application.LastCertificateRequestId = null;
                    m_agent.SaveApplication(application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void View_ManageHttpAccessRulesMI_Click(object sender, EventArgs e)
        {
            try
            {
                new ManageHttpAccessRulesDlg().ShowDialog(null);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_FirewallMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                if (application != null)
                {
                    if (new ManagePermissionsDlg().ShowDialog(m_agentConfiguration, application))
                    {
                        m_agent.SaveApplication(application);
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void View_ComPseudoServersMI_Click(object sender, EventArgs e)
        {
            try
            {
                new PseudoComServerListDlg().ShowDialog(m_configuration);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_CreateCertificateMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                if (application != null)
                {
                    m_agent.CreateCertificate(application);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }

        private void Application_ViewTrustMatrixMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = ApplicationsCTRL.GetSelectedApplication(0);

                if (application != null)
                {
                    new ViewTrustMatrixDlg().ShowDialog(application, m_agent.Applications);
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
            
        }

        private void Application_InstallHttpsRootMI_Click(object sender, EventArgs e)
        {
            try
            {
                m_application = ApplicationsCTRL.GetSelectedApplication(0);

                if (!IsConnected)
                {
                    ServerCTRL.Connect();
                }

                m_agent.UpdateTrustLists(m_application, true, false);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException(this.Text, exception);
            }
        }
    }
}
