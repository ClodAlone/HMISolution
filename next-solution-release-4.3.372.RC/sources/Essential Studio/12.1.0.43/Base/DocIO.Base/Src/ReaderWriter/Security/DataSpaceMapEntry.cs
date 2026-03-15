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
    /// Represents the data space map entry for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class DataSpaceMapEntry
    {
        #region Members
        /// <summary>
        /// List of the reference components.
        /// </summary>
        private List<DataSpaceReferenceComponent> m_lstComponents = new List<DataSpaceReferenceComponent>();
        /// <summary>
        /// DataSpace name.
        /// </summary>
        private string m_strDataSpaceName;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// List of the reference components.
        /// </summary>
        internal List<DataSpaceReferenceComponent> Components
        {
            get
            {
                return m_lstComponents;
            }
        }
        /// <summary>
        /// DataSpace name.
        /// </summary>
        internal string DataSpaceName
        {
            get
            {
                return m_strDataSpaceName;
            }
            set
            {
                m_strDataSpaceName = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal DataSpaceMapEntry()
        {
        }
        /// <summary>
        /// Initializes new instance of the map entry.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal DataSpaceMapEntry(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            int iLength = m_securityHelper.ReadInt32(stream, arrBuffer);
            int iReferenceCount = m_securityHelper.ReadInt32(stream, arrBuffer);

            for (int i = 0; i < iReferenceCount; i++)
            {
                DataSpaceReferenceComponent component = new DataSpaceReferenceComponent(stream);
                m_lstComponents.Add(component);
            }

            m_strDataSpaceName = m_securityHelper.ReadUnicodeString(stream);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes single dataspace map entry into the stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long lStartPosition = stream.Position;
            stream.Position += DLSConstants.IntSize;

            int iReferenceCount = m_lstComponents.Count;
            m_securityHelper.WriteInt32(stream, iReferenceCount);

            for (int i = 0; i < iReferenceCount; i++)
            {
                DataSpaceReferenceComponent component = m_lstComponents[i];
                component.Serialize(stream);
            }

            m_securityHelper.WriteUnicodeString(stream, m_strDataSpaceName);

            // TODO: we can evaluate this value before serialization.
            long lEndPosition = stream.Position;
            stream.Position = lStartPosition;
            m_securityHelper.WriteInt32(stream, (int)(lEndPosition - lStartPosition));
            stream.Position = lEndPosition;
        }
        #endregion
    }
}
