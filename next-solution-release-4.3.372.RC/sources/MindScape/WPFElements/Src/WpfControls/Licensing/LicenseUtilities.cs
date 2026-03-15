//
//      FILE:   LicenseUtilities.cs.
//
// COPYRIGHT:   Copyright 2008 
//              Infralution
//
using System;
using System.Text;
using System.Security;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Security.Cryptography;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
#if WINDOWS_FORMS
using System.Windows.Forms;
#endif
using System.Diagnostics;
namespace Infralution.Licensing
{

    /// <summary>
    /// Defines the types of encoding possible for license keys
    /// </summary>
#if PUBLIC_LICENSE_CLASS  // if true allows enum to be visible outside library  
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
  enum TextEncoding
    {
        /// <summary>
        /// Keys are encoded using hexadecimal notation (characters 0-9 and A-F)
        /// </summary>
        Hex = 0,

        /// <summary>
        /// Keys are encoding using base 32 with the following character set (23456789ABCDEFGHJKLMNPQRSTUVWXYZ)
        /// </summary>
        Base32 = 1
    }

    /// <summary>
    /// Provides common utility methods for the Infralution Licensing classes
    /// </summary>
#if PUBLIC_LICENSE_CLASS  // if true allows class to be visible outside library  
    public
#endif
  [System.Runtime.CompilerServices.CompilerGenerated]  // suppress FxCop warnings on external code that we don't want to modify
  static class LicenseUtilities
    {
        private static bool _handleIOExceptions = true;
        private static bool _useMachineKeyStore = false;
        private const string Base32Chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

#if CHECK_ILS_LICENSE

        // the license parameters for the Licensing System itself
        //
        const string SYSTEM_PARAMETERS =
            @"<AuthenticatedLicenseParameters>
	              <EncryptedLicenseParameters>
	                <ProductName>ILS 4</ProductName>
	                <RSAKeyValue>
	                  <Modulus>uNcdh3Bh6zOWT3W7lAsyj82A6WK5Fv19KJvm8NXsCzDTHjcxgKujNLZiNaL7fB0fefzy9lpVnKoT04PvKdl+sc828RD7+UM6y5pw6U4ADMXe+exKp7bNgD3VSgqKfLbUn3UjVf/5/8JDZP4J+Nc008tnnq3jiMKH2CCrPGybN2k=</Modulus>
	                  <Exponent>AQAB</Exponent>
	                </RSAKeyValue>
	                <DesignSignature>bublKC/7/zDYbnoDzuKKzjj+nwezcEwPlHiPaVNuhvO/rVfj0GBIzwHE6t4ywEZkennGa0Tm2c2PKbBDj5LQKh5wZhgnct69dCmqyflNKZjlnd87hN5FQItjfO6TLgGy8aBSO+KFRgQhP0fgohcrvnMGJCEHHstt4oi/OH70Uks=</DesignSignature>
	                <RuntimeSignature>Z6VRObi3Asv32iQEaSqryDJa7BD2Sp7963Li3GLGtHPvZZOBnxntFJ6TURyqOxrmwr1K6Hdg2yuZWmI2LMFeptaLUqJygKFBqQaG5vzvxHqrXd46WpR2CfkmkeuoRUhwFigZ8FHqpsVJ0WWG8EawA2J9maN8/4tzQALoUds6XQ8=</RuntimeSignature>
	                <KeyStrength>15</KeyStrength>
	                <ShortSerialNo>False</ShortSerialNo>
	              </EncryptedLicenseParameters>
	              <AuthenticationServerURL>http://www.infralution.net/authenticate/AuthenticationService.asmx</AuthenticationServerURL>
	              <ServerRSAKeyValue>
	                <Modulus>yq7ef375TuntDKZLI5u8g7A017UrXlZIl+VWrRkMZmZBEf8d1gcew969kf2MhFfrxy2fXqD+/VaC6sFuNRwtihpcjyC9VwWSEcy8bD8JEQsetnRT0rpAIyq5T6fhIxcXVbN5Zf6SrWAiIRnsP611WZxoMmWWzJzCcfA1tNv7ZTU=</Modulus>
	                <Exponent>AQAB</Exponent>
	              </ServerRSAKeyValue>
	            </AuthenticatedLicenseParameters>";
 
