#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml;
namespace Syncfusion.XlsIO.Implementation.Security
{
    /// <summary>
    /// Represents the Data Integrity attributes used to help ensure 
    /// that the integrity of the encrypted data.
    /// </summary>
    public class DataIntegrityInfo
    {           
        #region Members
        /// <summary>
        /// Represents the encrypted copy of the randomly generated value used when generating the encryption key.
        /// </summary>
        private byte[] m_HMacKey;
        /// <summary>
        /// Represents the encrypted copy of the hash value that is generated during the creation of the encryption key.
        /// </summary>
        private byte[] m_HmacValue;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the encrypted copy of the randomly generated value used when generating the encryption key.
        /// </summary>
        internal byte[] HMacKey
        {
            get
            {
                return m_HMacKey;
            }
            set
            {
                m_HMacKey = value;
            }
        }
        /// <summary>
        /// Represents the encrypted copy of the hash value that is generated during the creation of the encryption key.
        /// </summary>
        internal byte[] HMacValue
        {
            get
            {
                return m_HmacValue;
            }
            set
            {
                m_HmacValue = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse Data Integrity attributes.
        /// </summary>
        /// <param name="reader">XmlReader to Read from.</param>
        internal void Parse(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != EncryptionConstants.DataIntegrityTag)
                throw new XmlException();

            if (reader.MoveToAttribute(EncryptionConstants.EncryptedHmacKey))
                m_HMacKey = Convert.FromBase64String(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.EncryptedHmacValue))
                m_HmacValue = Convert.FromBase64String(reader.Value);

        }
        /// <summary>
        /// Serializes the Data Integrity attributes.
        /// </summary>
        /// <param name="writer">Writer to writes into.</param>
        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteStartElement(EncryptionConstants.DataIntegrityTag);
            writer.WriteAttributeString(EncryptionConstants.EncryptedHmacKey, Convert.ToBase64String(m_HMacKey));

            writer.WriteAttributeString(EncryptionConstants.EncryptedHmacValue, Convert.ToBase64String(m_HmacValue));
            writer.WriteEndElement();
        }
        #endregion

    }
}
