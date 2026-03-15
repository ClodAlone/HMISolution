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
using System.Xml;
using System.IO;
using Syncfusion.DocIO.DLS.Convertors;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Security
{
    /// <summary>
    /// Represents the xml encryption descriptor for Encryption/Decryption of Word 2010/2013 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class XmlEncryptionDescriptor
    {
        #region Constants
        private const string XMLNameSpace = "xmlns";
        #endregion

        #region Members
        /// <summary>
        /// Specifies the key data used for Encryption/Decryption.
        /// </summary>
        private KeyData m_keyData = new KeyData();
        /// <summary>
        /// Specifies the data integrity used for Encryption/Decryption.
        /// </summary>
        private DataIntegrity m_dataIntegrity = new DataIntegrity();
        /// <summary>
        /// Specifies the key encryptors used for Encryption/Decryption.
        /// </summary>
        private KeyEncryptors m_keyEncryptors = new KeyEncryptors();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the key data.
        /// </summary>
        /// <value>The key data.</value>
        internal KeyData KeyData
        {
            get
            {
                return m_keyData;
            }
            set
            {
                m_keyData = value;
            }
        }
        /// <summary>
        /// Gets or sets the data integrity.
        /// </summary>
        /// <value>The data integrity.</value>
        internal DataIntegrity DataIntegrity
        {
            get
            {
                return m_dataIntegrity;
            }
            set
            {
                m_dataIntegrity = value;
            }
        }
        /// <summary>
        /// Gets or sets the key encryptors.
        /// </summary>
        /// <value>The key encryptors.</value>
        internal KeyEncryptors KeyEncryptors
        {
            get
            {
                return m_keyEncryptors;
            }
            set
            {
                m_keyEncryptors = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlEncryptionDescriptor()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Parse(Stream stream)
        {
            XmlReader reader = CreateReader(stream);
            //string element = reader.LocalName;
            reader.Read();
            while (reader.LocalName != "encryption")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "keyData":
                            m_keyData.Parse(reader);
                            break;
                        case "dataIntegrity":
                            m_dataIntegrity.Parse(reader);
                            break;
                        case "keyEncryptors":
                            m_keyEncryptors.Parse(reader);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Serializes object into stream.
        /// </summary>
        /// <param name="stream">Stream to serialize into.</param>
        public void Serialize(Stream stream)
        {
            XmlWriter writer = CreateWriter(stream);
            writer.WriteStartElement("encryption", DocxConstants.E_namespace);
            writer.WriteAttributeString(XMLNameSpace, DocxConstants.E_namespace);
            writer.WriteAttributeString(XMLNameSpace, "p", null, DocxConstants.P_namespace);
            //For Word 2013 encryption, key bits - 256.
            if (m_keyData.KeyBits == 256)
                writer.WriteAttributeString(XMLNameSpace, "c", null, DocxConstants.Cert_namespace);
            m_keyData.Serialize(writer);
            m_dataIntegrity.Serialize(writer);
            m_keyEncryptors.Serialize(writer);
            writer.WriteEndElement();
            writer.Flush();
        }
        /// <summary>
        /// Create xml writer
        /// </summary>
        /// <param name="data">The stream</param>
        /// <returns>returns the xml writer</returns>
        private XmlWriter CreateWriter(Stream data)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(data, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
            return writer;
        }
        public XmlReader CreateReader(Stream data)
        {
            XmlReader reader = XmlReader.Create(data);
            while (reader.NodeType != XmlNodeType.Element)
            {
                reader.Read();
            }
            return reader;
        }
        #endregion
    }

    /// <summary>
    /// Represents the key data for Encryption/Decryption of Word 2010 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class KeyData
    {
        #region Members
        /// <summary>
        /// Specifies the number of bytes used by salt.
        /// </summary>
        private int m_iSaltSize;
        /// <summary>
        /// Specifies the number of bytes used to encrypt one block of data.
        /// </summary>
        private int m_iBlockSize;
        /// <summary>
        /// Specifies the number of bits used by encryption algorithm.
        /// </summary>
        private int m_iKeyBits;
        /// <summary>
        /// Specifies the number of bytes used by hash value.
        /// </summary>
        private int m_iHashSize;
        /// <summary>
        /// Specifies the cipher algorithm.
        /// </summary>
        private string m_sCipherAlgorithm;
        /// <summary>
        /// Specifies the chaining mode used by the cipher algorithm.
        /// </summary>
        private string m_sCipherChaining;
        /// <summary>
        /// Specifies the hashing algorithm.
        /// </summary>
        private string m_sHashAlgorithm;
        /// <summary>
        /// Specifies the salt value.
        /// </summary>
        private byte[] m_arrSalt;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the salt.
        /// </summary>
        /// <value>The size of the salt.</value>
        internal int SaltSize
        {
            get
            {
                return m_iSaltSize;
            }
            set
            {
                m_iSaltSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the block.
        /// </summary>
        /// <value>The size of the block.</value>
        internal int BlockSize
        {
            get
            {
                return m_iBlockSize;
            }
            set
            {
                m_iBlockSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the key bits.
        /// </summary>
        /// <value>The key bits.</value>
        internal int KeyBits
        {
            get
            {
                return m_iKeyBits;
            }
            set
            {
                m_iKeyBits = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the hash.
        /// </summary>
        /// <value>The size of the hash.</value>
        internal int HashSize
        {
            get
            {
                return m_iHashSize;
            }
            set
            {
                m_iHashSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the cipher algorithm.
        /// </summary>
        /// <value>The cipher algorithm.</value>
        internal string CipherAlgorithm
        {
            get
            {
                return m_sCipherAlgorithm;
            }
            set
            {
                m_sCipherAlgorithm = value;
            }
        }
        /// <summary>
        /// Gets or sets the cipher chaining.
        /// </summary>
        /// <value>The cipher chaining.</value>
        internal string CipherChaining
        {
            get
            {
                return m_sCipherChaining;
            }
            set
            {
                m_sCipherChaining = value;
            }
        }
        /// <summary>
        /// Gets or sets the hash algorithm.
        /// </summary>
        /// <value>The hash algorithm.</value>
        internal string HashAlgorithm
        {
            get
            {
                return m_sHashAlgorithm;
            }
            set
            {
                m_sHashAlgorithm = value;
            }
        }
        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>The salt.</value>
        internal byte[] Salt
        {
            get
            {
                return m_arrSalt;
            }
            set
            {
                m_arrSalt = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal KeyData()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void Parse(XmlReader reader)
        {
            m_iSaltSize = Int32.Parse(reader.GetAttribute("saltSize"));
            m_iBlockSize = Int32.Parse(reader.GetAttribute("blockSize"));
            m_iKeyBits = Int32.Parse(reader.GetAttribute("keyBits"));
            m_iHashSize = Int32.Parse(reader.GetAttribute("hashSize"));
            m_sCipherAlgorithm = reader.GetAttribute("cipherAlgorithm");
            m_sCipherChaining = reader.GetAttribute("cipherChaining");
            m_sHashAlgorithm = reader.GetAttribute("hashAlgorithm");
            m_arrSalt = Convert.FromBase64String(reader.GetAttribute("saltValue"));
        }
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("keyData");
            writer.WriteAttributeString("saltSize", m_iSaltSize.ToString());
            writer.WriteAttributeString("blockSize", m_iBlockSize.ToString());
            writer.WriteAttributeString("keyBits", m_iKeyBits.ToString());
            writer.WriteAttributeString("hashSize", m_iHashSize.ToString());
            writer.WriteAttributeString("cipherAlgorithm", m_sCipherAlgorithm);
            writer.WriteAttributeString("cipherChaining", m_sCipherChaining);
            writer.WriteAttributeString("hashAlgorithm", m_sHashAlgorithm);
            writer.WriteAttributeString("saltValue", Convert.ToBase64String(m_arrSalt));
            writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// Represents the data integrity for Encryption/Decryption of Word 2010 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class DataIntegrity
    {
        #region Members
        /// <summary>
        /// Specifies an encrypted key used for generating the encryptedHmacValue.
        /// </summary>
        private byte[] m_encryptedHmacKey;
        /// <summary>
        /// Specifies an HMAC derived from the encryptedHmacKey and the encrypted data.
        /// </summary>
        private byte[] m_encryptedHmacValue;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the encrypted hmac key.
        /// </summary>
        /// <value>The encrypted hmac key.</value>
        internal byte[] EncryptedHmacKey
        {
            get
            {
                return m_encryptedHmacKey;
            }
            set
            {
                m_encryptedHmacKey = value;
            }
        }
        /// <summary>
        /// Gets or sets the encrypted hmac value.
        /// </summary>
        /// <value>The encrypted hmac value.</value>
        internal byte[] EncryptedHmacValue
        {
            get
            {
                return m_encryptedHmacValue;
            }
            set
            {
                m_encryptedHmacValue = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal DataIntegrity()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void Parse(XmlReader reader)
        {
            m_encryptedHmacKey = Convert.FromBase64String(reader.GetAttribute("encryptedHmacKey"));
            m_encryptedHmacValue = Convert.FromBase64String(reader.GetAttribute("encryptedHmacValue"));
        }
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("dataIntegrity");
            writer.WriteAttributeString("encryptedHmacKey", Convert.ToBase64String(m_encryptedHmacKey));
            writer.WriteAttributeString("encryptedHmacValue", Convert.ToBase64String(m_encryptedHmacValue));
            writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// Represents the key encryptors for Encryption/Decryption of Word 2010 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class KeyEncryptors
    {
        #region Members
        /// <summary>
        /// Specifies encrypted key used for Encryption/Decryption.
        /// </summary>
        private EncryptedKey m_encryptedKey = new EncryptedKey();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the encrypted key.
        /// </summary>
        /// <value>The encrypted key.</value>
        internal EncryptedKey EncryptedKey
        {
            get
            {
                return m_encryptedKey;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal KeyEncryptors()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void Parse(XmlReader reader)
        {
            reader.Read();
            while (reader.LocalName != "keyEncryptors")
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "encryptedKey":
                            m_encryptedKey.Parse(reader);
                            break;
                    }
                    reader.Read();
                }
                else
                {
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("keyEncryptors");
            writer.WriteStartElement("keyEncryptor");
            writer.WriteAttributeString("uri", DocxConstants.P_namespace);
            m_encryptedKey.Serialize(writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// Represents the encrypted key for Encryption/Decryption of Word 2010 documents.
    /// </summary>
    [CLSCompliant(false)]
    internal class EncryptedKey
    {
        #region Members
        /// <summary>
        /// Specifies the spin count
        /// </summary>
        private int m_iSpinCount;
        /// <summary>
        /// Specifies the number of bytes used by salt.
        /// </summary>
        private int m_iSaltSize;
        /// <summary>
        /// Specifies the number of bytes used to encrypt one block of data.
        /// </summary>
        private int m_iBlockSize;
        /// <summary>
        /// Specifies the number of bits used by encryption algorithm.
        /// </summary>
        private int m_iKeyBits;
        /// <summary>
        /// Specifies the number of bytes used by hash value.
        /// </summary>
        private int m_iHashSize;
        /// <summary>
        /// Specifies the cipher algorithm.
        /// </summary>
        private string m_sCipherAlgorithm;
        /// <summary>
        /// Specifies the chaining mode used by the cipher algorithm.
        /// </summary>
        private string m_sCipherChaining;
        /// <summary>
        /// Specifies the hashing algorithm.
        /// </summary>
        private string m_sHashAlgorithm;
        /// <summary>
        /// Specifies the salt value.
        /// </summary>
        private byte[] m_arrSalt;
        /// <summary>
        /// Specifies the encrypted verifier hash input used in password verification.
        /// </summary>
        private byte[] m_encryptedVerifierHashInput;
        /// <summary>
        /// Specifies the encrypted verifier hash value used in password verification.
        /// </summary>
        private byte[] m_encryptedVerifierHashValue;
        /// <summary>
        /// Specifies the encrypted form of the intermediate key.
        /// </summary>
        private byte[] m_encryptedKeyValue;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the spin count.
        /// </summary>
        /// <value>The spin count.</value>
        internal int SpinCount
        {
            get
            {
                return m_iSpinCount;
            }
            set
            {
                m_iSpinCount = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the salt.
        /// </summary>
        /// <value>The size of the salt.</value>
        internal int SaltSize
        {
            get
            {
                return m_iSaltSize;
            }
            set
            {
                m_iSaltSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the block.
        /// </summary>
        /// <value>The size of the block.</value>
        internal int BlockSize
        {
            get
            {
                return m_iBlockSize;
            }
            set
            {
                m_iBlockSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the key bits.
        /// </summary>
        /// <value>The key bits.</value>
        internal int KeyBits
        {
            get
            {
                return m_iKeyBits;
            }
            set
            {
                m_iKeyBits = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the hash.
        /// </summary>
        /// <value>The size of the hash.</value>
        internal int HashSize
        {
            get
            {
                return m_iHashSize;
            }
            set
            {
                m_iHashSize = value;
            }
        }
        /// <summary>
        /// Gets or sets the cipher algorithm.
        /// </summary>
        /// <value>The cipher algorithm.</value>
        internal string CipherAlgorithm
        {
            get
            {
                return m_sCipherAlgorithm;
            }
            set
            {
                m_sCipherAlgorithm = value;
            }
        }
        /// <summary>
        /// Gets or sets the cipher chaining.
        /// </summary>
        /// <value>The cipher chaining.</value>
        internal string CipherChaining
        {
            get
            {
                return m_sCipherChaining;
            }
            set
            {
                m_sCipherChaining = value;
            }
        }
        /// <summary>
        /// Gets or sets the hash algorithm.
        /// </summary>
        /// <value>The hash algorithm.</value>
        internal string HashAlgorithm
        {
            get
            {
                return m_sHashAlgorithm;
            }
            set
            {
                m_sHashAlgorithm = value;
            }
        }
        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>The salt.</value>
        internal byte[] Salt
        {
            get
            {
                return m_arrSalt;
            }
            set
            {
                m_arrSalt = value;
            }
        }
        /// <summary>
        /// Gets or sets the encrypted verifier hash input.
        /// </summary>
        /// <value>The encrypted verifier hash input.</value>
        internal byte[] EncryptedVerifierHashInput
        {
            get
            {
                return m_encryptedVerifierHashInput;
            }
            set
            {
                m_encryptedVerifierHashInput = value;
            }
        }
        /// <summary>
        /// Gets or sets the encrypted verifier hash value.
        /// </summary>
        /// <value>The encrypted verifier hash value.</value>
        internal byte[] EncryptedVerifierHashValue
        {
            get
            {
                return m_encryptedVerifierHashValue;
            }
            set
            {
                m_encryptedVerifierHashValue = value;
            }
        }
        /// <summary>
        /// Gets or sets the encrypted key value.
        /// </summary>
        /// <value>The encrypted key value.</value>
        internal byte[] EncryptedKeyValue
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

        #region Constructor
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal EncryptedKey()
        {
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal void Parse(XmlReader reader)
        {
            m_iSpinCount = Int32.Parse(reader.GetAttribute("spinCount"));
            m_iSaltSize = Int32.Parse(reader.GetAttribute("saltSize"));
            m_iBlockSize = Int32.Parse(reader.GetAttribute("blockSize"));
            m_iKeyBits = Int32.Parse(reader.GetAttribute("keyBits"));
            m_iHashSize = Int32.Parse(reader.GetAttribute("hashSize"));
            m_sCipherAlgorithm = reader.GetAttribute("cipherAlgorithm");
            m_sCipherChaining = reader.GetAttribute("cipherChaining");
            m_sHashAlgorithm = reader.GetAttribute("hashAlgorithm");
            m_arrSalt = Convert.FromBase64String(reader.GetAttribute("saltValue"));
            m_encryptedVerifierHashInput = Convert.FromBase64String(reader.GetAttribute("encryptedVerifierHashInput"));
            m_encryptedVerifierHashValue = Convert.FromBase64String(reader.GetAttribute("encryptedVerifierHashValue"));
            m_encryptedKeyValue = Convert.FromBase64String(reader.GetAttribute("encryptedKeyValue"));
        }
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("p","encryptedKey", DocxConstants.P_namespace);
            writer.WriteAttributeString("spinCount", m_iSpinCount.ToString());
            writer.WriteAttributeString("saltSize", m_iSaltSize.ToString());
            writer.WriteAttributeString("blockSize", m_iBlockSize.ToString());
            writer.WriteAttributeString("keyBits", m_iKeyBits.ToString());
            writer.WriteAttributeString("hashSize", m_iHashSize.ToString());
            writer.WriteAttributeString("cipherAlgorithm", m_sCipherAlgorithm);
            writer.WriteAttributeString("cipherChaining", m_sCipherChaining);
            writer.WriteAttributeString("hashAlgorithm", m_sHashAlgorithm);
            writer.WriteAttributeString("saltValue", Convert.ToBase64String(m_arrSalt));
            writer.WriteAttributeString("encryptedVerifierHashInput", Convert.ToBase64String(m_encryptedVerifierHashInput));
            writer.WriteAttributeString("encryptedVerifierHashValue", Convert.ToBase64String(m_encryptedVerifierHashValue));
            writer.WriteAttributeString("encryptedKeyValue", Convert.ToBase64String(m_encryptedKeyValue));
            writer.WriteEndElement();
        }
        #endregion
    }
}
