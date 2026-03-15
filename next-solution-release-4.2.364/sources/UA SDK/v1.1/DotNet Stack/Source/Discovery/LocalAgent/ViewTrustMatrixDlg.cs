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

using Opc.Ua.Security;
using Opc.Ua.Configuration;
using Opc.Ua.Client.Controls;

namespace Opc.Ua.GdsLocalAgent
{
    /// <summary>
    /// Allows the use to manage the SSL certificate bindings.
    /// </summary>
    public partial class ViewTrustMatrixDlg : Form
    {
        #region Constructors
        /// <summary>
        /// Constructs the object.
        /// </summary>
        public ViewTrustMatrixDlg()
        {
            InitializeComponent();
            TrustMatrixDV.AutoGenerateColumns = false;
            TrustMatrixDV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            m_dataset = new DataSet();

            DataTable table = new DataTable();

            table.Columns.Add("ApplicationName", typeof(string));
            table.Columns.Add("TrustedByMe", typeof(string));
            table.Columns.Add("TrustsMe", typeof(string));
            table.Columns.Add("Application", typeof(GdsAgentApplication));

            m_dataset.Tables.Add(table);
            TrustMatrixDV.DataSource = m_dataset.Tables[0];
        }
        #endregion

        #region Private Fields
        private DataSet m_dataset;
        private GdsAgentApplication m_target;
        #endregion

        #region Public Interface
        /// <summary>
        /// Displays the dialog.
        /// </summary>
        public bool ShowDialog(GdsAgentApplication target, IList<GdsAgentApplication> applications)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (applications == null)
            {
                throw new ArgumentNullException("applications");
            }

            this.Text = "Trust Matrix for " + target.ApplicationName;
            m_target = target;

            CertificateValidator validator1 = CreateValidator(target);
            X509Certificate2 certificate1 = GetCertificate(target);

            foreach (GdsAgentApplication application in applications)
            {
                if (target.SecuritySettings.ApplicationType == application.SecuritySettings.ApplicationType)
                {
                    if (target.SecuritySettings.ApplicationType != Opc.Ua.Security.ApplicationType.ClientAndServer_2)
                    {
                        continue;
                    }
                }

                DataRow row = m_dataset.Tables[0].NewRow();
                UpdateRow(row, application, validator1, certificate1);
                m_dataset.Tables[0].Rows.Add(row);
            }

