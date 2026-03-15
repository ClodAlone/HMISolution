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
    public class Annotation : DiagramElement
    {
        public Dictionary<string, object> Attributes { get; set; }
        public ConnectorAnnotationAlignment Alignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        internal void Serializer(AnnotationEditorWrapper anno)
        {
            base.Serializer(anno);
            Alignment = anno.Alignment;
            HorizontalAlignment = anno.HorizontalAlignment;
            VerticalAlignment = anno.VerticalAlignment;
            Attributes = GraphSerializerController.GetAttributes(anno.Source);
        }

        internal AnnotationEditorWrapper DeSerialize(IGraphInternal diagram)
        {
            AnnotationEditorWrapper anno = diagram.GetAnnotationWrapper(base.DeSerialize() as IAnnotation, true);
            anno.Alignment = Alignment;
            anno.HorizontalAlignment = HorizontalAlignment;
            anno.VerticalAlignment = VerticalAlignment;
            GraphSerializerController.SetAttributes(anno.Source, Attributes);
            return anno;
        }
    }
}
