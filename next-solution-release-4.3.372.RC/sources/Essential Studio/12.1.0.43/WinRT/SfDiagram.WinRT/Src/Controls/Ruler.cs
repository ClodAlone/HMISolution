#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Diagram.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using CultureInfo = System.String;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Globalization;
#endif

namespace Syncfusion.UI.Xaml.Diagram.Controls
{
    public partial class Ruler : ContentControl
    {
        private readonly CompositeTransform _rulerTransform;
        private Panel _canvas;
        internal MeasurementUnit _unit = new LengthUnit() { Unit = LengthUnits.Pixels };
        internal double _segmentWidth;

#if WPF
        ///<summary>
        /// Initializes static members of the <see cref="Ruler"/> class.
        ///</summary>
        static Ruler()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ruler), new FrameworkPropertyMetadata(typeof(Ruler)));
        }
#endif

        /// <summary>
        /// Initializes a new instances of the <see cref="Ruler"/> class. 
        /// </summary>
        public Ruler()
        {
#if !WPF
            this.DefaultStyleKey = typeof(Ruler);
#endif
            _rulerTransform = new CompositeTransform();


        }


        #region Dependency Properties

        /// <summary>
        /// Gets or sets the ruler orientation
        /// </summary>
        /// <value>
        /// Type: <see cref="Orientation"/>
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Ruler),
                                        new PropertyMetadata(Orientation.Horizontal, Invalidate));

        /// <summary>
        /// Gets or sets the Offset value
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(Ruler), new PropertyMetadata(0d, Invalidate));

        /// <summary>
        /// Gets or sets the scale value
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Ruler), new PropertyMetadata(1.0d, Invalidate));

        /// <summary>
        /// Gets or sets the thickness of ruler
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(Ruler),
                                        new PropertyMetadata(25d, Invalidate));

        #endregion

        private static void Invalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Ruler).InvalidateMeasure();
        }

        private void DoApplyTemplate()
        {
            _canvas = GetTemplateChild("Part_RulerPanel") as Panel;
#if !WPF
            _canvas.RenderTransform = _rulerTransform;
#else
            _canvas.RenderTransform = _rulerTransform.Transform;
#endif
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateSegmentWidth();
            double length = 0;
            if (Orientation == Orientation.Horizontal)
            {
                length = availableSize.Width;
                availableSize.Height = Thickness;
            }
            else
            {
                length = availableSize.Height;
                availableSize.Width = Thickness;
            }
            if (length.IsValid())
            {
                double unitLength = _unit.ToUnit(length);
                double unitOffset = _unit.ToUnit(Offset);
                UpdateSegments(unitOffset, (unitLength + unitOffset));
                if (_canvas.Children.Count != 0)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        _rulerTransform.TranslateX = (_unit.ToPixel((_canvas.Children[0] as RulerSegment).StartValue) * Scale - Offset);
                    }
                    else
                    {
                        _rulerTransform.TranslateY = (_unit.ToPixel((_canvas.Children[0] as RulerSegment).StartValue) * Scale - Offset);
                    }
                }
            }
            base.MeasureOverride(availableSize);
            if (Orientation == Orientation.Horizontal)
            {
                return new Size(length, Thickness).Valid();
            }
            else
            {
                return new Size(Thickness, length).Valid();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);
            return new Size(finalSize.Width + .1, finalSize.Height + .1);
        }

        private void UpdateSegments(double start, double end)
        {
            double run = start;
            double trans = 0;
            foreach (RulerSegment rulerSegment in _canvas.Children)
            {
                run = UpdateSegment(start, end, rulerSegment, run, ref trans);
            }
            // Insufficient segments
            while (run < end)
            {
                RulerSegment rulerSegment = GetNewSegment();
                rulerSegment.Ruler = this;
                _canvas.Children.Add(rulerSegment);
                run = UpdateSegment(start, end, rulerSegment, run, ref trans);
            }
        }

        private double UpdateSegment(double start, double end, RulerSegment rulerSegment, double run, ref double trans)
        {
            double segWidth = rulerSegment.GetSegmentWidth();
            if (run == start)
            {
                //rulerSegment.Visibility = Visibility.Visible;
                rulerSegment.StartValue = Math.Floor(start / segWidth) * segWidth / Scale;
                run = rulerSegment.StartValue * Scale;
            }
            else if (run > end)
            {
                //rulerSegment.Visibility = Visibility.Collapsed;
            }
            else
            {
                //rulerSegment.Visibility = Visibility.Visible;
                rulerSegment.StartValue = run / Scale;
            }
            rulerSegment.SegmentWidth = segWidth;
            if (Orientation == Orientation.Horizontal)
            {
                rulerSegment._mTranslateTransform.X = trans;
            }
            else
            {
                rulerSegment._mTranslateTransform.Y = trans;
            }
            trans += rulerSegment.PxSegmentWidth;
            run += segWidth;
            return run;
        }

        /// <summary>
        /// To define custom ruler segment
        /// </summary>
        /// <returns>return new ruler segment</returns>
        protected virtual RulerSegment GetNewSegment()
        {
            return new RulerSegment();
        }

        internal void PrepareRuler(ScrollViewer scrollViewer)
        {
            Binding offset = new Binding()
                {
                    Path = new PropertyPath(Orientation == Orientation.Horizontal ? "HorizontalOffset" : "VerticalOffset"),
                    Source = scrollViewer,
                };
            Binding zoom = new Binding()
                {
                    Path = new PropertyPath("CurrentZoom"),
                    Source = scrollViewer
                };

            SetBinding(OffsetProperty, offset);
            SetBinding(ScaleProperty, zoom);
        }

        internal void PrepareUnit(MeasurementUnit unit)
        {
            if (_unit != null)
            {
                _unit.UnitChangedEvent -= UnitChangedEvent;
            }
            _unit = unit ?? new LengthUnit() { Unit = LengthUnits.Pixels };
            if (_unit != null)
            {
                _unit.UnitChangedEvent += UnitChangedEvent;
            }
            UpdateSegmentWidth();
        }

        void UnitChangedEvent(object sender, UnitToUnitEventArgs<double> current)
        {
            InvalidateMeasure();
        }

        private void UpdateSegmentWidth()
        {
            _segmentWidth = GetSegmentWidth(_unit, Scale);
        }

        private static double GetSegmentWidth(MeasurementUnit _unit, double scale)
        {
            //return 50*Ruler.Scale;
            double fifty = BestRound(_unit.ToUnit(100));
            double five = 25; // BestRound(Ruler._unit.ToUnit(25));

            double scaleRound = Math.Pow(2, Math.Round(Math.Log(scale) / Math.Log(2)));

            double div = fifty;
            //if (Ruler.Scale >= 1)
            {
                div = (fifty / scaleRound);
            }
            //else
            //{
            //    div = (fifty / scale);
            //    return Math.Round(div / fifty) * fifty * Ruler.Scale;
            //}
            double multiples = 1;
            while (div > 100)
            {
                multiples /= 10;
                div /= 10;
            }
            while (div < 25)
            {
                multiples *= 10;
                div *= 10;
            }
            if (div >= five && div % five != 0)
            {
                div = Math.Round(div / five) * five;
            }
            return div * scale / multiples;
        }

        private static double BestRound(double value)
        {
            int shift = 0;
            double temp = value;
            while (temp < 1)
            {
                shift++;
                temp *= 10;
            }
            return Math.Round(value, shift);
        }

        internal void GetRulerSegmentValues(out double segmentWidth, out int intervals)
        {
            if (Visibility == Visibility.Collapsed)
            {
                UpdateSegmentWidth();
            }
            RulerSegment segment=_canvas.Children[0] as RulerSegment;
            segmentWidth = segment.GetSegmentWidth();
            intervals = segment.Intervals;
        }

    }

    public partial class RulerSegment : ContentControl
    {
        private readonly List<Tick> _mTicks;
        private Panel _mCanvas;
        private TickAlignment _align;
        internal TranslateTransform _mTranslateTransform = new TranslateTransform();
        private TextBlock _label;
        private readonly CompositeTransform _labelPosition = new CompositeTransform();
        private double _segmentWidth;
        private double _thickness;

        /// <summary>
        /// Gets the ruler to which the segment is added.
        /// </summary>
        public Ruler Ruler { get; internal set; }

        /// <summary>
        /// Gets the segment width of each ruler segments.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double SegmentWidth
        {
            get { return _segmentWidth; }
            internal set
            {
                if (_segmentWidth != value)
                {
                    _segmentWidth = value;
                    InvalidateArrange();
                }
            }
        }

        internal double PxSegmentWidth {
            get { return Ruler._unit.ToPixel(SegmentWidth); }
        }

        /// <summary>
        /// Gets the start value for each segments.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            internal set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(RulerSegment), new PropertyMetadata(0d));

        public int Intervals
        {
            get { return (int)GetValue(IntervalsProperty); }
            set { SetValue(IntervalsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Intervals.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalsProperty =
            DependencyProperty.Register("Intervals", typeof(int), typeof(RulerSegment), new PropertyMetadata(5));

#if WPF
        ///<summary>
        /// Initializes static members of the <see cref="RulerSegment"/> class.
        /// 
        static RulerSegment()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulerSegment), new FrameworkPropertyMetadata(typeof(RulerSegment)));
        }
#endif
        /// <summary>
        /// Initializes a new instances of the <see cref="RulerSegment"/> class. 
        /// </summary>
        public RulerSegment()
        {
#if !WPF
            this.DefaultStyleKey = typeof(RulerSegment);
#endif
            _mTicks = new List<Tick>();
            this.RenderTransform = _mTranslateTransform;
        }

        void DoApplyTemplate()
        {
            base.OnApplyTemplate();
            _mCanvas = GetTemplateChild("Part_RulerSegmentPanel") as Panel;
            _label = GetTemplateChild("PART_Label") as TextBlock;
#if !WPF
            _label.RenderTransform = _labelPosition; 
#else
            _label.RenderTransform = _labelPosition.Transform;             
#endif
            _label.SizeChanged += (s, e) => UpdateLabel(_label);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _thickness = Ruler.Thickness;
            CreateUpdateTicks();
            base.MeasureOverride(availableSize);
            return availableSize.Valid();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _thickness = Ruler.Thickness;
            double value = 0;
            for (int i = 1; i <= Intervals; i++)
            {
                ArrangeLine(_mCanvas.Children[i] as Line, value);
                value += (PxSegmentWidth/Intervals);
            }
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        private void CreateUpdateTicks()
        {
            for (int i = _mTicks.Count; i < Intervals; i++)
            {
                Tick newTick = GetNewTick();
                newTick.Segment = this;
                _mTicks.Add(newTick);
                Line line = new Line()
                    {
                        RenderTransform = new TranslateTransform()
                    };
                line.SetBinding(Line.StrokeProperty,
                                new Binding()
                                    {
                                        Path = new PropertyPath("Foreground"),
                                        Source = this
                                    }
                    );
                _mCanvas.Children.Add(line);
            }
            double value = 0;
            int index = 1;
            foreach (Tick mTick in _mTicks)
            {
                Line line = _mCanvas.Children[index] as Line;
                mTick.Value = value;
                Point pos = mTick.GetLinePoint(PxSegmentWidth, _thickness, out _align);
                if (Ruler.Orientation == Orientation.Horizontal)
                {
                    line.Y1 = pos.X;
                    line.Y2 = pos.Y;
                }
                else
                {
                    line.X1 = pos.X;
                    line.X2 = pos.Y;
                }
                mTick.UpdateLine(line);
                index++;
                value += (PxSegmentWidth / Intervals);
            }
        }

        private void ArrangeLine(Line tick, double value)
        {
            if (Ruler.Orientation == Orientation.Horizontal)
            {
                (tick.RenderTransform as TranslateTransform).X = Math.Abs(value);
            }
            else
            {
                (tick.RenderTransform as TranslateTransform).Y = Math.Abs(value);
            }
        }

        /// <summary>
        /// To gets the width of each ruler segment
        /// </summary>
        /// <returns>Returns width of the ruler segment</returns>
        public virtual double GetSegmentWidth()
        {
            return Ruler._segmentWidth;
        }
        
        /// <summary>
        /// To define the label position
        /// </summary>
        /// <param name="label">Label for current ruler segment</param>
        /// <returns>Returns the label position</returns>
        protected internal virtual void UpdateLabel(TextBlock label)
        {
            Point p = new Point(0,0);
            if (Ruler.Orientation == Orientation.Horizontal)
            {
                _labelPosition.Rotation = 0;
                if (_align == TickAlignment.LeftOrTop)
                {
                    p.X = 3;
                    p.Y = _thickness - label.DesiredSize.Height - 2;
                }
                else if (_align == TickAlignment.RightOrBottom)
                {
                    p.X = 3;
                    p.Y = 2;
                }
                else
                {
                    p.X = (PxSegmentWidth - label.DesiredSize.Width) / 2;
                    p.Y = (_thickness - label.DesiredSize.Height) / 2;
                }
            }
            else
            {
                _labelPosition.Rotation = -90 ;
                if (_align == TickAlignment.LeftOrTop)
                {
                    p.X = _thickness - label.DesiredSize.Height - 2;
                    p.Y = label.DesiredSize.Width + 3;
                }
                else if (_align == TickAlignment.RightOrBottom)
                {
                    p.X = 2;
                    p.Y = label.DesiredSize.Width + 3;
                }
                else
                {
                    p.X = (_thickness - label.DesiredSize.Height) / 2;
                    p.Y = (PxSegmentWidth + label.DesiredSize.Width) / 2;
                }
            }
            _labelPosition.TranslateX = p.X;
            _labelPosition.TranslateY = p.Y;
        }

        /// <summary>
        /// To define custom ticks
        /// </summary>
        /// <returns>Returns new tick</returns>
        protected virtual Tick GetNewTick()
        {
            return new Tick();
        }
    }

    public class Tick
    {
        /// <summary>
        /// To gets the tick value
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double Value { get; internal set; }

        /// <summary>
        /// Gets the segment to which the tick line is added.
        /// </summary>
        public RulerSegment Segment { get; internal set; }

        internal Point GetLinePoint(double segmentWidth, double segmentHeight, out TickAlignment align)
        {
            double start, length;
            ArrangeTick(out start, out length, out align);
            double s, e;
            if (align == TickAlignment.LeftOrTop)
            {
                s = start;
                e = start + length;
            }
            else if (align == TickAlignment.RightOrBottom)
            {
                s = segmentHeight - start;
                e = segmentHeight - (start + length);
            }
            else
            {
                s = (segmentHeight - length) / 2;
                e = segmentHeight - ((segmentHeight - length) / 2);
            }
            return new Point(s, e);
        }

        /// <summary>
        /// To update the ticks values start value, length, alignment
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="length">Length of the tick</param>
        /// <param name="align">Alignment of the tick</param>
        protected virtual void ArrangeTick(out double start, out double length, out TickAlignment align)
        {
            align = TickAlignment.RightOrBottom;
            start = 0;
            if ((Value %  Segment.PxSegmentWidth) == 0)
            {
                length = Segment.Ruler.Thickness;
            }
            else
            {
                length = Segment.Ruler.Thickness * 0.3;
            }
        }

        /// <summary>
        /// To update the appearance of the ticks
        /// </summary>
        /// <param name="line">Tick line needto be updated</param>
        protected internal virtual void UpdateLine(Line line)
        {
        }
    }

    /// <summary>
    /// Tick line alignemnt
    /// </summary>
    public enum TickAlignment
    {
        /// <summary>
        /// Left or Top alignment
        /// </summary>
        LeftOrTop,

        ///// <summary>
        ///// Center alignment
        ///// </summary>
        //Center,

        /// <summary>
        /// Right or Bottom alignment
        /// </summary>
        RightOrBottom
    }
    
    public class LabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double temp = Math.Round((double)value, 3);
            return temp.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,CultureInfo culture)
        {
            return value;
        }
    }
}
