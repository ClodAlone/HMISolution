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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Diagnostics;

namespace Syncfusion.Windows.Diagram
{

    public class Ruler : Control
    {
        ResourceDictionary rs;

        internal ScrollViewer sv;

        internal DiagramView dv;

        internal StackPanel NegativeScale;

        internal StackPanel PositiveScale;

        internal FrameworkElement cpforMargin;

        internal Line Marker;

        internal double StartValue
        {
            get
            {
                return (double)GetValue(StartValueProperty);
            }

            set
            {
                SetValue(StartValueProperty, value);
            }
        }

        internal double IntervalValue
        {
            get
            {
                return (double)GetValue(IntervalValueProperty);
            }

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                SetValue(IntervalValueProperty, value);
            }
        }

        internal double PxStartValue
        {
            get
            {
                return (double)GetValue(PxStartValueProperty);
            }

            set
            {
                SetValue(PxStartValueProperty, value);
            }
        }

        internal double PxIntervalValue
        {
            get
            {
                return (double)GetValue(PxIntervalValueProperty);
            }

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                SetValue(PxIntervalValueProperty, value);
            }
        }

        internal double MUIntervalValue
        {
            get
            {
                double ret = PxIntervalValue;
                ret = MeasureUnitsConverter.FromPixels(ret, this.MeasurementUnits);
                return ret;
            }
        }

        internal double MURoundedIntervalValue
        {
            get
            {
                double ret = MUIntervalValue;
                int xten = 0;
                while (ret < 10)
                {
                    xten += 1;
                    ret *= 10;
                }
                if (xten < 3)
                {
                    ret = Math.Round(ret / 5, 0) * 5;
                }
                else
                {
                    ret = Math.Ceiling(ret / 20) * 20;
                }
                if (xten > 0)
                {
                    ret = ret / Math.Pow(10, xten);
                }
                ret = Math.Round(ret, 6);
                return ret;
            }
        }

        internal double PxRoundedIntervalValue
        {
            get
            {
                double ret = MURoundedIntervalValue;
                ret = MeasureUnitsConverter.ToPixels(ret, this.MeasurementUnits);
                return ret;
            }
        }

        internal double RulerWidth
        {
            get
            {
                return (double)GetValue(RulerWidthProperty);
            }
            set
            {
                SetValue(RulerWidthProperty, value);
            }
        }

        internal double OffsetX
        {
            get
            {
                return (double)GetValue(OffsetXProperty);
            }
            set
            {
                SetValue(OffsetXProperty, value);
            }
        }

        internal double OffsetY
        {
            get
            {
                return (double)GetValue(OffsetYProperty);
            }
            set
            {
                SetValue(OffsetYProperty, value);
            }
        }

        internal double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        internal double ScaleMultiples
        {
            get { return Math.Round(Scale, 0); }
        }
                
        internal double ValueRotation
        {
            get
            {
                return (double)GetValue(ValueRotationProperty);
            }
            set
            {
                SetValue(ValueRotationProperty, value);
            }
        }
        
        internal double MajorLength
        {
            get
            {
                return (double)GetValue(MajorLengthProperty);
            }
            set
            {
                SetValue(MajorLengthProperty, value);
            }
        }

        internal double MarkerLength
        {
            get
            {
                return (double)GetValue(MarkerLengthProperty);
            }
            set
            {
                SetValue(MarkerLengthProperty, value);
            }
        }

        internal double MinorLength
        {
            get
            {
                return (double)GetValue(MinorLengthProperty);
            }
            set
            {
                SetValue(MinorLengthProperty, value);
            }
        }

        internal double AlternateMajorLength
        {
            get
            {
                return (double)GetValue(AlternateMajorLengthProperty);
            }
            set
            {
                SetValue(AlternateMajorLengthProperty, value);
            }
        }

        internal bool MarkerUp
        {
            get
            {
                return (bool)GetValue(MarkerUpProperty);
            }
            set
            {
                SetValue(MarkerUpProperty, value);
            }
        }

        internal event EventHandler MarkerUpChanged;

        internal static readonly DependencyProperty IntervalValueProperty = DependencyProperty.Register("IntervalValue", typeof(double), typeof(Ruler), new PropertyMetadata(50d));

        internal static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(double), typeof(Ruler), new PropertyMetadata(-501d, new PropertyChangedCallback(OnStartValuePropertyChanged)));


        /// <summary>
        /// Gets or sets the Interval dependency property.
        /// </summary>
        internal static readonly DependencyProperty InvalidateRulerSegmentValueProperty = DependencyProperty.Register("InvalidateRulerSegmentValue", typeof(bool), typeof(Ruler), new PropertyMetadata(true));

        internal static readonly DependencyProperty PxIntervalValueProperty = DependencyProperty.Register("PxIntervalValue", typeof(double), typeof(Ruler), new PropertyMetadata(50.1112d, new PropertyChangedCallback(OnPxIntervalValuePropertyChanged)));

        internal static readonly DependencyProperty PxStartValueProperty = DependencyProperty.Register("PxStartValue", typeof(double), typeof(Ruler), new PropertyMetadata(0d, new PropertyChangedCallback(OnPxStartValuePropertyChanged)));

        internal static readonly DependencyProperty RulerWidthProperty = DependencyProperty.Register("RulerWidth", typeof(double), typeof(Ruler), new PropertyMetadata(0d, new PropertyChangedCallback(OnRulerWidthPropertyChanged)));

        internal static readonly DependencyProperty ValueRotationProperty = DependencyProperty.Register("ValueRotation", typeof(double), typeof(Ruler), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueInvertedPropertyChanged)));

        internal static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(Ruler), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetXChanged)));

        internal static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(Ruler), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetYChanged)));

        internal static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(Ruler), new PropertyMetadata(1d, new PropertyChangedCallback(OnScaleChanged)));        

        internal static readonly DependencyProperty MajorLengthProperty = DependencyProperty.Register("MajorLength", typeof(double), typeof(Ruler), new PropertyMetadata(20d, new PropertyChangedCallback(OnMajorLengthPropertyChanged)));

        internal static readonly DependencyProperty MarkerLengthProperty = DependencyProperty.Register("MarkerLength", typeof(double), typeof(Ruler), new PropertyMetadata(double.NaN));

        internal static readonly DependencyProperty MinorLengthProperty = DependencyProperty.Register("MinorLength", typeof(double), typeof(Ruler), new PropertyMetadata(10d, new PropertyChangedCallback(OnMinorLengthPropertyChanged)));

        internal static readonly DependencyProperty AlternateMajorLengthProperty = DependencyProperty.Register("AlternateMajorLength", typeof(double), typeof(Ruler), new PropertyMetadata(75d, new PropertyChangedCallback(OnAlternateMajorLengthPropertyChanged)));

        internal static readonly DependencyProperty MarkerUpProperty = DependencyProperty.Register("MarkerUp", typeof(bool), typeof(Ruler), new PropertyMetadata(true, new PropertyChangedCallback(OnMarkerUpPropertyChanged)));

        internal static void OnPxIntervalValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if (r != null)
            {
                bool trig = (bool)r.GetValue(Ruler.InvalidateRulerSegmentValueProperty);
                r.SetValue(Ruler.InvalidateRulerSegmentValueProperty, !trig);
            }
            if (e.NewValue != null && r != null)
            {
                UpdateRuler(r, (double)r.RulerWidth);
            }
            if (r.PxIntervalValueChanged != null)
            {
                ValueChangedEventArgs EventArgs = new ValueChangedEventArgs();
                EventArgs.NewValue = e.NewValue;
                EventArgs.OldValue = e.OldValue;
                r.PxIntervalValueChanged.Invoke(r, EventArgs);
            }
        }

        internal event EventHandler PxIntervalValueChanged;

        internal event EventHandler PxMinorLineCountPerIntervalChanged;

        internal static void OnStartValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if (e.NewValue != null && r != null)
            {
                UpdateRuler(r, (double)r.RulerWidth);
               
            }
        }

        internal static void OnPxStartValuePropertyChanged(DependencyObject d1, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d1 as Ruler;
            double d = (double)Math.Floor(r.Scale);
            if (d > 2)
            {
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
            }
            else if (d < 1)
            {
                d = Math.Floor(r.Scale * 10);
                d = Math.Floor(Math.Log(d, 2));
                d = Math.Pow(2, d);
                d /= 10;
            }
            foreach (RulerSegment rule in r.NegativeScale.Children)
            {
                rule.Width = r.PxRoundedIntervalValue * r.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
            }
            foreach (RulerSegment rule in r.PositiveScale.Children)
            {
                rule.Width = r.PxRoundedIntervalValue * r.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
            }
            if (e.NewValue != null && r != null)
            {
                UpdateRuler(r, (double)r.RulerWidth);
            }
        }

        internal static void OnRulerWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if (e.NewValue != null && r != null)
            {
                UpdateRuler(r, (double)e.NewValue);
            }
        }

        private static void UpdateRuler(Ruler r, double rulWidth)
        {
            
            if (!(double.IsInfinity(rulWidth) || double.IsNaN(rulWidth)))
            {
                double negCount, posCount;
                if (r.PxStartValue < 0)
                {
                    negCount = Math.Ceiling((-r.PxStartValue) / r.PxRoundedIntervalValue);
                    double multi = Math.Round(r.Scale, 0);
                    if (multi > 1)
                    {
                        negCount *= multi;
                    }
                }
                else
                {
                    negCount = 0;
                }
                posCount = Math.Ceiling((r.RulerWidth + r.PxStartValue) / r.PxRoundedIntervalValue);
                {
                    double multi = Math.Round(r.Scale, 0);
                    if (multi > 1)
                    {
                        posCount *= multi;
                    }
                }

                if (r.NegativeScale != null)
                {
                    var ch = from RulerSegment seg in r.NegativeScale.Children where !seg.Width.Equals(double.NaN) select seg;
                    foreach (RulerSegment ui in ch)
                    {
                        ui.Width = double.NaN;
                    }
                    UpdateNegativeScale(r, negCount);
                    if (r.NegativeScale.Children.Count > 0 && (-r.PxStartValue) % r.PxRoundedIntervalValue > 0)
                    {
                        int last = r.NegativeScale.Children.Count;
                        (r.NegativeScale.Children[last - 1] as RulerSegment).Width = (-r.PxStartValue) % r.PxRoundedIntervalValue;
                    }
                }
                if (r.PositiveScale != null)
                {
                    UpdatePositiveScale(r, posCount);
                }
            }
        }

        private static void UpdatePositiveScale(Ruler r, double posCount)
        {
            if (r.PositiveScale.Children.Count < posCount)
            {
                for (int i = r.PositiveScale.Children.Count; i < posCount; i++)
                {
                    RulerSegment rs = new RulerSegment();
                    r.PositiveScale.Children.Add(rs);
                    rs.Value = i;
                }
            }
            else
            {
                for (int i = r.PositiveScale.Children.Count; i > posCount; i--)
                {
                    //r.PositiveScale.Children.RemoveAt(i - 1);
                }
            }
        }

        private static void UpdateNegativeScale(Ruler r, double negCount)
        {
            if (r.NegativeScale.Children.Count < negCount)
            {
                for (int i = r.NegativeScale.Children.Count; i < negCount; i++)
                {
                    RulerSegment rs = new RulerSegment();
                    rs.Loaded += new RoutedEventHandler(rs_Loaded);
                    r.NegativeScale.Children.Add(rs);
                    rs.Value = -i - 1;
                }
            }
            else
            {
                for (int i = r.NegativeScale.Children.Count; i > negCount; i--)
                {
                    r.NegativeScale.Children.RemoveAt(i - 1);
                }
            }
        }

        static void rs_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as RulerSegment).ParentRuler != null)
            {
                double d = (double)Math.Floor((sender as RulerSegment).ParentRuler.Scale);
                if (d > 2)
                {
                    d = Math.Floor(Math.Log(d, 2));
                    d = Math.Pow(2, d);
                }
                else if (d < 1)
                {
                    d = Math.Floor((sender as RulerSegment).ParentRuler.Scale * 10);
                    d = Math.Floor(Math.Log(d, 2));
                    d = Math.Pow(2, d);
                    d /= 10;
                }
                (sender as RulerSegment).minorLineGrid.Width = (sender as RulerSegment).ParentRuler.PxRoundedIntervalValue * (sender as RulerSegment).ParentRuler.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
                //foreach (RulerSegment rule in r.NegativeScale.Children)
                //{
                //    rule.Width = r.PxRoundedIntervalValue * r.Scale / Math.Max(d, 0.1); //( 1 + ( ParentRuler.Scale % 1));
                //}
            }
            (sender as RulerSegment).Loaded -= new RoutedEventHandler(rs_Loaded);
        }

        internal static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler rul = (d as Ruler);
            if (rul.Orientation == Orientation.Horizontal)
            {
                
                double mar = (double)e.NewValue;
               
            }
        }

        internal static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler rul = (d as Ruler);
            if (rul.Orientation == Orientation.Vertical)
            {
              
            }
            else
            {
 
            }

        }

        internal static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler rul = d as Ruler;
            rul.PxIntervalValue += 0.00000001;// 50 * (1 + ((rul.Scale) % 1));// ((rul.Scale % 2) < 1 ? 1 : (rul.Scale % 2));
            UpdateRuler(rul, rul.RulerWidth);
        }
        
        internal static void OnValueInvertedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        
        internal static void OnMajorLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void OnMinorLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void OnAlternateMajorLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void OnMarkerUpPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler rul = d as Ruler;
            ValueChangedEventArgs evtArgs = new ValueChangedEventArgs();
            evtArgs.OldValue = e.OldValue;
            evtArgs.NewValue = e.NewValue;
            if (rul.MarkerUpChanged != null)
            {
                rul.MarkerUpChanged.Invoke(rul, evtArgs);
            }
        }

        internal static void OnMinorLineCountPerIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if (r.PxMinorLineCountPerIntervalChanged != null)
            {
                ValueChangedEventArgs EventArgs = new ValueChangedEventArgs();
                EventArgs.NewValue = e.NewValue;
                EventArgs.OldValue = e.OldValue;
                r.PxMinorLineCountPerIntervalChanged.Invoke(r, EventArgs);
            }
        }

        internal class ValueChangedEventArgs : EventArgs
        {
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }


        //......................

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the MajorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorLinesStrokeProperty = DependencyProperty.Register("MajorLinesStroke", typeof(Brush), typeof(Ruler), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the MinorLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorLinesStrokeProperty = DependencyProperty.Register("MinorLinesStroke", typeof(Brush), typeof(Ruler), new PropertyMetadata((new SolidColorBrush(Colors.Black))));

        /// <summary>
        /// Gets or sets the MarkerBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(Ruler), new PropertyMetadata((new SolidColorBrush(Colors.Red))));

        /// <summary>
        /// Gets or sets the ShowMarker dependency property.
        /// </summary>
        internal static readonly DependencyProperty ShowMarkerProperty = DependencyProperty.Register("ShowMarker", typeof(bool), typeof(Ruler), new PropertyMetadata(true, new PropertyChangedCallback(OnShowMarkerChanged)));

        /// <summary>
        /// Gets or sets the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Ruler), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets the MinorLineCountPerInterval dependency property.
        /// </summary>
        internal static readonly DependencyProperty MinorLineCountPerIntervalProperty = DependencyProperty.Register("MinorLinesCountPerInterval", typeof(int), typeof(Ruler), new PropertyMetadata(4, new PropertyChangedCallback(OnMinorLineCountPerIntervalPropertyChanged)));

        /// <summary>
        /// Gets or sets the MarkerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerPositionProperty = DependencyProperty.Register("MarkerPosition", typeof(double), typeof(Ruler), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the LabelPosition dependency property.
        /// </summary>
        internal static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(double), typeof(Ruler), new PropertyMetadata(100d, new PropertyChangedCallback(OnLabelPositionChanged)));

        /// <summary>
        /// Gets or sets the LabelFontColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontColorProperty = DependencyProperty.Register("LabelFontColor", typeof(Brush), typeof(Ruler), new PropertyMetadata((new SolidColorBrush(Colors.Black)), new PropertyChangedCallback(OnLabelFontColorChanged)));

        public static readonly DependencyProperty MajorLinesThicknessProperty = DependencyProperty.Register("MajorLinesThickness", typeof(double), typeof(Ruler), new PropertyMetadata(2d, new PropertyChangedCallback(MajorLinesThicknessChanged)));

        internal static readonly DependencyProperty MarkerThicknessProperty = DependencyProperty.Register("MarkerThickness", typeof(double), typeof(Ruler), new PropertyMetadata(1d, new PropertyChangedCallback(MarkerThicknessChanged)));

        internal static readonly DependencyProperty MinorLinesThicknessProperty = DependencyProperty.Register("MinorLinesThickness", typeof(double), typeof(Ruler), new PropertyMetadata(2d, new PropertyChangedCallback(MinorLinesThicknessChanged)));

        internal static readonly DependencyProperty IntermediateLinesThicknessProperty = DependencyProperty.Register("IntermediateLinesThickness", typeof(double), typeof(Ruler), new PropertyMetadata(1d, new PropertyChangedCallback(OnIntermediateLinesThicknessChanged)));

        internal static readonly DependencyProperty RulerThicknessProperty = DependencyProperty.Register("RulerThickness", typeof(double), typeof(Ruler), new PropertyMetadata(1d, new PropertyChangedCallback(RulerThicknessChanged)));

        /// <summary>
        /// Specifies the MeasurementUnit Dependency property.
        /// </summary>
        internal static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(Ruler), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnMeasurementUnitChanged)));

        #endregion

        #region DP Getter and Setters

        /// <summary>
        /// Gets or sets the LabelFontColor.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" 
        ///                                Background="WhiteSmoke"&gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.Ruler&gt;
        ///            &lt;syncfusion:Ruler Name="Ruler" 
        ///                                          LabelFontColor="Black" /&gt;
        ///          &lt;/syncfusion:DiagramView.Ruler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        Ruler hruler = new Ruler();
        ///        hruler.LabelFontColor = Brushes.Black;
        ///        View.Ruler = hruler;
        /// </code>
        /// </example>
        public Brush LabelFontColor
        {
            get
            {
                return (Brush)GetValue(LabelFontColorProperty);
            }

            set
            {
                SetValue(LabelFontColorProperty, value);
            }
        }

        public double MajorLinesThickness
        {
            get
            {
                return (double)GetValue(MajorLinesThicknessProperty);
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                SetValue(MajorLinesThicknessProperty, value);
            }
        }

        public double MinorLinesThickness
        {
            get
            {
                return (double)GetValue(MinorLinesThicknessProperty);
            }

            set
            {
                //value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                SetValue(MinorLinesThicknessProperty, value);
            }
        }

        internal double IntermediateLinesThickness
        {
            get
            {
                return (double)GetValue(IntermediateLinesThicknessProperty);
            }

            set
            {
                value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                SetValue(IntermediateLinesThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LabelPosition.
        /// </summary>
        internal double LabelPosition
        {
            get
            {
                return (double)GetValue(LabelPositionProperty);
            }

            set
            {
                SetValue(LabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MajorLinesStroke.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.Ruler&gt;
        ///            &lt;syncfusion:Ruler Name="Ruler" 
        ///                                          MajorLinesStroke="Orange" /&gt;
        ///          &lt;/syncfusion:DiagramView.Ruler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        Ruler hruler = new Ruler();
        ///        hruler.MajorLinesStroke = Brushes.Orange;
        ///        View.Ruler = hruler;
        /// </code>
        /// </example>
        public Brush MajorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MajorLinesStrokeProperty);
            }

            set
            {
                SetValue(MajorLinesStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesStroke.
        /// </summary>
        /// <remarks>
        /// Default value is Black.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.Ruler&gt;
        ///            &lt;syncfusion:Ruler Name="Ruler" 
        ///                                          MinorLinesStroke="Orange" /&gt;
        ///          &lt;/syncfusion:DiagramView.Ruler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        Ruler hruler = new Ruler();
        ///        hruler.MinorLinesStroke = Brushes.Orange;
        ///        View.Ruler = hruler;
        /// </code>
        /// </example>
        public Brush MinorLinesStroke
        {
            get
            {
                return (Brush)GetValue(MinorLinesStrokeProperty);
            }

            set
            {
                SetValue(MinorLinesStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MarkerBrush.
        /// </summary>
        /// <remarks>
        /// Default value is Red.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.Ruler&gt;
        ///            &lt;syncfusion:Ruler Name="Ruler" 
        ///                                          MarkerBrush="Red" /&gt;
        ///          &lt;/syncfusion:DiagramView.Ruler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        Ruler hruler = new Ruler();
        ///        hruler.MarkerBrush = Brushes.Red;
        ///        View.Ruler = hruler;
        /// </code>
        /// </example>
        public Brush MarkerBrush
        {
            get
            {
                return (Brush)GetValue(MarkerBrushProperty);
            }

            set
            {
                SetValue(MarkerBrushProperty, value);
            }
        }

        public double MarkerThickness
        {
            get
            {
                return (double)GetValue(MarkerThicknessProperty);
            }

            set
            {
                value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                SetValue(MarkerThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show marker].
        /// </summary>
        /// <value><c>true</c> if [show marker]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Default value is true.
        /// </remarks>
        /// <example>
        /// <code language="XAML">
        /// &lt;Window x:Class="RulersAndUnits.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
        /// Title="Rulers And Units Demo"  WindowState="Maximized" Name="mainwindow" 
        ///  xmlns:local="clr-namespace:Sample" FontWeight="Bold"
        ///  Icon="Images/App.ico" &gt;
        ///   &lt;syncfusion:DiagramControl Grid.Column="1" Name="diagramControl" 
        ///                                IsSymbolPaletteEnabled="True" &gt;
        ///           &lt;syncfusion:DiagramControl.View&gt;
        ///              &lt;syncfusion:DiagramView  IsPageEditable="True" 
        ///                                        Bounds="0,0,12,12"  
        ///                                        Name="diagramView"  &gt;
        ///            &lt;syncfusion:DiagramView.Ruler&gt;
        ///            &lt;syncfusion:Ruler Name="Ruler" 
        ///                                          ShowMarker="False" /&gt;
        ///          &lt;/syncfusion:DiagramView.Ruler&gt;
        ///       &lt;/syncfusion:DiagramView&gt;
        ///    &lt;/syncfusion:DiagramControl.View&gt;
        /// &lt;/syncfusion:DiagramControl&gt;
        /// &lt;/Window&gt;
        ///   </code>
        ///  <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        ///    public DiagramControl Control;
        ///    public DiagramModel Model;
        ///    public DiagramView View;
        ///    public Window1 ()
        ///    {
        ///        InitializeComponent ();
        ///        Control = new DiagramControl ();
        ///        Model = new DiagramModel ();
        ///        View = new DiagramView ();
        ///        Control.Model = Model;
        ///        Control.View = View;
        ///        Ruler hruler = new Ruler();
        ///        hruler.ShowMarker = false;
        ///        View.Ruler = hruler;
        /// </code>
        /// </example>
        public bool ShowMarker
        {
            get
            {
                return (bool)GetValue(ShowMarkerProperty);
            }

            set
            {
                SetValue(ShowMarkerProperty, value);
            }
        }

        /// <summary>
        /// Gets the Orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorLinesPerInterval.
        /// </summary>
        internal int MinorLinesPerInterval
        {
            get
            {
                return (int)GetValue(MinorLineCountPerIntervalProperty);
            }

            set
            {
                SetValue(MinorLineCountPerIntervalProperty, value);
            }
        }

        internal double RulerThickness
        {
            get
            {
                return (double)GetValue(RulerThicknessProperty);
            }

            set
            {
                value = MeasureUnitsConverter.ToPixels(value, DiagramPage.Munits);
                SetValue(RulerThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets the MarkerPosition.
        /// </summary>
        public double MarkerPosition
        {
            get
            {
                return (double)GetValue(MarkerPositionProperty);
            }

            set
            {
                value = value * this.Scale;
                SetValue(MarkerPositionProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the units
        /// </summary>
        internal MeasureUnits MeasurementUnits
        {
            get
            {
                return (MeasureUnits)GetValue(MeasurementUnitsProperty);
            }

            set
            {
                SetValue(MeasurementUnitsProperty, value);
            }
        }

        #endregion

        #region DP Changed
        internal static void OnShowMarkerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if ((bool)e.NewValue)
            {
                if (r.Marker != null)
                {
                    r.Marker.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (r.Marker != null)
                {
                    r.Marker.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Called when [orientation changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [label position changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [label font color changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal static void OnLabelFontColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void MajorLinesThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void MarkerThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void MinorLinesThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void RulerThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        internal static void OnIntermediateLinesThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [measurement unit changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal static void OnMeasurementUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ruler r = d as Ruler;
            if (r != null)
            {
                bool trig = (bool)r.GetValue(Ruler.InvalidateRulerSegmentValueProperty);
                r.SetValue(Ruler.InvalidateRulerSegmentValueProperty, !trig);
            }
        }

        #endregion

        public Ruler()
        {
            this.DefaultStyleKey = typeof(Ruler);
            this.rs = new ResourceDictionary();
            this.rs.Source = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/RulerStyle.xaml", UriKind.RelativeOrAbsolute);
            this.Template = ((rs["RulerStyle"] as Style).Setters[0] as Setter).Value as ControlTemplate;
            this.Loaded += new RoutedEventHandler(Ruler_Loaded);
            double x = this.MURoundedIntervalValue;
        }

        public Ruler(string name)
            : this()
        {
            Name = name;
            if (Name == null || Name.Equals(string.Empty))
            {
                Name = "Ruler" + Guid.NewGuid().ToString("N");
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.NegativeScale = GetTemplateChild("PART_NegativeStack") as StackPanel;
            this.PositiveScale = GetTemplateChild("PART_PositiveStack") as StackPanel;
            this.cpforMargin = GetTemplateChild("PART_ContentPresenter") as FrameworkElement;
            this.Marker = GetTemplateChild("PART_Marker") as Line;

            Binding MarkerPosBind = new Binding("MarkerPosition");
            MarkerPosBind.Source = this;
            Marker.SetBinding(Line.X1Property, MarkerPosBind);
            Marker.SetBinding(Line.X2Property, MarkerPosBind);

            Binding MarkerThicknessBind = new Binding("MarkerThickness");
            MarkerThicknessBind.Source = this;
            Marker.SetBinding(Line.StrokeThicknessProperty, MarkerThicknessBind);

            Binding MarkerBrushBind = new Binding("MarkerBrush");
            MarkerBrushBind.Source = this;
            Marker.SetBinding(Line.StrokeProperty, MarkerBrushBind);

            Binding MarkerLengthBind = new Binding("MarkerLength");
            MarkerLengthBind.Source = this;
            Marker.SetBinding(Line.HeightProperty, MarkerLengthBind);


            this.dv = GetParentDV(this);
            if (dv != null)
            {
                Binding MUBinding = new Binding("Page.MeasurementUnits");
                MUBinding.Source = dv;
                this.SetBinding(Ruler.MeasurementUnitsProperty, MUBinding);

                Binding zoom = new Binding("CurrentZoom");
                zoom.Source = dv;
                this.SetBinding(Ruler.ScaleProperty, zoom);
            }

            if (sv == null)
            {
                if (dv != null)
                {
                    sv = dv.Scrollviewer;
                }
            }
            if (sv != null)
            {
                ScrollViewerExtraEvent svee = new ScrollViewerExtraEvent(sv);
                svee.ScrollViewerExtra += new ScrollViewerExtraEventEventHandler(svee_ScrollViewerExtra);
                StackPanel sp = GetTemplateChild("InvisiblePanel") as StackPanel;
                sp.Children.Add(svee);

                //Binding ofxBind = new Binding("HorizontalOffset");
                //ofxBind.Source = sv;
                //this.SetBinding(Ruler.OffsetXProperty, ofxBind);

                //Binding ofyBind = new Binding("VerticalOffset");
                //ofyBind.Source = sv;
                //this.SetBinding(Ruler.OffsetYProperty, ofyBind);
                
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.RulerWidth = Math.Max(sv.ExtentWidth, sv.ViewportWidth);
                }
                else
                {
                    this.RulerWidth = Math.Max(sv.ExtentHeight, sv.ViewportHeight);
                }
                UpdateRuler(this, RulerWidth);
            }
        }

        void svee_ScrollViewerExtra(object sender, ScrollViewerExtraEventRoutedEventArgs evtArgs)
        {
            switch (evtArgs.PropertyChanged)
            {
                case "ActualHeight":
                    break;
                case "ExtentHeight":
                    break;
                case "ScrollableHeight":
                    break;
                case "ViewportHeight":
                    break;
                case "ActualWidth":
                    break;
                case "ExtentWidth":
                    break;
                case "ScrollableWidth":
                    break;
                case "ViewportWidth":
                    break;
            }
            if (this.Orientation == Orientation.Horizontal)
            {
                this.RulerWidth = Math.Max(evtArgs.ExtentWidth, evtArgs.ViewportWidth);
            }
            else
            {
                this.RulerWidth = Math.Max(evtArgs.ExtentHeight, evtArgs.ViewportHeight);
            }
        }

        void Ruler_Loaded(object sender, RoutedEventArgs e)
        {
            //Binding HorStartPoint = new Binding("Left");
            //HorStartPoint.Source = this.dv.scrollGrid.page as DiagramPage;
            //this.SetBinding(Ruler.PxStartValueProperty, HorStartPoint);
        }

        private DiagramView GetParentDV(UIElement dv)
        {
            Object obj = VisualTreeHelper.GetParent(dv);
            if (obj != null)
            {
                if (obj is DiagramView)
                {
                    return obj as DiagramView;
                }
                else
                {
                    return GetParentDV(obj as UIElement);
                }
            }
            return null;
        }
    }

    internal class ScrollViewerExtraEvent : StackPanel
    {
        public ScrollViewer ScrollViewerEvent
        {
            get
            {
                return (ScrollViewer)GetValue(ScrollViewerEventProperty);
            }

            set
            {
                SetValue(ScrollViewerEventProperty, value);
            }
        }

        public static readonly DependencyProperty ScrollViewerEventProperty = DependencyProperty.Register("ScrollViewerEvent", typeof(ScrollViewer), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnScrollViewerEventChanged)));

        public static void OnScrollViewerEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public double ScrollableHeight
        {
            get
            {
                return (double)GetValue(ScrollableHeightProperty);
            }

            set
            {
                SetValue(ScrollableHeightProperty, value);
            }
        }

        public static readonly DependencyProperty ScrollableHeightProperty = DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnScrollableHeightPropertyChanged)));

        public static void OnScrollableHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ScrollableHeight");
        }

        public static void RaiseEvent(ScrollViewerExtraEvent svee, string prop)
        {
            ScrollViewerExtraEventRoutedEventArgs args = new ScrollViewerExtraEventRoutedEventArgs(svee.ScrollableHeight, svee.ExtentHeight, svee.ScrollableHeight, svee.ViewportHeight, svee.ActualWidth, svee.ExtentWidth, svee.ScrollableWidth, svee.ViewportWidth, prop);
            if (svee.ScrollViewerExtra != null)
            {
                svee.ScrollViewerExtra.Invoke(svee, args);
            }
        }

        public double ViewportHeight
        {
            get
            {
                return (double)GetValue(ViewportHeightProperty);
            }

            set
            {
                SetValue(ViewportHeightProperty, value);
            }
        }

        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnViewportHeightPropertyChanged)));

        public static void OnViewportHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ViewportHeight");
        }

        public new double ActualHeight
        {
            get
            {
                return (double)GetValue(ActualHeightProperty);
            }

            set
            {
                SetValue(ActualHeightProperty, value);
            }
        }

        public new static readonly DependencyProperty ActualHeightProperty = DependencyProperty.Register("ActualHeight", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnActualHeightPropertyChanged)));

        public static void OnActualHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ActualHeight");
        }

        public double ExtentHeight
        {
            get
            {
                return (double)GetValue(ExtentHeightProperty);
            }

            set
            {
                SetValue(ExtentHeightProperty, value);
            }
        }

        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.Register("ExtentHeight", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnExtentHeightPropertyChanged)));

        public static void OnExtentHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ExtentHeight");
        }

        public double ScrollableWidth
        {
            get
            {
                return (double)GetValue(ScrollableWidthProperty);
            }

            set
            {
                SetValue(ScrollableWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ScrollableWidthProperty = DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnScrollableWidthPropertyChanged)));

        public static void OnScrollableWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ScrollableWidth");
        }

        public double ViewportWidth
        {
            get
            {
                return (double)GetValue(ViewportWidthProperty);
            }

            set
            {
                SetValue(ViewportWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnViewportWidthPropertyChanged)));

        public static void OnViewportWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ViewportWidth");
        }

        public new double ActualWidth
        {
            get
            {
                return (double)GetValue(ActualWidthProperty);
            }

            set
            {
                SetValue(ActualWidthProperty, value);
            }
        }

        public new static readonly DependencyProperty ActualWidthProperty = DependencyProperty.Register("ActualWidth", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnActualWidthPropertyChanged)));

        public static void OnActualWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ActualWidth");
        }

        public double ExtentWidth
        {
            get
            {
                return (double)GetValue(ExtentWidthProperty);
            }

            set
            {
                SetValue(ExtentWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ExtentWidthProperty = DependencyProperty.Register("ExtentWidth", typeof(double), typeof(ScrollViewerExtraEvent), new PropertyMetadata(new PropertyChangedCallback(OnExtentWidthPropertyChanged)));

        public static void OnExtentWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerExtraEvent svee = d as ScrollViewerExtraEvent;
            ScrollViewerExtraEvent.RaiseEvent(svee, "ExtentWidth");
        }

        public event ScrollViewerExtraEventEventHandler ScrollViewerExtra;

        public ScrollViewerExtraEvent(ScrollViewer sv)
        {
            this.ScrollViewerEvent = sv;

            this.SetBinding(ScrollViewerExtraEvent.ActualHeightProperty, GetBinding("ActualHeight"));
            this.SetBinding(ScrollViewerExtraEvent.ExtentHeightProperty, GetBinding("ExtentHeight"));
            this.SetBinding(ScrollViewerExtraEvent.ScrollableHeightProperty, GetBinding("ScrollableHeight"));
            this.SetBinding(ScrollViewerExtraEvent.ViewportHeightProperty, GetBinding("ViewportHeight"));

            this.SetBinding(ScrollViewerExtraEvent.ActualWidthProperty, GetBinding("ActualWidth"));
            this.SetBinding(ScrollViewerExtraEvent.ExtentWidthProperty, GetBinding("ExtentWidth"));
            this.SetBinding(ScrollViewerExtraEvent.ScrollableWidthProperty, GetBinding("ScrollableWidth"));
            this.SetBinding(ScrollViewerExtraEvent.ViewportWidthProperty, GetBinding("ViewportWidth"));
        }

        private System.Windows.Data.Binding GetBinding(string p)
        {
            Binding binding = new Binding(p);
            binding.Source = this.ScrollViewerEvent;
            return binding;
        }

    }

    internal delegate void ScrollViewerExtraEventEventHandler(object sender, ScrollViewerExtraEventRoutedEventArgs evtArgs);

    internal class ScrollViewerExtraEventRoutedEventArgs : RoutedEventArgs
    {
        public double ActualHeight { get; set; }
        public double ExtentHeight { get; set; }
        public double ScrollableHeight { get; set; }
        public double ViewportHeight { get; set; }

        public double ActualWidth { get; set; }
        public double ExtentWidth { get; set; }
        public double ScrollableWidth { get; set; }
        public double ViewportWidth { get; set; }

        public string PropertyChanged { get; set; }

        public ScrollViewerExtraEventRoutedEventArgs(double ah, double eh, double sh, double vh,
            double aw, double ew, double sw, double vw, string prop)
        {
            ActualHeight = ah;
            ExtentHeight = eh;
            ScrollableHeight = sh;
            ViewportHeight = vh;

            ActualWidth = aw;
            ExtentWidth = ew;
            ScrollableWidth = sw;
            ViewportWidth = vw;

            PropertyChanged = prop;
        }
    }
}
