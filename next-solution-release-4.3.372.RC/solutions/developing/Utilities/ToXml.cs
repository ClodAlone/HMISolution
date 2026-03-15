using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization.Json;
#if !NET_STANDARD
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
#endif
using System.Threading;

namespace Utilities
{
    public static class XmlHelper
    {
        /// <summary>
        /// Builds an object of the specified type from the given
        /// XML representation that can be passed to the DataContractSerializer
        /// </summary>
        public static object BuildFromDataContractXml(string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml)) { throw new ArgumentNullException("xml"); }
            if (type == null) { throw new ArgumentNullException("type"); }

            object result = null;
            DataContractSerializer dcs = new DataContractSerializer(type);
            //using (StringReader reader = new StringReader(xml))
            //using (XmlReader xmlReader = new XmlTextReader(reader))
            //{
            //    result = dcs.ReadObject(xmlReader);
            //}

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = dcs.ReadObject(ms);
            }

            return result;
        }
        /// <summary>
        /// Builds an object from its XML 
        /// representation that can be passed to the DataContractSerializer.
        /// </summary>
        public static T BuildFromDataContractXml<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml)) { throw new ArgumentNullException("xml"); }

            T result = default(T);

            object objResult = BuildFromDataContractXml(xml, typeof(T));
            if (objResult != null)
            {
                result = (T)objResult;
            }
            return result;
        }
        /// <summary>
        /// Gets the XML representation of the given object
        /// by using the DataContracSerializer.
        /// </summary>
        public static string GetDataContractXml<T>(T obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }

            return GetDataContractXml(obj.GetType(), obj);
        }
        /// <summary>
        /// Gets the XML representation of the given object
        /// of specfiied <c>type</c> by using the DataContracSerializer.
        /// </summary>
        public static string GetDataContractXml(Type type, object val)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (val == null) { throw new ArgumentNullException("val"); }

            //MemoryStream ms = new MemoryStream();
            //string xml = null;
            //try
            //{
                //DataContractSerializer dcs = new DataContractSerializer(type);

                //using (XmlTextWriter xmlTextWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
                //{
                //    xmlTextWriter.Formatting = System.Xml.Formatting.Indented;
                //    dcs.WriteObject(xmlTextWriter, val);
                //    xmlTextWriter.Flush();
                //    ms = (MemoryStream)xmlTextWriter.BaseStream;
                //    ms.Flush();
                //    xml = UTF8ByteArrayToString(ms.ToArray());
                //}
                using (MemoryStream memStm = new MemoryStream())
                {
                    var serializer = new DataContractSerializer(type);
                    serializer.WriteObject(memStm, val);

                    memStm.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(memStm))
                    {
                        string result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            //}
            //finally
            //{
            //    if (ms != null)
            //    {
            //        ms.Close();
            //        ms = null;
            //    }
            //}
            //return xml;

        }
        /// <summary>
        /// Writes the XML representation from the DataContractSerializer
        /// into the specified filename. If a file at <c>filename</c>
        /// already exists then an <c>Exception</c> will be thrown.
        /// </summary>
        public static void WriteDateContractToFile<T>(string filename, T obj)
        {
            WriteDateContractToFile(filename, obj.GetType(), obj);
        }
        /// <summary>
        /// Writes the XML representation from the DataContractSerializer
        /// into the specified filename. If a file at <c>filename</c>
        /// already exists then an <c>Exception</c> will be thrown.
        /// </summary>
        public static void WriteDateContractToFile(string filename, Type type, object val)
        {
            if (string.IsNullOrEmpty(filename)) { throw new ArgumentNullException("filename"); }
            if (val == null) { throw new ArgumentNullException("val"); }

            if (File.Exists(filename)) { throw new ArgumentException("filename"); }

            //TODO: Stream this into the file instead of this!!!
            File.WriteAllText(filename, GetDataContractXml(type, val));
        }
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }
        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }


        public static string ToXml(this object obj)
        {
            try
            {
                return GetDataContractXml(obj.GetType(), obj);
            }
            catch (Exception ex)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, obj);
                    return sw.ToString();
                }
            }
        }

        public static T FromXml<T>(this string data)
        {
            try
            {
                return BuildFromDataContractXml<T>(data);
            }
            catch (Exception ex)
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(data))
                {
                    object obj = s.Deserialize(reader);
                    return (T)obj;
                }
            }
        }

        public static string ToJSON(this object obj)
        {
            if (obj == null)
                return string.Empty;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            var json = Encoding.UTF8.GetString(ms.ToArray());
            return json;
        }

        public static IEnumerable<dynamic> GetExpandoCTSFromXml(string file, string descendantid,
        CancellationTokenSource cts = null, int maxTake = 0)
        {
            /* can be used...
            var expandolist = GetExpandoFromXml("http://phejndorf.wordpress.com/feed/", "item");
            expandolist.ToList().ForEach(element =>
            {
                var dictionary = element as IDictionary<string, object>;
                dictionary.ToList().ForEach(d => Console.WriteLine("{0}: {1}", d.Key, d.Value));
            });
            */
            var expandoFromXml = new List<dynamic>();

            var doc = XDocument.Load(file);
            // var nodes = doc.Root.Descendants(descendantid);

            int nCount = 0;
            foreach (var element in doc.Root.Descendants(descendantid).AsParallel())
            {
                dynamic expandoObject = new ExpandoObject();
                var dictionary = expandoObject as IDictionary<string, object>;
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                foreach (var child in element.Descendants().AsParallel())
                {
                    if (cts != null)
                        cts.Token.ThrowIfCancellationRequested();

                    if (child.Name.Namespace == "")
                    {
                        lock (dictionary)
                        {
                            dictionary[child.Name.ToString()] = child.Value.Trim();
                        }
                    }
                }
                if (maxTake > 0)
                {
                    var ret = Interlocked.Increment(ref nCount);
                    if (ret > maxTake)
                        break;
                }
                yield return expandoObject;
            }
        }

        public static IEnumerable<dynamic> GetExpandoFromXml(string file, string descendantid, bool fromcode = false, bool fromroot = true)
        {
            /* can be used...
            var expandolist = GetExpandoFromXml("http://phejndorf.wordpress.com/feed/", "item");
            expandolist.ToList().ForEach(element =>
            {
                var dictionary = element as IDictionary<string, object>;
                dictionary.ToList().ForEach(d => Console.WriteLine("{0}: {1}", d.Key, d.Value));
            });
            */
            var expandoFromXml = new List<dynamic>();
            XDocument doc = new XDocument();
            if (fromcode)
            {
              doc = XDocument.Parse(file);
            }
            else
            {
                doc = XDocument.Load(file);
            }

            if (fromroot)
            {
                //var nodes = doc.Root.Descendants(descendantid);
                foreach (var element in doc.Root.Descendants(descendantid)/*.AsParallel()*/)
                {
                    dynamic expandoObject = new ExpandoObject();
                    var dictionary = expandoObject as IDictionary<string, object>;
                    foreach (var child in element.Descendants())
                    {
                        if (child.Name.Namespace == "")
                            dictionary[child.Name.ToString()] = child.Value.Trim();
                    }
                    yield return expandoObject;
                }
            }
            else
            {
                //var nodes = doc.Root.Descendants(descendantid);

                foreach (var element in doc.Descendants(descendantid)/*.AsParallel()*/)
                {
                    dynamic expandoObject = new ExpandoObject();
                    var dictionary = expandoObject as IDictionary<string, object>;
                    foreach (var child in element.Descendants())
                    {
                        if (child.Name.Namespace == "")
                            dictionary[child.Name.ToString()] = child.Value.Trim();
                    }
                    yield return expandoObject;
                }
            }
        }

        public static IEnumerable<dynamic> GetExpandoAttributeFromXml(string file, string descendantid, bool fromcode = false)
        {
            /* can be used...
            var expandolist = GetExpandoFromXml("http://phejndorf.wordpress.com/feed/", "item");
            expandolist.ToList().ForEach(element =>
            {
                var dictionary = element as IDictionary<string, object>;
                dictionary.ToList().ForEach(d => Console.WriteLine("{0}: {1}", d.Key, d.Value));
            });
            */
            var expandoFromXml = new List<dynamic>();

            XDocument doc = new XDocument();
            if (fromcode)
            {
                doc = XDocument.Parse(file);
            }
            else
            {
                doc = XDocument.Load(file);
            }
            foreach (var element in doc.Descendants(descendantid)/*.AsParallel()*/)
            {
                dynamic expandoObject = new ExpandoObject();
                var dictionary = expandoObject as IDictionary<string, object>;
                foreach (var child in element.Descendants())
                {
                    if (child.Name.Namespace == "")
                        lock (dictionary)
                        {
                            //dictionary[child.Name.ToString()] = child.Value.Trim();
                            dictionary[child.FirstAttribute.Value.ToString()] = child.Value.Trim();
                        }
                }
                yield return expandoObject;
            }

        }

        public static bool ExportToFile<T>(T entity, string filePath, string title) where T : class
        {
            Type type = typeof(T);
            var folderPath = $"{System.IO.Path.GetDirectoryName(filePath)}";

            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.Unicode,
                Indent = true,
                CloseOutput = true
            };

            using (Stream ostrm = File.Open(filePath, FileMode.Create))
            {
                using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
                {
                    bool bRet = false;
                    try
                    {
                        DataContractSerializer serializer = new DataContractSerializer(type);
                        serializer.WriteObject(writer, entity);
                        bRet = true;
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        writer.Close();
                    }

                    return bRet;
                }
            }
        }
        public static T ImportFromFile<T>(string filePath, string title) where T : class
        {
            Type type = typeof(T);
            if (!System.IO.File.Exists(filePath))
                return (T)Activator.CreateInstance(typeof(T));
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    T ret = serializer.ReadObject(stream) as T;
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }

    }
}
