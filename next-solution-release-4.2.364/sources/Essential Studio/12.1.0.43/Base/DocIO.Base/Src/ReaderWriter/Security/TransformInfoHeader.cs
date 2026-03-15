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
    /// Represents the transform info header for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class TransformInfoHeader
    {
        #region Members
        /// <summary>
        /// An unsigned integer that specifies the type of transform to be applied.
        /// </summary>
        private int m_iTransformType = 1;
        /// <summary>
        /// An identifier associated with a specific transform.
        /// </summary>
        private string m_strTransformId;
        /// <summary>
        /// The friendly name of the transform.
        /// </summary>
        private string m_strTransformName;
        /// <summary>
        /// The reader version.
        /// </summary>
        private int m_iReaderVersion = 1;
        /// <summary>
        /// The updater version.
        /// </summary>
        private int m_iUpdaterVersion = 1;
        /// <summary>
        /// The writer version.
        /// </summary>
        private int m_iWriterVersion = 1;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// An unsigned integer that specifies the type of transform to be applied.
        /// </summary>
        internal int TransformType
        {
            get
            {
                return m_iTransformType;
            }
            set
            {
                m_iTransformType = value;
            }
        }
        /// <summary>
        /// An identifier associated with a specific transform.
        /// </summary>
        internal string TransformId
        {
            get
            {
                return m_strTransformId;
            }
            set
            {
                m_strTransformId = value;
            }
        }
        /// <summary>
        /// The friendly name of the transform.
        /// </summary>
        internal string TransformName
        {
            get
            {
                return m_strTransformName;
            }
            set
            {
                m_strTransformName = value;
            }
        }
        /// <summary>
        /// The reader version.
        /// </summary>
        internal int ReaderVersion
        {
            get
            {
                return m_iReaderVersion;
            }
            set
            {
                m_iReaderVersion = value;
            }
        }
        /// <summary>
        /// The updater version.
        /// </summary>
        internal int UpdaterVersion
        {
            get
            {
                return m_iUpdaterVersion;
            }
            set
            {
                m_iUpdaterVersion = value;
            }
        }
        /// <summary>
        /// The writer version.
        /// </summary>
        internal int WriterVersion
        {
            get
            {
                return m_iWriterVersion;
            }
            set
            {
                m_iWriterVersion = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal TransformInfoHeader()
        {
        }
        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal TransformInfoHeader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            int iLength = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iTransformType = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_strTransformId = m_securityHelper.ReadUnicodeString(stream);
            m_strTransformName = m_securityHelper.ReadUnicodeString(stream);
            m_iReaderVersion = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iUpdaterVersion = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_iWriterVersion = m_securityHelper.ReadInt32(stream, arrBuffer);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes object into stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long lStart = stream.Position;

            stream.Position += DLSConstants.IntSize;
            m_securityHelper.WriteInt32(stream, m_iTransformType);
            m_securityHelper.WriteUnicodeString(stream, m_strTransformId);

            long lEnd = stream.Position;
            int iLength = (int)(lEnd - lStart);
            stream.Position = lStart;
            m_securityHelper.WriteInt32(stream, iLength);
            stream.Position = lEnd;

            m_securityHelper.WriteUnicodeString(stream, m_strTransformName);
            m_securityHelper.WriteInt32(stream, m_iReaderVersion);
            m_securityHelper.WriteInt32(stream, m_iUpdaterVersion);
            m_securityHelper.WriteInt32(stream, m_iWriterVersion);
        }
        #endregion
    }
}
