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
    /// Represents the cryptographic attributes used to encrypt the data.
    /// </summary>
    public class DataEncryptionInfo
    {
        #region Members
        /// <summary>
        /// Represents a randomly generated Salt Value size.
        /// </summary>
        private int m_saltSize;
        /// <summary>
        /// Represents the block size used to encrypt each block of data.
        /// </summary>
        private int m_blockSize;
        /// <summary>
        /// Represents the Key size.
        /// </summary>
        private int m_keyBits;
        /// <summary>
        /// Represents the Compute Hash Size.
        /// </summary>
        private int m_hashSize;
        /// <summary>
        /// Represents the cipher algorithm used to encrypt the data
        /// </summary>
        private string m_cipherAlgorithm;
        /// <summary>
        /// Represents the chaining mode to encrypt the data.
        /// </summary>
        private string m_cipherChaining;
        /// <summary>
        /// Represents  that the hashing algorithm used to hash the data.
        /// </summary>
        private string m_hashAlgorithm;
        /// <summary>
        /// Represents a randomly generated value used when generating the encryption key
        /// </summary>
        private byte[] m_saltValue;
        #endregion

        #region Properties
        /// <summary>
        /// Represents a randomly generated value used when generating the encryption key
        /// </summary>
        internal byte[] SaltValue
        {
            get
            {
                return m_saltValue;
            }
            set
            {
                m_saltValue = value;
            }
        }
        #endregion

        #region Intialization & Finalization
        internal DataEncryptionInfo()
        {
            m_saltSize = Excel2007Encryptor.KeyLength;
            m_blockSize = Excel2007Encryptor.KeyLength;
            m_keyBits = EncryptedKeyInfo.DefaultKeyBits;
            m_hashSize = EncryptedKeyInfo.DefaultHashSize;
            m_cipherAlgorithm = EncryptedKeyInfo.DefaultCipherAlgorithm;
            m_cipherChaining = EncryptedKeyInfo.DefaultCipherChaining;

            m_hashAlgorithm = EncryptedKeyInfo.DefaultHashAlgorithm;
        }
        #endregion

        #region Parse and Serialize
        /// <summary>
        /// Parses the cryptographic attributes used to encrypt the data.
        /// </summary>
        /// <param name="reader">XmlReader to read from.</param>
        internal void Parse(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != EncryptionConstants.KeyDataTag)
                throw new XmlException();

            if (reader.MoveToAttribute(EncryptionConstants.SaltSize))
                m_saltSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.BlockSize))
                m_blockSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.KeyBits))
                m_keyBits = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.HashSize))
                m_hashSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.CipherAlgorithm))
                m_cipherAlgorithm = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.CipherChaining))
                m_cipherChaining = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.HashAlgorithm))
                m_hashAlgorithm = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.SaltValue))
            {
                string saltValue = reader.Value;
                m_saltValue = Convert.FromBase64String(saltValue);
            }
        }
        /// <summary>
        /// Serializes the cryptographic attributes used to encrypt the data.
        /// </summary>
        /// <param name="writer">Writer to writes into.</param>
        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteStartElement(EncryptionConstants.KeyDataTag);

            writer.WriteAttributeString(EncryptionConstants.SaltSize, m_saltSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.BlockSize, m_blockSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.KeyBits, m_keyBits.ToString());

            writer.WriteAttributeString(EncryptionConstants.HashSize, m_hashSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.CipherAlgorithm, m_cipherAlgorithm.ToString());

            writer.WriteAttributeString(EncryptionConstants.CipherChaining, m_cipherChaining.ToString());

            writer.WriteAttributeString(EncryptionConstants.HashAlgorithm, m_hashAlgorithm.ToString());

            writer.WriteAttributeString(EncryptionConstants.SaltValue, Convert.ToBase64String(m_saltValue));
            writer.WriteEndElement();
        }
        #endregion
    }
}
