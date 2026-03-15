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
using System.Collections;
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
    ///  CircularRange is a class that denotes various qualitative ranges for a
    /// CircularScale.
    /// </summary>
    public class CircularRange : DependencyObject
    {

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.CircularRange"/> class.
        /// </summary>
        public CircularRange()
        {
            //this.DefaultStyleKey = typeof(CircularRange);
            //this.Loaded += CircularRange_Loaded;
            //this.SizeChanged += CircularRange_SizeChanged;
            RangeSeg = new PathSegmentCollection();
            ArcSegment arcseg = new ArcSegment() { RotationAngle = 0 };
            Binding sweepbinding = new Binding() { Path = new PropertyPath("SweepDirection"), Source = this, Mode = BindingMode.TwoWay };
            Binding islargearcbinding = new Binding() { Path = new PropertyPath("IsLargeArc"), Source = this, Mode = BindingMode.TwoWay };
            Binding halfsizebinding = new Binding() { Path = new PropertyPath("Size"), Source = this, Mode = BindingMode.TwoWay };
            Binding rangeendptbinding = new Binding() { Path = new PropertyPath("EndPoint"), Source = this, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(arcseg, ArcSegment.SweepDirectionProperty, sweepbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.IsLargeArcProperty, islargearcbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.SizeProperty, halfsizebinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.PointProperty, rangeendptbinding);
            RangeSeg.Add(arcseg);
        }

        //void CircularRange_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    DrawArc();
        //}

        //void CircularRange_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DrawArc();
        //}



        public PathSegmentCollection RangeSeg
        {
            get { return (PathSegmentCollection)GetValue(RangeSegProperty); }
            set { SetValue(RangeSegProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSeg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeSegProperty =
            DependencyProperty.Register("RangeSeg", typeof(PathSegmentCollection), typeof(CircularRange), new PropertyMetadata(new PathSegmentCollection()));

        


        internal double RangeSize
        {
            get { return (double)GetValue(RangeSizeProperty); }
            set { SetValue(RangeSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangeSizeProperty =
            DependencyProperty.Register("RangeSize", typeof(double), typeof(CircularRange), new PropertyMetadata(4d));

        

        internal Size ParentSize
        {
            get { return (Size)GetValue(ParentSizeProperty); }
            set { SetValue(ParentSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentSizeProperty =
            DependencyProperty.Register("ParentSize", typeof(Size), typeof(CircularRange), new PropertyMetadata(new Size(0,0), OnParentPropertyChanged));

            
        internal  double ParentStartValue
        {
            get { return (double)GetValue(ParentStartValueProperty); }
            set { SetValue(ParentStartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentStartValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentStartValueProperty =
            DependencyProperty.Register("ParentStartValue", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));




        internal double ParentEndValue
        {
            get { return (double)GetValue(ParentEndValueProperty); }
            set { SetValue(ParentEndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentEndValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentEndValueProperty =
            DependencyProperty.Register("ParentEndValue", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));




        internal double ParentStartAngle
        {
            get { return (double)GetValue(ParentStartAngleProperty); }
            set { SetValue(ParentStartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentStartAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentStartAngleProperty =
            DependencyProperty.Register("ParentStartAngle", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));




        internal double ParentSweepAngle
        {
            get { return (double)GetValue(ParentSweepAngleProperty); }
            set { SetValue(ParentSweepAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentSweepAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentSweepAngleProperty =
            DependencyProperty.Register("ParentSweepAngle", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));

        internal CircularScale ParentScale
        {
            get { return (CircularScale)GetValue(ParentScaleProperty); }
            set { SetValue(ParentScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentScale.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentScaleProperty =
            DependencyProperty.Register("ParentScale", typeof(CircularScale), typeof(CircularRange), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the Value of the CircularRange's StartValue.
        /// </summary>
        /// <remarks>
        /// A range is a visual element which begins and ends at specified values within a scale. 
        /// These start and end values are set by the StartValue and EndValue properties of Range
        /// </remarks>
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
        ///            CircularRange range1 = new CircularRange();
        ///            range1.StartValue = 0;
        ///            range1.EndValue = 40;
        ///            range1.Stroke = new SolidColorBrush(Colors.Green);
        ///            range1.StrokeThickness = 10;            
        ///            gauge.MainScale.Ranges.Add(range1);            
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));


        /// <summary>
        /// Gets or sets the Value of the CircularRange's EndValue.
        /// </summary>
        /// <remarks>
        /// A range is a visual element which begins and ends at specified values within a scale. 
        /// These start and end values are set by the StartValue and EndValue properties of Range
        /// </remarks>
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
        ///            CircularRange range1 = new CircularRange();
        ///            range1.StartValue = 0;
        ///            range1.EndValue = 40;
        ///            range1.Stroke = new SolidColorBrush(Colors.Green);
        ///            range1.StrokeThickness = 10;            
        ///            gauge.MainScale.Ranges.Add(range1);            
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));



        internal double InternalEndValue
        {
            get { return (double)GetValue(InternalEndValueProperty); }
            set { SetValue(InternalEndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalEndValueProperty =
            DependencyProperty.Register("InternalEndValue", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnParentPropertyChanged));




        internal double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnStartAngleChanged));

        private static void OnStartAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularRange)
            {
                CircularRange circularScale = obj as CircularRange;
                circularScale.DrawArc();
            }
        }

        /// <summary>
        /// Gets value of the CircularRange's EndAngle.
        /// </summary>
        /// <remarks>
        /// EndAngle is a read only property used to get the value of the range's EndAngle.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            internal set { SetValue(EndAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(CircularRange), new PropertyMetadata(0d, OnEndAngleChanged));

        private static void OnEndAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularRange)
            {
                CircularRange circularScale = obj as CircularRange;
                circularScale.DrawArc();
            }
        }

        /// <summary>
        /// Gets Pointvalue of the CircularRange's StartPoint.
        /// </summary>
        /// <remarks>
        /// StartPoint is a read only property used to get the value of the StartPoint.
        /// </remarks>
        /// <value>
        /// Point
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            internal set { SetValue(StartPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(CircularRange), new PropertyMetadata(null));


        /// <summary>
        /// Gets pointvalue of the CircularRange's EndPoint.
        /// </summary>
        /// <remarks>
        /// EndPoint is a read only property used to get the value of the EndPoint.
        /// </remarks>
        /// <value>
        /// Point
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            internal set { SetValue(EndPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(CircularRange), new PropertyMetadata(null));


        /// <summary>
        /// Gets Size of the CircularRange which helps to position the CircularRange.
        /// </summary>
        /// <remarks>
        /// Size is a read only property used to get the value of the Size.
        /// </remarks>
        /// <value>
        /// Size
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            internal set { SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(CircularRange), new PropertyMetadata(null));



        internal bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLargeArc.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(CircularRange), new PropertyMetadata(false));



        internal SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweeepDirection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(CircularRange), new PropertyMetadata(SweepDirection.Clockwise,OnParentPropertyChanged));


        /// <summary>
        /// Gets or sets the brush that describes the Stroke value of the CircularRange.
        /// </summary>
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
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularRange range1 = new CircularRange();
        ///            range1.StartValue = 0;
        ///            range1.EndValue = 40;
        ///            range1.Stroke = new SolidColorBrush(Colors.Green);
        ///            range1.StrokeThickness = 10;            
        ///            gauge.MainScale.Ranges.Add(range1);            
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(CircularRange), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnStrokeChanged));

        private static void OnStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularRange)
            {
                CircularRange circularRange = obj as CircularRange;
                if (circularRange.ParentScale != null)
                {
                    if(circularRange.ParentScale.BindRangeStrokeToLabels)
                        circularRange.ParentScale.CreateLabels();
                    if(circularRange.ParentScale.BindRangeStrokeToTicks)
                        circularRange.ParentScale.CreateTicks();
                }
            }
        }

        /// <summary>
        /// Gets or sets thickness of the RangeStroke.
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
        ///            CircularRange range1 = new CircularRange();
        ///            range1.StartValue = 0;
        ///            range1.EndValue = 40;
        ///            range1.Stroke = new SolidColorBrush(Colors.Green);
        ///            range1.StrokeThickness = 10;            
        ///            gauge.MainScale.Ranges.Add(range1);            
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularRange), new PropertyMetadata(4d, new PropertyChangedCallback(OnStrokeThickness)));

        private static void OnStrokeThickness(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as CircularRange;
            if (instance != null)
                instance.RangeSize = (double)e.NewValue;
        }

        private static void OnParentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularRange circularRange = obj as CircularRange;
            if (circularRange != null)
            {
                circularRange.StartValue = double.IsNaN(circularRange.StartValue) ? 0 : circularRange.StartValue;
                circularRange.InternalEndValue = double.IsNaN(circularRange.EndValue) ? 0 : circularRange.EndValue;
                if (circularRange.StartValue > circularRange.EndValue)
                {
                    circularRange.StartAngle = circularRange.ValueToAngle(circularRange.InternalEndValue);
                    circularRange.EndAngle = circularRange.ValueToAngle(circularRange.StartValue);
                }
                else
                {
                    circularRange.StartAngle = circularRange.ValueToAngle(circularRange.StartValue);
                    circularRange.EndAngle = circularRange.ValueToAngle(circularRange.InternalEndValue);
                }
                circularRange.DrawArc();
                if(circularRange.ParentScale != null)
                {                   
                    //if (circularRange.ParentScale.BindRangeStrokeToTicks)
                    {
                        circularRange.ParentScale.CreateTicks();
                        if (circularRange.ParentScale.Tick_panel != null)
                        {
                            circularRange.ParentScale.Tick_panel.InvalidateMeasure();
                        }
                    }
                    //if (circularRange.ParentScale.BindRangeStrokeToLabels)
                    {
                        circularRange.ParentScale.CreateLabels();
                        if (circularRange.ParentScale.cir_panel != null)
                        {
                            circularRange.ParentScale.cir_panel.InvalidateMeasure();
                        }
                    }
                }
            }
        }

        private double ValueToAngle(double Value)
        {
            double angle;
            if (Value < ParentStartValue)
            {
                Value = ParentStartValue;
            }
            if (Value > ParentEndValue)
            {
                Value = ParentEndValue;
            }
            if (this.SweepDirection == SweepDirection.Clockwise)
            {
                angle = ParentStartAngle + ((ParentSweepAngle / Math.Abs(ParentEndValue - ParentStartValue)) * Math.Abs(ParentStartValue - Value));
            }
            else
            {
                angle = ParentStartAngle - ((ParentSweepAngle / Math.Abs(ParentEndValue - ParentStartValue)) * Math.Abs(ParentStartValue - Value));
            }
            return angle;
        }
        private void DrawArc()
        {
            double endX;
            double endY;
            double startX;
            double startY;
            double radiusx = ParentSize.Width / 2;
            double radiusy = ParentSize.Height / 2;
            double endAngle = EndAngle;
            if (this.SweepDirection == SweepDirection.Clockwise )
            {
                if ((endAngle % 360) == StartAngle && StartAngle != endAngle)
                {
                    endAngle = (StartAngle + 359.99) % 360;
                }
            }
            else
            {
                if ((endAngle + 360) == StartAngle && StartAngle != endAngle)
                {
                    endAngle = (StartAngle - 359.99) % 360;
                }
            }
            endX = radiusx + Math.Cos(DegToRad(endAngle)) * radiusx;
            endY = radiusy + Math.Sin(DegToRad(endAngle)) * radiusy;

            startX = radiusx + Math.Cos(DegToRad(StartAngle)) * radiusx;
            startY = radiusy + Math.Sin(DegToRad(StartAngle)) * radiusy;

            StartPoint = new Point(startX, startY);
            Size = new Size(radiusx, radiusy);
            EndPoint = new Point(endX, endY);
            IsLargeArc = Math.Abs(EndAngle - StartAngle) > 180;
        }



        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }
        
    }

    /// <summary>
    ///  It is a collection that contains a set of Ranges that can be used to specifiy
    /// various ranges of CircularScale .
    /// </summary>
    public class CircularRangeCollection : ObservableCollection<CircularRange>
    {
        
    }
#if WINRT
    public class MyRanges : Control
    {
        public MyRanges()
        {
            this.DefaultStyleKey = typeof(MyRanges);
        }

    }

    public class MyGrid : Grid
    {

        public MyGrid()
        {
            this.SetRanges();
        }
        #region Dependency Properties



        internal object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(MyGrid), new PropertyMetadata(null,OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyGrid)
            {
                MyGrid obj = (MyGrid)d;
                obj.SetRanges();
            }
        }
        
        #endregion
        private void SetRanges()
        {
            if (ItemsSource != null)
            {
                foreach (var item in (IEnumerable)ItemsSource)
                {
                    MyRanges child1 = new MyRanges() { DataContext = item };
                    UIElement child = child1;
                    this.Children.Add(child);
                } 
            }            
        }
    }
#endif
}
