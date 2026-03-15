#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Reflection;

#if SILVERLIGHT
namespace Syncfusion.Windows.PropertyGrid
{
    public class ImageViewerEditor : ITypeEditor
    {
        public event System.ComponentModel.PropertyChangedEventHandler ValueChanged;

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            var binding = new Binding("Value")
            {
                Mode = BindingMode.TwoWay,
                Source = info,
                //Source = property,
                ValidatesOnExceptions = true
            };
            //this.SetBinding(ImageViewerEditor.SourceProperty, binding);
            BindingOperations.SetBinding(imageViewer, ImageViewer.SourceProperty, binding);
        }

        ImageViewer imageViewer;
        public object Create(PropertyInfo propertyInfo)
        {
            imageViewer = new ImageViewer();
            return imageViewer;
            //return new ImageViewerEditor();
        }

        public void Detach(PropertyViewItem property)
        {
            throw new NotImplementedException();
        }

        public bool Supports(PropertyViewItem Property)
        {
            throw new NotImplementedException();
        }
    }
}
#endif

