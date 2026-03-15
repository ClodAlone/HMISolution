#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !WP

using System;
using System.Collections;
#if NETFX_CORE || WP
using Syncfusion.Pdf.Cryptography;
#else
using System.Security.Cryptography;
#endif
using System.Text;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// The Syncfusion.Pdf.Security namespace contains classes for creating protected PDF document.
/// </summary>
namespace Syncfusion.Pdf.Security
{
#if NETFX_CORE || WP
    public class PdfEncryptor
#else
    internal class PdfEncryptor
#endif
    {
#region Constants
        /// <summary>
        /// New key offset length.
        /// </summary>
        private const int c_newKeyOffset = 5;

        /// <summary>
        /// Key length of 40 bit key.
        /// </summary>
        private const int c_key40 = 5;

        /// <summary>
        /// Key length of 128 bit key.
        /// </summary>
        private const int c_key128 = 16;

        /// <summary>
        /// Key length of 256 bit key.
        /// </summary>
        private const int c_key256 = 32;

        /// <summary>
        /// Revision number. A number specifying which revision of the
        /// standard security handler should be used to interpret this dictionary.
        /// </summary>
        private const int c_40RevisionNumber = 2;

        /// <summary>
        /// Revision number. A number specifying which revision of the
        /// standard security handler should be used to interpret this dictionary.
        /// </summary>
        private const int c_128RevisionNumber = 3;

        /// <summary>
        /// Amount of bytes.
        /// </summary>
        private const int c_bytesAmount = 256;

        /// <summary>
        /// Amount of random bytes.
        /// </summary>
        private const int c_randomBytesAmount = 16;

        /// <summary>
        /// Optimal string length.
        /// </summary>
        private const int c_stringLength = 32;

        /// <summary>
        /// Number of iteration of the loop during owner pass calculation.
        /// </summary>
        private const int c_ownerLoopNum = 50;

        /// <summary>
        /// Number of iteration of the loop during owner pass calculation.
        /// </summary>
        private const int c_ownerLoopNum2 = 20;

        /// <summary>
        /// Flag, used during enc. key calculating.
        /// </summary>
        private const byte c_flagNum = 0xFF;

        /// <summary>
        /// Number of bits in one byte.
        /// </summary>
        internal const byte c_numBits = 8;
        private const int c_permissionSet = ~0x00f3f;
        private const int c_permissionCleared = ~0x3;
        private const int c_permissionRevisionTwoMask = 0xfff;
        #endregion

#region Fields
        /// <summary>
        /// A value indicating whether password values were already computed.
        /// </summary>
        private bool m_hasComputedPasswordValues;
        /// <summary>
        /// The object that allows to compute the MD5 hash for the input data
        /// using the implementation provided by the cryptographic service provider (CSP).
        /// </summary>
        private MD5CryptoServiceProvider m_provider;// = new MD5CryptoServiceProvider();
        /// <summary>
        /// Bytes array for manipulating by Custom algo.
        /// </summary>
        private byte[] m_customArray;
        /// <summary>
        /// Bytes array of random numbers.
        /// </summary>
        private byte[] m_randomBytes;
        /// <summary>
        /// Output owner password.
        /// </summary>
        private string m_ownerPassword = string.Empty;
        /// <summary>
        /// Output user password.
        /// </summary>
        private string m_userPassword = string.Empty;
        /// <summary>
        /// Changed owner password. It's needed to encryption purpose.
        /// </summary>
        private byte[] m_ownerPasswordOut;
        /// <summary>
        /// Changed user password. It's needed to encryption purpose.
        /// </summary>
        private byte[] m_userPasswordOut;
        /// <summary>
        /// The encryption key.
        /// </summary>
        private byte[] m_encryptionKey;
        /// <summary>
        /// Length of encryption key.
        /// </summary>
        private PdfEncryptionKeySize m_keyLength = PdfEncryptionKeySize.Key128Bit;
        /// <summary>
        /// Permission flags.
        /// </summary>
        private PdfPermissionsFlags m_permission = PdfPermissionsFlags.Default;
        /// <summary>
        /// A revision number that has been read from a PDF document.
        /// </summary>
        private int m_revision = 0;
        /// <summary>
        /// Shows if the encryptor's settings have been changed.
        /// </summary>
        private bool m_bChanged;
        /// <summary>
        /// Predefined bytes for empty string.
        /// </summary>
        private static byte[] s_paddingString;
        /// <summary>
        /// Helps to control access to s_paddingString.
        /// </summary>
        private static object s_lockObject = new object();
        /// <summary>
        /// Holds ecryptor status enable/disable ecryption.
        /// </summary>
        private bool m_encrypt;
        /// <summary>
        /// The valuse that should be stored in the encryptor dictionary.
        /// </summary>
        private int m_permissionValue;
		/// <summary>
        /// The array used to add padding to the encryption key in AES mode.
        /// </summary>
        private static readonly byte[] salt = { (byte)0x73, (byte)0x41, (byte)0x6c, (byte)0x54 };
		/// <summary>
        /// The object that describes the type of encryption algorithm that should be used.
        /// </summary>
        private PdfEncryptionAlgorithm m_encryptionAlgorithm = PdfEncryptionAlgorithm.RC4;
        /// <summary>
        /// The user encryption key (UE), that should stored in encryption dictionary.
        /// </summary>
        private byte[] m_userEncryptionKeyOut;
        /// <summary>
        /// The Owner encryption key (OE), that should stored in encryption dictionary.
        /// </summary>
        private byte[] m_ownerEncryptionKeyOut;
        /// <summary>
        /// The permission flag (Perms), that should stored in encryption dictionary.
        /// </summary>
        private byte[] m_permissionFlag;
        /// <summary>
        /// The 32 byte random number used as key for encrypting contents.
        /// </summary>
        private byte[] m_fileEncryptionKey;
        /// <summary>
        /// The random bytes used in computing U and UE entries.
        /// </summary>
        private byte[] m_userRandomBytes;
        /// <summary>
        /// The random bytes used in computing O and OE entries.
        /// </summary>
        private byte[] m_ownerRandomBytes;
        /// <summary>
        /// Used to derive random byte array.
        /// </summary>
        private Random m_randomArray = new Random();
        /// <summary>
        /// Used to compute SHA256 hash.
        /// </summary>
        SHA256Managed m_hashComputer;
		/// <summary>
        /// Shows if metadata has to be encrypted or not.
        /// </summary>
        private bool m_encryptMetadata = true;
		/// <summary>
        /// The integer which represents revision of the encryption dictionary. 
        /// </summary>
        private int m_revisionNumberOut;
		/// <summary>
        /// The integer which represents the version of the encryption dictionary.
        /// </summary>
        private int m_versionNumberOut;
        #endregion

#region Properties

        private SHA256Managed HashComputer
        {
            get
            {
                if (m_hashComputer == null)
                    m_hashComputer = new SHA256Managed();

                return m_hashComputer;
            }

        }
        /// <summary>
        /// Gets file ID.
        /// </summary>
        public PdfArray FileID
        {
            get
            {
                PdfString str = new PdfString(RandomBytes);
                PdfArray array = new PdfArray();

                array.Add(str);
                array.Add(str);

                return array;
            }
        }

        /// <summary>
        /// Gets security handler. Filter's value.
        /// </summary>
        public string Filter
        {
            get
            {
                return SecurityHandlers.Standard.ToString();
            }
        }

        /// <summary>
        /// Gets or sets cryptographic algorithm. V's value.
        /// </summary>
        public PdfEncryptionKeySize CryptographicAlgorithm
        {
            get
            {
                return m_keyLength;
            }
            set
            {
                if (m_keyLength != value)
                {
                    m_keyLength = value;
                    m_bChanged = true;
                    m_hasComputedPasswordValues = false;
                }
            }
        }
		
		/// <summary>
        /// Gets or sets encryption algorithm.
        /// </summary>
        public PdfEncryptionAlgorithm EncryptionAlgorithm
        {
            get
            {
                return m_encryptionAlgorithm;
            }
            set
            {
                m_encryptionAlgorithm = value;
            }
        }

        /// <summary>
        /// Gets or sets permission set.
        /// </summary>
        internal PdfPermissionsFlags Permissions
        {
            get
            {
                return m_permission;
            }
            set
            {
                m_bChanged = true;
                m_permission = value;
                m_permissionValue = ((int)m_permission | c_permissionSet) & c_permissionCleared;

                if (RevisionNumber > 2) m_permissionValue &= c_permissionRevisionTwoMask;

                m_hasComputedPasswordValues = false;
            }
        }

        /// <summary>
        /// Gets revision number. R's value.
        /// </summary>
        public int RevisionNumber
        {
            get
            {
                if (m_revision == 0)
                {
                    return /*m_revision =*/ ((CryptographicAlgorithm == PdfEncryptionKeySize.Key40Bit) ?
                    c_40RevisionNumber : c_128RevisionNumber);
                }
                else
                {

                    return m_revision;
                }
            }
        }

        /// <summary>
        /// Gets or sets the password required to change permissions
        /// for the PDF document. O's value.
        /// </summary>
        internal string OwnerPassword
        {
            get
            {
                return m_ownerPassword;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("OwnerPassword");
                if (Provider == null)
                    throw new NotSupportedException("Document encryption is not allowed in FIPS mode.");
                if (m_ownerPassword != value)
                {
                    m_bChanged = true;
                    m_ownerPassword = value;

                    // Set flag to False, which forces reinitialize data.
                    m_hasComputedPasswordValues = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the password required to open the PDF document. P's value.
        /// </summary>
        internal string UserPassword
        {
            get
            {
                return m_userPassword;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("UserPassword");
                if (Provider == null)
                    throw new NotSupportedException("Document encryption is not allowed in FIPS mode.");

                if (m_userPassword != value)
                {
                    m_bChanged = true;
                    m_userPassword = value;

                    // Set flag to False, which forces reinitialize data.
                    m_hasComputedPasswordValues = false;
                }
            }
        }

        /// <summary>
        /// Gets bytes array of random numbers.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected byte[] RandomBytes
        {
            get
            {
                if (m_randomBytes == null)
                {
                    m_randomBytes = new byte[c_randomBytesAmount];
                    m_randomArray.NextBytes(m_randomBytes);
                    //for (byte i = 0; i < c_randomBytesAmount; i++)
                    //{
                    //    m_randomBytes[i] = i;
                    //}
                }

                return m_randomBytes;
            }
        }

        /// <summary>
        /// Gets or sets bytes array for manipulating by Custom algo.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected byte[] CustomArray
        {
            get
            {
                return m_customArray;
            }
            set
            {
                if (m_customArray != value)
                {
                    m_customArray = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the object that allows to compute the MD5 hash for the input data
        /// using the implementation provided by the cryptographic service provider (CSP).
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected MD5CryptoServiceProvider Provider
        {
            get
            {
                return m_provider;
            }
        }

        /// <summary>
        /// Gets encoding.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected Encoding SecurityEncoding
        {
            get
            {
#if NETFX_CORE || WP
                return Encoding.UTF8;
#else
return Encoding.Default;
#endif
            }
        }

        /// <summary>
        /// Gets or sets encryption key.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected byte[] EncryptionKey
        {
            get
            {
                return m_encryptionKey;
            }
            set
            {
                if (m_encryptionKey != value)
                {
                    m_encryptionKey = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether document should be encrypted or not.
        /// </summary>
        internal bool Encrypt
        {
            get
            {
                PdfPermissionsFlags perm = Permissions;

                bool bEncrypt = (perm != PdfPermissionsFlags.Default ||
                    m_userPassword.Length > 0 ||
                    m_ownerPassword.Length > 0);

                if (!m_encrypt) return false;

                return bEncrypt;
            }
            set
            {
                m_encrypt = value;
            }
        }

        /// <summary>
        /// Gets calculated user password.
        /// </summary>
        internal byte[] UserPasswordOut
        {
            get
            {
                InitializeData();

                return m_userPasswordOut;
            }
        }

        /// <summary>
        /// Gets calculated owner password.
        /// </summary>
        internal byte[] OwnerPasswordOut
        {
            get
            {
                InitializeData();

                return m_ownerPasswordOut;
            }
        }

        /// <summary>
        /// Shows if the encryptor's setting have been changed.
        /// </summary>
        internal bool Changed
        {
            get
            {
                return m_bChanged;
            }
        }

        /// <summary>
        /// Shows if the metadata should be encrypted.
        /// </summary>
        internal bool EncryptMetaData
        {
            get
            {
                return m_encryptMetadata;
            }

            set
            {
                m_encryptMetadata = value;
            }
        }

        /// <summary>
        /// Bytes for empty string.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected static byte[] PaddingString
        {
            get
            {
                return s_paddingString;
            }
            set
            {
                lock (s_lockObject)
                {
                    if (s_paddingString != value)
                    {
                        s_paddingString = value;
                    }
                }
            }
        }
        #endregion

#region Construntors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfEncryptor"/> class.
        /// </summary>
        internal PdfEncryptor()
        {
            PaddingString = new byte[c_stringLength]
				{
					40, 191, 78, 94, 78, 117, 138, 65, 100, 0, 78, 86, 255, 250, 1, 8,
					46, 46, 0, 182, 208, 104, 62, 128, 47, 12, 169, 254, 100, 83, 105, 122
				};

            CustomArray = new byte[c_bytesAmount];
            Encrypt = true;
            Permissions = PdfPermissionsFlags.Default;
            try
            {
                m_provider = new MD5CryptoServiceProvider();
            }
            catch
            {
                // do nothing
            }
        }

        /// <summary>
        /// Clones the specified document.
        /// </summary>
        /// <returns>A new cloned encryptor.</returns>
        internal PdfEncryptor Clone()
        {
            PdfEncryptor cloneEncryptor = MemberwiseClone() as PdfEncryptor;

            cloneEncryptor.CryptographicAlgorithm = m_keyLength;
            cloneEncryptor.UserPassword = UserPassword;
            cloneEncryptor.OwnerPassword = OwnerPassword;
            cloneEncryptor.Permissions = Permissions;

            cloneEncryptor.m_randomBytes = m_randomBytes.Clone() as byte[];
            cloneEncryptor.m_customArray = m_customArray.Clone() as byte[];
            cloneEncryptor.m_revision = m_revision;
            if (m_encryptionKey != null)
            cloneEncryptor.m_encryptionKey = m_encryptionKey.Clone() as byte[];
            cloneEncryptor.m_customArray = m_customArray.Clone() as byte[];
            cloneEncryptor.m_ownerPasswordOut = m_ownerPasswordOut.Clone() as byte[];
            cloneEncryptor.m_userPasswordOut = m_userPasswordOut.Clone() as byte[];
            cloneEncryptor.m_hasComputedPasswordValues = m_hasComputedPasswordValues;
            cloneEncryptor.m_bChanged = m_bChanged;

            return cloneEncryptor;

        }
        #endregion

#region Public Methods
        /// <summary>
        /// Reads the essential values from a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal void ReadFromDictionary(PdfDictionary dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            IPdfPrimitive obj = PdfCrossTable.Dereference(dictionary[DictionaryProperties.Filter]);

            PdfName name = obj as PdfName;

            if (name.Value != DictionaryProperties.Standard)
            {
                throw new PdfDocumentException("Invalid Format: Unsupported security filter: " + name.Value);
            }
            m_permissionValue = dictionary.GetInt(DictionaryProperties.P);
            m_permission = (PdfPermissionsFlags)((int)m_permissionValue & ~c_permissionSet);
            m_keyLength = (PdfEncryptionKeySize)dictionary.GetInt(DictionaryProperties.V);
            m_revisionNumberOut = dictionary.GetInt(DictionaryProperties.R);
            m_versionNumberOut = dictionary.GetInt(DictionaryProperties.V);

            if ((int)m_keyLength == 4)
            {
                if ((int)m_keyLength != dictionary.GetInt(DictionaryProperties.R))
                {
                    throw new PdfDocumentException(
                     "Invalid Format: V and R entries of the Encryption dictionary doesn't match.");
                }
            }
            //else if ((int)m_keyLength + 1 != dictionary.GetInt(DictionaryProperties.R))
            //{
            //    throw new PdfDocumentException(
            //        "Invalid Format: V and R entries of the Encryption dictionary doesn't match.");
            //}

            if ((int)m_keyLength == 5)
            {
                m_userEncryptionKeyOut = dictionary.GetString(DictionaryProperties.UE).Bytes;
                m_ownerEncryptionKeyOut = dictionary.GetString(DictionaryProperties.OE).Bytes;
                m_permissionFlag = dictionary.GetString(DictionaryProperties.Perms).Bytes;

            }


            m_userPasswordOut = dictionary.GetString(DictionaryProperties.U).Bytes;
            m_ownerPasswordOut = dictionary.GetString(DictionaryProperties.O).Bytes;

            int keyLength;
            if (dictionary.ContainsKey("Length"))
                keyLength = dictionary.GetInt(DictionaryProperties.Length);
            else
            {
                if ((int)m_keyLength == 1)
                    keyLength = 40;
                else if ((int)m_keyLength == 2)
                    keyLength = 128;
                else
                    keyLength = 256;
                    
            }


            //if (m_keyLength.ToString() == "4")
            //{
                if (keyLength == 128 && dictionary.GetInt(DictionaryProperties.R) < 4)
                {
                    m_keyLength = PdfEncryptionKeySize.Key128Bit;
                    m_encryptionAlgorithm = PdfEncryptionAlgorithm.RC4;
                }
                else if (keyLength == 128 && dictionary.GetInt(DictionaryProperties.R) == 4)
                {
                    m_keyLength = PdfEncryptionKeySize.Key128Bit;
                    PdfDictionary cryptFilter = dictionary[DictionaryProperties.CF] as PdfDictionary;
                    PdfDictionary standardCryptFilter = cryptFilter[DictionaryProperties.StdCF] as PdfDictionary;
                    PdfName value = standardCryptFilter[new PdfName("CFM")] as PdfName;
                    if (value.Value != "V2")
                        m_encryptionAlgorithm = PdfEncryptionAlgorithm.AES;
                    else
                        m_encryptionAlgorithm = PdfEncryptionAlgorithm.RC4;
                }
                else if (keyLength == 40)
                {
                    m_keyLength = PdfEncryptionKeySize.Key40Bit;
                }
                else
                {
                    m_keyLength = PdfEncryptionKeySize.Key256Bit;
                }
            //}

            if (keyLength != 0 && ((m_keyLength == PdfEncryptionKeySize.Key40Bit && keyLength != c_key40 * 8) ||
                (m_keyLength == PdfEncryptionKeySize.Key128Bit && keyLength != c_key128 * 8) ||                
                (m_keyLength == PdfEncryptionKeySize.Key256Bit && keyLength != c_key256 * 8)))
            {
                throw new PdfDocumentException("Invalid format: Invalid/Unsupported security dictionary.");
            }

            m_hasComputedPasswordValues = true;
            //m_bChanged = false;
        }

        /// <summary>
        /// Checks the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="key">The key.</param>
        internal bool CheckPassword(string password, PdfString key)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (key == null)
                throw new ArgumentNullException("key");

            bool result = false;
            byte[] fileId = m_randomBytes;

            m_randomBytes = key.Bytes.Clone() as byte[];

            if (AuthenticateUserPassword(password))
            {
                m_userPassword = password;
                result = true;
            }
            else if (AuthenticateOwnerPassword(password))
            {
                m_ownerPassword = password;
                result = true;
            }
            else
            {
                m_encryptionKey = null;
                result = false;
            }

            if (!result)
            {
                m_randomBytes = fileId;
            }

            return result;
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="currObjNumber">The curr obj number.</param>
        /// <param name="data">The data.</param>
        /// <returns>Encrypted byte array.</returns>
        internal byte[] EncryptData(long currObjNumber, byte[] data,bool isEncryption)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            //Encrypt in 256 bit AES mode
            if (CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
            {
                if (isEncryption)
                    return EncryptData256(data);
                else
                    return DecryptData256(data);
            }

            InitializeData();
            int genNumber = 0;
            int keyLen = 0;
            byte[] newKey = null;

            //prepare key for 40 bit encryption
            if (EncryptionKey.Length == 5)
            {
                newKey = new byte[EncryptionKey.Length + c_newKeyOffset];

                // Standard key.
                for (int i = 0, len = EncryptionKey.Length; i < len; ++i)
                {
                    newKey[i] = EncryptionKey[i];
                }

                // Apply changes.
                int j = EncryptionKey.Length - 1;
                newKey[++j] = (byte)currObjNumber;
                newKey[++j] = (byte)(currObjNumber >> 8);
                newKey[++j] = (byte)(currObjNumber >> 16);
                newKey[++j] = (byte)genNumber;
                newKey[++j] = (byte)(genNumber >> 8);
                keyLen = newKey.Length;
                newKey = PrepareKeyForEncryption(newKey);
            }

            //prepare key for 128 bit encryption
            else
            {
                if (EncryptionAlgorithm == PdfEncryptionAlgorithm.AES)
                {
                    newKey = new byte[EncryptionKey.Length + 9];
                }
                else
                    newKey = new byte[EncryptionKey.Length+ 5];
              

                // Apply changes.

                Array.Copy(EncryptionKey, newKey, EncryptionKey.Length);
                int j = EncryptionKey.Length - 1;
                newKey[++j] = (byte)currObjNumber;
                newKey[++j] = (byte)(currObjNumber >> 8);
                newKey[++j] = (byte)(currObjNumber >> 16);
                newKey[++j] = (byte)genNumber;
                newKey[++j] = (byte)(genNumber >> 8);
                if (EncryptionAlgorithm == PdfEncryptionAlgorithm.AES)
                {
                    newKey[++j] = salt[0];
                    newKey[++j] = salt[1];
                    newKey[++j] = salt[2];
                    newKey[++j] = salt[3];
                }

                newKey = Provider.ComputeHash(newKey);
                keyLen = newKey.Length;
            }

            keyLen = Math.Min(keyLen, newKey.Length);
            
            //Encrypt in 128 bit AES mode
            if (EncryptionAlgorithm == PdfEncryptionAlgorithm.AES)
            {
                if (isEncryption)
                    return AESEncrypt(data, newKey);
                else
                    return AESDecrypt(data, newKey);
            }
            else
                return EncryptDataByCustom(data, newKey, keyLen);
           
        } 

		/// <summary>
        /// Saves this instance.
        /// </summary>
        internal void SaveToDictionary(PdfDictionary dictionary)
        {
            dictionary.SetName(DictionaryProperties.Filter, DictionaryProperties.Standard);
            dictionary.SetNumber(DictionaryProperties.P, m_permissionValue);
            dictionary.SetProperty(DictionaryProperties.U, new PdfString(UserPasswordOut));
            dictionary.SetProperty(DictionaryProperties.O, new PdfString(OwnerPasswordOut));
            dictionary.SetNumber(DictionaryProperties.Length, GetKeyLength() * 8);

            if (m_encryptionAlgorithm == PdfEncryptionAlgorithm.AES || CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
            {
                if (m_revisionNumberOut > 0)
                    dictionary.SetNumber(DictionaryProperties.R, m_revisionNumberOut);
                else
                    dictionary.SetNumber(DictionaryProperties.R, (int)m_keyLength + 2);
                if (m_versionNumberOut > 0)
                    dictionary.SetNumber(DictionaryProperties.V, m_versionNumberOut);
                else
                    dictionary.SetNumber(DictionaryProperties.V, (int)m_keyLength + 2);

                dictionary.SetName("StmF", "StdCF");
                dictionary.SetName("StrF", "StdCF");
                dictionary.SetProperty("CF", new PdfDictionary(AESDictionary()));
                if (CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
                {                   
                    dictionary.SetProperty(DictionaryProperties.UE, new PdfString(m_userEncryptionKeyOut));
                    dictionary.SetProperty(DictionaryProperties.OE, new PdfString(m_ownerEncryptionKeyOut));
                    dictionary.SetProperty(DictionaryProperties.Perms, new PdfString(m_permissionFlag));
                }               
            }
            else
            {
                if (m_revisionNumberOut > 0)
                    dictionary.SetNumber(DictionaryProperties.R, m_revisionNumberOut);
                else
                    dictionary.SetNumber(DictionaryProperties.R, (int)m_keyLength + 1);
                if (m_versionNumberOut > 0)
                    dictionary.SetNumber(DictionaryProperties.V, m_versionNumberOut);
                else
                    dictionary.SetNumber(DictionaryProperties.V, (int)m_keyLength);
            }            
            dictionary.Archive = false;
        }
        #endregion

#region Implementation
        /// <summary>
        /// Pads or truncates string to string with length equals to c_stringLength == 32.
        /// </summary>
        /// <returns>Bytes of newly created string.</returns>
        private byte[] PadTrancateString(string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            byte[] sourceBytes = SecurityEncoding.GetBytes(source);

            return PadTrancateString(sourceBytes);
        }

        /// <summary>
        /// Pads or truncates data with length equals to c_stringLength == 32.
        /// </summary>
        /// <returns>Bytes of newly created string.</returns>
        private byte[] PadTrancateString(byte[] sourceBytes)
        {
            if (sourceBytes == null)
                throw new ArgumentNullException("sourceBytes");

            byte[] dest = new byte[c_stringLength];
            int length = sourceBytes.Length;

            if (length > 0)
            {
                Array.Copy(sourceBytes, 0, dest, 0, Math.Min(length, c_stringLength));
            }

            if (length < c_stringLength)
            {
                Array.Copy(PaddingString, 0, dest, length, (c_stringLength - length));
            }

            return dest;
        }

        /// <summary>
        /// Preperes hash code for cryptographic algorithm and executes it.
        /// </summary>
        /// <param name="data">Data to be encrypted / decrypted.</param>
        /// <param name="key">Key for using by Custom algo.</param>
        private byte[] EncryptDataByCustom(byte[] data, byte[] key)
        {
            return EncryptDataByCustom(data, key, key.Length);
        }

        /// <summary>
        /// Prepares hash code for cryptographic algorithm and executes it.
        /// </summary>
        /// <param name="data">Data to be encrypted / decrypted.</param>
        /// <param name="key">Key for using by Custom algo.</param>
        /// <param name="keyLen">The length of a key.</param>
        private byte[] EncryptDataByCustom(byte[] data, byte[] key, int keyLen)
        {
            byte[] buf = new byte[data.Length];
            // Recreates CustomArray array.
            RecreateCustomArray(key, keyLen);

            // Reinitializes length.
            keyLen = data.Length;

            // Main algo.
            int tmp1 = 0;
            int tmp2 = 0;

            for (int i = 0; i < keyLen; ++i) //  { num3 }
            {
                // Define start indexes.
                tmp1 = (tmp1 + 1) % c_bytesAmount;
                tmp2 = (tmp2 + CustomArray[tmp1]) % c_bytesAmount;

                // Swap.
                byte tmp = CustomArray[tmp1];
                CustomArray[tmp1] = CustomArray[tmp2];
                CustomArray[tmp2] = tmp;

                // Encrypt one byte.
                int index = (CustomArray[tmp1] + CustomArray[tmp2]) % 256;
                byte byteXor = CustomArray[index];
                buf[i] = (byte)(data[i] ^ byteXor);
            }

            return buf;
        }

        /// <summary>
        /// Encrypts the data using AES cryptographic algorithm using initialization vector in CBC mode
        /// </summary>
        /// <param name="data">Data to be encrypted / decrypted.</param>
        /// <param name="key">Key for using by Custom algo.</param> 
        /// <returns> The encrypted data</returns>
        private byte[] AESEncrypt(byte[] data, byte[] key)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            MemoryStream ms = new MemoryStream();
            byte[] output = null;
            byte[] iv = GenerateIV();

            AesEncryptor encryptor = new AesEncryptor(key, iv,true);

            int lengthNeeded = encryptor.GetBlockSize(data.Length);
            output = new byte[lengthNeeded];

            //Step1 : Initial Processing of the data stream
            encryptor.ProcessBytes(data, 0, data.Length, output, 0);
            ms.Write(output, 0, output.Length);


            //Step2 : Final processing by applying padding
            lengthNeeded = encryptor.CalculateOutputSize();
            output = new byte[lengthNeeded];           
            encryptor.Finalize(output);
            ms.Write(output, 0, output.Length);
            ms.Dispose();
            return ms.ToArray();
        }

        /// <summary>
        /// Decrypts the data using AES cryptographic algorithm using initialization vector in CBC mode
        /// </summary>
        /// <param name="data">Data to be decrypted.</param>
        /// <param name="key">Key for using by Custom algo.</param>
        ///<returns>The decrypted data</returns>
        private byte[] AESDecrypt(byte[] data, byte[] key)
        {
            //Gets the decrypted data
            MemoryStream ms = new MemoryStream();            
            byte[] output;
            byte[] iv = new byte[16];
            int length = data.Length;
            int ivPtr = 0;


            int minBlock = Math.Min(iv.Length - ivPtr, length);
            System.Array.Copy(data, 0, iv, ivPtr, minBlock);
           
            length -= minBlock;
            ivPtr += minBlock;
            if (ivPtr == iv.Length)
            {
                AesEncryptor decryptor = new AesEncryptor(key, iv,false);
                int lengthNeeded = decryptor.GetBlockSize(length);
                output = new byte[lengthNeeded];

                //Step 1: initial decryption processing
                decryptor.ProcessBytes(data, ivPtr, length, output, 0);
                ms.Write(output, 0, output.Length);

                //Finalise the output by removing the padding
                lengthNeeded = decryptor.CalculateOutputSize();
                output = new byte[lengthNeeded];
                length = decryptor.Finalize(output);
                if (output.Length != length)
                {
                    byte[] temp = new byte[length];
                    Array.Copy(output, 0, temp, 0, length);
                    ms.Write(temp, 0, temp.Length);

                }
                else
                    ms.Write(output, 0, output.Length);

            }
            else
                return data;
            ms.Dispose();
            return ms.ToArray();
        }

        /// <summary>
        /// Encrypts the data using AES(256 Bit) cryptographic algorithm using initialization vector in CBC mode
        /// </summary>
        /// <param name="data">Data to be decrypted.</param>
        ///<returns>Encrypted data</returns>
        private byte[] EncryptData256(byte[] data)
        {
            return AESEncrypt(data, m_fileEncryptionKey);
        }

        /// <summary>
        /// Decrypts the data using AES(256 Bit) cryptographic algorithm using initialization vector in CBC mode
        /// </summary>
        /// <param name="data">Data to be decrypted.</param>
        /// <returns>Decrypted data</returns>
        private byte[] DecryptData256(byte[] data)
        {
            return AESDecrypt(data, m_fileEncryptionKey);
        }


        /// <summary>
        /// Creates initialization vector for AES encryption
        /// </summary>
        /// <returns>Initialization vector</returns>
        private byte[] GenerateIV()
        {
            byte[] result = new byte[16];
            m_randomArray.NextBytes(result);

            return result;
        }

        /// <summary>
        /// Recreates CustomArray array. This method is the part of 
        /// implementation Custom algo.
        /// </summary>
        /// <param name="key">Key for using by Custom algo.</param>
        /// <param name="keyLen">The length of a key.</param>
        private void RecreateCustomArray(byte[] key, int keyLen)
        {
            byte[] tmpArray = new byte[c_bytesAmount];

            // Default init
            for (int i = 0; i < c_bytesAmount; ++i)
            {
                tmpArray[i] = key[i % keyLen];
                CustomArray[i] = (byte)i;
            }

            int tmp = 0;
            for (int i = 0; i < c_bytesAmount; ++i)
            {
                tmp = (((tmp + CustomArray[i]) + tmpArray[i]) % c_bytesAmount);
                byte tmpByte = CustomArray[i];
                CustomArray[i] = CustomArray[tmp];
                CustomArray[tmp] = tmpByte;
            }
        }

        /// <summary>
        /// Returns length of the encryption key.
        /// </summary>
        /// <returns>Length of the encryption key.</returns>
        internal protected int GetKeyLength()
        {
            if (CryptographicAlgorithm == PdfEncryptionKeySize.Key40Bit)
                return c_key40;
            else if (CryptographicAlgorithm == PdfEncryptionKeySize.Key128Bit)
                return c_key128;
            else
                return c_key256;
        }

        /// <summary>
        /// Calculates owner password.
        /// </summary>
        /// <returns>Data of calculated owner password.</returns>
        /// <remarks>Algorithm 3.3 of PDF 1.6 reference.</remarks>
        private byte[] CreateOwnerPassword()
        {
            // If there is no owner password, use the user password instead.
            string password = (OwnerPassword == null || OwnerPassword.Length == 0) ?
            UserPassword : OwnerPassword;

            // Step 1-4.
            byte[] customKey = GetKeyFromOwnerPass(password);
            // Step 5.
            byte[] userPassBytes = PadTrancateString(UserPassword);

            // Step 6.
            byte[] dataFromCustom = EncryptDataByCustom(userPassBytes, customKey, customKey.Length);

            // Step 7.
            if (RevisionNumber > 2)
            {
                byte[] currKey = customKey;

                for (byte i = 1; i < c_ownerLoopNum2; i++)
                {
                    currKey = GetKeyForOwnerPassStep7(customKey, i);
                    dataFromCustom = EncryptDataByCustom(dataFromCustom, currKey, currKey.Length);
                }
            }

            // Step 8.
            //m_ownerPasswordOut = dataFromCustom;

            return dataFromCustom;
        }

        /// <summary>
        /// Calculates owner password for 256 bit encryption algorithm.
        /// </summary>
        /// <returns>Data of calculated owner password.</returns>
        /// <remarks>Algorithm 3.9 of adobe_supplement_iso32000.</remarks>
        private byte[] Create256BitOwnerPassword()
        {

            byte[] ownerValidationSalt = new byte[8];
            byte[] ownerKeySalt = new byte[8];
            byte[] hash;
            byte[] ownerPasswordOut;

            m_ownerRandomBytes = new byte[16];            
            m_randomArray.NextBytes(m_ownerRandomBytes);

            //Get the user password (SASL prep not implemented)
            byte[] ownerPassword = System.Text.Encoding.UTF8.GetBytes(m_ownerPassword);

            //Step1 : Compute hash
            Array.Copy(m_ownerRandomBytes, 0, ownerValidationSalt, 0, 8);
            Array.Copy(m_ownerRandomBytes, 8, ownerKeySalt, 0, 8);

            hash = new byte[ownerPassword.Length + ownerValidationSalt.Length + m_userPasswordOut.Length];
            Array.Copy(ownerPassword, 0, hash, 0, ownerPassword.Length);
            Array.Copy(ownerValidationSalt, 0, hash, ownerPassword.Length, ownerValidationSalt.Length);
            Array.Copy(m_userPasswordOut, 0, hash, (ownerPassword.Length + ownerValidationSalt.Length), m_userPasswordOut.Length);


            byte[] hashBytes = HashComputer.ComputeHash(hash);

            //Step2 :Concatenate and set userpasswordBytes
            ownerPasswordOut = new byte[hashBytes.Length + ownerValidationSalt.Length + ownerKeySalt.Length];
            Array.Copy(hashBytes, 0, ownerPasswordOut, 0, hashBytes.Length);
            Array.Copy(ownerValidationSalt, 0, ownerPasswordOut, hashBytes.Length, ownerValidationSalt.Length);
            Array.Copy(ownerKeySalt, 0, ownerPasswordOut, (hashBytes.Length + ownerValidationSalt.Length), ownerKeySalt.Length);

            return ownerPasswordOut;

        }

        /// <summary>
        /// Calculates owner encryption key for 256 bit encryption algorithm.
        /// </summary>
        /// <returns>Data of calculated owner encryption key.</returns>
        /// <remarks>Algorithm 3.9 of adobe_supplement_iso32000.</remarks>
        private byte[] CreateOwnerEncryptionKey()
        {

            byte[] ownerValidationSalt = new byte[8];
            byte[] ownerKeySalt = new byte[8];
            byte[] hash;
            byte[] ownerPasswordOut;

            //Get the user password (SASL prep not implemented)
            byte[] ownerPassword = System.Text.Encoding.UTF8.GetBytes(m_ownerPassword);

            //Step1 : Compute hash
            Array.Copy(m_ownerRandomBytes, 0, ownerValidationSalt, 0, 8);
            Array.Copy(m_ownerRandomBytes, 8, ownerKeySalt, 0, 8);


            hash = new byte[ownerPassword.Length + ownerValidationSalt.Length + m_userPasswordOut.Length];
            Array.Copy(ownerPassword, 0, hash, 0, ownerPassword.Length);
            Array.Copy(ownerKeySalt, 0, hash, ownerPassword.Length, ownerValidationSalt.Length);
            Array.Copy(m_userPasswordOut, 0, hash, (ownerPassword.Length + ownerValidationSalt.Length), m_userPasswordOut.Length);


            byte[] hashBytes = HashComputer.ComputeHash(hash);

            //Step 2: Encrypt using AES CBC mode without paddding and zero IV
            Rijndael alg = Rijndael.Create();

            alg.Mode = CipherMode.CBC;
            alg.KeySize = 256;
            alg.Key = hashBytes;
            alg.IV = new byte[16];
            alg.Padding = PaddingMode.None;

#if NETFX_CORE || WP
            byte[] temp = alg.Encrypt(m_fileEncryptionKey);
                        return temp;
#else
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(m_fileEncryptionKey, 0, m_fileEncryptionKey.Length);
            cs.Close();
            ms.Dispose();
            return ms.ToArray();
#endif

        }

        /// <summary>
        /// Computes first 4 steps from algorithm 3.3 to calculate the encryption key.
        /// </summary>
        /// <param name="password">The owner password.</param>
        /// <returns>The encryption key.</returns>
        private byte[] GetKeyFromOwnerPass(string password)
        {
            // Step 1.
            byte[] passwordBytes = PadTrancateString(password);

            // Step 2.
            byte[] curHash = Provider.ComputeHash(passwordBytes);

            // Step 3.
            if (RevisionNumber > 2)
            {
                for (int i = 0; i < c_ownerLoopNum; i++)
                {
                   //  = null;//Provider.Hash;
                    curHash = Provider.ComputeHash(curHash);
                }
            }

            // Step 4.
            byte[] customKey = new byte[GetKeyLength()];
            Array.Copy(curHash, customKey, customKey.Length);

            return customKey;
        }


        /// <summary>
        /// Computes the file encryption key of 256 bit AES encrypted documents.
        /// </summary>
        /// <param name="password">The owner/user password.</param>        
        /// <remarks>The algorithm 3.2a of adobe_supplement_iso32000</remarks>
        private void FindFileEncryptionKey(string password)
        {
            byte[] hash;
            byte[] hashFound = null;
            byte[] forDecryption = null;

            //From owner password
            if (m_ownerRandomBytes != null)
            {
                byte[] ownerValidationSalt = new byte[8];
                byte[] ownerKeySalt = new byte[8];


                //Get the user password (SASL prep not implemented)
                byte[] ownerPassword = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] userPasswordOut = new byte[48];
                Array.Copy(m_userPasswordOut, 0, userPasswordOut, 0, 48);

                //Step1 : Compute hash
                Array.Copy(m_ownerRandomBytes, 0, ownerValidationSalt, 0, 8);
                Array.Copy(m_ownerRandomBytes, 8, ownerKeySalt, 0, 8);

                hash = new byte[ownerPassword.Length + ownerValidationSalt.Length + userPasswordOut.Length];
                Array.Copy(ownerPassword, 0, hash, 0, ownerPassword.Length);
                Array.Copy(ownerKeySalt, 0, hash, ownerPassword.Length, ownerKeySalt.Length);
                Array.Copy(userPasswordOut, 0, hash, (ownerPassword.Length + ownerValidationSalt.Length), userPasswordOut.Length);


                hashFound = HashComputer.ComputeHash(hash);

                forDecryption = m_ownerEncryptionKeyOut;
            }
            
            //from user password
            else if (m_userRandomBytes != null)
            {

                byte[] userValidationSalt = new byte[8];
                byte[] userKeySalt = new byte[8];

                //Get the user password (SASL prep not implemented)
                byte[] userPassword = System.Text.Encoding.UTF8.GetBytes(password);

                //Step1 : Compute hash
                Array.Copy(m_userRandomBytes, 0, userValidationSalt, 0, 8);
                Array.Copy(m_userRandomBytes, 8, userKeySalt, 0, 8);


                hash = new byte[userPassword.Length + userKeySalt.Length];
                Array.Copy(userPassword, 0, hash, 0, userPassword.Length);
                Array.Copy(userKeySalt, 0, hash, userPassword.Length, userKeySalt.Length);

                hashFound = HashComputer.ComputeHash(hash);

                forDecryption = m_userEncryptionKeyOut;

            }

            //Step2 : Decrypt the UE or OE entries using the hash as Key.
            Rijndael alg = Rijndael.Create();

            alg.Mode = CipherMode.CBC;
            alg.KeySize = 256;
            alg.Key = hashFound;
            alg.IV = new byte[16];
            alg.Padding = PaddingMode.None;

#if NETFX_CORE || WP
            byte[]  temp = alg.Decrypt(forDecryption);
            m_fileEncryptionKey = temp;
#else
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(forDecryption, 0, forDecryption.Length);
            cs.Close();
            ms.Dispose();
            m_fileEncryptionKey = ms.ToArray();
#endif
        }

        /// <summary>
        /// Calculates temporary key used for calculating owner password value.
        /// </summary>
        /// <param name="originalKey">Orignial key value.</param>
        /// <param name="index">Current index.</param>
        /// <returns>Temporary key used for calculating owner password value.</returns>
        private byte[] GetKeyForOwnerPassStep7(byte[] originalKey, byte index)
        {
            if (originalKey == null)
                throw new ArgumentNullException("originalKey");

            byte[] result = new byte[originalKey.Length];

            for (int i = 0, len = originalKey.Length; i < len; i++)
            {
                result[i] = (byte)(originalKey[i] ^ index);
            }

            return result;
        }

        /// <summary>
        /// Creates encryption key.
        /// </summary>
        /// <param name="inputPass">Input password string.</param>
        /// <param name="ownerPass">Owner password value.</param>
        /// <returns>Key created.</returns>
        private byte[] CreateEncryptionKey(string inputPass, byte[] ownerPass)
        {
            if (inputPass == null)
                throw new ArgumentNullException("inputPass");

            if (ownerPass == null)
                throw new ArgumentNullException("ownerPass");

            // Step 1.
            byte[] passwordBytes = PadTrancateString(inputPass);

            // Step 2.
            List<byte> data = new List<byte>();
            data.AddRange(passwordBytes);

            // Step 3.
            data.AddRange(ownerPass);

            // Step 4.
            byte[] permBytes = new byte[]{ ( byte )m_permissionValue,
																		 ( byte )( ( int )m_permissionValue >> 8 ),
																		 ( byte )( ( int )m_permissionValue >> 16 ),
																		 ( byte )( ( int )m_permissionValue >> 24 ) };
            data.AddRange(permBytes);

            // Step 5.
            data.AddRange(RandomBytes); // File ID, the first entry.

            // Step 6.
            // NOTE: now document has no metadata, so adding this flag is not needed.
            if (RevisionNumber > 2 && !EncryptMetaData)
            {
                //Provider.ComputeHash( c_flagNumFLAG_NUM );
                data.Add(255);
                data.Add(255);
                data.Add(255);
                data.Add(255);
            }

            // Step 7.
            byte[] dataBytes = data.ToArray();

            if (Provider == null)
                throw new NotSupportedException("Document encryption is not allowed in FIPS mode.");

            byte[] curHash = Provider.ComputeHash(dataBytes);

            // Step 8.
            if (RevisionNumber > 2)
            {
                for (int i = 0; i < c_ownerLoopNum; i++)
                {
                    //= null;//Provider.Hash;
                    curHash = Provider.ComputeHash(curHash);
                }
            }

            // Step 9.
            EncryptionKey = new byte[GetKeyLength()];
            Array.Copy(curHash, EncryptionKey, EncryptionKey.Length);

            return EncryptionKey;
        }

        /// <summary>
        /// Creates file encryption key for 256 bit encryption scheme (random 32 byte array).
        /// </summary>  
        private void CreateFileEncryptionKey()
        {
            m_fileEncryptionKey = new byte[32];            
            m_randomArray.NextBytes(m_fileEncryptionKey);
        }

        /// <summary>
        /// Creates user password.
        /// </summary>
        /// <returns>Created user password.</returns>
        private byte[] CreateUserPassword()
        {
            byte[] result = null;

            if (RevisionNumber == 2)
            {
                result = Create40BitUserPassword();
            }
            else
            {
                result = Create128BitUserPassword();
            }

            return result;
        }

        /// <summary>
        /// Calculates user encryption key for 256 bit encryption algorithm.
        /// </summary>
        /// <returns>Data of calculated user encryption key.</returns>
        /// <remarks>Algorithm 3.8 of adobe_supplement_iso32000.</remarks>
        private byte[] Create256BitUserPassword()
        {

            byte[] userValidationSalt = new byte[8];
            byte[] userKeySalt = new byte[8];
            byte[] hash;
            byte[] userPasswordOut;

            m_userRandomBytes = new byte[16];            
            m_randomArray.NextBytes(m_userRandomBytes);


            //Get the user password (SASL prep not implemented)
            byte[] userPassword = System.Text.Encoding.UTF8.GetBytes(m_userPassword);


            //Step1 : Compute hash
            Array.Copy(m_userRandomBytes, 0, userValidationSalt, 0, 8);
            Array.Copy(m_userRandomBytes, 8, userKeySalt, 0, 8);

            hash = new byte[UserPassword.Length + userValidationSalt.Length];
            Array.Copy(userPassword, 0, hash, 0, userPassword.Length);
            Array.Copy(userValidationSalt, 0, hash, UserPassword.Length, userValidationSalt.Length);

            byte[] hashBytes = HashComputer.ComputeHash(hash);

            //Step2 :Concatenate and set userpasswordBytes
            userPasswordOut = new byte[hashBytes.Length + userValidationSalt.Length + userKeySalt.Length];
            Array.Copy(hashBytes, 0, userPasswordOut, 0, hashBytes.Length);
            Array.Copy(userValidationSalt, 0, userPasswordOut, hashBytes.Length, userValidationSalt.Length);
            Array.Copy(userKeySalt, 0, userPasswordOut, (hashBytes.Length + userValidationSalt.Length), userKeySalt.Length);

            return userPasswordOut;

        }

        /// <summary>
        /// Calculates user encryption key for 256 bit encryption algorithm.
        /// </summary>
        /// <returns>Data of calculated user encryption key.</returns>
        /// <remarks>Algorithm 3.8 of adobe_supplement_iso32000.</remarks>
        private byte[] CreateUserEncryptionKey()
        {

            byte[] userValidationSalt = new byte[8];
            byte[] userKeySalt = new byte[8];
            byte[] hash;
            byte[] userPasswordOut;

            //Get the user password (SASL prep not implemented)
            byte[] userPassword = System.Text.Encoding.UTF8.GetBytes(m_userPassword);


            //Step1 : Compute hash
            Array.Copy(m_userRandomBytes, 0, userValidationSalt, 0, 8);
            Array.Copy(m_userRandomBytes, 8, userKeySalt, 0, 8);

            hash = new byte[userPassword.Length + userKeySalt.Length];
            Array.Copy(userPassword, 0, hash, 0, userPassword.Length);
            Array.Copy(userKeySalt, 0, hash, userPassword.Length, userKeySalt.Length);

            byte[] hashBytes = HashComputer.ComputeHash(hash);

            //Step 2: Encrypt using AES CBC mode without paddding and zero IV
            
            Rijndael alg = Rijndael.Create();
            alg.Mode = CipherMode.CBC;
            alg.KeySize = 256;
            alg.Key = hashBytes;
            alg.IV = new byte[16];
            alg.Padding = PaddingMode.None;
#if NETFX_CORE || WP
            byte[] temp = alg.Encrypt(m_fileEncryptionKey);
            return temp;
#else
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(m_fileEncryptionKey, 0, m_fileEncryptionKey.Length);
            cs.Close();
            ms.Dispose();
            return ms.ToArray();
#endif

        }

        /// <summary>
        /// Calculates permission flag(Perms) for 256 bit encryption algorithm.
        /// </summary>
        /// <returns>Data of calculated Permission flag.</returns>
        /// <remarks>Algorithm 3.10 of adobe_supplement_iso32000.</remarks>
        private byte[] CreatePermissionFlag()
        {
            byte[] permission = new byte[16];

            byte[] permBytes = new byte[]{ ( byte )m_permissionValue,
																		 ( byte )( ( int )m_permissionValue >> 8 ),
																		 ( byte )( ( int )m_permissionValue >> 16 ),
																		 ( byte )( ( int )m_permissionValue >> 24 ) };

            //32 bit permission values
            Array.Copy(permBytes, 0, permission, 0, permBytes.Length);
            int length = permBytes.Length;

            //Extend permission to 64 bits
            permission[length++] = 255;
            permission[length++] = 255;
            permission[length++] = 255;
            permission[length++] = 255;

            //Encrypt metadata whether true or False 
            permission[length++] = 70;

            //'a','d','b' values at the successive entry
            permission[length++] = 97;
            permission[length++] = 100;
            permission[length++] = 98;

            //last four random numbers - This will be ignored
            permission[length++] = 98;
            permission[length++] = 98;
            permission[length++] = 98;
            permission[length++] = 98;

            Rijndael alg = Rijndael.Create();
            alg.Mode = CipherMode.ECB;
            alg.KeySize = 256;
            alg.Key = m_fileEncryptionKey;
            alg.IV = new byte[16];
            alg.Padding = PaddingMode.None;
#if NETFX_CORE || WP
            byte[] temp = alg.Encrypt(permission);          
            return temp;
#else
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(permission, 0, permission.Length);
            cs.Close();
            ms.Close();
            return ms.ToArray();
#endif


        }

        /// <summary>
        /// Creates user password when encryption key is 40 bits length.
        /// </summary>
        /// <returns>Created user password.</returns>
        private byte[] Create40BitUserPassword()
        {
            if (EncryptionKey == null)
                throw new ArgumentNullException("EncryptionKey");

            // Step 2.
            byte[] userPassBytes = PadTrancateString(string.Empty);

            byte[] data = EncryptDataByCustom(userPassBytes, EncryptionKey);

            //m_userPasswordOut = data;

            return data;
        }

        /// <summary>
        /// Creates user password when encryption key is 128 bits length.
        /// </summary>
        /// <returns>Created user password.</returns>
        private byte[] Create128BitUserPassword()
        {
            if (EncryptionKey == null)
                throw new ArgumentNullException("EncryptionKey");

            List<byte> data = new List<byte>();

            // Step 2.
            byte[] userPassBytes = PadTrancateString(string.Empty);
            data.AddRange(userPassBytes);

            // Step 3.
            data.AddRange(RandomBytes);
            byte[] inputData = data.ToArray();
            byte[] resultBytes = Provider.ComputeHash(inputData);

            // Step 4.
            byte[] dataForCustom = new byte[c_randomBytesAmount];
            Array.Copy(resultBytes, 0, dataForCustom, 0, dataForCustom.Length);
            byte[] dataFromCustom = EncryptDataByCustom(dataForCustom, EncryptionKey);

            // Step 5.
            byte[] currKey = EncryptionKey;

            for (byte i = 1; i < c_ownerLoopNum2; i++)
            {
                currKey = GetKeyForOwnerPassStep7(EncryptionKey, i);
                dataFromCustom = EncryptDataByCustom(dataFromCustom, currKey, currKey.Length);
            }

            // Step 6.
            byte[] result = PadTrancateString(dataFromCustom);
            //m_userPasswordOut = result;

            return result;
        }

        /// <summary>
        /// Initializes data.
        /// </summary>
        private void InitializeData()
        {
            if (!m_hasComputedPasswordValues)
            {
                if (CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
                {
                    m_userPasswordOut = Create256BitUserPassword();
                    m_ownerPasswordOut = Create256BitOwnerPassword();
                    CreateFileEncryptionKey();
                    m_userEncryptionKeyOut = CreateUserEncryptionKey();
                    m_ownerEncryptionKeyOut = CreateOwnerEncryptionKey();
                    m_permissionFlag = CreatePermissionFlag();
                }

                else
                {
                    if (Provider == null)
                        throw new NotSupportedException("Document encryption is not allowed in FIPS mode.");
                    m_ownerPasswordOut = CreateOwnerPassword();
                    m_encryptionKey = CreateEncryptionKey(UserPassword, m_ownerPasswordOut);
                    m_userPasswordOut = CreateUserPassword();
                }
                m_hasComputedPasswordValues = true;
            }
        }

        /// <summary>
        /// Transforms key before encryption.
        /// </summary>
        /// <param name="originalKey">Original key to be transformed.</param>
        /// <returns>Transformed key.</returns>
        private byte[] PrepareKeyForEncryption(byte[] originalKey)
        {
            if (originalKey == null)
                throw new ArgumentNullException("originalKey");

            int keyLen = originalKey.Length;
            byte[] newKey = Provider.ComputeHash(originalKey);
            byte[] result = newKey;

            if (keyLen > c_randomBytesAmount)
            {
                int newKeyLength = Math.Min(GetKeyLength() + c_newKeyOffset, c_randomBytesAmount);
                result = new byte[newKeyLength];
                Array.Copy(newKey, 0, result, 0, newKeyLength);
            }

            return result;
        }

        /// <summary>
        /// Checks if the password given is the user password.
        /// </summary>
        /// <param name="password">The password, which was given by the user.</param>
        /// <returns>A flag indicating if the password is the user password.</returns>
        private bool AuthenticateUserPassword(string password)
        {
            //Authenticate 256Bit user password
            if (m_keyLength == PdfEncryptionKeySize.Key256Bit)
            {
                return Authenticate256BitUserPassword(password);
            }

            // Step 1.
            // Create an encryption key.
            m_encryptionKey = CreateEncryptionKey(password, m_ownerPasswordOut);
            // Step 2.
            // Calculate U entry...
            byte[] userPass = CreateUserPassword();
            // ... and compare it with one read.
            bool result;

            if (RevisionNumber == 2)
            {
                result = CompareByteArrays(userPass, m_userPasswordOut);
            }
            else
            {
                // Compare first 16 bytest.
                result = CompareByteArrays(userPass, m_userPasswordOut, 0x10);
            }

            return result;
        }

        /// <summary>
        /// Checks if the password given is the user password (256 bit encryption scheme).
        /// </summary>
        /// <param name="password">The password, which was given by the user.</param>
        /// <returns>A flag indicating if the password is the user password.</returns>
        private bool Authenticate256BitUserPassword(string password)
        {
            byte[] userValidationSalt = new byte[8];
            byte[] userKeySalt = new byte[8];
            byte[] hashProvided = new byte[32];
            m_userRandomBytes = new byte[16];
            byte[] hash;


            byte[] userPassword = System.Text.Encoding.UTF8.GetBytes(password);
            Array.Copy(m_userPasswordOut, 0, hashProvided, 0, hashProvided.Length);
            Array.Copy(m_userPasswordOut, 32, m_userRandomBytes, 0, 16);
            Array.Copy(m_userRandomBytes, 0, userValidationSalt, 0, userValidationSalt.Length);
            Array.Copy(m_userRandomBytes, userValidationSalt.Length, userKeySalt, 0, userKeySalt.Length);

            hash = new byte[userPassword.Length + userValidationSalt.Length];
            Array.Copy(userPassword, 0, hash, 0, userPassword.Length);
            Array.Copy(userValidationSalt, 0, hash, userPassword.Length, userValidationSalt.Length);

            byte[] hashFound = HashComputer.ComputeHash(hash);

            bool bEqual = false;
            if (hashFound.Length == hashProvided.Length)
            {
                int i = 0;
                while ((i < hashFound.Length) && (hashFound[i] == hashProvided[i]))
                {
                    i += 1;
                }
                if (i == hashFound.Length)
                {
                    bEqual = true;
                }
            }

            FindFileEncryptionKey(password);

            return bEqual;

        }

        /// <summary>
        /// Checks if the password given is the owner password.
        /// </summary>
        /// <param name="password">The password, which was given by the user.</param>
        /// <returns>A flag indicating if the password is the owner password.</returns>
        private bool AuthenticateOwnerPassword(string password)
        {
            //Authenticate 256Bit user password
            if (m_keyLength == PdfEncryptionKeySize.Key256Bit)
            {
                return Authenticate256BitOwnerPassword(password);
            }

            // Step 1.
            // Create an encryption key.
            m_encryptionKey = GetKeyFromOwnerPass(password);
            // Step 2.
            byte[] buf = m_ownerPasswordOut;

            if (RevisionNumber == 2)
            {
                buf = EncryptDataByCustom(buf, m_encryptionKey);
            }
            else if (RevisionNumber > 2)
            {
                buf = m_ownerPasswordOut;

                for (int i = 0; i < c_ownerLoopNum2; ++i)
                {
                    byte[] currKey = GetKeyForOwnerPassStep7(m_encryptionKey,
                        (byte)(c_ownerLoopNum2 - i - 1));

                    buf = EncryptDataByCustom(buf, currKey);
                }
            }
            // Step 3.
            m_encryptionKey = null;

            // Cut off the padding string, if there is one, and convert it into a string.

            string userPassword = ConvertToPassword(buf);

            if (AuthenticateUserPassword(userPassword))
            {
                m_userPassword = userPassword;
                m_ownerPassword = password;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the password given is the owner password (256 bit encryption scheme).
        /// </summary>
        /// <param name="password">The password, which was given by the user.</param>
        /// <returns>A flag indicating if the password is the owner password.</returns>
        private bool Authenticate256BitOwnerPassword(string password)
        {
            byte[] ownerValidationSalt = new byte[8];
            byte[] ownerKeySalt = new byte[8];
            byte[] hashProvided = new byte[32];
            m_ownerRandomBytes = new byte[16];
            byte[] hash;

            byte[] userPasswordOut = new byte[48];
            Array.Copy(m_userPasswordOut, 0, userPasswordOut, 0, 48);
            byte[] ownerPassword = System.Text.Encoding.UTF8.GetBytes(password);
            Array.Copy(m_ownerPasswordOut, 0, hashProvided, 0, hashProvided.Length);
            Array.Copy(m_ownerPasswordOut, 32, m_ownerRandomBytes, 0, 16);
            Array.Copy(m_ownerRandomBytes, 0, ownerValidationSalt, 0, ownerValidationSalt.Length);
            Array.Copy(m_ownerRandomBytes, ownerValidationSalt.Length, ownerKeySalt, 0, ownerKeySalt.Length);

            hash = new byte[ownerPassword.Length + ownerValidationSalt.Length + userPasswordOut.Length];
            Array.Copy(ownerPassword, 0, hash, 0, ownerPassword.Length);
            Array.Copy(ownerValidationSalt, 0, hash, ownerPassword.Length, ownerValidationSalt.Length);
            Array.Copy(userPasswordOut, 0, hash, ownerPassword.Length + ownerValidationSalt.Length, userPasswordOut.Length);

            byte[] hashFound = HashComputer.ComputeHash(hash);

            bool bEqual = false;
            if (hashFound.Length == hashProvided.Length)
            {
                int i = 0;
                while ((i < hashFound.Length) && (hashFound[i] == hashProvided[i]))
                {
                    i += 1;
                }
                if (i == hashFound.Length)
                {
                    bEqual = true;
                }
            }

            FindFileEncryptionKey(password);
            return bEqual;
        }

        /// <summary>
        /// Converts an array into a password string. Before the convertion to string
        /// cutts off the padding string if there is one.
        /// </summary>
        /// <param name="array">The array, which should be converted.</param>
        /// <returns>A string that should be a valid password string.</returns>
        private string ConvertToPassword(byte[] array)
        {
            string result;
            int length = array.Length;

            for (int i = 0; i < length; ++i)
            {
                if (array[i] == s_paddingString[0]) // 0x28
                {
                    if (i < length - 1 && array[i + 1] == s_paddingString[1]) // 0xbf
                    {
                        length = i;
                        break;
                    }
                }
            }

            result = PdfString.ByteToString(array, length);

            return result;
        }

        /// <summary>
        /// Determines if the arrays are equal.
        /// </summary>
        /// <param name="array1">One array that should be compared.</param>
        /// <param name="array2">Another array.</param>
        /// <returns><b>True</b> if arrays are equal, false otherwise.</returns>
        private bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            bool result = true;

            if (array1 == null || array2 == null)
            {
                result = (array1 == array2);
            }
            else if (array1.Length != array2.Length)
            {
                result = false;
            }
            else
            {
                for (int i = 0, size = array1.Length; i < size; ++i)
                {
                    if (array1[i] != array2[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if the first <i>bytes</i> of the arrays are equal.
        /// </summary>
        /// <param name="array1">One array that should be compared.</param>
        /// <param name="array2">Another array.</param>
        /// <param name="size">The actual size of the arrays to compare.</param>
        /// <returns>
        /// 	<b>True</b> if arrays are equal, false otherwise.
        /// </returns>
        private bool CompareByteArrays(byte[] array1, byte[] array2, int size)
        {
            bool result = true;

            if (array1 == null || array2 == null)
            {
                result = (array1 == array2);
            }
            else if (array1.Length < size || array2.Length < size)
            {
                throw new ArgumentException(
                    "Size of one of the arrays are less then requisted size.");
            }
            else if (array1.Length != array2.Length)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < size; ++i)
                {
                    if (array1[i] != array2[i])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Standard encryption dictionary for AES encryption 
        /// </summary>
        private PdfDictionary AESDictionary()
        {
            PdfDictionary CFDic = new PdfDictionary();
            PdfDictionary StdCFDic = new PdfDictionary();
            if (!StdCFDic.ContainsKey(new PdfName("CFM")))
            {
                if(CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
                    StdCFDic[new PdfName("CFM")] = new PdfName("AESV3");
                else
                    StdCFDic[new PdfName("CFM")] = new PdfName("AESV2");
            }
            if (!StdCFDic.ContainsKey(new PdfName("AuthEvent")))
            {
                StdCFDic[new PdfName("AuthEvent")] = new PdfName("DocOpen");
            }
            if (!StdCFDic.ContainsKey(new PdfName("Length")))
            {
                if (CryptographicAlgorithm == PdfEncryptionKeySize.Key256Bit)
                    StdCFDic[new PdfName("Length")] = new PdfNumber(c_key256);
                else
                    StdCFDic[new PdfName("Length")] = new PdfNumber(128);
            }
            if (!CFDic.ContainsKey(new PdfName("StdCF")))
            {
                CFDic[new PdfName("StdCF")] = StdCFDic;
            }
            return CFDic;
        }

        #endregion

    }
}
#endif