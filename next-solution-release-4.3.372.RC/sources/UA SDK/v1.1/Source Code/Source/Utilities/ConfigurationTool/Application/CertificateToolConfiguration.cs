/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Opc.Ua.Configuration;

namespace Opc.Ua.Configuration
{
    /// <summary>
    /// Stores information about an account.
    /// </summary>
    [DataContract(Namespace="http://opcfoundation.org/UA/SDK/CertificateToolConfiguration.xsd")]
    public class CertificateToolConfiguration
    {
        #region Constructors
        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public CertificateToolConfiguration()
        {
            m_applications = new List<ConfigureableApplication>();
            m_stores = new List<CertificateStoreIdentifier>();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            m_applications = new List<ConfigureableApplication>();
            m_stores = new List<CertificateStoreIdentifier>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The applications known to the tool.
        /// </summary>
        [DataMember(Order = 1)]
        public List<ConfigureableApplication> Applications
        {
            get
            { 
                return m_applications;  
            } 
            
            set 
            { 
                m_applications = value; 

                if (m_applications == null)
                {
                    m_applications = new List<ConfigureableApplication>();
                }
            }
        }

        /// <summary>
        /// The certificate stores known to the tool.
        /// </summary>
        [DataMember(Order = 2)]
        public List<CertificateStoreIdentifier> Stores
        {
            get
            { 
                return m_stores;  
            } 
            
            set 
            { 
                m_stores = value; 

                if (m_stores == null)
                {
                    m_stores = new List<CertificateStoreIdentifier>();
                }
            }
        }
        #endregion 

        #region Private Fields
        private List<ConfigureableApplication> m_applications;
        private List<CertificateStoreIdentifier> m_stores;
        #endregion 
    }
    
    #region ConfigureableApplication Class
    /// <summary>
    /// An application which can be configured by the certificate tool.
    /// </summary>
    [DataContract(Namespace="http://opcfoundation.org/UA/SDK/CertificateToolConfiguration.xsd")]
    public class ConfigureableApplication
    {
        #region Public Properties
        /// <summary>
        /// The location of the configuration file for the application.
        /// </summary>
        [DataMember(Order = 1)]
        public string ConfigurationFile
        {
            get { return m_configurationFile;  } 
            set { m_configurationFile = value; }
        }
        
        /// <summary>
        /// The assembly qualified type name for a class that implements ISecurityConfiguration
        /// </summary>
        [DataMember(Order = 2)]
        public string LoaderType
        {
            get { return m_loaderType;  } 
            set { m_loaderType = value; }
        }
        #endregion 

        #region Private Fields
        private string m_configurationFile;
        private string m_loaderType;
        #endregion 
    }
    #endregion 
}
