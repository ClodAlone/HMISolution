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
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Opc.Ua.Server;

namespace Opc.Ua.GdsServer
{
    /// <summary>
    /// Stores the configuration the data access node manager.
    /// </summary>
    [DataContract(Namespace=Opc.Ua.Namespaces.OpcUaGds)]
    public class GdsServerConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public GdsServerConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {        
            DatabasePath = @"%CommonApplicationData%\OPC Foundation\GDS\ProtectedData\ApplicationDatabase.xml";
            CertificateAuthorityStorePath = @"%CommonApplicationData%\OPC Foundation\GDS\ProtectedData\CertificateStores\PrivateKeys";
            DefaultTrustListStorePath = @"%CommonApplicationData%\OPC Foundation\GDS\ProtectedData\CertificateStores\TrustList";
            DefaultIssuerListStorePath = @"%CommonApplicationData%\OPC Foundation\GDS\ProtectedData\CertificateStores\IssuerList";
            UseSingleRootCertificateAuthority = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The path to database which stores the information used by the GDS.
        /// </summary>
        [DataMember(Order = 1)]
        public string DatabasePath { get; set; }

        /// <summary>
        /// The path to the certificate store used to store the GDS certificate authorities.
        /// </summary>
        [DataMember(Order = 2)]
        public string CertificateAuthorityStorePath { get; set; }

        /// <summary>
        /// The path to the certificate store used to store the default trust list for UA Applications.
        /// </summary>
        [DataMember(Order = 3)]
        public string DefaultTrustListStorePath { get; set; }

        /// <summary>
        /// The path to the certificate store used to store the default issuer list for UA Applications.
        /// </summary>
        [DataMember(Order = 4)]
        public string DefaultIssuerListStorePath { get; set; }
        
        /// <summary>
        /// Whether a single root certificate authority should be created.
        /// </summary>
        [DataMember(Order = 5)]
        public bool UseSingleRootCertificateAuthority { get; set; }
        #endregion
    }
}
