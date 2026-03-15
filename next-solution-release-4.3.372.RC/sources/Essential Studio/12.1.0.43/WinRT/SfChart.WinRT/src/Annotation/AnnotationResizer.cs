#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows;
using System.Windows.Controls.Primitives;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    internal class AnnotationResizer : SolidShapeAnnotation
    {
        internal override UIElement CreateAnnotation()
        {
            ResizerControl = new Resizer();
            ResizerControl.AnnotationResizer = this;
            AnnotationElement.Children.Add(ResizerControl);
            return AnnotationElement;
        }

        internal void MapActualValueToPixels()
        {
            ResizerControl.MapActualValueToPixels();
        }
    }
}
