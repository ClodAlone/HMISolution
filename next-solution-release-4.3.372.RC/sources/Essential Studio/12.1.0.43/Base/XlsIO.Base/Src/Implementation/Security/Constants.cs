#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.Security
{
    /// <summary>
    /// This class contains constants used for Encryption Info parsing/serialization
    /// in Excel 2010 SpreadsheetML format.
    /// </summary>
    class EncryptionConstants
    {
        /// <summary>
        /// Represents the Salt Size attribute.
        /// </summary>
        public const string SaltSize = "saltSize";
        /// <summary>
        /// Represents the Salt Size attribute.
        /// </summary>
        public const string SaltValue = "saltValue";
        /// <summary>
        ///  Represents the Block Size attribute.
        /// </summary>
        public const string BlockSize = "blockSize";
        /// <summary>
        /// Represents the KeyBits attribute.
        /// </summary>
        public const string KeyBits = "keyBits";
        /// <summary>
        /// Represents the HashSize attribute.
        /// </summary>
        public const string HashSize = "hashSize";
        /// <summary>
        /// Represents the CipherAlgorithm attribute.
        /// </summary>
        public const string CipherAlgorithm = "cipherAlgorithm";
        /// <summary>
        /// Represents the cipher Chaining used to encrypt the data is AES.
        /// </summary>
        public const string CipherChaining = "cipherChaining";
        /// <summary>
        /// Represents the hashing algorithm used in the Encryption.
        /// </summary>
        public const string HashAlgorithm = "hashAlgorithm";
        /// <summary>
        /// Represents the encrypted copy of the randomly generated value used when generating the encryption key.
        /// </summary>
        public const string EncryptedHmacKey = "encryptedHmacKey";
        /// <summary>
        /// Represents the encrypted copy of the hash value that is generated during the creation of the encryption key.
        /// </summary>
        public const string EncryptedHmacValue = "encryptedHmacValue";
        /// <summary>
        /// Represents the iterations count on the hash of the password.
        /// </summary>
        public const string SpinCount = "spinCount";
        /// <summary>
        /// Represents the password verification Hash input.
        /// </summary>
        public const string EncryptedVerifierHashInput = "encryptedVerifierHashInput";
        /// <summary>
        ///  Represents the password verifcation hash value.
        /// </summary>
        public const string EncryptedVerifierHashValue = "encryptedVerifierHashValue";
        /// <summary>
        /// Represents the Intermediate key used to Encrypt and Decrypt the data.
        /// </summary>
        public const string EncryptedKeyValue = "encryptedKeyValue";
        /// <summary>
        /// Represents the Encryption tags namespace.
        /// </summary>
        public const string EncryptionNameSpace = "http://schemas.microsoft.com/office/2006/encryption";
        /// <summary>
        /// Represents the password encryption attribute tags namespace.
        /// </summary>
        public const string PasswordNameSpace = "http://schemas.microsoft.com/office/2006/keyEncryptor/password";
        /// <summary>
        /// Represents the attributes used to encrypt and decrypt the data.
        /// </summary>
        public const string EncryptionTag = "encryption";
        /// <summary>
        /// Represents the cryptographic attributes used to encrypt the data.
        /// </summary>
        public const string KeyDataTag = "keyData";
        /// <summary>
        /// Represents the Data Integrity attributes used to help ensure 
        /// that the integrity of the encrypted data.
        /// </summary>
        public const string DataIntegrityTag = "dataIntegrity";
        /// <summary>
        /// Represents the attributes used to generate the encrypting key.
        /// </summary>
        public const string KeyEncryptorTag = "keyEncryptor";
        /// <summary>
        /// Represents the attributes used to generate the encrypting keys.
        /// </summary>
        public const string KeyEncryptorsTag = "keyEncryptors";
        /// <summary>
        ///  Represents the attributes used to generate the encrypting key.
        /// </summary>
        public const string EncryptedKeyTag = "encryptedKey";
        /// <summary>
        /// Represents the Prefix p.
        /// </summary>
        public const string PPrefix = "p";
        /// <summary>
        /// Represents the Xmlns attribute
        /// </summary>
        public const string Xmlns = "xmlns";
        /// <summary>
        /// Represents the Uir attribute.
        /// </summary>
        public const string Uri = "uri";
    }
}
