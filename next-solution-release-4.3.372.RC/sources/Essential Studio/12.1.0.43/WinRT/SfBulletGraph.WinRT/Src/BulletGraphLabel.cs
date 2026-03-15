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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;

#endif

namespace Syncfusion.UI.Xaml.BulletGraph
{
    #region BulletGraphLabel

    public class BulletGraphLabel : DependencyObject
    {
        #region Public Dependency Properties

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(BulletGraphLabel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region LabelSize
        public double LabelSize
        {
            get { return (double)GetValue(LabelSizeProperty); }
            set { SetValue(LabelSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelSizeProperty =
            DependencyProperty.Register("LabelSize", typeof(double), typeof(BulletGraphLabel), new PropertyMetadata(40d));
        #endregion

        #region LabelContent
        public Object LabelContent
        {
            get { return GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(Object), typeof(BulletGraphLabel), new PropertyMetadata(string.Empty));
        #endregion

        #region ParentBulletGraph
        public SfBulletGraph ParentBulletGraph
        {
            get { return (SfBulletGraph)GetValue(ParentBulletGraphProperty); }
            set { SetValue(ParentBulletGraphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentBulletGraph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentBulletGraphProperty =
            DependencyProperty.Register("ParentBulletGraph", typeof(SfBulletGraph), typeof(BulletGraphLabel), new PropertyMetadata(null));
        #endregion

        #region Transform
        public TransformGroup Transform
        {
            get { return (TransformGroup)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(TransformGroup), typeof(BulletGraphLabel), new PropertyMetadata(new TransformGroup()));
        #endregion

        #endregion
    }

    #endregion

    #region BulletGraphLabelCollection

    public class BulletGraphLabelCollection : ObservableCollection<BulletGraphLabel>
    {
    }

    #endregion
}
