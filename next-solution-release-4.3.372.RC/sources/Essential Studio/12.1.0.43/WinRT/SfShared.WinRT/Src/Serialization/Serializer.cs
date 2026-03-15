#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
#if SILVERLIGHT
using System.Threading.Tasks;
#else
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using System.Threading.Tasks;
#endif
namespace Syncfusion.UI.Xaml.Controls.Serialization
{
    /// <summary>
    /// Represents a class for serialization
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="file"></param>
        /// <returns></returns>
#if !SILVERLIGHT
        public async static Task SerializeObject<T>(T serializableObject, StorageFile file)
        {
            if (serializableObject == null) { return; }

            try
            {
                XDocument xmlDocument = new XDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    var document  = XDocument.Load(stream);
                    var writestream = await file.OpenStreamForWriteAsync();
                    document.Save(writestream);
                    await writestream.FlushAsync();
                    writestream.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deserializes the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public async static Task<T> DeSerializeObject<T>(StorageFile file)
        {
            if (file == null) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;
                var readstream = await file.OpenStreamForReadAsync();
                var document = XDocument.Load(readstream);
              
                string xmlString = document.ToString(SaveOptions.None);

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    objectOut = (T)serializer.Deserialize(read);
                    read.Dispose();
                }
                await readstream.FlushAsync();
                readstream.Dispose();
            }
            catch (Exception)
            {
                throw;
            }

            return objectOut;
        }
#endif
    }
}
