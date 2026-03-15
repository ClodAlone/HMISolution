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
    /// Represents the data space reference component for Encryption/Decryption of Word documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class DataSpaceReferenceComponent
    {
        #region Members
        /// <summary>
        /// Component type.
        /// </summary>
        private int m_iComponentType;
        /// <summary>
        /// Component name.
        /// </summary>
        private string m_strName;
        /// <summary>
        /// Security helper contains utility methods for encryption/decryption.
        /// </summary>
        private SecurityHelper m_securityHelper = new SecurityHelper();
        #endregion

        #region Properties
        /// <summary>
        /// Component type.
        /// </summary>
        internal int ComponentType
        {
            get
            {
                return m_iComponentType;
            }
        }
        /// <summary>
        /// Component name.
        /// </summary>
        internal string Name
        {
            get
            {
                return m_strName;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes new instance of the reference component.
        /// </summary>
        /// <param name="type">Component type.</param>
        /// <param name="name">Component name.</param>
        internal DataSpaceReferenceComponent(int type, string name)
        {
            m_iComponentType = type;
            m_strName = name;
        }
        /// <summary>
        /// Initializes new instance of the component reference.
        /// </summary>
        /// <param name="stream">Stream to get data from.</param>
        internal DataSpaceReferenceComponent(Stream stream)
        {
            byte[] arrBuffer = new byte[DLSConstants.IntSize];
            m_iComponentType = m_securityHelper.ReadInt32(stream, arrBuffer);
            m_strName = m_securityHelper.ReadUnicodeString(stream);
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

            m_securityHelper.WriteInt32(stream, m_iComponentType);
            m_securityHelper.WriteUnicodeString(stream, m_strName);
        }
        #endregion
    }
}
