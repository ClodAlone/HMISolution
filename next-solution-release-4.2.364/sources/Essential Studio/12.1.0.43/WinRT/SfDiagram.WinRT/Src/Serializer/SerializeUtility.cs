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
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Serializer
{
    internal static class SerializeUtility
    {
        internal static Node Serialize(this IInternalNode node)
        {
            Node n = new Node();
            n.Serializer(node);
            return n;
        }

        internal static IInternalNode DeSerialize(this Node n, IGraphInternal graphInternal)
        {
            return n.DeSerializeNode(graphInternal);
        }

        internal static Connector Serialize(this IInternalConnector internalConnector)
        {
            Connector con = new Connector();
            con.Serializer(internalConnector);
            return con;
        }

        internal static IInternalConnector DeSerialize(this Connector con, IGraphInternal graphInternal)
        {
            return con.DeSerialize(graphInternal);
        }

        internal static Group Serialize(this IInternalGroup group)
        {
            Group g = new Group();
            g.Serializer(group);
            return g;
        }

        internal static IInternalGroup DeSerialize(this Group g, IGraphInternal graphInternal)
        {
            return g.DeSerializeGroup(graphInternal);
        }

        internal static Annotation Serialize(this AnnotationEditorWrapper annotation)
        {
            Annotation a = new Annotation();
            a.Serializer(annotation);
            return a;
        }

        internal static AnnotationEditorWrapper DeSerialize(this Annotation a, IGraphInternal graphInternal)
        {
            return a.DeSerialize(graphInternal);
        }

        internal static NodePort Serialize(this IInternalNodePort port)
        {
            NodePort p = new NodePort();
            p.Serializer(port);
            return p;
        }

        internal static IInternalNodePort DeSerialize(this NodePort p, IGraphInternal graphInternal)
        {
            return p.DeSerialize(graphInternal);
        }

        internal static Graph Serialize(this IGraphInternal graph)
        {
            Graph g = new Graph();
            g.Serialize(graph);
            return g;
        }

        internal static Graph Copy(this IGraphInternal graph)
        {
            Graph g = new Graph();
            g.Copy(graph);
            return g;
        }

        internal static void DeSerialize(this IGraphInternal graph, Graph g)
        {
            g.DeSerialize(graph);
        }

        internal static Version GetVersion(this object graph)
        {
            string type = graph.GetType().AssemblyQualifiedName;
            int start = type.IndexOf("Version=") + 8;
            int end = type.IndexOf(", Culture=");
            return new Version(type.Substring(start, end - start));
        }

#if WINRT
        internal static void Save(this XmlDocument document, Stream stream)
        {
            string output = document.GetXml();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(output);
            writer.Flush();
            stream.Position = 0;
        }

        internal static string Name(this IXmlNode node)
        {
            return node.NodeName;
        }

        internal static string Value(this IXmlNode node)
        {
            return node.NodeValue.ToString();
        }
#elif WPF
        internal static string Name(this XmlNode node)
        {
            return node.Name;
        }

        internal static string Value(this XmlNode node)
        {
            return node.Value;
        }
#endif
    }
}
