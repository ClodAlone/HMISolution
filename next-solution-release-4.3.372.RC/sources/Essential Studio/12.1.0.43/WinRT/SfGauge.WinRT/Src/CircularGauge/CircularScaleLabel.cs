#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
    /// <summary>
    /// It helps the user to display scale labels that associate a numeric value with
    /// major scale tick marks
    /// </summary>
    public class CircularScaleLabel : DependencyObject
    {

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.CircularScaleLabel"/> class.
        /// </summary>
        public CircularScaleLabel()
        {

        }

        /// <summary>
        /// Gets or sets the label has to be displayed.
        /// </summary>
        /// <value>
        /// object
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(CircularScaleLabel), new PropertyMetadata(null));





        /// <summary>
        /// Gets or sets the brush that describes the Foreground of the CircularScaleLabel.
        /// </summary>
        /// <value>
        /// Brush
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(CircularScaleLabel), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        /// <summary>
        /// Gets or sets the Angle of the CircularScaleLabel.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularScaleLabel), new PropertyMetadata(0d));



        internal double LabelFontSize
        {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelFontSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(CircularScaleLabel), new PropertyMetadata(null));

        
        
    }

    /// <summary>
    ///  It is a collection that contains a set of Labels that denote a numeric value of
    /// a major tick.
    /// </summary>
    public class CircularScaleLabelCollection : ObservableCollection<CircularScaleLabel>
    {
        
    }
}
