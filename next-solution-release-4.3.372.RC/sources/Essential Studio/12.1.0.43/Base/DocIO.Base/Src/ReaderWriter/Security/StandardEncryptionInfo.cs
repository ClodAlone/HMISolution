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
    /// Represents the standard encryption info for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class StandardEncryptionInfo
    {
        #region Members
        /// <summary>
        /// A Version structure where Version.vMajor MUST be 0x0003, and Version.vMinor MUST be 0x0002.
        /// </summary>
        private int m_iVersionInfo;
        /// <summary>
        /// A copy of the Flags stored in the EncryptionHeader field of this structure.
        /// </summary>
        private int m_iFlags;
        /// <summary>
        /// An EncryptionHeader structure that specifies parameters used to encrypt data.
        /// </summary>
        private EncryptionHeader m_header = new EncryptionHeader();
        /// <summary>
        /// An EncryptionVerifier structure.
        /// </summary>
        private EncryptionVerifier m_verifier = new EncryptionVerifier();
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// A Version structure where Version.vMajor MUST be 0x0003, and Version.vMinor MUST be 0x0002.
        /// </summary>
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
        /// A copy of the Flags stored in the EncryptionHeader field of this structure.
        /// </summary>
        internal int Flags
        {
            get
            {
                return m_iFlags;
            }
            set
            {
                m_iFlags = value;
            }
        }
        /// <summary>
        /// An EncryptionHeader structure that specifies parameters used to encrypt data.
        /// </summary>
        internal EncryptionHeader Header
        {
            get
            {
                return m_header;
            }
        }
        /// <summary>
        /// An EncryptionVerifier structure.
        /// </summary>
        internal EncryptionVerifier Verifier
        {
            get
            {
                return m_verifier;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal StandardEncryptionInfo()
        {
        }
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal StandardEncryptionInfo(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            m_iVersionInfo = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iFlags = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_header.Parse(stream);
            m_verifier.Parse(stream);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes object into stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            m_securityHelper.WriteInt32(stream, m_iVersionInfo);
            m_securityHelper.WriteInt32(stream, m_iFlags);
            m_header.Serialize(stream);
            m_verifier.Serialize(stream);
        }
        #endregion
    }
}
