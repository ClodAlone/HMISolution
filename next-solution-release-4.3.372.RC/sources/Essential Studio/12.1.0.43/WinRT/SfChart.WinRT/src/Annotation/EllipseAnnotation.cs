#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class EllipseAnnotation : SolidShapeAnnotation
    {
        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                shape = new Ellipse();
                if (ShowToolTip)
                    shape.Tag = this;
                SetBindings();
                AnnotationElement.Children.Add(shape);
                TextElementCanvas.Children.Add(TextElement);
                AnnotationElement.Children.Add(TextElementCanvas);
            }
            return AnnotationElement;
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            return base.CloneAnnotation(new EllipseAnnotation());
        }
    }
}
