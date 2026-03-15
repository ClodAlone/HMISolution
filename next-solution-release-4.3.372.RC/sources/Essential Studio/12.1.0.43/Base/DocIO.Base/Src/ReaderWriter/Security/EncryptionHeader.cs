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
    /// Represents the encryption header for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class EncryptionHeader
    {
        #region Members
        /// <summary>
        /// An EncryptionHeaderFlags structure that specifies properties of the encryption algorithm used.
        /// </summary>
        private int m_iFlags;
        /// <summary>
        /// Reserved, MUST be 0x00000000.
        /// </summary>
        private int m_iSizeExtra;
        /// <summary>
        /// A signed integer that specifies the encryption algorithm.
        /// </summary>
        private int m_iAlgorithmId;
        /// <summary>
        /// A signed integer that specifies the hashing algorithm in concert with the Flags.fExternal bit.
        /// </summary>
        private int m_iAlgorithmIdHash;
        /// <summary>
        /// An unsigned integer that specifies the number of bits in the encryption key.
        /// MUST be a multiple of 8.
        /// </summary>
        private int m_iKeySize;
        /// <summary>
        /// An implementation specified value which corresponds to constants accepted by
        /// the specified CSP. MUST be compatible with the chosen CSP.
        /// </summary>
        private int m_iProviderType;
        /// <summary>
        /// Undefined and MUST be ignored.
        /// </summary>
        private int m_iReserved1;
        /// <summary>
        /// MUST be 0x00000000 and MUST be ignored.
        /// </summary>
        private int m_iReserved2;
        /// <summary>
        /// A null-terminated Unicode string that specifies the CSP name.
        /// </summary>
        private string m_strCSPName;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// An EncryptionHeaderFlags structure that specifies properties of the encryption algorithm used.
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
        /// Reserved, MUST be 0x00000000.
        /// </summary>
        internal int SizeExtra
        {
            get
            {
                return m_iSizeExtra;
            }
            set
            {
                m_iSizeExtra = value;
            }
        }
        /// <summary>
        /// A signed integer that specifies the encryption algorithm.
        /// </summary>
        internal int AlgorithmId
        {
            get
            {
                return m_iAlgorithmId;
            }
            set
            {
                m_iAlgorithmId = value;
            }
        }
        /// <summary>
        /// A signed integer that specifies the hashing algorithm in concert with the Flags.fExternal bit.
        /// </summary>
        internal int AlgorithmIdHash
        {
            get
            {
                return m_iAlgorithmIdHash;
            }
            set
            {
                m_iAlgorithmIdHash = value;
            }
        }
        /// <summary>
        /// An unsigned integer that specifies the number of bits in the encryption key.
        /// MUST be a multiple of 8.
        /// </summary>
        internal int KeySize
        {
            get
            {
                return m_iKeySize;
            }
            set
            {
                m_iKeySize = value;
            }
        }
        /// <summary>
        /// An implementation specified value which corresponds to constants accepted by
        /// the specified CSP. MUST be compatible with the chosen CSP.
        /// </summary>
        internal int ProviderType
        {
            get
            {
                return m_iProviderType;
            }
            set
            {
                m_iProviderType = value;
            }
        }
        /// <summary>
        /// Undefined and MUST be ignored.
        /// </summary>
        internal int Reserved1
        {
            get
            {
                return m_iReserved1;
            }
            set
            {
                m_iReserved1 = value;
            }
        }
        /// <summary>
        /// MUST be 0x00000000 and MUST be ignored.
        /// </summary>
        internal int Reserved2
        {
            get
            {
                return m_iReserved2;
            }
            set
            {
                m_iReserved2 = value;
            }
        }
        /// <summary>
        /// A null-terminated Unicode string that specifies the CSP name.
        /// </summary>
        internal string CSPName
        {
            get
            {
                return m_strCSPName;
            }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentOutOfRangeException();

                m_strCSPName = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal EncryptionHeader()
        {
        }
        /// <summary>
        /// Initializes new instance of the header and extracts its data from the stream.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal EncryptionHeader(Stream stream)
        {
            Parse(stream);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Extracts item's data from the specified stream.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal void Parse(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            long lStart = stream.Position;
            int iHeaderSize = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iFlags = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iSizeExtra = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iAlgorithmId = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iAlgorithmIdHash = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iKeySize = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iProviderType = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iReserved1 = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iReserved2 = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_strCSPName = m_securityHelper.ReadUnicodeStringZero(stream);

            // TODO: check this.
            stream.Position = lStart + iHeaderSize + DLSConstants.IntSize;
        }
        /// <summary>
        /// Serialize item in the specified stream.
        /// </summary>
        /// <param name="stream">Stream to serialize data into.</param>
        internal void Serialize(Stream stream)
        {
            long lStart = stream.Position;
            stream.Position += DLSConstants.IntSize;
            m_securityHelper.WriteInt32(stream, m_iFlags);
            m_securityHelper.WriteInt32(stream, m_iSizeExtra);
            m_securityHelper.WriteInt32(stream, m_iAlgorithmId);
            m_securityHelper.WriteInt32(stream, m_iAlgorithmIdHash);
            m_securityHelper.WriteInt32(stream, m_iKeySize);
            m_securityHelper.WriteInt32(stream, m_iProviderType);
            m_securityHelper.WriteInt32(stream, m_iReserved1);
            m_securityHelper.WriteInt32(stream, m_iReserved2);
            m_securityHelper.WriteUnicodeStringZero(stream, m_strCSPName);
            long lEnd = stream.Position;
            int iHeaderSize = (int)(lEnd - lStart) - DLSConstants.IntSize;
            stream.Position = lStart;
            m_securityHelper.WriteInt32(stream, iHeaderSize);
            stream.Position = lEnd;
        }
        #endregion
    }
}
