#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.BulletGraph
{
    #region BulletGraphTick

    public class BulletGraphTick : DependencyObject
    {
        #region Public Dependency Properties

        #region TickLength
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }

        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(BulletGraphTick), new PropertyMetadata(10d));
        #endregion

        #region TickStroke
        public Brush TickStroke
        {
            get { return (Brush)GetValue(TickStrokeProperty); }
            set { SetValue(TickStrokeProperty, value); }
        }

        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(BulletGraphTick), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region TickStrokeThickness
        public double TickStrokeThickness
        {
            get { return (double)GetValue(TickStrokeThicknessProperty); }
            set { SetValue(TickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeThicknessProperty =
            DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(BulletGraphTick), new PropertyMetadata(1d));
        #endregion

        #region TickVerticalAlignment
        public VerticalAlignment TickVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(TickVerticalAlignmentProperty); }
            set { SetValue(TickVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickVerticalAlignmentProperty =
            DependencyProperty.Register("TickVerticalAlignment", typeof(VerticalAlignment), typeof(BulletGraphTick), new PropertyMetadata(VerticalAlignment.Stretch)); 
        #endregion

        #region ParentBulletGraph
        public SfBulletGraph ParentBulletGraph
        {
            get { return (SfBulletGraph)GetValue(ParentBulletGraphProperty); }
            set { SetValue(ParentBulletGraphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentBulletGraph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentBulletGraphProperty =
            DependencyProperty.Register("ParentBulletGraph", typeof(SfBulletGraph), typeof(BulletGraphTick), new PropertyMetadata(null)); 
        #endregion

        #endregion
    }

    #endregion

    #region BulletGraphTickCollection

    public class BulletGraphTickCollection : ObservableCollection<BulletGraphTick>
    {
    }

    #endregion
}
