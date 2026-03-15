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
    /// Represents the data space map for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class DataSpaceMap
    {
        #region Constants
        private const int DefaultHeaderSize = 8;
        #endregion

        #region Members
        /// <summary>
        /// Size of the header.
        /// </summary>
        private int m_iHeaderSize = DefaultHeaderSize;
        /// <summary>
        /// Map entries.
        /// </summary>
        private List<DataSpaceMapEntry> m_lstMapEntries = new List<DataSpaceMapEntry>();
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// Map entries.
        /// </summary>
        internal List<DataSpaceMapEntry> MapEntries
        {
            get
            {
                return m_lstMapEntries;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal DataSpaceMap()
        {
        }
        /// <summary>
        /// Initializes new instance of the DataSpaceMap.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal DataSpaceMap(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] arrBuffer = new byte[DLSConstants.IntSize];

            m_iHeaderSize = m_securityHelper.ReadInt32(stream, arrBuffer);
            int iCount = m_securityHelper.ReadInt32(stream, arrBuffer);

            if (m_lstMapEntries.Capacity < iCount)
                m_lstMapEntries.Capacity = iCount;

            if (m_iHeaderSize != DefaultHeaderSize)
                stream.Position += m_iHeaderSize - DefaultHeaderSize;

            for (int i = 0; i < iCount; i++)
            {
                DataSpaceMapEntry entry = new DataSpaceMapEntry(stream);
                m_lstMapEntries.Add(entry);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes dataspace map into the stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            m_securityHelper.WriteInt32(stream, m_iHeaderSize);
            int iCount = m_lstMapEntries.Count;
            m_securityHelper.WriteInt32(stream, iCount);

            for (int i = 0; i < iCount; i++)
            {
                DataSpaceMapEntry entry = m_lstMapEntries[i];
                entry.Serialize(stream);
            }
        }
        #endregion
    }
}