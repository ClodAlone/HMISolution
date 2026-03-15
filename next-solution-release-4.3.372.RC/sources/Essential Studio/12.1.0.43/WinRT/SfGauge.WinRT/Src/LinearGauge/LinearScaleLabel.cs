#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    #region LinearScaleLabel

    public class LinearScaleLabel : DependencyObject
    {
        #region Public Dependency Properties

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(LinearScaleLabel), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region LabelSize
        public double LabelSize
        {
            get { return (double)GetValue(LabelSizeProperty); }
            set { SetValue(LabelSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelSizeProperty =
            DependencyProperty.Register("LabelSize", typeof(double), typeof(LinearScaleLabel), new PropertyMetadata(0d)); 
        #endregion

        #region LabelContent
        public Object LabelContent
        {
            get { return GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(Object), typeof(LinearScaleLabel), new PropertyMetadata(null));
        #endregion

        #region Transform
        public TransformGroup Transform
        {
            get { return (TransformGroup)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(TransformGroup), typeof(LinearScaleLabel), new PropertyMetadata(null)); 
        #endregion

        #region TransformOrigin
        public Point TransformOrigin
        {
            get { return (Point)GetValue(TransformOriginProperty); }
            set { SetValue(TransformOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransformOrigin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformOriginProperty =
            DependencyProperty.Register("TransformOrigin", typeof(Point), typeof(LinearScaleLabel), new PropertyMetadata(new Point()));
        #endregion

        #region ParentScale
        public LinearScale ParentScale
        {
            get { return (LinearScale)GetValue(ParentScaleProperty); }
            set { SetValue(ParentScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentScaleProperty =
            DependencyProperty.Register("ParentScale", typeof(LinearScale), typeof(LinearScaleLabel), new PropertyMetadata(null));
        #endregion

        #endregion
    }

    #endregion

    #region LinearScaleLabelCollection

    public class LinearScaleLabelCollection : ObservableCollection<LinearScaleLabel>
    {
    }

    #endregion
}
