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
    public class Connector : DiagramElement
    {
        #region IGroupable

        //public object ID { get; set; }
        public object GroupId { get; set; }
        public bool IsSelected { get; set; }
        public int ZIndex { get; set; }
        //List<IAnnotation> Annotations { get; set; } 

        #endregion

        public object SourceId { get; set; }
        public object TargetId { get; set; }

        public Point SourcePoint { get; set; }
        public Point TargetPoint { get; set; }

        public ConnectorConstraints Constraints { get; set; }
        public object SourcePortId { get; set; }
        public object TargetPortId { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<Annotation> Annotations { get; set; }
        public string AnnotationColT { get; set; }

        internal void Serializer(IInternalConnector wrapper)
        {
            base.Serializer(wrapper);
            ID = wrapper.ID;
            wrapper.InternalID = Guid.NewGuid();
            InternalID = wrapper.InternalID;
            Constraints = wrapper.Constraints;
            IsSelected = wrapper.IsSelected;
            if (wrapper.SourcePort != null)
            {
                SourcePortId = wrapper.KnownSourcePort.InternalID;
                SourceId = wrapper.KnownSourceNode.InternalID;
            }
            else if (wrapper.SourceNode != null)
            {
                SourceId = wrapper.KnownSourceNode.InternalID;
            }
            else
            {
                SourcePoint = wrapper.SourcePoint;
            }

            if (wrapper.TargetPort != null)
            {
                TargetPortId = wrapper.KnownTargetPort.InternalID;
                TargetId = wrapper.KnownTargetNode.InternalID;
            }
            else if (wrapper.TargetNode != null)
            {
                TargetId = wrapper.KnownTargetNode.InternalID;
            }
            else
            {
                TargetPoint = wrapper.TargetPoint;
            }
            Annotations = new List<Annotation>();
            if (wrapper.InternalAnnotations != null)
            {
                foreach (AnnotationEditorWrapper anno in wrapper.InternalAnnotations)
                {
                    Annotations.Add(anno.Serialize());
                }
            }
            Attributes = GraphSerializerController.GetAttributes(wrapper.Source);
        }

        internal IInternalConnector DeSerialize(IGraphInternal diagram)
        {
            IInternalConnector wrapper = diagram.GetConnectorWrapper(base.DeSerialize(), true);
            wrapper.ID = ID;
            wrapper.InternalID = InternalID ?? ID;
            wrapper.Constraints = Constraints;
            wrapper.IsSelected = IsSelected;

            if (!string.IsNullOrEmpty(AnnotationColT))
            {
                wrapper.Annotations =
                    Activator.CreateInstance(System.Type.GetType(AnnotationColT));
            }

            //wrapper.KnownSourcePort = connector.SourcePort;
            //wrapper.KnownTargetPort = connector.TargetPort;
            //wrapper.SourcePort.ID = connector.SourcePortId;
            //wrapper.TargetPort.ID = connector.TargetPortId;
            if (Annotations != null)
            {
                foreach (Annotation a in Annotations)
                {
                    AnnotationEditorWrapper wra = a.DeSerialize(diagram);
                    wrapper.InternalAnnotations.Add(wra, ItemSource.UnKnown);
                }
            }
            GraphSerializerController.SetAttributes(wrapper.Source, Attributes);


            IInternalConnector internalConnector = wrapper;
            //List<IAnnotation> Annotation = new List<IAnnotation>();
            //internalConnector.InternalAnnotations = new ObservableElements<IAnnotation, AnnotationEditorWrapper>(Annotation, ElementType.Annotation, SourceType.Node, _mSharedData.EventAggregator, _mSharedData.Graph.GetAnnotationWrapper);
            if (SourceId != null)
            {
                internalConnector.KnownSourceNode = diagram.InternalNodes.Last(e => e.InternalID != null && e.InternalID.Equals(SourceId));
                if (SourcePortId != null)
                {
                    internalConnector.KnownSourcePort = internalConnector.KnownSourceNode.InternalPorts.FirstOrDefault(e => e.InternalID.Equals(SourcePortId));
                }
            }
            else
            {
                internalConnector.SourcePoint = SourcePoint;
            }
            if (TargetId != null)
            {
                internalConnector.KnownTargetNode = diagram.InternalNodes.Last(e => e.InternalID != null && e.InternalID.Equals(TargetId));
                if (TargetPortId != null)
                {
                    internalConnector.KnownTargetPort = internalConnector.KnownTargetNode.InternalPorts.Last(e => e.InternalID.Equals(TargetPortId));
                }
            }
            else
            {
                internalConnector.TargetPoint = TargetPoint;
            }

            //if (!Serialization)
            //{
            //    if (!_misCutEnabled)
            //    {
            //        internalConnector.SourcePoint = new Point((internalConnector.SourcePoint.X + (25 * (_mPasteCount + 1))), (internalConnector.SourcePoint.Y + (25 * (_mPasteCount + 1))));
            //        internalConnector.TargetPoint = new Point((internalConnector.TargetPoint.X + (25 * (_mPasteCount + 1))), (internalConnector.TargetPoint.Y + (25 * (_mPasteCount + 1))));
            //    }
            //}

            return wrapper;
        }
    }
}
