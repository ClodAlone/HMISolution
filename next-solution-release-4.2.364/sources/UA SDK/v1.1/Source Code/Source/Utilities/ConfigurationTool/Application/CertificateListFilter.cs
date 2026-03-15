using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// A filter that can be applied to a list of certificates.
    /// </summary>
    public class CertificateListFilter
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the subject name filter.
        /// </summary>
        /// <value>The subject name filter.</value>
        public string SubjectName
        {
            get { return m_subjectName; }
            set { m_subjectName = value; }
        }

        /// <summary>
        /// Gets or sets the issuer name filter.
        /// </summary>
        /// <value>The issuer name filter.</value>
        public string IssuerName
        {
            get { return m_issuerName; }
            set { m_issuerName = value; }
        }

        /// <summary>
        /// Gets or sets the domain name filter.
        /// </summary>
        /// <value>The issuer domain filter.</value>
        public string Domain
        {
            get { return m_domain; }
            set { m_domain = value; }
        }

        /// <summary>
        /// Gets or sets the certificate type filter.
        /// </summary>
        /// <value>The issuer certificate type filter.</value>
        public CertificateListFilterType[] CertificateTypes
        {
            get { return m_certificateTypes; }
            set { m_certificateTypes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the private key filter.
        /// </summary>
        /// <value><c>true</c> if the private key filter is turned on; otherwise, <c>false</c>.</value>
        public bool PrivateKey
        {
            get { return m_privateKey; }
            set { m_privateKey = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if the certicate meets the filter criteria.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns>True if it meets the criteria.</returns>
        public bool Match(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                return false;
            }

            try
            {
                if (!String.IsNullOrEmpty(m_subjectName))
                {
                    if (!Utils.Match(certificate.Subject, "CN*" + m_subjectName + ",*", false))
                    {
                        return false;
                    }
                }

                if (!String.IsNullOrEmpty(m_issuerName))
                {
                    if (!Utils.Match(certificate.Issuer, "CN*" + m_issuerName + ",*", false))
                    {
                        return false;
                    }
                }

                if (!String.IsNullOrEmpty(m_domain))
                {
                    IList<string> domains = Utils.GetDomainsFromCertficate(certificate);

                    bool found = false;

                    for (int ii = 0; ii < domains.Count; ii++)
                    {
                        if (Utils.Match(domains[ii], m_domain, false))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }

                // check for private key.
                if (m_privateKey)
                {
                    if (!certificate.HasPrivateKey)
                    {
                        return false;
                    }
                }

                if (m_certificateTypes != null)
                {
                    // determine if a CA certificate.
                    bool isCA = false;

                    foreach (X509Extension extension in certificate.Extensions)
                    {
                        X509BasicConstraintsExtension basicContraints = extension as X509BasicConstraintsExtension; 
                        
                        if (basicContraints != null)
                        {
                            isCA = basicContraints.CertificateAuthority;
                            break;
                        }
                    }

                    // determine if self-signed.
                    bool isSelfSigned = Utils.CompareDistinguishedName(certificate.Subject, certificate.Issuer);

                    // match if one or more of the criteria match.
                    bool found = false;

                    for (int ii = 0; ii < m_certificateTypes.Length; ii++)
                    {
                        switch (m_certificateTypes[ii])
                        {
                            case CertificateListFilterType.Application:
                            {
                                if (!isCA)
                                {
                                    found = true;
                                }

                                break;
                            }

                            case CertificateListFilterType.CA:
                            {
                                if (isCA)
                                {
                                    found = true;
                                }

                                break;
                            }

                            case CertificateListFilterType.SelfSigned:
                            {
                                if (isSelfSigned)
                                {
                                    found = true;
                                }

                                break;
                            }

                            case CertificateListFilterType.Issued:
                            {
                                if (!isSelfSigned)
                                {
                                    found = true;
                                }

                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Private Fields
        private string m_subjectName;
        private string m_issuerName;
        private string m_domain;
        private CertificateListFilterType[] m_certificateTypes;
        private bool m_privateKey;
        #endregion
    }

    /// <summary>
    /// The available certificate filter types.
    /// </summary>
    public enum CertificateListFilterType
    {
        /// <summary>
        /// The certificate is an application instance certificate.
        /// </summary>
        Application,

        /// <summary>
        /// The certificate is an certificate authority certificate.
        /// </summary>
        CA,

        /// <summary>
        /// The certificate is self-signed.
        /// </summary>
        SelfSigned,

        /// <summary>
        /// The certificate was issued by a certificate authority.
        /// </summary>
        Issued
    }
}
