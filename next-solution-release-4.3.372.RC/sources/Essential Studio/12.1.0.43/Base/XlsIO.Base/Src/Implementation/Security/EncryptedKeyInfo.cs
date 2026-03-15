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
    /// Represents the attributes used to generate the encrypting key.
    /// </summary>
    public class EncryptedKeyInfo
    {
        #region Constants
        /// <summary>
        /// Default Spin Count to iterations on the hash of the password.
        /// </summary>
        internal  const int DefaultSpinCount = 100000;
        /// <summary>
        /// Represents the Default hash size in bytes.
        /// </summary>
        internal const int DefaultHashSize = 20;
        /// <summary>
        /// Represents the Default cipher used to encrypt the data is AES.
        /// </summary>
        internal const string DefaultCipherAlgorithm = "AES";
        /// <summary>
        /// Represents the Default chaining mode used for encrypting.
        /// </summary>
        internal const string DefaultCipherChaining = "ChainingModeCBC";
        /// <summary>
        /// Represents the Default hashing algorithm used in the Encryption.
        /// </summary>
        internal const string DefaultHashAlgorithm = "SHA1";
        /// <summary>
        /// Represents the default key length in bits.
        /// </summary>
        internal const int DefaultKeyBits = 128;
        #endregion

        #region Members
        /// <summary>
        /// Represents the iterations count on the hash of the password.
        /// </summary>
        private int m_spintCount;
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
        private int m_hasSize;
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
        /// <summary>
        /// Represents the password verification Hash input.
        /// </summary>
        private byte[] m_verifierHashInput;
        /// <summary>
        /// Represents the password verifcation hash value.
        /// </summary>
        private byte[] m_verifierHashValue;
        /// <summary>
        /// Represents the Intermediate key used to Encrypt and Decrypt the data.
        /// </summary>
        private byte[] m_encryptedKeyValue;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the Compute Hash Size.
        /// </summary>
        internal int HashSize
        {
            get
            {
                return m_hasSize;
            }
            set
            {
                m_hasSize = value;
            }
        }
        /// <summary>
        /// Represents the iterations count on the hash of the password.
        /// </summary>
        internal int SpinCount
        {
            get
            {
                return m_spintCount;
            }
            set
            {
                m_spintCount = value;
            }
        }
        /// <summary>
        /// Represents the block size used to encrypt each block of data.
        /// </summary>
        internal int BlockSize
        {
            get
            {
                return m_blockSize;
            }
            set
            {
                m_blockSize = value;
            }
        }
        /// <summary>
        /// Represents the Key size.
        /// </summary>
        internal int KeyBits
        {
            get
            {
                return m_keyBits / 8;
            }
            set
            {
                m_keyBits = value;
            }
        }
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
        /// <summary>
        /// Represents the password verifcation hash value.
        /// </summary>
        internal byte[] VerifierHashValue
        {
            get
            {
                return m_verifierHashValue;
            }
            set
            {
                m_verifierHashValue = value;
            }
        }
        /// <summary>
        /// Represents the password verification Hash input.
        /// </summary>
        internal byte[] VerifierHashInput
        {
            get
            {
                return m_verifierHashInput;
            }
            set
            {
                m_verifierHashInput = value;
            }
        }
        /// <summary>
        /// Represents the Intermediate key used to Encrypt and Decrypt the data.
        /// </summary>
        internal byte[] KeyValue
        {
            get
            {
                return m_encryptedKeyValue;
            }
            set
            {
                m_encryptedKeyValue = value;
            }
        }
        #endregion

        #region Initialization.
        internal EncryptedKeyInfo()
        {
            m_spintCount = DefaultSpinCount;
            m_saltSize = Excel2007Encryptor.KeyLength;
            m_blockSize = Excel2007Encryptor.KeyLength;
            m_hasSize = DefaultHashSize;
            m_keyBits = DefaultKeyBits;
            m_cipherAlgorithm = DefaultCipherAlgorithm;
            m_cipherChaining = DefaultCipherChaining;
            m_hashAlgorithm = DefaultHashAlgorithm;
        }
#endregion

        #region Parse and Serialize methods.
        /// <summary>
        /// Parses the attributes which is used to generate the encrypting key.
        /// </summary>
        /// <param name="reader">Xml Reader to read from.</param>
        internal void Parse(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != EncryptionConstants.EncryptedKeyTag)
                throw new XmlException();

            if (reader.MoveToAttribute(EncryptionConstants.SpinCount))
                m_spintCount = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.SaltSize))
                m_saltSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.BlockSize))
                m_blockSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.KeyBits))
                m_keyBits = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.HashSize))
                m_hasSize = Convert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.CipherAlgorithm))
                m_cipherAlgorithm = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.CipherChaining))
                m_cipherChaining = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.HashAlgorithm))
                m_hashAlgorithm = reader.Value;

            if (reader.MoveToAttribute(EncryptionConstants.SaltValue))
                m_saltValue = Convert.FromBase64String(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.EncryptedVerifierHashInput))
                m_verifierHashInput = Convert.FromBase64String(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.EncryptedVerifierHashValue))
                m_verifierHashValue = Convert.FromBase64String(reader.Value);

            if (reader.MoveToAttribute(EncryptionConstants.EncryptedKeyValue))
                m_encryptedKeyValue = Convert.FromBase64String(reader.Value);

        }
        /// <summary>
        /// Serializes the attributes which used to generate the encrypting key.
        /// </summary>
        /// <param name="writer">Xml Writer to write into.</param>
        internal void Serialize(XmlWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteStartElement(EncryptionConstants.KeyEncryptorTag);
            writer.WriteAttributeString(EncryptionConstants.Uri, EncryptionConstants.PasswordNameSpace);
            writer.WriteStartElement(EncryptionConstants.PPrefix, EncryptionConstants.EncryptedKeyTag, EncryptionConstants.PasswordNameSpace);
            writer.WriteAttributeString(EncryptionConstants.SpinCount, m_spintCount.ToString());

            writer.WriteAttributeString(EncryptionConstants.SaltSize, m_saltSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.BlockSize, m_blockSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.KeyBits, m_keyBits.ToString());

            writer.WriteAttributeString(EncryptionConstants.HashSize, m_hasSize.ToString());

            writer.WriteAttributeString(EncryptionConstants.CipherAlgorithm, m_cipherAlgorithm.ToString());

            writer.WriteAttributeString(EncryptionConstants.CipherChaining, m_cipherChaining.ToString());

            writer.WriteAttributeString(EncryptionConstants.HashAlgorithm, m_hashAlgorithm.ToString());

            writer.WriteAttributeString(EncryptionConstants.SaltValue, Convert.ToBase64String(m_saltValue));

            writer.WriteAttributeString(EncryptionConstants.EncryptedVerifierHashInput, Convert.ToBase64String(m_verifierHashInput));

            writer.WriteAttributeString(EncryptionConstants.EncryptedVerifierHashValue, Convert.ToBase64String(m_verifierHashValue));

            writer.WriteAttributeString(EncryptionConstants.EncryptedKeyValue, Convert.ToBase64String(m_encryptedKeyValue));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        #endregion
    }
}
