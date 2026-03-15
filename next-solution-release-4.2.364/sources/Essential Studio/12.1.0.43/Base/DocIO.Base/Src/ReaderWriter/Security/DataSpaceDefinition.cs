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
    /// Represents the data space definition for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class DataSpaceDefinition
    {
        #region Constants
        /// <summary>
        /// Default header size.
        /// </summary>
        private const int DefaultHeaderLength = 8;
        #endregion

        #region Members
        /// <summary>
        /// Header length.
        /// </summary>
        private int m_iHeaderLength = DefaultHeaderLength;
        /// <summary>
        /// List with transform references.
        /// </summary>
        private List<string> m_lstTransformRefs = new List<string>();
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// List with transform references.
        /// </summary>
        internal List<string> TransformRefs
        {
            get
            {
                return m_lstTransformRefs;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes new instance.
        /// </summary>
        internal DataSpaceDefinition()
        {
        }
        /// <summary>
        /// Initializes new instance of DataSpaceDefinition.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal DataSpaceDefinition(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            m_iHeaderLength = m_securityHelper.ReadInt32(stream, arrBuffer);
            int iCount = m_securityHelper.ReadInt32(stream, arrBuffer);

            if (m_iHeaderLength != DefaultHeaderLength)
                stream.Position += m_iHeaderLength - DefaultHeaderLength;

            for (int i = 0; i < iCount; i++)
            {
                string strTransform = m_securityHelper.ReadUnicodeString(stream);
                m_lstTransformRefs.Add(strTransform);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes dataspace definition into the stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            m_securityHelper.WriteInt32(stream, m_iHeaderLength);

            int iCount = m_lstTransformRefs.Count;
            m_securityHelper.WriteInt32(stream, iCount);

            for (int i = 0; i < iCount; i++)
            {
                string strTransform = m_lstTransformRefs[i];
                m_securityHelper.WriteUnicodeString(stream, strTransform);
            }
        }
        #endregion
    }
}
