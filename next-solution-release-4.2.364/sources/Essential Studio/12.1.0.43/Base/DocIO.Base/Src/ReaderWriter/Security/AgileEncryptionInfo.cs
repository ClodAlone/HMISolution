#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// Represents the agile encryption info for Encryption/Decryption of Word 2010/2013 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class AgileEncryptionInfo
    {
        #region Members
        /// <summary>
        /// A Version structure where Version.vMajor MUST be 0x0004, and Version.vMinor MUST be 0x0004.
        /// </summary>
        private int m_iVersionInfo;
        /// <summary>
        /// A Reserved 4 bytes, MUST be 0x00000040.
        /// </summary>
        private int m_iReserved;
        /// <summary>
        /// An XmlEncryptionDescriptor structure that specifies encryption and hashing algorithm.
        /// </summary>
        private XmlEncryptionDescriptor m_xmlEncryptionDescriptor = new XmlEncryptionDescriptor();
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the version info.
        /// </summary>
        /// <value>The version info.</value>
        internal int VersionInfo
        {
            get
            {
                return m_iVersionInfo;
            }
            set
            {
                m_iVersionInfo = value;
            }
        }
        /// <summary>
        /// Gets or sets the reserved.
        /// </summary>
        /// <value>The reserved.</value>
        internal int Reserved
        {
            get
            {
                return m_iReserved;
            }
            set
            {
                m_iReserved = value;
            }
        }
        /// <summary>
        /// An XmlEncryptionDescriptor structure that specifies encryption and hashing algorithm.
        /// </summary>
        internal XmlEncryptionDescriptor XmlEncryptionDescriptor
        {
            get
            {
                return m_xmlEncryptionDescriptor;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal AgileEncryptionInfo()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal AgileEncryptionInfo(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            m_iVersionInfo = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iReserved = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_xmlEncryptionDescriptor.Parse(stream);
        }
        /// <summary>
        /// Serializes object into stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            m_securityHelper.WriteInt32(stream, m_iVersionInfo);
            m_securityHelper.WriteInt32(stream, m_iReserved);
            m_xmlEncryptionDescriptor.Serialize(stream);
        }
        #endregion
    }
}