            m_dataset.AcceptChanges();
            TrustMatrixDV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            if (base.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks the trust relationships and updates the row with the results.
        /// </summary>
        private void UpdateRow(DataRow row, GdsAgentApplication application, CertificateValidator validator1, X509Certificate2 certificate1)
        {
            row[0] = application.ApplicationName;
            row[1] = "Certificate Missing";
            row[2] = "Certificate Missing";
            row[3] = application;

            X509Certificate2 certificate2 = GetCertificate(application);

            if (certificate2 != null)
            {
                try
                {
                    validator1.Validate(certificate2);
                    row[1] = "Yes";
                }
                catch (ServiceResultException e)
                {
                    row[1] = new StatusCode(e.StatusCode);
                }
                catch (Exception e)
                {
                    row[1] = e.Message;
                }
            }

            if (certificate1 != null)
            {
                CertificateValidator validator2 = CreateValidator(application);

                try
                {
                    validator1.Validate(certificate1);
                    row[2] = "Yes";
                }
                catch (ServiceResultException e)
                {
                    row[2] = new StatusCode(e.StatusCode);
                }
                catch (Exception e)
                {
                    row[2] = e.Message;
                }
            }
        }

        /// <summary>
        /// Gets the certificate for the application.
        /// </summary>
        private X509Certificate2 GetCertificate(GdsAgentApplication target)
        {
            if (target.SecuritySettings.ApplicationCertificate != null)
            {
                X509Certificate2 certificate = target.SecuritySettings.ApplicationCertificate.Find(false);

                if (certificate != null)
                {
                    return new X509Certificate2(certificate.RawData);
                }
            }

            if (!String.IsNullOrEmpty(target.PublicKeyFilePath))
            {
                if (System.IO.File.Exists(target.PublicKeyFilePath))
                {
                    return new X509Certificate2(target.PublicKeyFilePath);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the issuers for the certificate.
        /// </summary>
        private List<X509Certificate2> GetIssuers(GdsAgentApplication target, X509Certificate2 certificate, List<X509CRL> crls)
        {
            CertificateValidator validator = CreateValidator(target);

            List<CertificateIdentifier> issuerCIDs = new List<CertificateIdentifier>();
            validator.GetIssuers(certificate, issuerCIDs);

            List<X509Certificate2> issuers = new List<X509Certificate2>();

            foreach (CertificateIdentifier issuerCID in issuerCIDs)
            {
                X509Certificate2 issuer = issuerCID.Find(false);

                if (issuer != null)
                {
                    issuers.Add(issuer);

                    ICertificateStore store = issuerCID.OpenStore();

                    try
                    {
                        if (store.SupportsCRLs)
                        {
                            List<X509CRL> issuerCrls = store.EnumerateCRLs(issuer);
                            crls.AddRange(issuerCrls);
                        }
                    }
                    finally
                    {
                        store.Close();
                    }
                }
            }

            return issuers;
        }

        /// <summary>
        /// Adds a certificate to a trust list.
        /// </summary>
        private void AddToTrustList(Opc.Ua.Security.CertificateStoreIdentifier trustList, X509Certificate2 certificate)
        {
            if (trustList != null)
            {
                ICertificateStore store = trustList.OpenStore();

                try
                {
                    if (store.FindByThumbprint(certificate.Thumbprint) == null)
                    {
                        store.Add(new X509Certificate2(certificate));
                    }
                }
                finally
                {
                    store.Close();
                }
            }
        }

        /// <summary>
        /// Adds the CRL to the trust list.
        /// </summary>
        private void AddToTrustList(Opc.Ua.Security.CertificateStoreIdentifier trustList, List<X509CRL> crls)
        {
            if (trustList != null)
            {
                ICertificateStore store = trustList.OpenStore();

                try
                {
                    if (store.SupportsCRLs)
                    {
                        foreach (X509CRL crl in crls)
                        {
                            store.AddCRL(crl);
                        }
                    }
                }
                finally
                {
                    store.Close();
                }
            }
        }

        /// <summary>
        /// Adds a certificate and its issuers from one application to the trust list of another application.
        /// </summary>
        private void AddToTrustList(GdsAgentApplication dst, GdsAgentApplication src, X509Certificate2 certificate)
        {
            if (dst.SecuritySettings != null)
            {
                AddToTrustList(dst.SecuritySettings.TrustedCertificateStore, certificate);

                List<X509CRL> crls = new List<X509CRL>();
                List<X509Certificate2> issuers = GetIssuers(src, certificate, crls);

                if (issuers.Count > 0)
                {
                    if (dst.SecuritySettings.IssuerCertificateStore == null || dst.SecuritySettings.IssuerCertificateStore.StorePath == dst.SecuritySettings.TrustedCertificateStore.StorePath)
                    {
                        StringBuilder builder = new StringBuilder();

                        builder.Append("The application '");
                        builder.Append(dst.ApplicationName);
                        builder.Append("' does not have a seperate issuer store configured.\r\n");
                        builder.Append("This means that creating a trust relationship requires that all certificates issued by '.");
                        builder.Append(issuers[0].Subject);
                        builder.Append("' will have to be trusted.\r\nIs this OK?");

                        DialogResult result = new YesNoDlg().ShowDialog(
                            builder.ToString(),
                            "Confirm Add Trust Relationship");

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

                foreach (X509Certificate2 issuer in issuers)
                {
                    if (dst.SecuritySettings.IssuerCertificateStore != null)
                    {
                        AddToTrustList(dst.SecuritySettings.IssuerCertificateStore, issuer);
                        continue;
                    }

                    if (dst.SecuritySettings.TrustedCertificateStore != null)
                    {
                        AddToTrustList(dst.SecuritySettings.TrustedCertificateStore, issuer);
                    }
                }

                AddToTrustList(dst.SecuritySettings.IssuerCertificateStore, crls);
            }
        }

        /// <summary>
        /// Creates a certificate validator for the application.
        /// </summary>
        private CertificateValidator CreateValidator(GdsAgentApplication target)
        {
            CertificateValidator validator = new CertificateValidator();

            validator.Update(
                SecuredApplication.ToCertificateTrustList(target.SecuritySettings.IssuerCertificateStore),
                SecuredApplication.ToCertificateTrustList(target.SecuritySettings.TrustedCertificateStore),
                null);

            return validator;                
        }
        #endregion

        #region Event Handlers
        private void ManageHttpAccessRulesDlg_Shown(object sender, EventArgs e)
        {
            TrustMatrixDV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void ViewTargetTrustListMI_Click(object sender, EventArgs e)
        {
            try
            {
                GdsAgentApplication application = null;

                foreach (DataGridViewRow row in TrustMatrixDV.SelectedRows)
                {
                    DataRowView source = row.DataBoundItem as DataRowView;
                    application = (GdsAgentApplication)source.Row[3];

                    if (application.SecuritySettings != null && application.SecuritySettings.TrustedCertificateStore != null)
                    {
                        CertificateStoreIdentifier csid = SecuredApplication.FromCertificateStoreIdentifier(application.SecuritySettings.TrustedCertificateStore);

                        if (csid != null)
                        {
                            new CertificateListDlg().ShowDialog(csid, false);
                        }
                    }

                    break;
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void ViewMyTrustListMI_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_target.SecuritySettings.TrustedCertificateStore != null)
                {
                    CertificateStoreIdentifier csid = SecuredApplication.FromCertificateStoreIdentifier(m_target.SecuritySettings.TrustedCertificateStore);

                    if (csid != null)
                    {
                        new CertificateListDlg().ShowDialog(csid, false);
                    }
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void EnableMutualTrustMI_Click(object sender, EventArgs e)
        {
            try
            {
                CertificateValidator validator1 = CreateValidator(m_target);
                X509Certificate2 certificate1 = GetCertificate(m_target);

                GdsAgentApplication application = null;

                foreach (DataGridViewRow row in TrustMatrixDV.SelectedRows)
                {
                    DataRowView source = row.DataBoundItem as DataRowView;
                    application = (GdsAgentApplication)source.Row[3];

                    if ((string)source.Row[2] != "Yes" && certificate1 != null)
                    {
                        if (application.SecuritySettings != null)
                        {
                            AddToTrustList(application, m_target, certificate1);
                        }
                    }

                    X509Certificate2 certificate2 = GetCertificate(application);

                    if ((string)source.Row[1] != "Yes" && certificate2 != null)
                    {
                        if (m_target.SecuritySettings != null)
                        {
                            AddToTrustList(m_target, application, certificate2);
                        }
                    }

                    UpdateRow(source.Row, application, validator1, certificate1);
                    break;
                }
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
    }
}