        // the license for the Licensing System
        //
        private static AuthenticatedLicense _ilsLicense;

        /// <summary>
        /// Return the path to the ILS License File
        /// </summary>
        private static string ILSLicensePath
        {
            get
            {
                // For ASP.NET apps look in the App_Data directory (to support IPN.NET)
                //
                if (System.Web.HttpContext.Current != null)
                {
                    return @"App_Data\ILS4.lic";
                }
                else
                {
                    string commonDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    return Path.Combine(commonDir, @"Infralution\Licenses\ILS4.lic");
                }
            }
        }

        /// <summary>
        /// Return the license for the Infralution Licensing System itself
        /// </summary>
        /// <returns>The ILS license if installed or null if not</returns>
        public static AuthenticatedLicense ILSLicense
        {
            get
            {
                if (_ilsLicense == null)
                {
                    AuthenticatedLicenseProvider provider = new AuthenticatedLicenseProvider();
                    _ilsLicense = provider.GetLicense(SYSTEM_PARAMETERS, ILSLicensePath, true);
                }
                return _ilsLicense;
            }
        }

        /// <summary>
        /// Install a license for ILS
        /// </summary>
        /// <param name="authenticationKey">The ILS authentication key</param>
        /// <returns>True if successful</returns>
        public static bool InstallILSLicense(string authenticationKey)
        {
            AuthenticatedLicenseProvider provider = new AuthenticatedLicenseProvider();
            AuthenticatedLicense license = provider.AuthenticateKey(SYSTEM_PARAMETERS, authenticationKey);
            if (license != null)
            {
                provider.InstallLicense(ILSLicensePath, license);
                _ilsLicense = license;
            }
            return (_ilsLicense != null);
        }

#endif

        /// <summary>
        /// Format license parameters nicely for inclusion in VB code
        /// </summary>
        /// <param name="licenseParameters"></param>
        /// <returns></returns>
        public static string FormatVBParameters(string licenseParameters)
        {
            string[] lines = GetXmlLines(licenseParameters);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Const LICENSE_PARAMETERS As String = _");
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    sb.AppendLine(" + _");
                }
                sb.AppendFormat("\t\"{0}\"", lines[i]);
            }
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Format license parameters nicely for inclusion in C# code
        /// </summary>
        /// <param name="licenseParameters"></param>
        /// <returns></returns>
        public static string FormatCSParameters(string licenseParameters)
        {
            string[] lines = GetXmlLines(licenseParameters);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("const string LICENSE_PARAMETERS = ");
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    sb.AppendFormat("\t@\"{0}", lines[i]);
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendFormat("\t{0}", lines[i]);
                }
            }
            sb.AppendLine("\";");
            return sb.ToString();
        }

#if UNWANTED_ILS_FEATURES
        /// <summary>
        /// Read License Parameters from an XML reader
        /// </summary>
        /// <param name="reader">The XML Reader</param>
        /// <returns>The license parameters</returns>
        /// <remarks>
        /// This function can read both <see cref="AuthenticatedLicenseParameters"/> and
        /// <see cref="EncryptedLicenseParameters"/>.
        /// </remarks>
#endif
      public static EncryptedLicenseParameters ReadLicenseParameters(XmlReader reader)
        {
            EncryptedLicenseParameters parameters = null;
            reader.MoveToContent();
            if (reader.Name == "AuthenticatedLicenseParameters")
            {
#if UNWANTED_ILS_FEATURES
                parameters = new AuthenticatedLicenseParameters();
#endif
            }
            else if (reader.Name == "EncryptedLicenseParameters")
            {
                parameters = new EncryptedLicenseParameters();
            }
            else
            {
                throw new XmlException(LicenseResources.InvalidILSFile);
            }
            parameters.Read(reader);
            return parameters;
        }

