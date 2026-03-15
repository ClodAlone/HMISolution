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
    public class Node : DiagramElement
    {
        #region IGroupable

        //public object ID { get; set; }
        public object GroupId { get; set; }
        public bool IsSelected { get; set; }
        public int ZIndex { get; set; }
        //List<IAnnotation> Annotations { get; set; } 

        #endregion

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public Point Pivot { get; set; }
        public double RotateAngle { get; set; }
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double Width { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }
        public double Height { get; set; }
        public NodeConstraints Constraints { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<Annotation> Annotations { get; set; }
        public List<NodePort> Ports { get; set; }
        public string AnnotationColT { get; set; }
        public string PortColT { get; set; }
        public string Geometry { get; set; }
        public Flip Flip { get; set; }

        internal void Serializer(IInternalNode wrapper)
        {
            base.Serializer(wrapper);
            ID = wrapper.ID;
            wrapper.InternalID = Guid.NewGuid();
            InternalID = wrapper.InternalID;
            OffsetX = wrapper.OffsetX;
            OffsetY = wrapper.OffsetY;
            Constraints = wrapper.Constraints;
            Height = wrapper.UnitHeight;
            Width = wrapper.UnitWidth;
            IsSelected = wrapper.IsSelected;
            MaxHeight = wrapper.MaxHeight;
            MinHeight = wrapper.MinHeight;
            MaxWidth = wrapper.MaxWidth;
            MinWidth = wrapper.MinWidth;
            Pivot = wrapper.Pivot;
            RotateAngle = wrapper.RotateAngle;
            Flip = wrapper.Flip;
            if (wrapper.Shape is Geometry)
            {
                Geometry = (wrapper.Shape as Geometry).ToStreamGeometry();
            }
            else if (wrapper.Shape is string)
            {
                Geometry = (string)wrapper.Shape;
            }
            //Geometry = wrapper.Shape.ToStreamGeometry();
            SerializeAnnotationsandPort(wrapper);
            Attributes = GraphSerializerController.GetAttributes(wrapper.Source);
        }

        internal void SerializeAnnotationsandPort(IInternalNode wrapper)
        {
            Annotations = new List<Annotation>();
            if (wrapper.InternalAnnotations != null)
            {
                foreach (AnnotationEditorWrapper ano in wrapper.InternalAnnotations)
                {
                    Annotations.Add(ano.Serialize());
                }
            }

            Ports = new List<NodePort>();
            if (wrapper.InternalPorts != null)
            {
                foreach (NodePortWrapper pw in wrapper.InternalPorts)
                {
                    NodePort n1 = pw.Serialize();
                    n1.Node = wrapper;
                    Ports.Add(n1);
                }
            }
        }

        internal IInternalNode DeSerializeNode(IGraphInternal diagram)
        {
            object newNode = base.DeSerialize();
            IInternalNode wrapper = diagram.GetNodeWrapper(newNode, true);

            wrapper.ID = ID;
            wrapper.InternalID = InternalID ?? ID;
            wrapper.OffsetX = OffsetX;
            wrapper.OffsetY = OffsetY;
            wrapper.Constraints = Constraints;
            wrapper.UnitHeight = Height;
            wrapper.UnitWidth = Width;
            wrapper.IsSelected = IsSelected;
            wrapper.MaxHeight = MaxHeight;
            wrapper.Pivot = Pivot;
            wrapper.RotateAngle = RotateAngle;
            wrapper.Shape = Geometry;
            wrapper.Flip = Flip;
            //wrapper.Shape = Geometry.ParseGeometry();

            if (!string.IsNullOrEmpty(PortColT))
            {
                wrapper.Ports = Activator.CreateInstance(System.Type.GetType(PortColT));
            }

            if (!string.IsNullOrEmpty(AnnotationColT))
            {
                wrapper.Annotations =
                    Activator.CreateInstance(System.Type.GetType(AnnotationColT));
            }

            DeSerializeAnnotationsandPorts(wrapper, diagram);
            GraphSerializerController.SetAttributes(wrapper.Source, Attributes);



            //IInternalNode internalNode = wrapper;
            //List<IAnnotation> Annotation = new List<IAnnotation>();
            //internalNode.InternalAnnotations = new ObservableElements<IAnnotation, AnnotationEditorWrapper>(Annotation, ElementType.Annotation, SourceType.Node, _mSharedData.EventAggregator, _mSharedData.Graph.GetAnnotationWrapper);
            //List<INodePort> Port = new List<INodePort>();
            //internalNode.Ports = new ObservableElements<INodePort, IInternalNodePort>(Port, ElementType.Port, SourceType.Node, _mSharedData.EventAggregator, _mSharedData.Graph.GetNodePortWrapper);


            //if (!Serialization)
            //{
            //    if (!_misCutEnabled)
            //    {
            //        internalNode.OffsetX = internalNode.OffsetX + (25 * (_mPasteCount + 1));
            //        internalNode.OffsetY = internalNode.OffsetY + (25 * (_mPasteCount + 1));
            //    }
            //}


            return wrapper;
        }

        internal void DeSerializeAnnotationsandPorts(IInternalNode wrapper, IGraphInternal diagram)
        {
            if (Annotations != null)
            {
                foreach (Annotation a in Annotations)
                {
                    AnnotationEditorWrapper annotatewrapper = a.DeSerialize(diagram);
                    wrapper.InternalAnnotations.Add(annotatewrapper, ItemSource.UnKnown);
                }
            }

            if (Ports != null)
            {
                foreach (NodePort n in Ports)
                {
                    IInternalNodePort wrap = n.DeSerialize(diagram);
                    wrap.KnownNode = wrapper;
                    wrapper.InternalPorts.Add(wrap, ItemSource.UnKnown);
                }
            }
        }
    }
}
