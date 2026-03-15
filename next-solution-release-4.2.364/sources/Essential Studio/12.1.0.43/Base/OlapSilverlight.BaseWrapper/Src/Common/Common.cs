#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Security.Cryptography;

namespace Syncfusion.OlapSilverlight.Common
{
    /// <summary>
    /// Common helper class.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <typeparam name="T">An object of generic type T.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>A serialized string.</returns>
        public static string SerializeObject<T>(T obj)
        {
            try
            {
                string xmlString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, obj);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                xmlString = UTF8ByteArrayToString(memoryStream.ToArray()); return xmlString;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// De-serializes the object.
        /// </summary>
        /// <typeparam name="T">De-serialization based on this object.</typeparam>
        /// <param name="xml">The XML string.</param>
        /// <returns>De-serialized object.</returns>
        public static T DeserializeObject<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        private static Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Decrypts the specified input string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="decryptKey">The decrypt key.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decrypt(string inputString, string decryptKey)
        {
            byte[] encryptedStringBytes;
            try
            {
                encryptedStringBytes = Convert.FromBase64String(inputString);
            }
            catch
            {
                return inputString;
            }

            byte[] decryptKeyBytes = Encoding.UTF8.GetBytes(decryptKey);


            string outputString = string.Empty;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(decryptKey, decryptKeyBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedStringBytes, 0, encryptedStringBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        outputString =
                            UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }

            return outputString;
        }
    }
}