#if UNWANTED_ILS_FEATURES
        /// <summary>
        /// Read License Parameters from an XML string
        /// </summary>
        /// <param name="xmlParameters">The XML Parameters string</param>
        /// <returns>The license parameters</returns>
        /// <remarks>
        /// This function can read both <see cref="AuthenticatedLicenseParameters"/> and
        /// <see cref="EncryptedLicenseParameters"/>.
        /// </remarks>
#endif
        public static EncryptedLicenseParameters ReadLicenseParameters(string xmlParameters)
        {
            XmlReader reader = new XmlTextReader(xmlParameters, XmlNodeType.Element, null);
            EncryptedLicenseParameters parameters = ReadLicenseParameters(reader);
            reader.Close();
            return parameters;
        }


        /// <summary>
        /// Should the licensing classes handle exceptions when reading and writing license files
        /// </summary>
        /// <remarks>
        /// Set this to false if you wish to handle these exceptions yourself
        /// </remarks>
        public static bool HandleIOExceptions
        {
            get { return _handleIOExceptions; }
            set { _handleIOExceptions = value; }
        }

        /// <summary>
        /// Determines whether RSA keys used to verify licenses are stored on a user or machine level
        /// </summary>
        /// <remarks>
        /// Setting this value to true may be useful when impersonating or running under an account 
        /// whose user profile is not loaded.  ILS will by default use the MachineKeyStore when there
        /// is no interactive user (ie services and ASP.NET) otherwise it will use the UserKeyStore.
        /// </remarks>
        public static bool UseMachineKeyStore
        {
            get { return _useMachineKeyStore; }
            set { _useMachineKeyStore = value; }
        }

        /// <summary>
        /// Returns a three character checksum based on the given input string
        /// </summary>
        /// <param name="input">The input string to return a checksum for</param>
        /// <returns>An checksum that can be used to validate the given input string</returns>
        /// <remarks>
        /// <para>
        /// This function can be used to generate a short checksum that can be embedded in a
        /// license key as <see cref="EncryptedLicense.ProductInfo"/>.  This allows you to tie 
        /// the license key to information supplied by the user (for instance the name of the 
        /// purchaser) without having to include the full information in the license key.  
        /// This enables license keys to be kept reasonably short.
        /// </para>
        /// <para>
        /// When the license is checked by the application it performs a checksum on the information
        /// supplied by the user and checks that it matches the information in the ProductInfo that
        /// was generated when the license was issued.   The License Tracker application provides
        /// support for "CustomGenerators" which allow you provide the code to generate the ProductInfo
        /// from customer and other information.
        /// </para>
        /// </remarks>
        public static string Checksum(string input)
        {
            int hash = (input == null) ? 0 : HashString(input);
            hash = Math.Abs(hash % 1000);
            return hash.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Return the given input string stripped of the given characters
        /// </summary>
        /// <param name="value">The string to strip</param>
        /// <param name="characters">The characters to strip from the string</param>
        /// <returns>The input string with the given characters removed</returns>
        public static string Strip(string value, string characters)
        {
            if (value == null) return null;
            StringBuilder sb = new StringBuilder();
            foreach (char ch in value)
            {
                if (characters.IndexOf(ch, 0) < 0)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read a Base64 string from an XmlReader into a byte array
        /// </summary>
        /// <param name="reader">The XmlReader to read from</param>
        /// <returns>The byte data for the given element</returns>
        public static byte[] ReadElementBase64(XmlReader reader)
        {
            string value = reader.ReadElementString();
            return Convert.FromBase64String(Strip(value, "\t\r\n "));
        }

        /// <summary>
        /// Read a Base64 string from an XmlReader into a byte array
        /// </summary>
        /// <param name="reader">The XmlReader to read from</param>
        /// <param name="name">The name of the element</param>
        /// <returns>The byte data for the given element</returns>
        public static byte[] ReadElementBase64(XmlReader reader, string name)
        {
            string value = reader.ReadElementString(name);
            return Convert.FromBase64String(Strip(value, "\t\r\n "));
        }

        /// <summary>
        /// Write a byte array into Base64 string of an XmlWriter
        /// </summary>
        /// <param name="writer">The XmlWriter to write to</param>
        /// <param name="name">The name of the element</param>
        /// <param name="value">The data to write</param>
        public static void WriteElementBase64(XmlWriter writer, string name, byte[] value)
        {
             writer.WriteElementString(name, Convert.ToBase64String(value));
        }

        /// <summary>
        /// Read RSA Parameters for an RSA Provider to an XmlWriter
        /// </summary>
        /// <param name="provider">The provider to writer the parameters for</param>
        /// <param name="writer">The XmlWriter to write to</param>
        /// <param name="localName">The name of the element</param>
        /// <param name="includePrivateParameters">Should the private RSA parameters be included</param>
        public static void WriteRSAParameters(RSACryptoServiceProvider provider, 
                                              XmlWriter writer, 
                                              string localName,  
                                              bool includePrivateParameters)
        {
            RSAParameters parameters = provider.ExportParameters(includePrivateParameters);
            writer.WriteStartElement(localName);
            WriteElementBase64(writer, "Modulus", parameters.Modulus);
            WriteElementBase64(writer, "Exponent", parameters.Exponent);
            if (includePrivateParameters)
            {
                WriteElementBase64(writer, "P", parameters.P);
                WriteElementBase64(writer, "Q", parameters.Q);
                WriteElementBase64(writer, "DP", parameters.DP);
                WriteElementBase64(writer, "DQ", parameters.DQ);
                WriteElementBase64(writer, "InverseQ", parameters.InverseQ);
                WriteElementBase64(writer, "D", parameters.D);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Read RSA Parameters for an RSA Provider from an XmlReader
        /// </summary>
        /// <param name="provider">The provider to read the parameters for</param>
        /// <param name="reader">The XmlReader to read from</param>
        /// <param name="localName">The name of the element</param>
        public static void ReadRSAParameters(RSACryptoServiceProvider provider, 
                                             XmlReader reader, string localName)
        {
            RSAParameters parameters = new RSAParameters();
            reader.ReadStartElement(localName);
            while (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "Modulus":
                        parameters.Modulus = ReadElementBase64(reader);
                        break;
                    case "Exponent":
                        parameters.Exponent = ReadElementBase64(reader);
                        break;
                    case "P":
                        parameters.P = ReadElementBase64(reader);
                        break;
                    case "Q":
                        parameters.Q = ReadElementBase64(reader);
                        break;
                    case "DP":
                        parameters.DP = ReadElementBase64(reader);
                        break;
                    case "DQ":
                        parameters.DQ = ReadElementBase64(reader);
                        break;
                    case "InverseQ":
                        parameters.InverseQ = ReadElementBase64(reader);
                        break;
                    case "D":
                        parameters.D = ReadElementBase64(reader);
                        break;
                    default:
                        string error = "Unexpected XML Element: {0}";
                        throw new XmlSyntaxException(string.Format(error, reader.Name));
                }
            }
            reader.ReadEndElement();
            provider.ImportParameters(parameters);
        }
 
        /// <summary>
        /// Converts a byte array into a hexadecimal representation.
        /// </summary>
        /// <param name="data">The byte data to convert</param>
        /// <returns>Hexadecimal representation of the data</returns>
        public static string ToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0 && i % 2 == 0)
                {
                    sb.Append("-");
                }
                sb.Append(data[i].ToString("X2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a hexadecimal string into a byte array.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert</param>
        /// <returns>The converted byte data</returns>
        public static byte[] FromHex(string hex)
        {
            string strippedHex = Strip(hex, "\t\r\n -");
            if (strippedHex == null || strippedHex.Length % 2 != 0)
                throw new FormatException("Invalid hexadecimal string");
            byte[] result = new byte[ArraySize(strippedHex.Length / 2)];
            for (int i = 0, j = 0; i < strippedHex.Length; i += 2, j++)
            {
                string s = strippedHex.Substring(i, 2);
                result[j] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return result;
        }

        /// <summary>
        /// Converts a byte array into a base 32 representation.
        /// </summary>
        /// <param name="data">The byte data to convert</param>
        /// <returns>Base32 representation of the data</returns>
        public static string ToBase32(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            byte index;
            int hi = 5;
            int currentByte = 0;

            while (currentByte < data.Length)
            {
                // do we need to use the next byte?
                if (hi > 8)
                {
                    // get the last piece from the current byte, shift it to the right
                    // and increment the byte counter
                    index = (byte)(data[currentByte++] >> (hi - 5));
                    if (currentByte != data.Length)
                    {
                        // if we are not at the end, get the first piece from
                        // the next byte, clear it and shift it to the left
                        index = (byte)(((byte)(data[currentByte] << (16 - hi)) >> 3) | index);
                    }
                    hi -= 3;
                }
                else if (hi == 8)
                {
                    index = (byte)(data[currentByte++] >> 3);
                    hi -= 3;
                }
                else
                {
                    // simply get the stuff from the current byte
                    index = (byte)((byte)(data[currentByte] << (8 - hi)) >> 3);
                    hi += 5;
                }
                sb.Append(Base32Chars[index]);
                int i = sb.Length + 1;
                if (i > 0 && i % 5 == 0)
                {
                    sb.Append('-');
                }
            }

            // ensure we don't have a trailing separator
            //
            if (sb.Length > 0)
            {
                if (sb[sb.Length - 1] == '-')
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a base32 string into a byte array.
        /// </summary>
        /// <param name="str">The base32 string to convert</param>
        /// <returns>The converted byte data</returns>
        public static byte[] FromBase32(string str)
        {
            str = Strip(str, "\t\r\n -");
            str = str.ToUpper();

            int numBytes = str.Length * 5 / 8;
            byte[] bytes = new Byte[numBytes];

            int bitBuffer;
            int currentCharIndex;
            int bitsInBuffer;

            if (str.Length < 3)
            {
                bytes[0] = (byte)(Base32Chars.IndexOf(str[0]) | Base32Chars.IndexOf(str[1]) << 5);
                return bytes;
            }

            bitBuffer = (Base32Chars.IndexOf(str[0]) | Base32Chars.IndexOf(str[1]) << 5);
            bitsInBuffer = 10;
            currentCharIndex = 2;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)bitBuffer;
                bitBuffer >>= 8;
                bitsInBuffer -= 8;
                while (bitsInBuffer < 8 && currentCharIndex < str.Length)
                {
                    bitBuffer |= Base32Chars.IndexOf(str[currentCharIndex++]) << bitsInBuffer;
                    bitsInBuffer += 5;
                }
            }
            return bytes;
        }

        /// <summary>
        /// Converts a byte array into a text representation.
        /// </summary>
        /// <param name="data">The byte data to convert</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>Text representation of the data</returns>
        public static string EncodeToText(byte[] data, TextEncoding encoding)
        {
            switch (encoding)
            {
                case TextEncoding.Base32:
                    return ToBase32(data);
                default:
                    return ToHex(data);
            }
        }

        /// <summary>
        /// Converts a string into a byte array.
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>The converted byte data</returns>
        public static byte[] DecodeFromText(string text, TextEncoding encoding)
        {
            switch (encoding)
            {
                case TextEncoding.Base32:
                    return FromBase32(text);
                default:
                    return FromHex(text);
            }
        }

        /// <summary>
        /// Create an instance of the RSACryptoServiceProvider.
        /// </summary>
        /// <returns>An instance of the RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider CreateRSACryptoServiceProvider()
        {
            const int keySize = 1024;

            // If this is not a service or ASP then create the RSA Service Provicer
            // using the default user profile key store - if this fails then fall back
            // to using the machine key store
            //
            RSACryptoServiceProvider rsa = null;
            if (Environment.UserInteractive && !UseMachineKeyStore)
            {
                try
                {
                    rsa = new RSACryptoServiceProvider(keySize);
                }
                catch
                {
                }
            }

            if (rsa == null)
            {
                CspParameters cspParams = new CspParameters();
                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
                rsa = new RSACryptoServiceProvider(keySize, cspParams);
            }
            return rsa;
        }

#if UNWANTED_ILS_FEATURES
        /// <summary>
        /// Sign the given data using the given RSA parameters
        /// </summary>
        /// <param name="rsaProvider">The RSA Provider to use</param>
        /// <param name="data">The data to sign</param>
        /// <returns>The signature for the data</returns>
        /// <remarks>
        /// Uses <see cref="RSACryptoServiceProvider.SignHash"/> instead of
        /// <see cref="RSACryptoServiceProvider.SignData"/> to workaround bug in standard Microsoft
        /// <see cref="RSACryptoServiceProvider"/> that can cause a lengthy delay.   
        /// See http://support.microsoft.com/default.aspx?scid=kb;en-us;948080
        /// </remarks>
#endif
        static public byte[] SignData(RSACryptoServiceProvider rsaProvider, byte[] data)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);
            return rsaProvider.SignHash(hash, null);
        }

        /// <summary>
        /// Verify the signature for the given data using the given RSA parameters
        /// </summary>
        /// <param name="rsaProvider">The RSA Provider to use</param>
        /// <param name="data">The data to verify</param>
        /// <param name="signature">The signature for the data</param>
        /// <returns>True if the data matches the signature</returns>
        /// <remarks>
        /// Uses <see cref="RSACryptoServiceProvider.VerifyHash(byte[], string, byte[])"/> instead of
        /// <see cref="RSACryptoServiceProvider.VerifyData"/> to workaround bug in standard Microsoft
        /// <see cref="RSACryptoServiceProvider"/> that can cause a lengthy delay.  
        /// See http://support.microsoft.com/default.aspx?scid=kb;en-us;948080
        /// </remarks>
        static public bool VerifyData(RSACryptoServiceProvider rsaProvider, byte[] data, byte[] signature)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);
            return rsaProvider.VerifyHash(hash, null, signature);
        }

        /// <summary>
        /// Encrypt the keys of the given symmetric algorithm using an RSA public key 
        /// </summary>
        /// <param name="rsaProvider">The RSA provider to use to encrypt the symmetric keys</param>
        /// <param name="algorithm">The symmetric algorithm</param>
        /// <returns>String containing the encrypted keys</returns>
        public static string EncryptKeys(RSACryptoServiceProvider rsaProvider,
                                         SymmetricAlgorithm algorithm)
        {
            MemoryStream ms = new MemoryStream();

            // encrypt the keys using RSA
            //
            byte[] encryptedKey = rsaProvider.Encrypt(algorithm.Key, false);
            byte[] encryptedIV = rsaProvider.Encrypt(algorithm.IV, false);

            // write the encrypted symmetric keys to the stream
            //
            Int32 encryptedKeyLength = encryptedKey.Length;
            ms.Write(BitConverter.GetBytes(encryptedKeyLength), 0, sizeof(Int32));
            ms.Write(encryptedKey, 0, encryptedKey.Length);

            Int32 encryptedIVLength = encryptedIV.Length;
            ms.Write(BitConverter.GetBytes(encryptedIVLength), 0, sizeof(Int32));
            ms.Write(encryptedIV, 0, encryptedIV.Length);
            ms.Flush();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Set the Key and IV for the given symmetric algorithm by decrypting the keys from a string 
        /// </summary>
        /// <param name="rsaProvider">The RSA provider to use to decrypt the keys</param>
        /// <param name="algorithm">The symmetric algorithm to set the keys for</param>
        /// <param name="encryptedKeys">String containing encrypted keys</param>
        public static void DecryptKeys(RSACryptoServiceProvider rsaProvider, 
                                       SymmetricAlgorithm algorithm, 
                                       string encryptedKeys)
        {
            byte[] streamData = Convert.FromBase64String(encryptedKeys);
            MemoryStream ms = new MemoryStream(streamData);

            byte[] lengthBuffer = new Byte[sizeof(Int32)];

            // read the encrypted key and IV from the stream
            //
            ms.Read(lengthBuffer, 0, sizeof(Int32));
            Int32 encryptedKeyLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] encryptedKey = new byte[encryptedKeyLength];
            ms.Read(encryptedKey, 0, encryptedKeyLength);

            ms.Read(lengthBuffer, 0, sizeof(Int32));
            Int32 encryptedIVLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] encryptedIV = new byte[encryptedIVLength];
            ms.Read(encryptedIV, 0, encryptedIV.Length);

            // Decrypt the key and IV and setup the algorithm
            //
            algorithm.Key = rsaProvider.Decrypt(encryptedKey, false);
            algorithm.IV = rsaProvider.Decrypt(encryptedIV, false);
        }
   
        /// <summary>
        /// Encrypt a set of key/values using the given algorithm
        /// </summary>
        /// <param name="algorithm">The algorithm to use to encrypt the data</param>
        /// <param name="values">A hash table containing string key/value pairs</param>
        /// <returns>The encrypted key/values</returns>
        public static byte[] EncryptValues(SymmetricAlgorithm algorithm,
                                           Hashtable values)
        {
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);
            XmlTextWriter xmlWriter = new XmlTextWriter(cryptoStream, Encoding.UTF8);
            xmlWriter.WriteStartElement("Values");
            foreach (DictionaryEntry entry in values)
            {
                xmlWriter.WriteElementString(entry.Key.ToString(), entry.Value.ToString());
            }
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            cryptoStream.Close();
            memoryStream.Flush();
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Decrypt a set of key/values using the given algorithm
        /// </summary>
        /// <param name="algorithm">The algorithm to use to decrypt the values</param>
        /// <param name="encryptedValues">The encrypted data</param>
        /// <returns>A hashtable containing the string key/values</returns>
        public static Hashtable DecryptValues(SymmetricAlgorithm algorithm, byte[] encryptedValues)
        {
            Hashtable values = new Hashtable();
            using (MemoryStream memoryStream = new MemoryStream(encryptedValues))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(cryptoStream, XmlNodeType.Element, null))
                    {
                        xmlReader.ReadStartElement("Values");
                        while (xmlReader.IsStartElement())
                        {
                            values[xmlReader.Name] = xmlReader.ReadElementString();
                        }
                        xmlReader.ReadEndElement();
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Encrypt text using the given algorithm
        /// </summary>
        /// <param name="algorithm">The algorithm to use to encrypt the data</param>
        /// <param name="text">The text to encrypt</param>
        /// <returns>The encrypted data</returns>
        public static byte[] EncryptText(SymmetricAlgorithm algorithm, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return algorithm.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// Decrypt text using the given algorithm
        /// </summary>
        /// <param name="algorithm">The algorithm to use to decrypt the values</param>
        /// <param name="encryptedData">The encrypted text</param>
        /// <returns>The decryptedText</returns>
        public static string DecryptText(SymmetricAlgorithm algorithm, byte[] encryptedData)
        {
            byte[] data = algorithm.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Retrieve the license key for the given type from the given DLL/EXE assembly resources
        /// </summary>
        /// <param name="assembly">The assembly containing the license resources</param>
        /// <param name="type">The type to get the license key for</param>
        /// <returns>The license key if any</returns>
        internal static string GetSavedLicenseKey(Assembly assembly, Type type)
        {
            string key = null;
            string assemblyName = assembly.GetName().Name;
            string resourceName = assemblyName + ".dll.licenses";
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                resourceName = assemblyName + ".exe.licenses";
                stream = assembly.GetManifestResourceStream(resourceName);
            }
            if (stream != null)
            {
                IFormatter formatter = new BinaryFormatter();
                object[] values = formatter.Deserialize(stream) as object[];
                if (values != null)
                {
                    Hashtable keys = values[1] as Hashtable;
                    if (keys != null)
                    {
                        foreach (DictionaryEntry entry in keys)
                        {
                            string typeName = entry.Key as String;
                            if (typeName != null)
                            {
                                typeName = typeName.Trim();
                                if (typeName.IndexOf(type.FullName) == 0)
                                {
                                    key = entry.Value as String;
                                    break;
                                }
                            }
                        }
                    }
                }
                stream.Close();
            }
            return key;
        }

        /// <summary>
        /// Uninstall the given license file by deleting it
        /// </summary>
        /// <param name="path">The full file path</param>
        internal static void UninstallLicenseFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                if (!LicenseUtilities.HandleIOExceptions) throw;
                string msg = string.Format(LicenseResources.UninstallErrorMsg, ex.Message, path);
                ShowError(LicenseResources.UninstallErrorTitle, msg);
            }
        }

        /// <summary>
        /// Display an error to a message box or the trace output
        /// </summary>
        /// <param name="title">The title for the error</param>
        /// <param name="message">The error message</param>
        internal static void ShowError(string title, string message)
        {
#if WINDOWS_FORMS
            if (Environment.UserInteractive)
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                Trace.WriteLine(title + ": " + message);
#endif
        }

        /// <summary>
        /// Return the default directory used to store license files
        /// </summary>
        /// <param name="context">The license context</param>
        /// <param name="type">The type being licensed</param>
        /// <returns>The directory to look for license files</returns>
        internal static string DefaultLicenseDirectory(LicenseContext context, Type type)
        {
            string result = null;

            // try to use the type resolver service if available
            //
            if (context != null && type != null)
            {
                ITypeResolutionService resolver = (ITypeResolutionService)context.GetService(typeof(ITypeResolutionService));
                if (resolver != null)
                {
                    result = resolver.GetPathOfAssembly(type.Assembly.GetName());
                    result = Path.GetDirectoryName(result);
                }
            }

            if (result == null)
            {
                if (type == null)
                {
                    result = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    Assembly assembly = type.Assembly;

                    // use the code base if possible 
                    //
                    result = assembly.CodeBase;
                    if (result.StartsWith(@"file:///"))
                    {
                        result = result.Replace(@"file:///", "");
                    }
                    else
                    {
                        result = type.Module.FullyQualifiedName;
                    }
                    result = Path.GetDirectoryName(result);
                }
            }
            return result;
        }

        /// <summary>
        /// Return the array size to use when declaring an array of the given length.
        /// </summary>
        /// <param name="length">The length of the array you are declaring</param>
        /// <returns>The size to declare the array</returns>
        /// <remarks>
        /// This is used to account for the difference between declaring VB and C# arrays and
        /// permit automatic conversion of the code to VB
        /// </remarks>
        internal static int ArraySize(int length)
        {
            return length;
        }

        /// <summary>
        /// Are the contents of the two byte arrays equal
        /// </summary>
        /// <param name="a1">The first array</param>
        /// <param name="a2">The second array </param>
        /// <returns>True if the contents of the arrays is equal</returns>
        internal static bool ArrayEqual(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if ((a1 == null) || (a2 == null)) return false;
            if (a1.Length != a2.Length) return false;
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Create a checksum for the given block of data
        /// </summary>
        /// <param name="data">The block of data to create a checksum for</param>
        /// <returns>An integer checksum</returns>
        internal static UInt16 Checksum(byte[] data)
        {
            int hash = 5381;
            int c;
            int i = 0;

            while (i < data.Length)
            {
                c = data[i];

                hash = ((hash << 5) + hash) ^ c;
                i++;
            }
            hash = hash % UInt16.MaxValue;
            return (UInt16)hash;
        }

        /// <summary>
        /// Implements a string hashing code algorithm equivalent to the .NET 2003 String.GetHashCode()
        /// </summary>
        /// <remarks>
        /// Microsoft have changed the underlying String.GetHashCode algorithm.  This method provides an
        /// equivalent compatible method that can be used on all platforms returning the same result.
        /// </remarks>
        /// <param name="szStr">The string to get the hash code for</param>
        /// <returns>The hash code</returns>
        private static int HashString(string szStr)
        {
            int hash = 5381;
            int c;
            int i = 0;

            while(i < szStr.Length)
            {
                c = (int)szStr[i]; // TODO: I don't think this cast is neccessary

                hash = ((hash << 5) + hash) ^ c;
                i++;
            }
            return hash;
        }

        /// <summary>
        /// Break the given xml fragment into lines
        /// </summary>
        /// <param name="xml">The xml fragment</param>
        /// <returns></returns>
        private static string[] GetXmlLines(string xml)
        {
            string[] crlfs = { "\r\n", "\n", "\r" };
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            xmlWriter.Formatting = Formatting.Indented;
            doc.WriteTo(xmlWriter);
            return stringWriter.ToString().Split(crlfs, StringSplitOptions.None);
        }
    }
}
