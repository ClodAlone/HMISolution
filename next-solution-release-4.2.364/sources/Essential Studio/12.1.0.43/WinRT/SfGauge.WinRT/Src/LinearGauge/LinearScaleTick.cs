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
    #region LinearScaleTick

    public class LinearScaleTick : DependencyObject
    {
        #region Public Dependency Properties

        #region TickLength
        public double TickLength
        {
            get { return (double)GetValue(TickLengthProperty); }
            set { SetValue(TickLengthProperty, value); }
        }

        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register("TickLength", typeof(double), typeof(LinearScaleTick), new PropertyMetadata(double.NaN));
        #endregion

        #region TickStrokeThickness
        public double TickStrokeThickness
        {
            get { return (double)GetValue(TickStrokeThicknessProperty); }
            set { SetValue(TickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeThicknessProperty =
            DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(LinearScaleTick), new PropertyMetadata(double.NaN));
        #endregion

        #region TickStroke
        public Brush TickStroke
        {
            get { return (Brush)GetValue(TickStrokeProperty); }
            set { SetValue(TickStrokeProperty, value); }
        }

        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(LinearScaleTick), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region TickVerticalAlignment
        public VerticalAlignment TickVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(TickVerticalAlignmentProperty); }
            set { SetValue(TickVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickVerticalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickVerticalAlignmentProperty =
            DependencyProperty.Register("TickVerticalAlignment", typeof(VerticalAlignment), typeof(LinearScaleTick), new PropertyMetadata(VerticalAlignment.Stretch)); 
        #endregion

        #region ParentScale
        public LinearScale ParentScale
        {
            get { return (LinearScale)GetValue(ParentScaleProperty); }
            set { SetValue(ParentScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentScaleProperty =
            DependencyProperty.Register("ParentScale", typeof(LinearScale), typeof(LinearScaleTick), new PropertyMetadata(null)); 
        #endregion

        #endregion
    }

    #endregion

    #region LinearScaleTickCollection

    public class LinearScaleTickCollection : ObservableCollection<LinearScaleTick>
    {
    }

    #endregion
}
