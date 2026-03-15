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
    /// It helps the user to display ticks that helps to identify the gauge’s data value
    /// by marking the gauge scale into regular increments
    /// </summary>
    public class CircularScaleTick : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.CircularScaleTick"/> class.
        /// </summary>
        public CircularScaleTick()
        {
            //this.DefaultStyleKey = typeof(CircularScaleTick);
        }

        #region Length

        /// <summary>
        /// Gets or sets value that decides the length of the Ticks.
        /// </summary>
        /// <value>
        /// double
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
        ///            CircularScale scale = new CircularScale();
        ///            scale.TickLength = 10;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Length.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(CircularScaleTick), new PropertyMetadata(10d, OnLengthChanged));

        private static void OnLengthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularScaleTick tick = obj as CircularScaleTick;
            if (obj != null)
            {
                tick.HalfLength = ((double)args.NewValue / 2);
            }
        }

        #endregion

        #region HalfLength

        /// <summary>
        /// Gets HalfLength of the CircularScaleTick used to position the Ticks.
        /// </summary>
        /// <remarks>
        /// HalfLength is a read only property used to get the value of the HalfLength.
        /// </remarks>
        /// <value>
        /// Point
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double HalfLength
        {
            get { return (double)GetValue(HalfLengthProperty); }
            internal set { SetValue(HalfLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HalfLengthProperty =
            DependencyProperty.Register("HalfLength", typeof(double), typeof(CircularScaleTick), new PropertyMetadata(5d));

        #endregion

        #region TickStroke

        /// <summary>
        /// Gets or sets the brush that describes the TickStroke of the CircularScaleTick.
        /// </summary>
        /// <remarks>
        /// TickStroke property will remains effective until the value of the
        /// BindRangeStrokeToTicks property is false.
        /// </remarks>
        /// <value>
        /// Brush
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
        ///             CircularScale scale = new CircularScale();
        ///             scale.TickStroke = new SolidColorBrush(Colors.Red);
        ///             gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush TickStroke
        {
            get { return (Brush)GetValue(TickStrokeProperty); }
            set { SetValue(TickStrokeProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for TickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeProperty =
            DependencyProperty.Register("TickStroke", typeof(Brush), typeof(CircularScaleTick), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        #endregion

        #region TickStrokeThickness

        /// <summary>
        /// Gets or sets the thickness of the TickStroke of the CircularScaleTick.
        /// </summary>
        /// <value>
        /// double
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
        ///            CircularScale scale = new CircularScale();
        ///            scale.TickStrokeThickness =3;
        ///            gauge.MainScale = scale;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double TickStrokeThickness
        {
            get { return (double)GetValue(TickStrokeThicknessProperty); }
            set { SetValue(TickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickStrokeThicknessProperty =
            DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(CircularScaleTick), new PropertyMetadata(1d));

        #endregion

        #region Angle

        /// <summary>
        /// Gets or sets the Angle of the CircularScaleTick.
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
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularScaleTick), new PropertyMetadata(0d));
        #endregion

        internal TickType Ticktype { get; set; }

    }

    /// <summary>
    /// It is a collection that contains a set of Ticks that helps to identify the gauge’s data value
    /// by marking the gauge scale into regular increments
    /// </summary>
    public class CircularScaleTickCollection : ObservableCollection<CircularScaleTick>
    {
        
    }
}
