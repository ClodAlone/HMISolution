using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WPFUtilities.CryptString
{
    class CryptString
    {
        static byte[] rgbIV = Encoding.ASCII.GetBytes("ryoalyglrjjvlzmd");
        static byte[] key = Encoding.ASCII.GetBytes("czfeultgbskdmaunivmfuohcxilkqbbh");
        private string Unchecked
        {
            get { return WPFUtilities.CryptString.CryptString.EncryptString("ghkjdzfjbjk"); }
        }

        private string Checked
        {
            get { return WPFUtilities.CryptString.CryptString.EncryptString("hjshgfjj"); }
        }


        internal static String EncryptString(String ClearText, byte[] key = null)
        {
            if (String.IsNullOrEmpty(ClearText))
                return String.Empty;

            byte[] clearTextBytes = Encoding.UTF8.GetBytes(ClearText);
            using (SymmetricAlgorithm rijn =
#if !NET_STANDARD
                SymmetricAlgorithm.Create()
#else
                RijndaelManaged.Create()
#endif
                )
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key ?? CryptString.key, rgbIV), CryptoStreamMode.Write);
                    cs.Write(clearTextBytes, 0, clearTextBytes.Length);
                    cs.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        internal static byte[] EncryptData(byte[] ClearText, byte[] key = null)
        {
            if (ClearText == null || ClearText.Length == 0)
                return null;

            using (SymmetricAlgorithm rijn =
#if !NET_STANDARD
                SymmetricAlgorithm.Create()
#else
                RijndaelManaged.Create()
#endif
                )
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key ?? CryptString.key, rgbIV), CryptoStreamMode.Write);
                    cs.Write(ClearText, 0, ClearText.Length);
                    cs.Close();
                    return ms.ToArray();
                }
            }
        }

        internal static String DecryptString(String EncryptedText, byte[] key = null)
        {
            if (String.IsNullOrEmpty(EncryptedText))
                return String.Empty;

            byte[] encryptedTextBytes = Convert.FromBase64String(EncryptedText);
            using (MemoryStream ms = new MemoryStream())
            {
                using (SymmetricAlgorithm rijn =
#if !NET_STANDARD
                SymmetricAlgorithm.Create()
#else
                RijndaelManaged.Create()
#endif
                    )
                {
                    CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key ?? CryptString.key, rgbIV), CryptoStreamMode.Write);
                    cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
                    cs.Close();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        internal static byte[] DecryptData(byte[] EncryptedText, byte[] key = null)
        {
            if (EncryptedText == null || EncryptedText.Length == 0)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (SymmetricAlgorithm rijn =
#if !NET_STANDARD
                SymmetricAlgorithm.Create()
#else
                RijndaelManaged.Create()
#endif
                )
                {
                    CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key ?? CryptString.key, rgbIV), CryptoStreamMode.Write);
                    cs.Write(EncryptedText, 0, EncryptedText.Length);
                    cs.Close();
                }
                return ms.ToArray();
            }
        }
    }


    class X509Certificate2_Cryptographer
    {
        //creating the certificate
        //makecert -r -pe -n "CN=WWW.AGGREGATEDINTELLIGENCE.COM" -b 01/01/2005 -e 01/01/2020 -sky exchange -ss my
        /*
        public static void Main()
        {
            try
            {
                X509Certificate2 x509_2 = LoadCertificate(StoreLocation.CurrentUser, "CN=WWW.PROGEA.COM");
                //DisplayX509CertificateInfo(x509_2);

                string plaintext = "HelloWorld";
                Console.WriteLine("Plain text: " + plaintext + Environment.NewLine);

                string encryptedstring = Encrypt(x509_2, plaintext);
                Console.WriteLine("Encrypted text: " + Environment.NewLine + encryptedstring + Environment.NewLine);

                string decryptedstring = Decrypt(x509_2, encryptedstring);
                Console.WriteLine("decrypted text: " + decryptedstring + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            Console.ReadLine();
        }
        */

        public static X509Certificate2 LoadCertificate(StoreLocation storeLocation, string certificateName)
        {
            X509Store store = new X509Store(storeLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2 x509 = null;
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.Subject == certificateName)
                {
                    x509 = c;
                    break;
                }
            }
            if (x509 == null)
                Console.WriteLine("A x509 certificate for " + certificateName + " was not found");
            store.Close();
            return x509;
        }

        public static void DisplayX509CertificateInfo(X509Certificate2 x509)
        {
            if (x509 == null)
                throw new Exception("A x509 certificate must be provided");

            Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, x509.Subject);
            Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, x509.Issuer);
            Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, x509.Version);
            Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, x509.NotBefore);
            Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, x509.NotAfter);
            Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, x509.Thumbprint);
            Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, x509.SerialNumber);
            Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, x509.PublicKey.Oid.FriendlyName);
            Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, x509.PublicKey.EncodedKeyValue.Format(true));
            Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, x509.RawData.Length);
            Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, x509.ToString(true));

            Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine, x509.PublicKey.Key.ToXmlString(false));
        }

        public static string Encrypt(X509Certificate2 x509, string stringToEncrypt)
        {
            if (x509 == null || string.IsNullOrEmpty(stringToEncrypt))
                throw new Exception("A x509 certificate and string for encryption must be provided");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PublicKey.Key;
            byte[] bytestoEncrypt = ASCIIEncoding.ASCII.GetBytes(stringToEncrypt);
            byte[] encryptedBytes = rsa.Encrypt(bytestoEncrypt, false);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(X509Certificate2 x509, string stringTodecrypt)
        {
            if (x509 == null || string.IsNullOrEmpty(stringTodecrypt))
                throw new Exception("A x509 certificate and string for decryption must be provided");

            if (!x509.HasPrivateKey)
                throw new Exception("x509 certicate does not contain a private key for decryption");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509.PrivateKey;
            byte[] bytestodecrypt = Convert.FromBase64String(stringTodecrypt);
            byte[] plainbytes = rsa.Decrypt(bytestodecrypt, false);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(plainbytes);
        }
    }
}
