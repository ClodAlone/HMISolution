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
    /// Represents the version information for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class VersionInfo
    {
        #region Members
        /// <summary>
        /// The functionality for which the DataSpaceVersionInfo structure specifies
        /// version information. MUST be "Microsoft.Container.DataSpaces".
        /// </summary>
        private string m_strFeatureId = "Microsoft.Container.DataSpaces";
        /// <summary>
        /// The reader version of the data spaces structure.
        /// </summary>
        private int m_iReaderVersion = 1;
        /// <summary>
        /// The updater version of the data spaces structure.
        /// </summary>
        private int m_iUpdaterVersion = 1;
        /// <summary>
        /// The writer version of the data spaces structure.
        /// </summary>
        private int m_iWriterVersion = 1;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// The functionality for which the DataSpaceVersionInfo structure specifies
        /// version information. MUST be "Microsoft.Container.DataSpaces".
        /// </summary>
        internal string FeatureId
        {
            get
            {
                return m_strFeatureId;
            }
            set
            {
                m_strFeatureId = value;
            }
        }
        /// <summary>
        /// The reader version of the data spaces structure.
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
        /// The updater version of the data spaces structure.
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
        /// The writer version of the data spaces structure.
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
        internal VersionInfo()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Serializes object into specified stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        internal void Serialize(Stream stream)
        {
            m_securityHelper.WriteUnicodeString(stream, m_strFeatureId);
            m_securityHelper.WriteInt32(stream, m_iReaderVersion);
            m_securityHelper.WriteInt32(stream, m_iUpdaterVersion);
            m_securityHelper.WriteInt32(stream, m_iWriterVersion);
        }
        #endregion
    }
}
