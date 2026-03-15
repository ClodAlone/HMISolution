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
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Serializer
{
    public class NodePort : DiagramElement
    {
        public double NodeOffsetX { get; set; }
        public double NodeOffsetY { get; set; }

        internal IInternalNode Node
        {
            get;
            set;
        }

        public Dictionary<string, object> Attributes { get; set; }

        public UnitMode UnitMode
        {
            get;
            set;
        }

        //public object ID { get; set; }
        public string Geometry { get; set; }

        internal void Serializer(IInternalNodePort pw)
        {
            base.Serializer(pw);
            ID = pw.ID;
            pw.InternalID = Guid.NewGuid();
            InternalID = pw.InternalID;
            NodeOffsetX = pw.NodeOffsetX;
            NodeOffsetY = pw.NodeOffsetY;
            UnitMode = pw.UnitMode;
            if (pw.Shape is Geometry)
            {
                Geometry = (pw.Shape as Geometry).ToStreamGeometry();
            }
            else if(pw.Shape is string)
            {
                Geometry = (string)pw.Shape;
            }
            //Geometry = pw.Shape.ToStreamGeometry();
            Attributes = GraphSerializerController.GetAttributes(pw.Source);
        }

        internal IInternalNodePort DeSerialize(IGraphInternal diagram)
        {
            IInternalNodePort wrapper = diagram.GetNodePortWrapper(base.DeSerialize() as INodePort, true);
            wrapper.NodeOffsetX = NodeOffsetX;
            wrapper.NodeOffsetY = NodeOffsetY;
            wrapper.UnitMode = UnitMode;
            wrapper.ID = ID;
            wrapper.InternalID = InternalID ?? ID;
            wrapper.Shape = Geometry.ParseGeometry();
            GraphSerializerController.SetAttributes(wrapper.Source, Attributes);
            return wrapper;
        }
    }       
}
