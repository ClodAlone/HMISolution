//-------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace Syncfusion.Olap.Common
{
    /// <summary>
    /// Common class will holds the common methods.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Common"/> class.
        /// </summary>
        public Common()
        {
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
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

        /// <summary>
        /// Converts the UTF8 byte array to string.
        /// </summary>
        /// <param name="characters">Characters as an array of byte.</param>
        /// <returns></returns>
        static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// De-serializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML string.</param>
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
        /// Converts object to XML string format.
        /// </summary>
        /// <param name="objToXml">The input object.</param>
        /// <param name="includeNameSpace">if set to <c>true</c> [include name space].</param>
        /// <returns></returns>
        public static string ToXml(Object objToXml, bool includeNameSpace)
        {
            TextWriter tWriter = null;
            XmlSerializer xmlSerializer;
            try
            {
                xmlSerializer = new XmlSerializer(objToXml.GetType());
                tWriter = new StringWriter();
                if (!includeNameSpace)
                {
                    System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                    ////To remove namespace and any other inline information tag
                    xs.Add(string.Empty, string.Empty);
                    xmlSerializer.Serialize(tWriter, objToXml, xs);
                }
                else
                {
                    xmlSerializer.Serialize(tWriter, objToXml);
                }
                return tWriter.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (tWriter != null)
                {
                    tWriter.Close();
                }
            }
        }


        /// <summary>
        /// De-serializes the object given with the type from the given string
        /// </summary>
        /// <param name="filePath">String containing the serialized xml form of the object</param>
        /// <param name="type">Type of the object to be de-serialized</param>
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
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
    }

    /// <summary>
    /// Creates class for the given DataRowCollection.
    /// </summary>
    public class RuntimClassGenerator
    {
        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static TypeBuilder CreateObject(System.Data.DataRowCollection element)
        {
            // create a dynamic assembly and module 
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "OlapBase";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("AdomdEnumerableCollection");

            // create a new type builder 
            TypeBuilder typeBuilder = module.DefineType("AdomdEnumerableCollection", TypeAttributes.Public | TypeAttributes.Class);

            foreach (System.Data.DataRow schemaRow in element)
            {
                var propertyName = schemaRow[0].ToString();
                Type propertyType = schemaRow[6] as Type;

                // Generate a private field 
                FieldBuilder field = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                // Generate a public property 
                PropertyBuilder property =
                typeBuilder.DefineProperty(propertyName,
                PropertyAttributes.None,
                propertyType,
                new Type[] { propertyType });

                // The property set and property get methods require a special set of attributes:
                MethodAttributes GetSetAttr =
                MethodAttributes.Public |
                MethodAttributes.HideBySig;

                // Define the "get" accessor method for current private field. 
                MethodBuilder currGetPropMthdBldr =
                typeBuilder.DefineMethod("get_value",
                GetSetAttr,
                propertyType,
                Type.EmptyTypes);

                // Intermediate Language stuff... 
                ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for current private field. 
                MethodBuilder currSetPropMthdBldr =
                typeBuilder.DefineMethod("set_value",
                GetSetAttr,
                null,
                new Type[] { propertyType });

                // Again some Intermediate Language stuff... 
                ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to 
                // their corresponding behaviors, "get" and "set" respectively. 
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
            }

            return typeBuilder;
        }
    }
}
