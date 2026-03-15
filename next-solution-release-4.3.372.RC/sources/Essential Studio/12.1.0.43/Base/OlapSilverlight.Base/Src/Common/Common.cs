#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Syncfusion.OlapSilverlight.Common
{
    /// <summary>
    /// Utility class for enumerations.
    /// </summary>
    /// <typeparam name="T">Enumeration type</typeparam>
    public class Enum<T>
    {
        /// <summary>
        /// Gets the enumeration constants.
        /// </summary>
        /// <returns>Collection constant names.</returns>
        public static IEnumerable<string> GetNames()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
              where field.IsLiteral
              select field.Name).ToList<string>();
        }
    }

    /// <summary>
    /// Helper class for Drag-Drop operations.
    /// </summary>
    public class DragDropManager
    {
        /// <summary>
        /// Gets or sets the Drag-Source.
        /// </summary>
        /// <value>The source.</value>
        public object Source { get; set; }
        /// <summary>
        /// Gets or sets the source axis.
        /// </summary>
        /// <value>The source axis.</value>
        public AxisPosition SourceAxis { get; set; }
        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        public MetaTreeNode SelectedNode { get; set; }
        /// <summary>
        /// Gets or sets the drag drop popup.
        /// </summary>
        /// <value>The drag drop popup.</value>
        public Popup DragDropPopup { get; set; }
    }

    /// <summary>
    /// Parser class for XML manipulations.
    /// </summary>
    public class XmlParser
    {
        /// <summary>
        /// Converts to XML format.
        /// </summary>
        /// <param name="objToXml">The object which will be serialized to XML.</param>
        /// <param name="filePath">The file path as stream object.</param>
        /// <param name="includeNameSpace">if set to <c>true</c> [include name space].</param>
        public static void ToXml(Object objToXml, Stream filePath, bool includeNameSpace)
        {
            StreamWriter stWriter = new StreamWriter(filePath);
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(objToXml.GetType());

                if (!includeNameSpace)
                {
                    System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                    ////To remove namespace and any other inline information tag
                    xs.Add(string.Empty, string.Empty);
                    xmlSerializer.Serialize(stWriter, objToXml, xs);
                }
                else
                {
                    xmlSerializer.Serialize(stWriter, objToXml);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stWriter != null)
                {
                    stWriter.Close();
                }
            }
        }

        /// <summary>
        /// Converts the XML format object.
        /// </summary>
        /// <param name="filePath">The file path as a stream.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object XmlToFromFile(Stream filePath, Type type)
        {
            XmlSerializer xmlSerializer;
            FileStream streamReader = null;
            try
            {
                xmlSerializer = new XmlSerializer(type);
                streamReader = filePath as FileStream;
                object objectFromXml = xmlSerializer.Deserialize(streamReader);
                return objectFromXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
        }
    }

    /// <summary>
    /// Common helper class.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Encrypts the specified input string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <returns>An encrypted string.</returns>
        public static string Encrypt(string inputString, string encryptKey)
        {
            byte[] utf8EncodedKey = Encoding.UTF8.GetBytes(encryptKey);
            byte[] utf8EncodedOriginal = UTF8Encoding.UTF8.GetBytes(inputString);

            string outputString = string.Empty; //initialization for storing the encrypted output.
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(encryptKey, utf8EncodedKey);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize; aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // Key and Initialization Vector calculation.
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream encryptor =
                            new CryptoStream(encryptedStream, encryptTransform, CryptoStreamMode.Write))
                        {
                            encryptor.Write(utf8EncodedOriginal, 0, utf8EncodedOriginal.Length);
                            encryptor.Flush();
                            encryptor.Close();

                            byte[] encryptBytes = encryptedStream.ToArray();
                            outputString = Convert.ToBase64String(encryptBytes); //get string from bytes value
                        }
                    }
                }
            }
            return outputString;
        }
    }

}
