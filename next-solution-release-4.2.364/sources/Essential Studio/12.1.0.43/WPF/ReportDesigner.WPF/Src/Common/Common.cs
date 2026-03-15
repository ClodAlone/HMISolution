#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Syncfusion.Windows.Reports.Designer.Serializer
{

    /// <summary>
    /// A Common class for Serialization stuffs
    /// </summary>
    internal class Common
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Common"/> class.
        /// </summary>
        public Common()
        {
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
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

        static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        static Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Serializes a given object to the file given with the path
        /// </summary>
        /// <param name="objToXml">Object wanted to be serialized to the file</param>
        /// <param name="filePath">Path of the file</param>
        /// <param name="includeNameSpace">if set to <c>true</c> [include name space].</param>
        public static void ToXml(Object objToXml, string filePath, bool includeNameSpace)
        {
            StreamWriter stWriter = null;
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(objToXml.GetType());
                stWriter = new StreamWriter(filePath);
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
        /// Deserializes the object given with the type from the given string
        /// </summary>
        /// <param name="filePath">String containing the serialized xml form of the object</param>
        /// <param name="type">Type of the object to be deserialized</param>
        /// <returns>Deserialized object</returns>
        public static object XmlToFromFile(string filePath, Type type)
        {
            XmlSerializer xmlSerializer;
            FileStream fileStream = null;
            try
            {
                xmlSerializer = new XmlSerializer(type);
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                object objectFromXml = xmlSerializer.Deserialize(fileStream);
                return objectFromXml;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        #endregion
    }
}
