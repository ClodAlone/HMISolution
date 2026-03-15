#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Data.Xml.Dom;
using XmlNode = Windows.Data.Xml.Dom.IXmlNode;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Serializer
{
    //[KnownType("GetKnownType")]
    //[KnownType(typeof(GraphConstraints))]
    //[KnownType(typeof(MultipleSelectionMode))]
    //[KnownType(typeof(Tool))]
    //[KnownType(typeof(DrawingTool))]
    internal class GraphSerializerController : ISharedData
    {
        private SharedData _mSharedData;
        private DataContractSerializer serializer;

        private void InitalizeSerializer()
        {
#if WINRT
            DataContractSerializerSettings settings = new DataContractSerializerSettings();
            settings.KnownTypes = GetKnownType(_mSharedData.Graph);
            serializer = new DataContractSerializer(typeof(Graph), settings);
#else
                serializer = new DataContractSerializer(typeof(Graph), GetKnownType(_mSharedData.Graph)); 
#endif
        }

        public void DataContractSerializer(Stream stream)
        {
            InitalizeSerializer();
            Graph graph = _mSharedData.Graph.Serialize();
            serializer.WriteObject(stream, graph);
        }

        public void DataContractSerializer(XmlWriter stream)
        {
            InitalizeSerializer();
            Graph graph = _mSharedData.Graph.Copy();
            serializer.WriteObject(stream, graph);
        }
#if !SILVERLIGHT
        private void Upgrade(Version oldVersion, XmlDocument document)
        {
            // 11.2 SP1 [v11.2.0.29] - No changes
            // 11.2 SP2 [v11.2.0.64] - changes
            if (oldVersion < new Version(11, 2010, 0, 64))
            {
                foreach (XmlNode node in document.GetElementsByTagName("Type").Cast<XmlNode>().ToList())
                {
                    var parent = node.ParentNode;
                    parent.RemoveChild(node);
                    parent.InsertBefore(node, parent.FirstChild);
                }
                foreach (XmlNode node in document.GetElementsByTagName("Raw").Cast<XmlNode>().ToList())
                {
                    var parent = node.ParentNode;
                    parent.RemoveChild(node);
                    parent.InsertBefore(node, parent.FirstChild);
                }
            }
            // 11.3 [v11.3.0.x] - changes
            if (oldVersion < new Version(11, 3010, 0, 1))
            {
                foreach (XmlNode node in document.GetElementsByTagName("ID").Cast<XmlNode>().ToList())
                {
                    var parent = node.ParentNode;
                    parent.RemoveChild(node);
                    parent.InsertBefore(node, parent.FirstChild);
                }
            }
        }

        public void Upgrade(Stream stream)
        {
            TextReader re = new StreamReader(stream);
            XmlDocument document = new XmlDocument();
            document.LoadXml(re.ReadToEnd());
            var graph = document.ChildNodes[0];
            Version fileVersion = new Version(11, 2010, 0, 29);
            foreach (XmlNode graphProperties in graph.ChildNodes)
            {
                if (graphProperties.Name() == "V")
                {
                    int major = 0, minor = 0, build = 0, revision = 0;
                    foreach (XmlNode ver in graphProperties.ChildNodes)
                    {
                        if (ver.Name().Contains("Build"))
                        {
                            build = int.Parse(ver.InnerText);
                        }
                        else if (ver.Name().Contains("Major"))
                        {
                            major = int.Parse(ver.InnerText);
                        }
                        else if (ver.Name().Contains("Minor"))
                        {
                            minor = int.Parse(ver.InnerText);
                        }
                        else if (ver.Name().Contains("Revision"))
                        {
                            revision = int.Parse(ver.InnerText);
                        }
                    }
                    fileVersion = new Version(major, minor, build, revision);
                    break;
                }
            }
            Upgrade(fileVersion, document);
            //string output = document.GetXml();
            stream.Flush();
            stream.Position = 0;
            document.Save(stream);
            //XmlReader reader = XmlReader.Create(new StringReader(output));
            //stream.Flush();
            //stream.Position = 0;
            //StreamWriter writer = new StreamWriter(stream);
            //writer.Write(output);
            //writer.Flush();
            //stream.Position = 0;
        }
#endif

        public void DataContractDeSerializer(Stream stream)
        {
            InitalizeSerializer();
            Graph g = serializer.ReadObject(stream) as Graph;
            _mSharedData.Graph.DeSerialize(g);
        }

        public void DataContractDeSerializer(XmlReader stream)
        {
            InitalizeSerializer();
            Graph g = serializer.ReadObject(stream) as Graph;
            _mSharedData.Graph.DeSerialize(g);
        }

        //internal void Cut()
        //{
        //    IGraphInternal diagram = _mSharedData.Graph;
        //    _misCutEnabled = true;
        //    (diagram as SfDiagram).Delete.Execute(null);
        //}

        private int pasteCount = 0;

        public string Copy()
        {
            pasteCount = 0;
            using (TextWriter textWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(textWriter))
                {
                    DataContractSerializer(writer);
                }
                return textWriter.ToString();
            }
        }

        public void Paste(object param, string stringBuilder)
        {
            pasteCount++;
            _mSharedData.Graph.InternalSelectedItems.ClearSelection();
            using (TextReader reader = new StringReader(stringBuilder))
            {
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    DataContractDeSerializer(xmlReader);
                }
            }
            if (param != null && param is IDuplicateParameter && (param as IDuplicateParameter).DragClone
                && (param as IDuplicateParameter).PointerArgs != null)
            {

            }
            else
            {
                _mSharedData.Graph.InternalSelectedItems.OffsetX += (25 * pasteCount);
                _mSharedData.Graph.InternalSelectedItems.OffsetY += (25 * pasteCount);
            }

        }

        internal IEnumerable<Type> GetKnownType(IGraphInternal graph)
        {
            foreach (var type in typeof(INode).GetTypeInfo()
                                               .Assembly.DefinedTypes()
                                               .Where(
                                                type => (type.IsPublic
                                                        && !type.IsInterface
                                                        && !type.IsGenericType
                                                        && !typeof(UIElement).GetTypeInfo().IsAssignableFrom(type)
                                                        ))
                                               .Select(typeinfo => typeinfo.AsType()))
            {
                if (type.Name.Contains("ViewModel"))
                {
                    yield return type;
                }
            }
            if (graph.KnownTypes != null)
            {
                foreach (var type in graph.KnownTypes())
                {
                    yield return type;
                }
            }
        }

        internal static Dictionary<string, object> GetAttributes(object source)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            foreach (var property in source.GetType().GetRuntimeProperties())
            {
                foreach (var attribute in property.GetCustomAttributes())
                {
                    if (attribute is DataMemberAttribute)
                    {
                        DataMemberAttribute att = attribute as DataMemberAttribute;
                        if (string.IsNullOrEmpty(att.Name))
                        {
                            object value = property.GetValue(source);
                            if (value != null)
                            {
                                attributes.Add(property.Name, value);
                            }
                        }
                        else
                        {
                            object value = property.GetValue(source);
                            if (value != null)
                            {
                                attributes.Add(att.Name, value);
                            }
                        }
                    }
                }
            }
            return attributes;
        }

        internal static void SetAttributes(object source, Dictionary<string, object> attributes)
        {
            if (attributes == null)
            {
                return;
            }
            var attributeEnum = attributes.GetEnumerator();
            if (attributeEnum.MoveNext())
            {
                foreach (var property in source.GetType().GetRuntimeProperties())
                {
                    if (property.Name == attributeEnum.Current.Key)
                    {
                        property.SetValue(source, attributeEnum.Current.Value);
                        if (!attributeEnum.MoveNext())
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Init(SharedData shared)
        {
            _mSharedData = shared;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


}