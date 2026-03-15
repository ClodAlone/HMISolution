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
    /// Represents the encryption transform info for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class EncryptionTransformInfo
    {
        #region Members
        /// <summary>
        /// Transform name.
        /// </summary>
        private string m_strName;
        /// <summary>
        /// Block size.
        /// </summary>
        private int m_iBlockSize;
        /// <summary>
        /// Cipher mode.
        /// </summary>
        private int m_iCipherMode;
        /// <summary>
        /// Reserved.
        /// </summary>
        private int m_iReserved = 0x04;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// Transform name.
        /// </summary>
        internal string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }
        /// <summary>
        /// Block size.
        /// </summary>
        internal int BlockSize
        {
            get
            {
                return m_iBlockSize;
            }
            set
            {
                m_iBlockSize = value;
            }
        }
        /// <summary>
        /// Cipher mode.
        /// </summary>
        internal int CipherMode
        {
            get
            {
                return m_iCipherMode;
            }
        }
        /// <summary>
        /// Reserved.
        /// </summary>
        internal int Reserved
        {
            get
            {
                return m_iReserved;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal EncryptionTransformInfo()
        {
        }
        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal EncryptionTransformInfo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            m_strName = m_securityHelper.ReadUnicodeString(stream);
            m_iBlockSize = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iCipherMode = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iReserved = m_securityHelper.ReadInt32(stream, arrBuffer);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes object into stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        internal void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            m_securityHelper.WriteUnicodeString(stream, m_strName);
            m_securityHelper.WriteInt32(stream, m_iBlockSize);
            m_securityHelper.WriteInt32(stream, m_iCipherMode);
            m_securityHelper.WriteInt32(stream, m_iReserved);
        }
        #endregion
    }
}
