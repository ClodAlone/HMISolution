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
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Serializer
{
    public abstract class DiagramElement
    {
        public string Type { get; set; }
        public object Raw { get; set; }
        public object ID { get; set; }
        public object InternalID { get; set; }

        internal void Serializer(IWrapper wrapper)
        {
            InternalID = Guid.NewGuid();
            string type = wrapper.Source.GetType().AssemblyQualifiedName;
            Type = type.Substring(0, type.IndexOf(", Version="));

            foreach (Attribute attribute in wrapper.Source.GetType().GetTypeInfo().GetCustomAttributes())
            {
                if (attribute is DataContractAttribute || attribute is CollectionDataContractAttribute)
                {
                    Raw = wrapper.Source;
                }
            }

            if (this is Node)
            {
                Node n = this as Node;
                IInternalNode node = wrapper as IInternalNode;
                if (node.Ports != null)
                {
                    n.PortColT = node.Ports.GetType().AssemblyQualifiedName;
                }

                if (node.Annotations != null)
                {
                    n.AnnotationColT = node.Annotations.GetType().AssemblyQualifiedName;
                }
            }


            if (this is Connector)
            {
                Connector c = this as Connector;
                IInternalConnector con = wrapper as IInternalConnector;

                if (con.Annotations != null)
                {
                    c.AnnotationColT = con.Annotations.GetType().AssemblyQualifiedName;
                }
            }
        }

        internal object DeSerialize()
        {
            object source;
            if (Raw != null)
            {
                source = Raw;
            }
            else
            {
                source = Activator.CreateInstance(System.Type.GetType(Type));
            }
            return source;
        }
    }
}
