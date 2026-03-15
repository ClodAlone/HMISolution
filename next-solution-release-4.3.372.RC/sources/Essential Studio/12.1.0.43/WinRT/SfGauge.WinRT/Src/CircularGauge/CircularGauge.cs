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
    /// <para>Represents a control that helps user to visualize the single numerical data.</para>
    /// <toolboxitem>true</toolboxitem>
    /// <toolboxvscategory>Syncfusion controls for Metro</toolboxvscategory>
    /// <toolboxblendcategory>Syncfusion controls for Metro</toolboxblendcategory>
    /// </summary>
    [StyleTypedProperty(Property = "CircularScaleStyle", StyleTargetType = typeof(CircularScale))]
    public class SfCircularGauge : Control 
    {

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.SfCircularGauge"/> class.
        /// </summary>
        public SfCircularGauge()
        {
            this.DefaultStyleKey = typeof(SfCircularGauge);

            this.MainScale = new CircularScale();
            SubScales = new ObservableCollection<CircularScale>();
        }

        /// <summary>
        /// Gets or sets the header for CircularGauge.
        /// </summary>
        /// <value>
        /// Object
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            gauge.GaugeHeader = "Syncfusion";
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public object GaugeHeader
        {
            get { return (object)GetValue(GaugeHeaderProperty); }
            set { SetValue(GaugeHeaderProperty, value); }
        }
		
        // Using a DependencyProperty as the backing store for CustomObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GaugeHeaderProperty =
            DependencyProperty.Register("GaugeHeader", typeof(object), typeof(SfCircularGauge), new PropertyMetadata(null));

        

        internal Thickness GaugeHeaderMargin
        {
            get { return (Thickness)GetValue(GaugeHeaderMarginProperty); }
            set { SetValue(GaugeHeaderMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GaugeHeaderMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GaugeHeaderMarginProperty =
            DependencyProperty.Register("GaugeHeaderMargin", typeof(Thickness), typeof(SfCircularGauge), new PropertyMetadata(new Thickness(0, 0, 0, 0)));



        /// <summary>
        /// Gets or sets the position of the GaugeHeader of the SfCircularGauge.
        /// </summary>
        /// <value>
        /// Point
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            gauge.GaugeHeader = "Syncfusion";
        ///            gauge.GaugeHeaderPosition = new Point(100, 200);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public Point GaugeHeaderPosition
        {
            get { return (Point)GetValue(GaugeHeaderPositionProperty); }
            set { SetValue(GaugeHeaderPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GaugeHeaderPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GaugeHeaderPositionProperty =
            DependencyProperty.Register("GaugeHeaderPosition", typeof(Point), typeof(SfCircularGauge), new PropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnGaugeHeaderPositionChanged)));

        private static void OnGaugeHeaderPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SfCircularGauge)
            {
                SfCircularGauge circularGauge = obj as SfCircularGauge;
                if (circularGauge.GaugeHeader != null && (circularGauge.GaugeHeader is FrameworkElement))
                {
                    if (circularGauge != null && circularGauge.GaugeHeaderPosition.X < (circularGauge.MainScale.EndPoint.X - (circularGauge.GaugeHeader as FrameworkElement).ActualWidth))
                    {
                        circularGauge.GaugeHeaderMargin = new Thickness(circularGauge.GaugeHeaderPosition.X, circularGauge.GaugeHeaderPosition.Y, 0, 0);
                    }
                }
                else
                {
                    circularGauge.GaugeHeaderMargin = new Thickness(circularGauge.GaugeHeaderPosition.X, circularGauge.GaugeHeaderPosition.Y, 0, 0);
                }
            }
        }



        /// <summary>
        /// Gets or sets the collection of subscales to be added in the same gauge.
        /// </summary>
        /// <remarks>
        /// SubScale is used to add multiple scale in the same gauge.
        /// </remarks>
        /// <value>
        /// ObservableCollection
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///             this.InitializeComponent();
        ///             SfCircularGauge gauge = new SfCircularGauge();
        ///             CircularScale subScale1 = new CircularScale();
        ///             CircularScale subScale2 = new CircularScale();
        ///             subScale1.Height = 300;
        ///             subScale1.Width = 300;
        ///             subScale1.StartAngle = 180;
        ///             subScale1.SweepAngle = 180;
        ///              subScale1.SweepDirection = SweepDirection.Clockwise;
        ///              subScale1.StartValue = 0;
        ///              subScale1.EndValue = 100;
        ///              subScale1.Height = 200;
        ///              subScale1.Width = 200;
        ///              subScale2.StartAngle = 270;
        ///              subScale2.SweepAngle = 180;
        ///              subScale2.SweepDirection = SweepDirection.Clockwise;
        ///              subScale2.StartValue = 0;
        ///              subScale2.EndValue = 200;
        ///              gauge.SubScales.Add(subScale1);
        ///              gauge.SubScales.Add(subScale2);
        ///             
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public ObservableCollection<CircularScale> SubScales
        {
            get { return (ObservableCollection<CircularScale>)GetValue(SubScalesProperty); }
            set { SetValue(SubScalesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubScales.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubScalesProperty =

  DependencyProperty.Register("SubScales", typeof(ObservableCollection<CircularScale>), typeof(SfCircularGauge), new PropertyMetadata(null));

        
        
        /// <summary>
        /// <para>Gets or sets the MainScale that specifies basic look and feel of Circular Gauge.</para>
        /// </summary>
        /// <remarks>
        /// The MainScale contains Scale that integrates labels, tick marks and Rim to
        /// specify basic look and feel of Circular Gauge. It defines start angle, sweep
        /// direction and sweep angle and overall minimum and maximum values, as well as
        /// frequency of labels and tick marks to be drawn. It can has multiple Ranges. A
        /// range is a visual element which begins and ends at specified values within a
        /// scale. It can has one or more pointers to point out the values in the scale. It
        /// also has GaugeHeader that can be used to set a unique header for the Circular
        /// Gauge
        /// </remarks>
        /// <value>
        /// CircularScale
        /// </value>
        [ClassReference(IsReviewed = false)]
        public CircularScale MainScale
        {
            get { return (CircularScale)GetValue(MainScaleProperty); }
            set { SetValue(MainScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scales.  This enables animation, styling, binding, etc...

        public static readonly DependencyProperty MainScaleProperty =
            DependencyProperty.Register("MainScale", typeof(CircularScale), typeof(SfCircularGauge), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the style to the CircularScale in the SfCircularGauge.
        /// </summary>
        /// <value>
        /// Style
        /// </value>
        public Style CircularScaleStyle
        {
            get { return (Style)GetValue(CircularScaleStyleProperty); }
            set { SetValue(CircularScaleStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircularScaleStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircularScaleStyleProperty =
            DependencyProperty.Register("CircularScaleStyle", typeof(Style), typeof(SfCircularGauge), new PropertyMetadata(null));


        private static void OnCircularScaleStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is SfCircularGauge && args.NewValue is Style)
            {
                SfCircularGauge circulaGaugeInstance = (SfCircularGauge)obj;
                circulaGaugeInstance.MainScale.Style = (Style)args.NewValue;
            }
        }
        /// <summary>
        /// Gets or sets the outer margin of the SfCircularGauge.
        /// </summary>
        /// <remarks>
        /// SpacingMargin property is used to place the SfCircularGauge in the center of the
        /// container. And also it decides the size of the SfCircularGauge. Value of the property should reside between 0.1 and 1.0.
        /// The default value is 1.0.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double SpacingMargin
        {
            get { return (double)GetValue(SpacingMarginProperty); }
            set { SetValue(SpacingMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpacingMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpacingMarginProperty =
            DependencyProperty.Register("SpacingMargin", typeof(double), typeof(SfCircularGauge), new PropertyMetadata(1d));

        
       
    }
}
