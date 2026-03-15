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
using Windows.UI.Xaml;
#else
using System.Windows;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class SolidShapeAnnotation : ShapeAnnotation
    {
        /// <summary>
        /// Gets or Sets the Rotation Angle for Annotation
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// The angle property
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(SolidShapeAnnotation), new PropertyMetadata(0d, OnUpdatePropertyChanged));

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        /// <summary>
        /// Gets or Sets the Annotation Re-sizing Path
        /// </summary>
        public AxisMode ResizingMode
        {
            get { return (AxisMode)GetValue(ResizingModeProperty); }
            set { SetValue(ResizingModeProperty, value); }
        }

        /// <summary>
        /// The resizing path property
        /// </summary>
        public static readonly DependencyProperty ResizingModeProperty =
            DependencyProperty.Register("ResizingMode", typeof(AxisMode), typeof(SolidShapeAnnotation), new PropertyMetadata(AxisMode.All, OnResizingPathChanged));

        private static void OnResizingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && (d as SolidShapeAnnotation).Chart != null)
                (d as SolidShapeAnnotation).UpdateResizingPath((AxisMode)e.NewValue);
        }

        private void UpdateResizingPath(AxisMode path)
        {
            AnnotationResizer annotationResizer=Chart.AnnotationManager.AnnotationResizer;
            if (annotationResizer!=null)
            {
                if (annotationResizer.ResizingMode != path)
                {
                    annotationResizer.ResizingMode = path;
                    annotationResizer.ResizerControl.ChangeView();
                }
            }
        }
    }
}
